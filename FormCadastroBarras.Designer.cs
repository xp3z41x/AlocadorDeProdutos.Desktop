namespace AlocadorDeProdutos
{
    partial class FormCadastroBarras
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblEANLabel = new System.Windows.Forms.Label();
            this.lblEAN = new System.Windows.Forms.Label();
            this.lblInstrucao = new System.Windows.Forms.Label();
            this.txtMatricula = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblAtalhos = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(520, 50);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "CODIGO DE BARRAS NAO ENCONTRADO";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblEANLabel
            //
            this.lblEANLabel.AutoSize = true;
            this.lblEANLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblEANLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.lblEANLabel.Location = new System.Drawing.Point(20, 60);
            this.lblEANLabel.Name = "lblEANLabel";
            this.lblEANLabel.Size = new System.Drawing.Size(100, 21);
            this.lblEANLabel.TabIndex = 1;
            this.lblEANLabel.Text = "EAN BIPADO:";
            //
            // lblEAN
            //
            this.lblEAN.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblEAN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.lblEAN.Location = new System.Drawing.Point(130, 55);
            this.lblEAN.Name = "lblEAN";
            this.lblEAN.Size = new System.Drawing.Size(370, 32);
            this.lblEAN.TabIndex = 2;
            this.lblEAN.Text = "---";
            //
            // lblInstrucao
            //
            this.lblInstrucao.AutoSize = true;
            this.lblInstrucao.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblInstrucao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblInstrucao.Location = new System.Drawing.Point(85, 105);
            this.lblInstrucao.Name = "lblInstrucao";
            this.lblInstrucao.Size = new System.Drawing.Size(350, 25);
            this.lblInstrucao.TabIndex = 3;
            this.lblInstrucao.Text = "INFORME A MATRICULA DO PRODUTO:";
            //
            // txtMatricula
            //
            this.txtMatricula.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.txtMatricula.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMatricula.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.txtMatricula.ForeColor = System.Drawing.Color.White;
            this.txtMatricula.Location = new System.Drawing.Point(100, 145);
            this.txtMatricula.MaxLength = 15;
            this.txtMatricula.Name = "txtMatricula";
            this.txtMatricula.Size = new System.Drawing.Size(320, 57);
            this.txtMatricula.TabIndex = 0;
            this.txtMatricula.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMatricula.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMatricula_KeyDown);
            this.txtMatricula.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMatricula_KeyPress);
            //
            // lblStatus
            //
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 215);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(480, 28);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblAtalhos
            //
            this.lblAtalhos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblAtalhos.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAtalhos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(145)))));
            this.lblAtalhos.Location = new System.Drawing.Point(0, 255);
            this.lblAtalhos.Name = "lblAtalhos";
            this.lblAtalhos.Size = new System.Drawing.Size(520, 30);
            this.lblAtalhos.TabIndex = 5;
            this.lblAtalhos.Text = "ENTER = Cadastrar e alocar    |    F4 = Buscar produto    |    ESC = Cancelar";
            this.lblAtalhos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // FormCadastroBarras
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(520, 285);
            this.Controls.Add(this.lblAtalhos);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtMatricula);
            this.Controls.Add(this.lblInstrucao);
            this.Controls.Add(this.lblEAN);
            this.Controls.Add(this.lblEANLabel);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCadastroBarras";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastrar Codigo de Barras";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblEANLabel;
        private System.Windows.Forms.Label lblEAN;
        private System.Windows.Forms.Label lblInstrucao;
        private System.Windows.Forms.TextBox txtMatricula;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblAtalhos;
    }
}
