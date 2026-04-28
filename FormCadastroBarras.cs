using Npgsql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlocadorDeProdutos
{
    public partial class FormCadastroBarras : Form
    {
        private readonly NpgsqlConnection conn;
        private readonly string codigoBarras;

        public int MatriculaCadastrada { get; private set; }
        public bool Cadastrado { get; private set; }

        public FormCadastroBarras(NpgsqlConnection conn, string codigoBarras)
        {
            this.conn = conn;
            this.codigoBarras = codigoBarras;
            InitializeComponent();
            lblEAN.Text = codigoBarras;
        }

        private void txtMatricula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                Cadastrado = false;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            if (e.KeyCode == Keys.F4)
            {
                e.SuppressKeyPress = true;
                using (var formBusca = new FormBuscaProduto(conn))
                {
                    if (formBusca.ShowDialog(this) == DialogResult.OK && formBusca.Selecionado)
                    {
                        txtMatricula.Text = formBusca.MatriculaSelecionada.ToString();
                        lblStatus.ForeColor = Color.FromArgb(39, 174, 96);
                        lblStatus.Text = "Matricula " + formBusca.MatriculaSelecionada + " selecionada.";
                    }
                }
                txtMatricula.Focus();
                return;
            }

            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;
            string texto = txtMatricula.Text.Trim();

            if (string.IsNullOrEmpty(texto) || !int.TryParse(texto, out int matricula))
            {
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = "Digite uma matricula valida!";
                return;
            }

            try
            {
                // Verificar se a matrícula existe
                string sqlCheck = "SELECT cadpro.matricula, cadpro.descricao FROM cadpro WHERE cadpro.matricula = @matricula";
                bool existe = false;

                using (var cmd = new NpgsqlCommand(sqlCheck, conn))
                {
                    cmd.CommandTimeout = 5;
                    cmd.Parameters.AddWithValue("matricula", matricula);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            existe = true;
                        }
                    }
                }

                if (!existe)
                {
                    lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                    lblStatus.Text = "Matricula " + matricula + " nao encontrada!";
                    txtMatricula.SelectAll();
                    return;
                }

                // Registrar o código de barras
                string sqlUpdate = "UPDATE cadpro SET codigo_barra = @codigo_barra WHERE matricula = @matricula";
                using (var cmd = new NpgsqlCommand(sqlUpdate, conn))
                {
                    cmd.CommandTimeout = 5;
                    cmd.Parameters.AddWithValue("codigo_barra", codigoBarras);
                    cmd.Parameters.AddWithValue("matricula", matricula);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                        lblStatus.Text = "Falha ao gravar! Matricula pode ter sido removida.";
                        return;
                    }
                }

                MatriculaCadastrada = matricula;
                Cadastrado = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                bool isConexao = ex is NpgsqlException || ex is System.Net.Sockets.SocketException ||
                                 ex is TimeoutException;
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = isConexao ? "FALHA DE CONEXAO! Verifique a rede." : "Erro: " + ex.Message;
            }
        }

        private void txtMatricula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
