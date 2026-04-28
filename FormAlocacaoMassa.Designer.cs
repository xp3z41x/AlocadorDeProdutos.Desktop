namespace AlocadorDeProdutos
{
    partial class FormAlocacaoMassa
    {
        private System.ComponentModel.IContainer components = null;

        // Dispose() esta em FormAlocacaoMassa.cs (custom) — desinscreve eventos
        // e libera fontes. Nao duplicar aqui.

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblAlocacaoLabel = new System.Windows.Forms.Label();
            this.txtAlocacao = new System.Windows.Forms.TextBox();
            this.lblFilialLabel = new System.Windows.Forms.Label();
            this.lblFilial = new System.Windows.Forms.Label();
            this.lblBuscaPrefixoLabel = new System.Windows.Forms.Label();
            this.txtBuscaPrefixo = new System.Windows.Forms.TextBox();
            this.lblBuscaContemLabel = new System.Windows.Forms.Label();
            this.txtBuscaContem = new System.Windows.Forms.TextBox();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colMatricula = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescricao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReferencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocalAtual = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnMarcarTodos = new System.Windows.Forms.Button();
            this.btnDesmarcarTodos = new System.Windows.Forms.Button();
            this.lblContador = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnAlocar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(900, 45);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "📦  ALOCAÇÃO EM MASSA";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblAlocacaoLabel
            //
            this.lblAlocacaoLabel.AutoSize = true;
            this.lblAlocacaoLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblAlocacaoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblAlocacaoLabel.Location = new System.Drawing.Point(20, 60);
            this.lblAlocacaoLabel.Name = "lblAlocacaoLabel";
            this.lblAlocacaoLabel.Size = new System.Drawing.Size(125, 20);
            this.lblAlocacaoLabel.TabIndex = 1;
            this.lblAlocacaoLabel.Text = "ALOCAÇÃO ALVO:";
            //
            // txtAlocacao
            //
            this.txtAlocacao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.txtAlocacao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAlocacao.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAlocacao.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.txtAlocacao.ForeColor = System.Drawing.Color.White;
            this.txtAlocacao.Location = new System.Drawing.Point(20, 85);
            this.txtAlocacao.MaxLength = 10;
            this.txtAlocacao.Name = "txtAlocacao";
            this.txtAlocacao.Size = new System.Drawing.Size(280, 39);
            this.txtAlocacao.TabIndex = 0;
            this.txtAlocacao.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // lblFilialLabel
            //
            this.lblFilialLabel.AutoSize = true;
            this.lblFilialLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblFilialLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblFilialLabel.Location = new System.Drawing.Point(330, 60);
            this.lblFilialLabel.Name = "lblFilialLabel";
            this.lblFilialLabel.Size = new System.Drawing.Size(58, 20);
            this.lblFilialLabel.TabIndex = 2;
            this.lblFilialLabel.Text = "FILIAL:";
            //
            // lblFilial
            //
            this.lblFilial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(54)))));
            this.lblFilial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFilial.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblFilial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.lblFilial.Location = new System.Drawing.Point(330, 85);
            this.lblFilial.Name = "lblFilial";
            this.lblFilial.Size = new System.Drawing.Size(80, 39);
            this.lblFilial.TabIndex = 3;
            this.lblFilial.Text = "01";
            this.lblFilial.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblBuscaPrefixoLabel
            //
            this.lblBuscaPrefixoLabel.AutoSize = true;
            this.lblBuscaPrefixoLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBuscaPrefixoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.lblBuscaPrefixoLabel.Location = new System.Drawing.Point(20, 145);
            this.lblBuscaPrefixoLabel.Name = "lblBuscaPrefixoLabel";
            this.lblBuscaPrefixoLabel.Size = new System.Drawing.Size(166, 19);
            this.lblBuscaPrefixoLabel.TabIndex = 4;
            this.lblBuscaPrefixoLabel.Text = "BUSCA EXATA (início):";
            //
            // txtBuscaPrefixo
            //
            this.txtBuscaPrefixo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.txtBuscaPrefixo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBuscaPrefixo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtBuscaPrefixo.ForeColor = System.Drawing.Color.White;
            this.txtBuscaPrefixo.Location = new System.Drawing.Point(200, 142);
            this.txtBuscaPrefixo.MaxLength = 60;
            this.txtBuscaPrefixo.Name = "txtBuscaPrefixo";
            this.txtBuscaPrefixo.Size = new System.Drawing.Size(680, 28);
            this.txtBuscaPrefixo.TabIndex = 1;
            //
            // lblBuscaContemLabel
            //
            this.lblBuscaContemLabel.AutoSize = true;
            this.lblBuscaContemLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBuscaContemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.lblBuscaContemLabel.Location = new System.Drawing.Point(20, 180);
            this.lblBuscaContemLabel.Name = "lblBuscaContemLabel";
            this.lblBuscaContemLabel.Size = new System.Drawing.Size(176, 19);
            this.lblBuscaContemLabel.TabIndex = 5;
            this.lblBuscaContemLabel.Text = "BUSCA NORMAL (contém):";
            //
            // txtBuscaContem
            //
            this.txtBuscaContem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.txtBuscaContem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBuscaContem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtBuscaContem.ForeColor = System.Drawing.Color.White;
            this.txtBuscaContem.Location = new System.Drawing.Point(200, 177);
            this.txtBuscaContem.MaxLength = 60;
            this.txtBuscaContem.Name = "txtBuscaContem";
            this.txtBuscaContem.Size = new System.Drawing.Size(680, 28);
            this.txtBuscaContem.TabIndex = 2;
            //
            // dgvProdutos
            //
            this.dgvProdutos.AllowUserToAddRows = false;
            this.dgvProdutos.AllowUserToDeleteRows = false;
            this.dgvProdutos.AllowUserToResizeRows = false;
            this.dgvProdutos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProdutos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(44)))));
            this.dgvProdutos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProdutos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvProdutos.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvProdutos.ColumnHeadersHeight = 32;
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvProdutos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCheck,
            this.colMatricula,
            this.colDescricao,
            this.colReferencia,
            this.colLocalAtual});
            this.dgvProdutos.EnableHeadersVisualStyles = false;
            this.dgvProdutos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.dgvProdutos.Location = new System.Drawing.Point(20, 220);
            this.dgvProdutos.MultiSelect = false;
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.RowHeadersVisible = false;
            this.dgvProdutos.RowTemplate.Height = 28;
            this.dgvProdutos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvProdutos.Size = new System.Drawing.Size(860, 290);
            this.dgvProdutos.TabIndex = 3;
            //
            // colCheck
            //
            this.colCheck.HeaderText = "";
            this.colCheck.Name = "colCheck";
            this.colCheck.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colCheck.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCheck.Width = 40;
            //
            // colMatricula
            //
            this.colMatricula.HeaderText = "Mat.";
            this.colMatricula.Name = "colMatricula";
            this.colMatricula.ReadOnly = true;
            this.colMatricula.Width = 80;
            //
            // colDescricao
            //
            this.colDescricao.HeaderText = "Descrição";
            this.colDescricao.Name = "colDescricao";
            this.colDescricao.ReadOnly = true;
            this.colDescricao.Width = 380;
            //
            // colReferencia
            //
            this.colReferencia.HeaderText = "Referência";
            this.colReferencia.Name = "colReferencia";
            this.colReferencia.ReadOnly = true;
            this.colReferencia.Width = 140;
            //
            // colLocalAtual
            //
            this.colLocalAtual.HeaderText = "Alocação Atual";
            this.colLocalAtual.Name = "colLocalAtual";
            this.colLocalAtual.ReadOnly = true;
            this.colLocalAtual.Width = 200;
            //
            // btnMarcarTodos
            //
            this.btnMarcarTodos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMarcarTodos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnMarcarTodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMarcarTodos.FlatAppearance.BorderSize = 0;
            this.btnMarcarTodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMarcarTodos.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMarcarTodos.ForeColor = System.Drawing.Color.White;
            this.btnMarcarTodos.Location = new System.Drawing.Point(20, 525);
            this.btnMarcarTodos.Name = "btnMarcarTodos";
            this.btnMarcarTodos.Size = new System.Drawing.Size(140, 32);
            this.btnMarcarTodos.TabIndex = 4;
            this.btnMarcarTodos.Text = "MARCAR TODOS";
            this.btnMarcarTodos.UseVisualStyleBackColor = false;
            //
            // btnDesmarcarTodos
            //
            this.btnDesmarcarTodos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDesmarcarTodos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(110)))));
            this.btnDesmarcarTodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDesmarcarTodos.FlatAppearance.BorderSize = 0;
            this.btnDesmarcarTodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesmarcarTodos.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDesmarcarTodos.ForeColor = System.Drawing.Color.White;
            this.btnDesmarcarTodos.Location = new System.Drawing.Point(170, 525);
            this.btnDesmarcarTodos.Name = "btnDesmarcarTodos";
            this.btnDesmarcarTodos.Size = new System.Drawing.Size(160, 32);
            this.btnDesmarcarTodos.TabIndex = 5;
            this.btnDesmarcarTodos.Text = "DESMARCAR TODOS";
            this.btnDesmarcarTodos.UseVisualStyleBackColor = false;
            //
            // lblContador
            //
            this.lblContador.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContador.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblContador.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.lblContador.Location = new System.Drawing.Point(680, 528);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(200, 28);
            this.lblContador.TabIndex = 6;
            this.lblContador.Text = "Selecionados: 0";
            this.lblContador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 565);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(860, 18);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelBottom
            //
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(54)))));
            this.panelBottom.Controls.Add(this.btnAlocar);
            this.panelBottom.Controls.Add(this.btnCancelar);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 590);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(900, 60);
            this.panelBottom.TabIndex = 8;
            //
            // btnAlocar
            //
            this.btnAlocar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlocar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnAlocar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlocar.FlatAppearance.BorderSize = 0;
            this.btnAlocar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlocar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAlocar.ForeColor = System.Drawing.Color.White;
            this.btnAlocar.Location = new System.Drawing.Point(710, 12);
            this.btnAlocar.Name = "btnAlocar";
            this.btnAlocar.Size = new System.Drawing.Size(170, 38);
            this.btnAlocar.TabIndex = 1;
            this.btnAlocar.Text = "ALOCAR (0)";
            this.btnAlocar.UseVisualStyleBackColor = false;
            //
            // btnCancelar
            //
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(580, 12);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 38);
            this.btnCancelar.TabIndex = 0;
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.UseVisualStyleBackColor = false;
            //
            // FormAlocacaoMassa
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(900, 650);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblContador);
            this.Controls.Add(this.btnDesmarcarTodos);
            this.Controls.Add(this.btnMarcarTodos);
            this.Controls.Add(this.dgvProdutos);
            this.Controls.Add(this.txtBuscaContem);
            this.Controls.Add(this.lblBuscaContemLabel);
            this.Controls.Add(this.txtBuscaPrefixo);
            this.Controls.Add(this.lblBuscaPrefixoLabel);
            this.Controls.Add(this.lblFilial);
            this.Controls.Add(this.lblFilialLabel);
            this.Controls.Add(this.txtAlocacao);
            this.Controls.Add(this.lblAlocacaoLabel);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.panelBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAlocacaoMassa";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alocação em Massa";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblAlocacaoLabel;
        private System.Windows.Forms.TextBox txtAlocacao;
        private System.Windows.Forms.Label lblFilialLabel;
        private System.Windows.Forms.Label lblFilial;
        private System.Windows.Forms.Label lblBuscaPrefixoLabel;
        private System.Windows.Forms.TextBox txtBuscaPrefixo;
        private System.Windows.Forms.Label lblBuscaContemLabel;
        private System.Windows.Forms.TextBox txtBuscaContem;
        private System.Windows.Forms.DataGridView dgvProdutos;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMatricula;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescricao;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReferencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocalAtual;
        private System.Windows.Forms.Button btnMarcarTodos;
        private System.Windows.Forms.Button btnDesmarcarTodos;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnAlocar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
