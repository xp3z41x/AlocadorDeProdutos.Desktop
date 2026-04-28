using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlocadorDeProdutos
{
    /// <summary>
    /// Janela modal de alocacao em massa. Recebe a conexao da UI do FormMain,
    /// a filial corrente e um callback (EnfileirarLote) que injeta os itens
    /// selecionados na fila write-ahead ja existente — preservando idempotencia
    /// e resiliencia a kill abrupto.
    /// </summary>
    public partial class FormAlocacaoMassa : Form
    {
        private readonly NpgsqlConnection conn;
        private readonly int filial;
        private readonly Action<IEnumerable<Tuple<int, string, string>>> enfileirarLote;
        private readonly Func<int> getFilaCount;

        // Selecao persistente entre buscas: matricula -> dados do produto.
        private readonly Dictionary<int, ProdutoSelecionado> selecionados =
            new Dictionary<int, ProdutoSelecionado>();

        // Anchor para Shift+click (range selection)
        private int? lastCheckedRowIndex = null;

        // Cancelamento da operacao de espera quando o form fecha durante a drenagem.
        private CancellationTokenSource ctsDrenagem;

        private const int LIMIT_GRID = 500;
        // Tempo maximo aguardando fila drenar antes de re-buscar no banco.
        // Worker processa ~10-30 UPDATEs/s; 60s cobre lotes de 500 itens com folga.
        private const int TIMEOUT_DRENAGEM_MS = 60000;

        // Fontes mantidas como campos para dispose explicito (evita leak de GDI handles
        // ao reabrir o modal multiplas vezes na mesma sessao).
        private Font fontGridCell;
        private Font fontGridHeader;
        private Font fontLocalAtualBold;

        // Handler nomeado das duas barras de busca (precisa ser nomeado para
        // poder fazer -= em Dispose; lambda nao permite unsubscribe).
        private void TxtBuscaTextChanged(object sender, EventArgs e) => Buscar();

        private struct ProdutoSelecionado
        {
            public int Matricula;
            public string Descricao;
            public string Referencia;
            public string LocalAtual;
        }

        public FormAlocacaoMassa(
            NpgsqlConnection conn,
            int filial,
            Action<IEnumerable<Tuple<int, string, string>>> enfileirarLote,
            Func<int> getFilaCount)
        {
            this.conn = conn;
            this.filial = filial;
            this.enfileirarLote = enfileirarLote;
            this.getFilaCount = getFilaCount;

            InitializeComponent();
            EstilizarGrid();

            lblFilial.Text = filial.ToString("D2");

            // Wire-up de eventos (todos com handlers NOMEADOS para permitir unsubscribe
            // no Dispose abaixo — lambdas seriam impossiveis de remover).
            txtAlocacao.TextChanged += txtAlocacao_TextChanged;
            txtBuscaPrefixo.TextChanged += TxtBuscaTextChanged;
            txtBuscaContem.TextChanged += TxtBuscaTextChanged;
            dgvProdutos.CellContentClick += dgvProdutos_CellContentClick;
            dgvProdutos.CellMouseUp += dgvProdutos_CellMouseUp;
            dgvProdutos.CellValueChanged += dgvProdutos_CellValueChanged;
            btnMarcarTodos.Click += btnMarcarTodos_Click;
            btnDesmarcarTodos.Click += btnDesmarcarTodos_Click;
            btnAlocar.Click += btnAlocar_Click;
            btnCancelar.Click += btnCancelar_Click;
            this.KeyDown += FormAlocacaoMassa_KeyDown;

            AtualizarBotaoAlocar();
        }

        /// <summary>
        /// Desinscreve eventos e libera fontes. Critico em modais que sao reabertos
        /// na mesma sessao — sem isso, handlers e GDI handles vazam acumulativamente.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Se uma drenagem estiver em andamento, cancelar o await — Task.Delay
            // joga OperationCanceledException, que e tratada no btnAlocar_Click.
            // A fila continua drenando em background no FormMain (sem prejuizo).
            try { ctsDrenagem?.Cancel(); } catch { }
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { ctsDrenagem?.Cancel(); } catch { }
                try { ctsDrenagem?.Dispose(); } catch { }
                ctsDrenagem = null;

                try
                {
                    txtAlocacao.TextChanged -= txtAlocacao_TextChanged;
                    txtBuscaPrefixo.TextChanged -= TxtBuscaTextChanged;
                    txtBuscaContem.TextChanged -= TxtBuscaTextChanged;
                    dgvProdutos.CellContentClick -= dgvProdutos_CellContentClick;
                    dgvProdutos.CellMouseUp -= dgvProdutos_CellMouseUp;
                    dgvProdutos.CellValueChanged -= dgvProdutos_CellValueChanged;
                    btnMarcarTodos.Click -= btnMarcarTodos_Click;
                    btnDesmarcarTodos.Click -= btnDesmarcarTodos_Click;
                    btnAlocar.Click -= btnAlocar_Click;
                    btnCancelar.Click -= btnCancelar_Click;
                    this.KeyDown -= FormAlocacaoMassa_KeyDown;
                }
                catch { }

                fontGridCell?.Dispose();
                fontGridHeader?.Dispose();
                fontLocalAtualBold?.Dispose();

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EstilizarGrid()
        {
            // Fontes em campos para dispose explicito (evita leak de GDI handles).
            fontGridCell = new Font("Segoe UI", 10F);
            fontGridHeader = new Font("Segoe UI", 10F, FontStyle.Bold);
            fontLocalAtualBold = new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvProdutos.DefaultCellStyle.BackColor = Color.FromArgb(28, 28, 44);
            dgvProdutos.DefaultCellStyle.ForeColor = Color.White;
            dgvProdutos.DefaultCellStyle.Font = fontGridCell;
            dgvProdutos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvProdutos.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvProdutos.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

            dgvProdutos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(34, 34, 52);
            dgvProdutos.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;

            dgvProdutos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 44, 66);
            dgvProdutos.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 215, 255);
            dgvProdutos.ColumnHeadersDefaultCellStyle.Font = fontGridHeader;
            dgvProdutos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProdutos.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

            dgvProdutos.Columns["colMatricula"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvProdutos.Columns["colLocalAtual"].DefaultCellStyle.Font = fontLocalAtualBold;
        }

        // ===== BUSCA =====

        private void Buscar()
        {
            string prefixo = txtBuscaPrefixo.Text.Trim();
            string contem = txtBuscaContem.Text.Trim();

            // Reset do anchor de Shift+click — indices da busca anterior nao
            // sao validos para a nova lista de linhas.
            lastCheckedRowIndex = null;

            if (string.IsNullOrEmpty(prefixo) && string.IsNullOrEmpty(contem))
            {
                dgvProdutos.Rows.Clear();
                lblStatus.Text = "Digite ao menos um termo de busca.";
                lblStatus.ForeColor = Color.FromArgb(160, 160, 185);
                return;
            }

            try
            {
                string sql = @"SELECT cadpro.matricula, cadpro.descricao, cadpro.referencia, estsal.local
                               FROM cadpro
                               JOIN estsal ON cadpro.matricula = estsal.matricula
                                           AND estsal.filial = @filial
                               WHERE 1=1";

                if (!string.IsNullOrEmpty(prefixo))
                    sql += " AND cadpro.descricao ILIKE @prefixo";
                if (!string.IsNullOrEmpty(contem))
                    sql += " AND cadpro.descricao ILIKE @contem";

                sql += " ORDER BY cadpro.descricao LIMIT " + LIMIT_GRID;

                dgvProdutos.Rows.Clear();

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 5;
                    cmd.Parameters.AddWithValue("filial", (short)filial);
                    if (!string.IsNullOrEmpty(prefixo))
                        cmd.Parameters.AddWithValue("prefixo", prefixo + "%");
                    if (!string.IsNullOrEmpty(contem))
                        cmd.Parameters.AddWithValue("contem", "%" + contem + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int matricula = Convert.ToInt32(reader["matricula"]);
                            string descricao = reader["descricao"]?.ToString() ?? string.Empty;
                            string referencia = reader["referencia"]?.ToString() ?? string.Empty;
                            string localAtual = reader["local"] != DBNull.Value
                                ? reader["local"].ToString()
                                : string.Empty;

                            int idx = dgvProdutos.Rows.Add(
                                selecionados.ContainsKey(matricula),
                                matricula.ToString(),
                                descricao,
                                referencia,
                                localAtual);

                            // Pinta a celula da Alocacao Atual conforme estado
                            ColorirLocalAtual(dgvProdutos.Rows[idx], localAtual);
                        }
                    }
                }

                int total = dgvProdutos.Rows.Count;
                if (total >= LIMIT_GRID)
                {
                    lblStatus.ForeColor = Color.FromArgb(243, 156, 18);
                    lblStatus.Text = "Limite de " + LIMIT_GRID +
                                     " atingido — refine a busca para ver mais.";
                }
                else if (total == 0)
                {
                    lblStatus.ForeColor = Color.FromArgb(160, 160, 185);
                    lblStatus.Text = "Nenhum produto encontrado para a filial " + filial + ".";
                }
                else
                {
                    lblStatus.ForeColor = Color.FromArgb(160, 160, 185);
                    lblStatus.Text = total + " produto(s) listado(s).";
                }
            }
            catch (Exception ex)
            {
                bool isConexao = ex is NpgsqlException ||
                                 ex is System.Net.Sockets.SocketException ||
                                 ex is TimeoutException;
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = isConexao
                    ? "FALHA DE CONEXAO! Feche e tente novamente."
                    : "Erro: " + ex.Message;
            }
        }

        private void ColorirLocalAtual(DataGridViewRow row, string localAtual)
        {
            string nova = txtAlocacao.Text.Trim().ToUpper();
            var cell = row.Cells["colLocalAtual"];

            if (string.IsNullOrEmpty(localAtual))
            {
                cell.Style.ForeColor = Color.White;
            }
            else if (string.Equals(localAtual.Trim(), nova, StringComparison.OrdinalIgnoreCase)
                     && !string.IsNullOrEmpty(nova))
            {
                cell.Style.ForeColor = Color.FromArgb(39, 174, 96);   // verde — OK
            }
            else
            {
                cell.Style.ForeColor = Color.FromArgb(243, 156, 18);  // laranja — SUBSTITUI
            }
        }

        private void txtAlocacao_TextChanged(object sender, EventArgs e)
        {
            // Re-colorir todas as linhas conforme nova alocacao alvo
            foreach (DataGridViewRow row in dgvProdutos.Rows)
            {
                string localAtual = row.Cells["colLocalAtual"].Value?.ToString() ?? string.Empty;
                ColorirLocalAtual(row, localAtual);
            }
            AtualizarBotaoAlocar();
        }

        // ===== SELECAO / GRID EVENTS =====

        private void dgvProdutos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != colCheck.Index || e.RowIndex < 0) return;
            // Force commit imediato para que CellValueChanged dispare ja
            dgvProdutos.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvProdutos_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != colCheck.Index || e.RowIndex < 0) return;

            bool shift = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

            if (shift && lastCheckedRowIndex.HasValue
                && lastCheckedRowIndex.Value < dgvProdutos.Rows.Count
                && lastCheckedRowIndex.Value != e.RowIndex)
            {
                int from = Math.Min(lastCheckedRowIndex.Value, e.RowIndex);
                int to = Math.Max(lastCheckedRowIndex.Value, e.RowIndex);

                // Estado-alvo = estado da celula clicada agora (apos toggle natural).
                bool target = (bool)(dgvProdutos.Rows[e.RowIndex].Cells[colCheck.Index].Value ?? false);

                for (int i = from; i <= to; i++)
                {
                    if (i == e.RowIndex) continue; // ja esta no estado correto
                    var cell = dgvProdutos.Rows[i].Cells[colCheck.Index];
                    if (!Equals(cell.Value, target))
                    {
                        cell.Value = target;
                        SincronizarSelecao(i, target);
                    }
                }
            }

            lastCheckedRowIndex = e.RowIndex;
        }

        private void dgvProdutos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != colCheck.Index || e.RowIndex < 0) return;
            bool marcado = (bool)(dgvProdutos.Rows[e.RowIndex].Cells[colCheck.Index].Value ?? false);
            SincronizarSelecao(e.RowIndex, marcado);
        }

        private void SincronizarSelecao(int rowIndex, bool marcado)
        {
            if (rowIndex < 0 || rowIndex >= dgvProdutos.Rows.Count) return;
            var row = dgvProdutos.Rows[rowIndex];
            if (!int.TryParse(row.Cells["colMatricula"].Value?.ToString(), out int matricula))
                return;

            if (marcado)
            {
                selecionados[matricula] = new ProdutoSelecionado
                {
                    Matricula = matricula,
                    Descricao = row.Cells["colDescricao"].Value?.ToString() ?? string.Empty,
                    Referencia = row.Cells["colReferencia"].Value?.ToString() ?? string.Empty,
                    LocalAtual = row.Cells["colLocalAtual"].Value?.ToString() ?? string.Empty
                };
            }
            else
            {
                selecionados.Remove(matricula);
            }

            AtualizarContador();
        }

        private void AtualizarContador()
        {
            lblContador.Text = "Selecionados: " + selecionados.Count;
            AtualizarBotaoAlocar();
        }

        // ===== BOTOES SUPERIORES =====

        private void btnMarcarTodos_Click(object sender, EventArgs e)
        {
            // Marca TODAS as linhas visiveis (do resultado da busca corrente)
            foreach (DataGridViewRow row in dgvProdutos.Rows)
            {
                var cell = row.Cells[colCheck.Index];
                if (!Equals(cell.Value, true))
                    cell.Value = true;
                if (int.TryParse(row.Cells["colMatricula"].Value?.ToString(), out int mat))
                {
                    selecionados[mat] = new ProdutoSelecionado
                    {
                        Matricula = mat,
                        Descricao = row.Cells["colDescricao"].Value?.ToString() ?? string.Empty,
                        Referencia = row.Cells["colReferencia"].Value?.ToString() ?? string.Empty,
                        LocalAtual = row.Cells["colLocalAtual"].Value?.ToString() ?? string.Empty
                    };
                }
            }
            AtualizarContador();
        }

        private void btnDesmarcarTodos_Click(object sender, EventArgs e)
        {
            // Desmarca apenas as visiveis (consistente com Marcar Todos);
            // selecoes feitas em buscas anteriores permanecem.
            foreach (DataGridViewRow row in dgvProdutos.Rows)
            {
                var cell = row.Cells[colCheck.Index];
                if (!Equals(cell.Value, false))
                    cell.Value = false;
                if (int.TryParse(row.Cells["colMatricula"].Value?.ToString(), out int mat))
                    selecionados.Remove(mat);
            }
            AtualizarContador();
        }

        private void FormAlocacaoMassa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                // Se foco esta num TextBox, deixa o comportamento padrao do Windows
                // (selecionar todo o texto). Atalho de marcar todos so vale fora deles.
                if (txtAlocacao.Focused || txtBuscaPrefixo.Focused || txtBuscaContem.Focused)
                    return;

                e.SuppressKeyPress = true;
                btnMarcarTodos_Click(sender, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                btnCancelar_Click(sender, EventArgs.Empty);
            }
        }

        // ===== ALOCAR =====

        private bool ValidarAlocacao(out string alocacaoLimpa)
        {
            alocacaoLimpa = (txtAlocacao.Text ?? string.Empty).Trim().ToUpper();

            if (string.IsNullOrEmpty(alocacaoLimpa))
                return false;
            if (alocacaoLimpa.Length > 10)
                return false;
            if (!char.IsLetter(alocacaoLimpa[0]))
                return false;

            foreach (char c in alocacaoLimpa)
            {
                if (!char.IsLetterOrDigit(c) && c != '-')
                    return false;
            }
            return true;
        }

        private void AtualizarBotaoAlocar()
        {
            bool ok = ValidarAlocacao(out _) && selecionados.Count > 0;
            btnAlocar.Enabled = ok;
            btnAlocar.Text = "ALOCAR (" + selecionados.Count + ")";
            btnAlocar.BackColor = ok
                ? Color.FromArgb(39, 174, 96)
                : Color.FromArgb(80, 80, 110);
        }

        private async void btnAlocar_Click(object sender, EventArgs e)
        {
            if (!ValidarAlocacao(out string alocacao))
            {
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = "Alocacao invalida — deve comecar com letra, " +
                                 "ter no maximo 10 caracteres e usar apenas letras, " +
                                 "digitos e hifens.";
                txtAlocacao.Focus();
                return;
            }

            if (selecionados.Count == 0)
            {
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = "Selecione ao menos um produto.";
                return;
            }

            // Particionar para o aviso de substituicao
            var substituir = selecionados.Values
                .Where(p => !string.IsNullOrEmpty(p.LocalAtual)
                         && !string.Equals(p.LocalAtual.Trim(), alocacao, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (substituir.Count > 0)
            {
                var top = substituir.Take(10)
                    .Select(p => "  • Mat " + p.Matricula + " (" + p.LocalAtual.Trim() + ")");
                string lista = string.Join("\n", top);
                if (substituir.Count > 10)
                    lista += "\n  ... e mais " + (substituir.Count - 10);

                string msg = substituir.Count + " produto(s) ja tem alocacao diferente:\n\n"
                           + lista
                           + "\n\nSubstituir TODAS as alocacoes pela nova (" + alocacao + ")?";

                var res = MessageBox.Show(this, msg, "Confirmar substituicao",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res != DialogResult.Yes)
                {
                    lblStatus.ForeColor = Color.FromArgb(160, 160, 185);
                    lblStatus.Text = "Operacao cancelada — nenhum produto enfileirado.";
                    return;
                }
            }

            int total = selecionados.Count;
            var itens = selecionados.Values
                .Select(p => Tuple.Create(p.Matricula, alocacao, p.Descricao))
                .ToList();

            // Enfileirar (write-ahead atomico no FormMain.EnfileirarLote)
            try
            {
                enfileirarLote(itens);
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = "Erro ao enfileirar: " + ex.Message;
                return;
            }

            // Limpar selecao em memoria. As linhas do grid ainda mostram o estado
            // antigo do banco — vamos re-buscar apos a fila drenar.
            selecionados.Clear();
            lastCheckedRowIndex = null;
            AtualizarContador();

            // Desabilitar UI durante a drenagem. Operador ainda pode fechar o modal
            // pelo X — fila continua drenando em background no FormMain.
            DesabilitarUIDuranteDrenagem();
            ctsDrenagem = new CancellationTokenSource();

            try
            {
                bool drenou = await AguardarFilaDrenarAsync(total, TIMEOUT_DRENAGEM_MS, ctsDrenagem.Token);

                // Form pode ter sido fechado durante a espera.
                if (this.IsDisposed) return;

                if (drenou)
                {
                    lblStatus.ForeColor = Color.FromArgb(243, 156, 18);
                    lblStatus.Text = "Fila drenada. Atualizando grid com dados do banco...";
                    Application.DoEvents(); // permite que o lblStatus redesenhe antes do SELECT

                    // Re-busca do banco — agora reflete estado real apos os UPDATEs
                    // confirmados. Se nada mudou (cache do banco?), grid mostra
                    // exatamente o que foi gravado.
                    Buscar();

                    lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                    lblStatus.Text = total + " produto(s) alocados em " + alocacao +
                                     " e confirmados pelo banco.";
                }
                else
                {
                    int restantes = SafeGetFilaCount();
                    lblStatus.ForeColor = Color.FromArgb(243, 156, 18);
                    lblStatus.Text = "Timeout aguardando drenagem (" + restantes +
                                     " ainda na fila). Itens serao gravados em background — " +
                                     "atualize a busca quando o contador zerar no FormMain.";
                }
            }
            catch (OperationCanceledException)
            {
                // Operador fechou o modal durante a espera. Fila continua drenando
                // em background no FormMain — nada a fazer aqui.
            }
            finally
            {
                if (!this.IsDisposed)
                    ReabilitarUIAposDrenagem();
                ctsDrenagem?.Dispose();
                ctsDrenagem = null;
            }
        }

        /// <summary>
        /// Faz polling do contador da fila do FormMain a cada 200ms ate zerar
        /// (sucesso) ou atingir o timeout (falha). Atualiza lblStatus em tempo
        /// real para o operador ver o progresso.
        /// </summary>
        private async Task<bool> AguardarFilaDrenarAsync(int totalEnfileirado, int timeoutMs, CancellationToken ct)
        {
            const int pollMs = 200;
            int waited = 0;

            while (waited < timeoutMs)
            {
                ct.ThrowIfCancellationRequested();

                int restantes = SafeGetFilaCount();
                if (restantes == 0)
                    return true;

                int processados = Math.Max(0, totalEnfileirado - restantes);
                lblStatus.ForeColor = Color.FromArgb(243, 156, 18);
                lblStatus.Text = "Aguardando confirmacao do banco... " +
                                 processados + "/" + totalEnfileirado + " gravados, " +
                                 restantes + " na fila.";

                try { await Task.Delay(pollMs, ct); }
                catch (TaskCanceledException) { throw new OperationCanceledException(ct); }
                waited += pollMs;
            }

            return SafeGetFilaCount() == 0;
        }

        private int SafeGetFilaCount()
        {
            try { return getFilaCount?.Invoke() ?? 0; }
            catch { return 0; }
        }

        private void DesabilitarUIDuranteDrenagem()
        {
            btnAlocar.Enabled = false;
            btnMarcarTodos.Enabled = false;
            btnDesmarcarTodos.Enabled = false;
            txtAlocacao.Enabled = false;
            txtBuscaPrefixo.Enabled = false;
            txtBuscaContem.Enabled = false;
            dgvProdutos.Enabled = false;
            // btnCancelar fica HABILITADO — operador pode fechar o modal a qualquer
            // momento; a fila continua drenando em background no FormMain.
        }

        private void ReabilitarUIAposDrenagem()
        {
            btnMarcarTodos.Enabled = true;
            btnDesmarcarTodos.Enabled = true;
            txtAlocacao.Enabled = true;
            txtBuscaPrefixo.Enabled = true;
            txtBuscaContem.Enabled = true;
            dgvProdutos.Enabled = true;
            AtualizarBotaoAlocar(); // re-avalia se botao Alocar deve ficar enabled
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
