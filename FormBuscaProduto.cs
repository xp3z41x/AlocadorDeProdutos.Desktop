using Npgsql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlocadorDeProdutos
{
    public partial class FormBuscaProduto : Form
    {
        private readonly NpgsqlConnection conn;

        // Fontes em campos para dispose explicito (evita leak de GDI handles).
        private Font fontGridCell;
        private Font fontGridHeader;

        public int MatriculaSelecionada { get; private set; }
        public bool Selecionado { get; private set; }

        public FormBuscaProduto(NpgsqlConnection conn)
        {
            this.conn = conn;
            InitializeComponent();
            EstilizarGrid();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                fontGridCell?.Dispose();
                fontGridHeader?.Dispose();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EstilizarGrid()
        {
            fontGridCell = new Font("Segoe UI", 10F);
            fontGridHeader = new Font("Segoe UI", 10F, FontStyle.Bold);

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
        }

        private void Buscar()
        {
            string descricao = txtDescricao.Text.Trim();
            string referencia = txtReferencia.Text.Trim();

            if (string.IsNullOrEmpty(descricao) && string.IsNullOrEmpty(referencia))
            {
                dgvProdutos.Rows.Clear();
                lblSelecionado.Text = string.Empty;
                return;
            }

            try
            {
                string sql = @"SELECT cadpro.descricao, cadpro.referencia, cadpro.matricula, formar.descricao AS marca
                               FROM cadpro
                               LEFT JOIN formar ON cadpro.marca = formar.marca
                               WHERE 1=1";

                if (!string.IsNullOrEmpty(descricao))
                    sql += " AND cadpro.descricao ILIKE @descricao";

                if (!string.IsNullOrEmpty(referencia))
                    sql += " AND cadpro.referencia ILIKE @referencia";

                sql += " ORDER BY cadpro.descricao LIMIT 200";

                dgvProdutos.Rows.Clear();

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 5;

                    if (!string.IsNullOrEmpty(descricao))
                        cmd.Parameters.AddWithValue("descricao", descricao + "%");

                    if (!string.IsNullOrEmpty(referencia))
                        cmd.Parameters.AddWithValue("referencia", referencia + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dgvProdutos.Rows.Add(
                                reader["descricao"].ToString(),
                                reader["referencia"].ToString(),
                                reader["matricula"].ToString(),
                                reader["marca"] != DBNull.Value ? reader["marca"].ToString() : ""
                            );
                        }
                    }
                }

                if (dgvProdutos.Rows.Count > 0)
                {
                    dgvProdutos.ClearSelection();
                    dgvProdutos.Rows[0].Selected = true;
                    AtualizarLabelSelecionado();
                }
                else
                {
                    lblSelecionado.Text = "Nenhum produto encontrado.";
                }
            }
            catch (Exception ex)
            {
                bool isConexao = ex is NpgsqlException || ex is System.Net.Sockets.SocketException ||
                                 ex is TimeoutException;
                lblSelecionado.ForeColor = Color.FromArgb(231, 76, 60);
                lblSelecionado.Text = isConexao ? "FALHA DE CONEXAO! Verifique a rede." : "Erro: " + ex.Message;
            }
        }

        private void AtualizarLabelSelecionado()
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                var row = dgvProdutos.SelectedRows[0];
                lblSelecionado.ForeColor = Color.FromArgb(120, 215, 255);
                lblSelecionado.Text = "Mat: " + row.Cells["colMatricula"].Value +
                                      " | " + row.Cells["colDescricao"].Value;
            }
        }

        private void ConfirmarSelecao()
        {
            if (dgvProdutos.SelectedRows.Count == 0 || dgvProdutos.Rows.Count == 0)
                return;

            var row = dgvProdutos.SelectedRows[0];
            string matStr = row.Cells["colMatricula"].Value.ToString();

            if (int.TryParse(matStr, out int matricula))
            {
                MatriculaSelecionada = matricula;
                Selecionado = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            Buscar();
        }

        private void txtBusca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ConfirmarSelecao();
                return;
            }

            if (e.KeyCode == Keys.Down && dgvProdutos.Rows.Count > 0)
            {
                e.SuppressKeyPress = true;
                int idx = dgvProdutos.SelectedRows.Count > 0 ? dgvProdutos.SelectedRows[0].Index : -1;
                if (idx < dgvProdutos.Rows.Count - 1)
                {
                    dgvProdutos.ClearSelection();
                    dgvProdutos.Rows[idx + 1].Selected = true;
                    dgvProdutos.FirstDisplayedScrollingRowIndex = Math.Max(0, idx + 1 - 5);
                    AtualizarLabelSelecionado();
                }
                return;
            }

            if (e.KeyCode == Keys.Up && dgvProdutos.Rows.Count > 0)
            {
                e.SuppressKeyPress = true;
                int idx = dgvProdutos.SelectedRows.Count > 0 ? dgvProdutos.SelectedRows[0].Index : 0;
                if (idx > 0)
                {
                    dgvProdutos.ClearSelection();
                    dgvProdutos.Rows[idx - 1].Selected = true;
                    dgvProdutos.FirstDisplayedScrollingRowIndex = Math.Max(0, idx - 1 - 5);
                    AtualizarLabelSelecionado();
                }
                return;
            }
        }

        private void dgvProdutos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ConfirmarSelecao();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void dgvProdutos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                ConfirmarSelecao();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            ConfirmarSelecao();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
