using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AlocadorDeProdutos
{
    public partial class FormMain : Form
    {
        [DllImport("user32.dll")]
        private static extern bool MessageBeep(uint uType);

        private const uint MB_ICONHAND = 0x00000010;

        private static readonly string configPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "db_config.txt");

        private static readonly string filaPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "fila_pendente.txt");

        private string dbHost;
        private int filial = FilialConfig.Default;
        private NpgsqlConnection conn;         // conexão da UI (SELECT)
        private NpgsqlConnection connFila;     // conexão da fila (UPDATE)
        private System.Windows.Forms.Timer timerHealthCheck;
        private bool ultimoStatusConectado = true;
        private int comandoTimeout = 5;

        // ===== FILA DE OPERACOES =====
        private readonly ConcurrentQueue<AlocacaoPendente> filaOperacoes = new ConcurrentQueue<AlocacaoPendente>();
        private Thread threadFila;
        private readonly AutoResetEvent sinalFila = new AutoResetEvent(false);
        private volatile bool appFechando = false;

        // Lock que protege TODA escrita/leitura de fila_pendente.txt.
        // Tomado pela UI (EnfileirarAlocacao) e pelo worker (apos UPDATE bem-sucedido).
        private readonly object filaLock = new object();

        // Debounce para nMatricula_TextChanged: cada keystroke restarta o timer;
        // a query so dispara apos 300ms de inatividade. Reduz queries ~10x.
        private System.Windows.Forms.Timer timerDebounceMatricula;

        private struct AlocacaoPendente
        {
            public int Matricula;
            public string Alocacao;
            public string Descricao;
            public int Filial;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        // ===== INFRAESTRUTURA DE CONEXAO =====

        private static string LerHost()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string host = File.ReadAllText(configPath).Trim();
                    if (!string.IsNullOrEmpty(host))
                        return host;
                }
            }
            catch { }

            File.WriteAllText(configPath, "127.0.0.1");
            return "127.0.0.1";
        }

        /// <summary>
        /// Monta a connection string final delegando para <see cref="DbConfig"/>.
        /// O template (Username, Database, SSL Mode etc.) vem de App.config;
        /// apenas o Host e injetado em runtime. Senha tambem fica em App.config —
        /// se vazia, a abertura de conexao falhara e o usuario sera direcionado
        /// para a engrenagem.
        /// </summary>
        private string MontarConnString(string host)
        {
            return DbConfig.BuildConnString(host);
        }

        private bool Conectar()
        {
            try
            {
                FecharConexao(ref conn);
                conn = new NpgsqlConnection(MontarConnString(dbHost));
                conn.Open();
                return true;
            }
            catch
            {
                conn = null;
                return false;
            }
        }

        private bool ConectarFila()
        {
            try
            {
                FecharConexao(ref connFila);
                connFila = new NpgsqlConnection(MontarConnString(dbHost));
                connFila.Open();
                return true;
            }
            catch
            {
                connFila = null;
                return false;
            }
        }

        private void FecharConexao(ref NpgsqlConnection c)
        {
            if (c != null)
            {
                try { c.Close(); } catch { }
                try { c.Dispose(); } catch { }
                c = null;
            }
        }

        private bool ConexaoAtiva()
        {
            return ConexaoAtiva(conn);
        }

        private bool ConexaoAtiva(NpgsqlConnection c)
        {
            try
            {
                if (c == null || c.State != System.Data.ConnectionState.Open)
                    return false;

                using (var cmd = new NpgsqlCommand("SELECT 1", c))
                {
                    cmd.CommandTimeout = 3;
                    cmd.ExecuteScalar();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GarantirConexao()
        {
            if (ConexaoAtiva())
                return true;

            bool ok = Conectar();
            AtualizarStatusConexao(ok);

            if (!ok)
                AlertarDesconexao();

            return ok;
        }

        private void AlertarDesconexao()
        {
            MessageBeep(MB_ICONHAND);
            pDescricao.ForeColor = Color.FromArgb(231, 76, 60);
            string motivo = DbConfig.SenhaVazia()
                ? "SENHA NAO CONFIGURADA! Abra a engrenagem para cadastrar."
                : "SEM CONEXAO COM O BANCO!";
            pDescricao.Text = motivo;
            pReferencia.Text = "Verifique a rede ou ajuste a configuracao.";
            pMarca.Text = string.Empty;
            pAlocacaoAtual.Text = string.Empty;
        }

        private void AtualizarStatusConexao(bool conectado)
        {
            if (conectado)
            {
                lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                lblStatus.Text = "● Conectado (" + dbHost + " | F" + filial + ")";
            }
            else
            {
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = "● Desconectado (" + dbHost + " | F" + filial + ")";
            }
        }

        private NpgsqlCommand CriarComando(string sql)
        {
            var cmd = new NpgsqlCommand(sql, conn);
            cmd.CommandTimeout = comandoTimeout;
            return cmd;
        }

        // ===== FILA EM BACKGROUND =====

        private void IniciarFila()
        {
            CarregarFilaPendente();

            threadFila = new Thread(ProcessarFila);
            threadFila.IsBackground = true;
            threadFila.Name = "FilaAlocacao";
            threadFila.Start();
        }

        private void ProcessarFila()
        {
            while (!appFechando)
            {
                sinalFila.WaitOne(2000);

                // Peek-then-dequeue: o item permanece na fila ate o UPDATE confirmar.
                // Se o processo morrer durante o UPDATE, na proxima execucao o item
                // ainda estara em fila_pendente.txt e o UPDATE rodara de novo
                // (e idempotente: UPDATE estsal SET local=X WHERE matricula=Y AND filial=Z
                // produz o mesmo resultado independente de quantas vezes for executado).
                while (filaOperacoes.TryPeek(out var op) && !appFechando)
                {
                    AtualizarUIFila();

                    bool processado = false;
                    bool removerDaFila = false;
                    int tentativas = 0;

                    while (!processado && tentativas < 5 && !appFechando)
                    {
                        tentativas++;

                        try
                        {
                            if (!ConexaoAtiva(connFila))
                                ConectarFila();

                            if (connFila == null || connFila.State != System.Data.ConnectionState.Open)
                            {
                                // WaitOne em vez de Sleep: acorda imediatamente em
                                // appFechando + sinalFila.Set() do FormClosing.
                                sinalFila.WaitOne(2000 * tentativas);
                                if (appFechando) return;
                                continue;
                            }

                            using (var cmd = new NpgsqlCommand(
                                "UPDATE estsal SET local = @local WHERE matricula = @matricula AND filial = @filial", connFila))
                            {
                                cmd.CommandTimeout = 10;
                                cmd.Parameters.AddWithValue("local", op.Alocacao);
                                cmd.Parameters.AddWithValue("matricula", op.Matricula);
                                cmd.Parameters.AddWithValue("filial", (short)op.Filial);

                                int rows = cmd.ExecuteNonQuery();
                                processado = true;
                                removerDaFila = true;

                                if (rows == 0)
                                {
                                    // Matricula/filial sem linha em estsal — descarta da fila
                                    // (tentar de novo nao adianta).
                                    var capt = op;
                                    MostrarNaUI(() =>
                                    {
                                        MessageBeep(MB_ICONHAND);
                                        pDescricao.ForeColor = Color.FromArgb(231, 76, 60);
                                        pDescricao.Text = "ERRO FILA: Mat. " + capt.Matricula +
                                                          " sem estoque na filial " + capt.Filial + "!";
                                    });
                                }
                                else
                                {
                                    var capt = op;
                                    MostrarNaUI(() =>
                                    {
                                        pDescricao.ForeColor = Color.FromArgb(39, 174, 96);
                                        pDescricao.Text = "ALOCADO! " + capt.Descricao;
                                    });
                                }
                            }
                        }
                        catch
                        {
                            if (tentativas < 5)
                            {
                                // WaitOne em vez de Sleep — cancelavel via sinalFila.Set().
                                sinalFila.WaitOne(2000 * tentativas);
                                if (appFechando) return;
                            }
                        }
                    }

                    if (processado && removerDaFila)
                    {
                        // So agora removemos da fila em memoria e do arquivo,
                        // depois que o UPDATE foi confirmado pelo banco.
                        filaOperacoes.TryDequeue(out _);
                        SalvarFilaPendente();
                        AtualizarUIFila();
                    }
                    else if (!appFechando)
                    {
                        // 5 tentativas falharam: deixa na fila (nao remove),
                        // sinaliza erro e aguarda antes da proxima rodada.
                        MostrarNaUI(() =>
                        {
                            MessageBeep(MB_ICONHAND);
                            pDescricao.ForeColor = Color.FromArgb(231, 76, 60);
                            pDescricao.Text = "FILA: falha de gravacao. Tentando novamente...";
                            AtualizarStatusConexao(false);
                            ultimoStatusConectado = false;
                        });

                        // WaitOne para que FormClosing acorde imediatamente em vez
                        // de esperar 5s perdidos.
                        sinalFila.WaitOne(5000);
                        if (appFechando) return;
                        break; // sai do loop interno; o externo re-entra com TryPeek
                    }
                }
            }
        }

        /// <summary>
        /// Enfileira UMA alocacao de forma atomica: Enqueue + write-ahead persist.
        /// Mantem o lock durante a sequencia (Enqueue + save) — fundamental para
        /// que o worker nao salve um snapshot intermediario que mistura estados.
        /// O sinalFila.Set() fica fora do lock (deadlock-safe).
        /// </summary>
        private void EnfileirarAlocacao(int matricula, string alocacao, string descricao)
        {
            lock (filaLock)
            {
                filaOperacoes.Enqueue(new AlocacaoPendente
                {
                    Matricula = matricula,
                    Alocacao = alocacao,
                    Descricao = descricao,
                    Filial = filial
                });
                SalvarFilaPendenteUnsafe();
            }

            sinalFila.Set();
            AtualizarContadorFila();
        }

        /// <summary>
        /// Enfileira um lote de alocacoes de forma atomica: todos os Enqueue +
        /// uma unica gravacao em disco, tudo dentro do mesmo lock. Usado pelo
        /// FormAlocacaoMassa — evita N writes e impede que o worker drene
        /// itens parcialmente enfileirados.
        /// </summary>
        public void EnfileirarLote(System.Collections.Generic.IEnumerable<System.Tuple<int, string, string>> itens)
        {
            if (itens == null) return;

            int adicionados = 0;
            lock (filaLock)
            {
                foreach (var it in itens)
                {
                    if (it == null) continue;
                    filaOperacoes.Enqueue(new AlocacaoPendente
                    {
                        Matricula = it.Item1,
                        Alocacao = it.Item2,
                        Descricao = it.Item3 ?? string.Empty,
                        Filial = filial
                    });
                    adicionados++;
                }

                if (adicionados > 0)
                    SalvarFilaPendenteUnsafe();
            }

            if (adicionados == 0) return;

            sinalFila.Set();
            AtualizarContadorFila();
        }

        /// <summary>
        /// Numero de itens pendentes na fila write-ahead. Thread-safe via
        /// <see cref="System.Collections.Concurrent.ConcurrentQueue{T}.Count"/>.
        /// Usado pelo FormAlocacaoMassa para fazer poll e aguardar drenagem
        /// antes de re-buscar no banco.
        /// </summary>
        public int FilaCount => filaOperacoes.Count;

        private void AtualizarContadorFila()
        {
            int count = filaOperacoes.Count;
            if (count > 0)
            {
                lblFila.ForeColor = Color.FromArgb(243, 156, 18);
                lblFila.Text = "▶ Fila: " + count + " pendente" + (count > 1 ? "s" : "");
                lblFila.Visible = true;
            }
            else
            {
                lblFila.Visible = false;
            }
        }

        private void AtualizarUIFila()
        {
            if (appFechando) return;
            if (this.InvokeRequired)
            {
                try
                {
                    if (!this.IsDisposed && this.IsHandleCreated)
                        this.BeginInvoke((Action)AtualizarContadorFila);
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
            else
                AtualizarContadorFila();
        }

        /// <summary>
        /// Marshalls uma acao para a UI thread de forma SEGURA contra deadlock.
        /// - Aborta se app esta fechando (UI thread esta em FormClosing.Join,
        ///   nao processara nosso Invoke ate timeout = 15s de espera inutil).
        /// - Usa BeginInvoke (assincrono) para nao bloquear a thread chamadora.
        /// - Engole exceptions de race entre check de IsDisposed e Invoke.
        /// </summary>
        private void MostrarNaUI(Action acao)
        {
            if (appFechando) return;
            try
            {
                if (!this.IsDisposed && this.IsHandleCreated)
                    this.BeginInvoke(acao);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// Grava o estado atual da fila no disco de forma atomica.
        /// Wrapper publico que adquire o filaLock antes de chamar a versao Unsafe.
        /// Use por chamadores que NAO ja detem o lock (ex.: worker em ProcessarFila).
        /// </summary>
        private void SalvarFilaPendente()
        {
            lock (filaLock)
            {
                SalvarFilaPendenteUnsafe();
            }
        }

        /// <summary>
        /// Implementacao real do save da fila. NAO adquire lock — o caller deve
        /// estar dentro de <see cref="filaLock"/>. Usado por EnfileirarAlocacao
        /// e EnfileirarLote para manter atomicidade Enqueue+Save sob o mesmo lock.
        /// File.Move com overwrite:true e atomico em NTFS — sem janela onde
        /// o arquivo nao existe.
        /// Formato: <c>matricula|alocacao|filial|descricao</c> por linha.
        /// </summary>
        private void SalvarFilaPendenteUnsafe()
        {
            try
            {
                var snapshot = filaOperacoes.ToArray();

                if (snapshot.Length == 0)
                {
                    if (File.Exists(filaPath))
                        File.Delete(filaPath);
                    return;
                }

                var linhas = new List<string>(snapshot.Length);
                foreach (var op in snapshot)
                {
                    // Sanitiza o pipe-separator caso descricao tenha '|'
                    string desc = op.Descricao != null
                        ? op.Descricao.Replace('|', '/')
                        : string.Empty;
                    linhas.Add(op.Matricula + "|" + op.Alocacao + "|" + op.Filial + "|" + desc);
                }

                string tempPath = filaPath + ".tmp";
                File.WriteAllLines(tempPath, linhas);

                // overwrite:true e atomico em NTFS: sem janela onde filaPath nao existe.
                File.Move(tempPath, filaPath, overwrite: true);
            }
            catch { /* best-effort: nao queremos derrubar a UI por falha de IO */ }
        }

        /// <summary>
        /// Le fila_pendente.txt no startup e re-injeta operacoes na fila em memoria.
        /// NAO deleta o arquivo: o worker mantem o arquivo sincronizado a cada UPDATE.
        /// Suporta formato novo (4 partes: matricula|alocacao|filial|descricao) e
        /// formato legado (3 partes, sem filial — nesse caso usa a filial atual).
        /// </summary>
        private void CarregarFilaPendente()
        {
            lock (filaLock)
            {
                if (!File.Exists(filaPath))
                    return;

                try
                {
                    var linhas = File.ReadAllLines(filaPath);
                    int carregados = 0;
                    foreach (var linha in linhas)
                    {
                        if (string.IsNullOrWhiteSpace(linha)) continue;

                        var partes = linha.Split('|');
                        if (partes.Length < 2) continue;
                        if (!int.TryParse(partes[0], out int mat)) continue;

                        string alocacao = partes[1];
                        int filialOp;
                        string descricao;

                        if (partes.Length >= 4)
                        {
                            // formato novo
                            if (!int.TryParse(partes[2], out filialOp))
                                filialOp = filial;
                            descricao = partes[3];
                        }
                        else
                        {
                            // formato legado (3 partes ou menos): usa filial atual
                            filialOp = filial;
                            descricao = partes.Length > 2 ? partes[2] : string.Empty;
                        }

                        filaOperacoes.Enqueue(new AlocacaoPendente
                        {
                            Matricula = mat,
                            Alocacao = alocacao,
                            Descricao = descricao,
                            Filial = filialOp
                        });
                        carregados++;
                    }

                    if (carregados > 0)
                    {
                        sinalFila.Set();
                        pDescricao.ForeColor = Color.FromArgb(243, 156, 18);
                        pDescricao.Text = carregados + " alocacao(oes) pendente(s) recuperada(s).";
                        AtualizarContadorFila();
                    }
                }
                catch { }
            }

            // Se o arquivo veio em formato legado, re-grava em formato novo
            // (sem ficar dentro do lock para nao re-entrar).
            if (filaOperacoes.Count > 0)
                SalvarFilaPendente();
        }

        // ===== TIMER DE HEALTH CHECK =====

        private void PausarHealthCheck()
        {
            if (timerHealthCheck != null)
                timerHealthCheck.Stop();
        }

        private void RetomarHealthCheck()
        {
            if (timerHealthCheck != null)
                timerHealthCheck.Start();
        }

        private void IniciarHealthCheck()
        {
            timerHealthCheck = new System.Windows.Forms.Timer();
            timerHealthCheck.Interval = 10000;
            timerHealthCheck.Tick += TimerHealthCheck_Tick;
            timerHealthCheck.Start();
        }

        private void TimerHealthCheck_Tick(object sender, EventArgs e)
        {
            bool conectado = ConexaoAtiva();

            if (!conectado && ultimoStatusConectado)
            {
                conectado = Conectar();

                if (!conectado)
                {
                    MessageBeep(MB_ICONHAND);
                    pDescricao.ForeColor = Color.FromArgb(231, 76, 60);
                    pDescricao.Text = "CONEXAO PERDIDA! Tentando reconectar...";
                }
            }
            else if (!conectado)
            {
                conectado = Conectar();

                if (conectado)
                {
                    pDescricao.ForeColor = Color.FromArgb(39, 174, 96);
                    pDescricao.Text = "Conexao restabelecida!";
                    sinalFila.Set(); // reprocessar fila pendente
                }
            }

            AtualizarStatusConexao(conectado);
            ultimoStatusConectado = conectado;
        }

        // ===== EVENTOS DO FORMULARIO =====

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Detecta primeira execucao (sem db_config.txt) — depois usado para
            // forcar abertura da engrenagem se conexao falhar.
            bool primeiraExecucao = !File.Exists(configPath);

            dbHost = LerHost();

            // Le filial diferenciando "nao existe" (OK, default) de "corrompido" (ERRO).
            bool filialOk = FilialConfig.TryLer(out filial);
            if (!filialOk)
            {
                MessageBox.Show(
                    "filial.txt corrompido ou inacessivel.\n\n" +
                    "Confirme a filial na engrenagem ANTES de bipar produtos — " +
                    "do contrario as alocacoes serao gravadas na filial padrao (1).",
                    "Filial invalida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            bool ok = Conectar();
            AtualizarStatusConexao(ok);
            ultimoStatusConectado = ok;

            bool senhaVazia = DbConfig.SenhaVazia();

            if (!ok)
            {
                string motivo = senhaVazia
                    ? "Senha do banco nao configurada.\nUse a engrenagem para cadastrar a senha."
                    : "Nao foi possivel conectar ao banco em " + dbHost + ".\nUse a engrenagem para alterar o IP.";
                MessageBox.Show(motivo, "Conexao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                ConectarFila();
            }

            nMatricula.ReadOnly = true;
            btnSalvar.Enabled = false;

            IniciarFila();
            IniciarHealthCheck();

            // Fluxo de primeira execucao OU senha vazia OU filial corrompida:
            // abre engrenagem direto para o operador resolver antes de operar.
            // Health-check ja esta rodando, sera pausado pelo btnConfig_Click.
            if (!ok && (primeiraExecucao || senhaVazia || !filialOk))
            {
                this.BeginInvoke((Action)(() => btnConfig_Click(this, EventArgs.Empty)));
            }
            else
            {
                txtBipe.Focus();
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            PausarHealthCheck();
            using (var formConfig = new FormConfig(dbHost, conn, filial))
            {
                if (formConfig.ShowDialog(this) == DialogResult.OK)
                {
                    dbHost = formConfig.NovoHost;
                    filial = formConfig.NovaFilial;
                    File.WriteAllText(configPath, dbHost);
                    FilialConfig.Salvar(filial);

                    bool ok = Conectar();
                    AtualizarStatusConexao(ok);
                    ultimoStatusConectado = ok;

                    if (ok)
                    {
                        ConectarFila();
                        sinalFila.Set();
                        pDescricao.ForeColor = Color.FromArgb(39, 174, 96);
                        pDescricao.Text = "Conectado em " + dbHost + " (filial " + filial + ")";
                    }
                    else
                    {
                        pDescricao.ForeColor = Color.FromArgb(231, 76, 60);
                        pDescricao.Text = "Falha ao conectar em " + dbHost;
                    }
                }
            }
            RetomarHealthCheck();
            txtBipe.Focus();
        }

        private void btnAlocacaoMassa_Click(object sender, EventArgs e)
        {
            if (!GarantirConexao())
                return;

            PausarHealthCheck();
            try
            {
                using (var formMassa = new FormAlocacaoMassa(conn, filial, EnfileirarLote, () => FilaCount))
                {
                    formMassa.ShowDialog(this);
                }
            }
            finally
            {
                RetomarHealthCheck();
                txtBipe.Focus();
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nAlocacao.Text))
            {
                nAlocacao.Focus();
                return;
            }

            BtnOk.Text = "🔒";
            BtnOk.ForeColor = Color.Black;
            nAlocacao.ReadOnly = true;
            nMatricula.ReadOnly = false;
            btnSalvar.Enabled = true;
            nMatricula.Focus();

            BtnOk.Enabled = false;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Editar();
        }

        private void nMatricula_TextChanged(object sender, EventArgs e)
        {
            // Debounce: cada keystroke restarta o timer de 300ms; query so dispara
            // depois que o operador para de digitar. Cria preguicosamente.
            if (timerDebounceMatricula == null)
            {
                timerDebounceMatricula = new System.Windows.Forms.Timer();
                timerDebounceMatricula.Interval = 300;
                timerDebounceMatricula.Tick += TimerDebounceMatricula_Tick;
            }
            timerDebounceMatricula.Stop();
            timerDebounceMatricula.Start();
        }

        private void TimerDebounceMatricula_Tick(object sender, EventArgs e)
        {
            timerDebounceMatricula.Stop();
            BuscarProdutoPorMatricula();
        }

        private void BuscarProdutoPorMatricula()
        {
            if (string.IsNullOrWhiteSpace(nMatricula.Text))
                return;

            if (!int.TryParse(nMatricula.Text, out int matricula))
                return;

            if (!GarantirConexao())
                return;

            try
            {
                string sql = @"SELECT cadpro.descricao, cadpro.referencia, cadpro.marca, estsal.local
                               FROM cadpro
                               LEFT JOIN estsal ON cadpro.matricula = estsal.matricula
                                                AND estsal.filial = @filial
                               WHERE cadpro.matricula = @matricula;";
                string descricao = null;
                string referencia = null;
                string alocacaoAtual = null;
                int marca = 0;
                bool encontrado = false;

                using (var cmdProduto = CriarComando(sql))
                {
                    cmdProduto.Parameters.AddWithValue("matricula", matricula);
                    cmdProduto.Parameters.AddWithValue("filial", (short)filial);

                    using (var reader = cmdProduto.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            encontrado = true;
                            descricao = reader["descricao"].ToString();
                            referencia = reader["referencia"].ToString();
                            alocacaoAtual = reader["local"].ToString();
                            marca = Convert.ToInt32(reader["marca"]);
                        }
                    }
                }

                if (encontrado)
                {
                    pDescricao.Text = descricao;
                    pReferencia.Text = referencia;
                    pAlocacaoAtual.Text = alocacaoAtual;

                    using (var cmdMarca = CriarComando("SELECT descricao FROM formar WHERE marca = @marca"))
                    {
                        cmdMarca.Parameters.AddWithValue("marca", marca);

                        using (var marcaReader = cmdMarca.ExecuteReader())
                        {
                            if (marcaReader.Read())
                                pMarca.Text = marcaReader["descricao"].ToString();
                            else
                                pMarca.Text = "Marca nao encontrada";
                        }
                    }
                }
                else
                {
                    pDescricao.Text = "Matricula nao encontrada";
                    pReferencia.Text = string.Empty;
                    pMarca.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                TratarErroConexao(ex);
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nMatricula.Text))
            {
                MessageBox.Show("O campo de matricula esta vazio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nMatricula.Focus();
                return;
            }

            DialogResult result = MessageBox.Show("Deseja salvar a alocacao?", "Confirmacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;

            string alocacaoTexto = nAlocacao.Text;
            if (!int.TryParse(nMatricula.Text, out int matricula))
            {
                MessageBox.Show("Matricula invalida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Enfileirar ao inves de gravar direto
            EnfileirarAlocacao(matricula, alocacaoTexto, pDescricao.Text);

            pDescricao.ForeColor = Color.FromArgb(39, 174, 96);
            pDescricao.Text = "Na fila! Mat. " + matricula;

            nMatricula.Text = string.Empty;
            pAlocacaoAtual.Text = string.Empty;
            pReferencia.Text = string.Empty;
            pMarca.Text = string.Empty;
            nMatricula.Focus();
        }

        private void nAlocacao_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && nAlocacao.Text != string.Empty)
            {
                BtnOk_Click(sender, e);
                nMatricula.Focus();
            }

            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                nAlocacao.Focus();
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.E)
                btnEditar_Click(sender, e);

            if (e.Control && e.KeyCode == Keys.L)
                LimparTudo();
        }

        private void LimparTudo()
        {
            nAlocacao.Text = string.Empty;
            nMatricula.Text = string.Empty;
            pAlocacaoAtual.Text = string.Empty;
            pDescricao.Text = string.Empty;
            pReferencia.Text = string.Empty;
            pMarca.Text = string.Empty;

            nAlocacao.ReadOnly = false;
            nMatricula.ReadOnly = true;
            btnSalvar.Enabled = false;

            BtnOk.Text = "🔓";
            BtnOk.ForeColor = Color.Green;
            BtnOk.Enabled = true;
            pDescricao.ForeColor = Color.Black;

            txtBipe.Focus();
        }

        private void Editar()
        {
            nAlocacao.ReadOnly = false;
            nMatricula.ReadOnly = true;
            btnSalvar.Enabled = false;
            nAlocacao.Focus();
            pAlocacaoAtual.Text = string.Empty;

            BtnOk.Enabled = true;
            BtnOk.Text = "🔓";
            BtnOk.ForeColor = Color.Green;
            pDescricao.ForeColor = Color.Black;

            nMatricula.Text = string.Empty;
            pDescricao.Text = string.Empty;
            pReferencia.Text = string.Empty;
            pMarca.Text = string.Empty;

            nAlocacao.ReadOnly = false;
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparTudo();
        }

        private void nMatricula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSalvar_Click(sender, e);
            }
        }

        private void nMatricula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F4)
                return;

            e.SuppressKeyPress = true;

            if (nMatricula.ReadOnly || string.IsNullOrWhiteSpace(nAlocacao.Text))
                return;

            if (!GarantirConexao())
                return;

            PausarHealthCheck();
            using (var formBusca = new FormBuscaProduto(conn))
            {
                if (formBusca.ShowDialog(this) == DialogResult.OK && formBusca.Selecionado)
                {
                    nMatricula.Text = formBusca.MatriculaSelecionada.ToString();
                }
            }
            RetomarHealthCheck();
            nMatricula.Focus();
        }

        // ===== LOGICA DO BIPE =====

        private void txtBipe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;
            string input = txtBipe.Text.Trim();
            txtBipe.Clear();

            if (string.IsNullOrEmpty(input))
                return;

            // Se começa com letra → é alocação
            if (char.IsLetter(input[0]))
            {
                if (input.Length > 10)
                {
                    pDescricao.ForeColor = Color.Red;
                    pDescricao.Text = "Alocacao excede 10 caracteres!";
                    txtBipe.Focus();
                    return;
                }

                nAlocacao.Text = input.ToUpper();
                nAlocacao.ReadOnly = true;
                nMatricula.ReadOnly = false;
                btnSalvar.Enabled = true;

                BtnOk.Text = "🔒";
                BtnOk.ForeColor = Color.Black;
                BtnOk.Enabled = false;

                pDescricao.Text = string.Empty;
                pReferencia.Text = string.Empty;
                pMarca.Text = string.Empty;
                pAlocacaoAtual.Text = string.Empty;
                nMatricula.Text = string.Empty;

                pDescricao.ForeColor = Color.Green;
                pDescricao.Text = "Alocacao definida: " + input.ToUpper();

                txtBipe.Focus();
                return;
            }

            // Validar GTIN/EAN
            if (!ValidarEAN(input))
            {
                MessageBeep(MB_ICONHAND);
                pDescricao.ForeColor = Color.Red;
                pDescricao.Text = "EAN/GTIN invalido: " + input;
                txtBipe.Focus();
                return;
            }

            // Verificar se alocação já foi definida
            if (string.IsNullOrWhiteSpace(nAlocacao.Text))
            {
                pDescricao.ForeColor = Color.Red;
                pDescricao.Text = "Bipe uma alocacao primeiro!";
                txtBipe.Focus();
                return;
            }

            // Verificar conexão antes de consultar
            if (!GarantirConexao())
            {
                txtBipe.Focus();
                return;
            }

            try
            {
                string sql = @"SELECT cadpro.matricula, cadpro.descricao, cadpro.referencia, cadpro.marca, estsal.local
                               FROM cadpro
                               JOIN estsal ON cadpro.matricula = estsal.matricula
                                           AND estsal.filial = @filial
                               WHERE cadpro.codigo_barra = @codigo_barra";

                int matriculaEncontrada = 0;
                string descricao = string.Empty;
                string referencia = string.Empty;
                string alocacaoAtual = string.Empty;
                int marca = 0;
                bool encontrado = false;

                using (var cmdBusca = CriarComando(sql))
                {
                    cmdBusca.Parameters.AddWithValue("codigo_barra", input);
                    cmdBusca.Parameters.AddWithValue("filial", (short)filial);

                    using (var reader = cmdBusca.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            encontrado = true;
                            matriculaEncontrada = Convert.ToInt32(reader["matricula"]);
                            descricao = reader["descricao"].ToString();
                            referencia = reader["referencia"].ToString();
                            alocacaoAtual = reader["local"].ToString();
                            marca = Convert.ToInt32(reader["marca"]);
                        }
                    }
                }

                if (!encontrado)
                {
                    MessageBeep(MB_ICONHAND);

                    PausarHealthCheck();
                    try
                    {
                        using (var formCadastro = new FormCadastroBarras(conn, input))
                        {
                            formCadastro.ShowDialog(this);

                            if (!formCadastro.Cadastrado)
                            {
                                pDescricao.ForeColor = Color.Red;
                                pDescricao.Text = "Cadastro cancelado.";
                                pReferencia.Text = string.Empty;
                                pMarca.Text = string.Empty;
                                pAlocacaoAtual.Text = string.Empty;
                                nMatricula.Text = string.Empty;
                                txtBipe.Focus();
                                return;
                            }

                            int matCadastrada = formCadastro.MatriculaCadastrada;
                            string sqlRebusca = @"SELECT cadpro.matricula, cadpro.descricao, cadpro.referencia, cadpro.marca, estsal.local
                                                  FROM cadpro
                                                  JOIN estsal ON cadpro.matricula = estsal.matricula
                                                              AND estsal.filial = @filial
                                                  WHERE cadpro.matricula = @matricula";

                            using (var cmdRebusca = CriarComando(sqlRebusca))
                            {
                                cmdRebusca.Parameters.AddWithValue("matricula", matCadastrada);
                                cmdRebusca.Parameters.AddWithValue("filial", (short)filial);
                                using (var readerRebusca = cmdRebusca.ExecuteReader())
                                {
                                    if (readerRebusca.Read())
                                    {
                                        matriculaEncontrada = Convert.ToInt32(readerRebusca["matricula"]);
                                        descricao = readerRebusca["descricao"].ToString();
                                        referencia = readerRebusca["referencia"].ToString();
                                        alocacaoAtual = readerRebusca["local"].ToString();
                                        marca = Convert.ToInt32(readerRebusca["marca"]);
                                        encontrado = true;
                                    }
                                }
                            }

                            if (!encontrado)
                            {
                                pDescricao.ForeColor = Color.Red;
                                pDescricao.Text = "Erro: matricula sem estoque na filial " + filial + ".";
                                pReferencia.Text = string.Empty;
                                pMarca.Text = string.Empty;
                                pAlocacaoAtual.Text = string.Empty;
                                nMatricula.Text = string.Empty;
                                txtBipe.Focus();
                                return;
                            }
                        }
                    }
                    finally
                    {
                        RetomarHealthCheck();
                    }
                }

                // Exibir dados do produto
                nMatricula.Text = matriculaEncontrada.ToString();
                pDescricao.ForeColor = Color.Black;
                pDescricao.Text = descricao;
                pReferencia.Text = referencia;
                pAlocacaoAtual.Text = alocacaoAtual;

                // Buscar nome da marca
                using (var cmdMarca = CriarComando("SELECT descricao FROM formar WHERE marca = @marca"))
                {
                    cmdMarca.Parameters.AddWithValue("marca", marca);
                    using (var marcaReader = cmdMarca.ExecuteReader())
                    {
                        if (marcaReader.Read())
                            pMarca.Text = marcaReader["descricao"].ToString();
                        else
                            pMarca.Text = "Marca nao encontrada";
                    }
                }

                // Enfileirar gravação (nao bloqueia a UI)
                EnfileirarAlocacao(matriculaEncontrada, nAlocacao.Text, descricao);

                pDescricao.ForeColor = Color.FromArgb(243, 156, 18);
                pDescricao.Text = "NA FILA! " + descricao;
            }
            catch (Exception ex)
            {
                TratarErroConexao(ex);
            }

            txtBipe.Focus();
        }

        // ===== TRATAMENTO DE ERROS =====

        private void TratarErroConexao(Exception ex)
        {
            bool isConexao = ex is NpgsqlException ||
                             ex is System.Net.Sockets.SocketException ||
                             ex is TimeoutException ||
                             (ex.InnerException != null && (
                                 ex.InnerException is System.Net.Sockets.SocketException ||
                                 ex.InnerException is TimeoutException));

            if (isConexao)
            {
                MessageBeep(MB_ICONHAND);
                pDescricao.ForeColor = Color.FromArgb(231, 76, 60);
                pDescricao.Text = "FALHA DE CONEXAO! Reconectando...";
                pReferencia.Text = string.Empty;
                pMarca.Text = string.Empty;
                pAlocacaoAtual.Text = string.Empty;

                bool ok = Conectar();
                AtualizarStatusConexao(ok);
                ultimoStatusConectado = ok;

                if (ok)
                {
                    pDescricao.ForeColor = Color.FromArgb(243, 156, 18);
                    pDescricao.Text = "Reconectado! Tente novamente.";
                }
                else
                {
                    pDescricao.Text = DbConfig.SenhaVazia()
                        ? "SENHA NAO CONFIGURADA! Abra a engrenagem."
                        : "SEM CONEXAO! Verifique a rede.";
                }
            }
            else
            {
                pDescricao.ForeColor = Color.FromArgb(231, 76, 60);
                pDescricao.Text = "Erro: " + ex.Message;
                pReferencia.Text = string.Empty;
                pMarca.Text = string.Empty;
                pAlocacaoAtual.Text = string.Empty;
            }
        }

        // ===== VALIDACAO =====

        private bool ValidarEAN(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return false;

            foreach (char c in codigo)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            int len = codigo.Length;
            if (len != 8 && len != 12 && len != 13 && len != 14)
                return false;

            int soma = 0;
            for (int i = 0; i < len - 1; i++)
            {
                int digito = codigo[i] - '0';
                bool posicaoImpar = (len - 1 - i) % 2 != 0;
                soma += posicaoImpar ? digito * 3 : digito;
            }

            int checkDigitCalculado = (10 - (soma % 10)) % 10;
            int checkDigitReal = codigo[len - 1] - '0';

            return checkDigitCalculado == checkDigitReal;
        }

        // ===== FECHAMENTO =====

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            appFechando = true;
            sinalFila.Set();

            if (timerHealthCheck != null)
            {
                timerHealthCheck.Stop();
                timerHealthCheck.Tick -= TimerHealthCheck_Tick;
                timerHealthCheck.Dispose();
                timerHealthCheck = null;
            }

            if (timerDebounceMatricula != null)
            {
                timerDebounceMatricula.Stop();
                timerDebounceMatricula.Tick -= TimerDebounceMatricula_Tick;
                timerDebounceMatricula.Dispose();
                timerDebounceMatricula = null;
            }

            // Esperar thread da fila finalizar para que UPDATEs em andamento
            // tenham chance de confirmar (e remover o item do arquivo).
            if (threadFila != null && threadFila.IsAlive)
                threadFila.Join(15000);

            // Garantia final: o arquivo ja deveria estar sincronizado pelo
            // padrao write-ahead, mas regravamos por seguranca.
            SalvarFilaPendente();

            FecharConexao(ref connFila);
            FecharConexao(ref conn);
        }
    }
}
