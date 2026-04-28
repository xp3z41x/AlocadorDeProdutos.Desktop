using Npgsql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlocadorDeProdutos
{
    public partial class FormConfig : Form
    {
        private readonly string hostAtual;
        private readonly NpgsqlConnection connAtual;

        public string NovoHost { get; private set; }
        public int NovaFilial { get; private set; }

        public FormConfig(string hostAtual, NpgsqlConnection connAtual, int filialAtual)
        {
            this.hostAtual = hostAtual;
            this.connAtual = connAtual;
            InitializeComponent();

            txtHost.Text = hostAtual;
            numFilial.Value = Math.Max(FilialConfig.Min, Math.Min(FilialConfig.Max, filialAtual));

            // txtSenha NAO e pre-preenchido por seguranca: nao exibimos a senha
            // existente. Vazio = manter; preenchido = sobrescrever no App.config.
            txtSenha.Text = string.Empty;
            AplicarPlaceholderSenha();

            // Re-mascarar automaticamente quando o operador sai do campo de senha
            // (mesmo que tenha clicado em "Mostrar"). Mitiga shoulder-surfing
            // se ele se afastar com a senha visivel.
            txtSenha.Leave += txtSenha_Leave;

            TestarConexao(hostAtual);
        }

        private void txtSenha_Leave(object sender, EventArgs e)
        {
            if (chkMostrarSenha.Checked)
                chkMostrarSenha.Checked = false; // dispara CheckedChanged → re-mascara
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { txtSenha.Leave -= txtSenha_Leave; } catch { }
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Atualiza o placeholder do txtSenha indicando se ha senha configurada.
        /// PlaceholderText e nativo em .NET 5+ (estamos em .NET 10).
        /// </summary>
        private void AplicarPlaceholderSenha()
        {
            txtSenha.PlaceholderText = DbConfig.SenhaVazia()
                ? "(senha nao configurada — preencha aqui)"
                : "(deixe em branco para manter a atual)";
        }

        private void TestarConexao(string host)
        {
            lblStatusConexao.ForeColor = Color.FromArgb(243, 156, 18);
            lblStatusConexao.Text = "● Testando conexao...";
            this.Refresh();

            try
            {
                string connStr = DbConfig.BuildConnString(host);
                using (var conn = new NpgsqlConnection(connStr))
                {
                    conn.Open();
                    conn.Close();
                }

                lblStatusConexao.ForeColor = Color.FromArgb(39, 174, 96);
                lblStatusConexao.Text = "● Conectado com sucesso em " + host;
            }
            catch (InvalidOperationException ex)
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusConexao.Text = "● Configuracao invalida: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                string msg = DbConfig.SenhaVazia()
                    ? "Senha nao configurada — preencha o campo SENHA DO BANCO e clique TESTAR."
                    : ex.Message;
                lblStatusConexao.Text = "● Falha: " + msg;
            }
        }

        /// <summary>
        /// Persiste a senha no App.config se o campo estiver preenchido.
        /// Retorna true se conseguiu (ou se nao precisava); false em caso de erro
        /// (e ja escreveu mensagem em lblStatusConexao).
        /// </summary>
        private bool TentarSalvarSenha()
        {
            string novaSenha = txtSenha.Text;
            if (string.IsNullOrEmpty(novaSenha))
                return true; // nao alterar

            try
            {
                DbConfig.SalvarSenha(novaSenha);
                txtSenha.Text = string.Empty; // limpa apos salvar
                AplicarPlaceholderSenha();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusConexao.Text = "● Permissao negada — execute como Administrador OU instale fora de Program Files.";
                return false;
            }
            catch (System.Configuration.ConfigurationErrorsException ex)
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusConexao.Text = "● App.config invalido: " + ex.Message;
                return false;
            }
            catch (System.IO.IOException ex)
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusConexao.Text = "● Falha de I/O ao gravar App.config: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusConexao.Text = "● Falha ao gravar senha: " + ex.Message;
                return false;
            }
        }

        private void btnTestar_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            if (string.IsNullOrEmpty(host))
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusConexao.Text = "Informe um IP valido!";
                return;
            }

            // Se o operador digitou senha nova, persiste antes de testar
            // (caso contrario o teste continuaria usando a senha antiga).
            if (!TentarSalvarSenha())
                return;

            TestarConexao(host);
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            if (string.IsNullOrEmpty(host))
            {
                lblStatusConexao.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusConexao.Text = "Informe um IP valido!";
                return;
            }

            if (!TentarSalvarSenha())
                return;

            NovoHost = host;
            NovaFilial = (int)numFilial.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtHost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnTestar_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.PasswordChar = chkMostrarSenha.Checked ? '\0' : '●';
        }
    }
}
