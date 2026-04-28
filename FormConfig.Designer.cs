namespace AlocadorDeProdutos
{
    partial class FormConfig
    {
        private System.ComponentModel.IContainer components = null;

        // Dispose() esta em FormConfig.cs (custom) — desinscreve txtSenha.Leave.

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblHostLabel = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.btnTestar = new System.Windows.Forms.Button();
            this.lblFilialLabel = new System.Windows.Forms.Label();
            this.numFilial = new System.Windows.Forms.NumericUpDown();
            this.lblSenhaLabel = new System.Windows.Forms.Label();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.chkMostrarSenha = new System.Windows.Forms.CheckBox();
            this.lblStatusConexao = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numFilial)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(480, 45);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "⚙  CONFIGURACAO DO BANCO DE DADOS";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblHostLabel
            //
            this.lblHostLabel.AutoSize = true;
            this.lblHostLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblHostLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblHostLabel.Location = new System.Drawing.Point(20, 60);
            this.lblHostLabel.Name = "lblHostLabel";
            this.lblHostLabel.Size = new System.Drawing.Size(130, 21);
            this.lblHostLabel.TabIndex = 1;
            this.lblHostLabel.Text = "IP DO SERVIDOR:";
            //
            // txtHost
            //
            this.txtHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.txtHost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHost.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.txtHost.ForeColor = System.Drawing.Color.White;
            this.txtHost.Location = new System.Drawing.Point(20, 90);
            this.txtHost.MaxLength = 50;
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(310, 40);
            this.txtHost.TabIndex = 0;
            this.txtHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHost_KeyDown);
            //
            // btnTestar
            //
            this.btnTestar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnTestar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestar.FlatAppearance.BorderSize = 0;
            this.btnTestar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnTestar.ForeColor = System.Drawing.Color.White;
            this.btnTestar.Location = new System.Drawing.Point(345, 90);
            this.btnTestar.Name = "btnTestar";
            this.btnTestar.Size = new System.Drawing.Size(115, 40);
            this.btnTestar.TabIndex = 1;
            this.btnTestar.Text = "TESTAR";
            this.btnTestar.UseVisualStyleBackColor = false;
            this.btnTestar.Click += new System.EventHandler(this.btnTestar_Click);
            //
            // lblFilialLabel
            //
            this.lblFilialLabel.AutoSize = true;
            this.lblFilialLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblFilialLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblFilialLabel.Location = new System.Drawing.Point(20, 145);
            this.lblFilialLabel.Name = "lblFilialLabel";
            this.lblFilialLabel.Size = new System.Drawing.Size(70, 21);
            this.lblFilialLabel.TabIndex = 5;
            this.lblFilialLabel.Text = "FILIAL:";
            //
            // numFilial
            //
            this.numFilial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.numFilial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numFilial.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.numFilial.ForeColor = System.Drawing.Color.White;
            this.numFilial.Location = new System.Drawing.Point(20, 175);
            this.numFilial.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.numFilial.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numFilial.Name = "numFilial";
            this.numFilial.Size = new System.Drawing.Size(120, 40);
            this.numFilial.TabIndex = 2;
            this.numFilial.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFilial.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblSenhaLabel
            //
            this.lblSenhaLabel.AutoSize = true;
            this.lblSenhaLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSenhaLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblSenhaLabel.Location = new System.Drawing.Point(20, 230);
            this.lblSenhaLabel.Name = "lblSenhaLabel";
            this.lblSenhaLabel.Size = new System.Drawing.Size(155, 21);
            this.lblSenhaLabel.TabIndex = 6;
            this.lblSenhaLabel.Text = "SENHA DO BANCO:";
            //
            // txtSenha
            //
            this.txtSenha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.txtSenha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSenha.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtSenha.ForeColor = System.Drawing.Color.White;
            this.txtSenha.Location = new System.Drawing.Point(20, 260);
            this.txtSenha.MaxLength = 100;
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '●';
            this.txtSenha.Size = new System.Drawing.Size(310, 33);
            this.txtSenha.TabIndex = 3;
            this.txtSenha.UseSystemPasswordChar = false;
            //
            // chkMostrarSenha
            //
            this.chkMostrarSenha.AutoSize = true;
            this.chkMostrarSenha.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkMostrarSenha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.chkMostrarSenha.Location = new System.Drawing.Point(345, 268);
            this.chkMostrarSenha.Name = "chkMostrarSenha";
            this.chkMostrarSenha.Size = new System.Drawing.Size(70, 19);
            this.chkMostrarSenha.TabIndex = 4;
            this.chkMostrarSenha.Text = "Mostrar";
            this.chkMostrarSenha.UseVisualStyleBackColor = true;
            this.chkMostrarSenha.CheckedChanged += new System.EventHandler(this.chkMostrarSenha_CheckedChanged);
            //
            // lblStatusConexao
            //
            this.lblStatusConexao.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatusConexao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.lblStatusConexao.Location = new System.Drawing.Point(20, 305);
            this.lblStatusConexao.Name = "lblStatusConexao";
            this.lblStatusConexao.Size = new System.Drawing.Size(440, 50);
            this.lblStatusConexao.TabIndex = 7;
            this.lblStatusConexao.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // panelBottom
            //
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(54)))));
            this.panelBottom.Controls.Add(this.btnSalvar);
            this.panelBottom.Controls.Add(this.btnCancelar);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 365);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(480, 50);
            this.panelBottom.TabIndex = 8;
            //
            // btnSalvar
            //
            this.btnSalvar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnSalvar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvar.FlatAppearance.BorderSize = 0;
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSalvar.ForeColor = System.Drawing.Color.White;
            this.btnSalvar.Location = new System.Drawing.Point(240, 8);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(110, 34);
            this.btnSalvar.TabIndex = 5;
            this.btnSalvar.Text = "SALVAR";
            this.btnSalvar.UseVisualStyleBackColor = false;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(360, 8);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(105, 34);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // FormConfig
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(480, 415);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.lblStatusConexao);
            this.Controls.Add(this.chkMostrarSenha);
            this.Controls.Add(this.txtSenha);
            this.Controls.Add(this.lblSenhaLabel);
            this.Controls.Add(this.numFilial);
            this.Controls.Add(this.lblFilialLabel);
            this.Controls.Add(this.btnTestar);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.lblHostLabel);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfig";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuracao";
            ((System.ComponentModel.ISupportInitialize)(this.numFilial)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblHostLabel;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Button btnTestar;
        private System.Windows.Forms.Label lblFilialLabel;
        private System.Windows.Forms.NumericUpDown numFilial;
        private System.Windows.Forms.Label lblSenhaLabel;
        private System.Windows.Forms.TextBox txtSenha;
        private System.Windows.Forms.CheckBox chkMostrarSenha;
        private System.Windows.Forms.Label lblStatusConexao;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
