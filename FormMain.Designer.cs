namespace AlocadorDeProdutos
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.btnAlocacaoMassa = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.nAlocacao = new System.Windows.Forms.TextBox();
            this.btnEditar = new System.Windows.Forms.Button();
            this.BtnOk = new System.Windows.Forms.Button();
            this.nMatricula = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pReferencia = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pMarca = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pDescricao = new System.Windows.Forms.TextBox();
            this.btnLimpar = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.pAlocacaoAtual = new System.Windows.Forms.TextBox();
            this.lblBipe = new System.Windows.Forms.Label();
            this.txtBipe = new System.Windows.Forms.TextBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFila = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelInfo.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(44)))));
            this.panelHeader.Controls.Add(this.lblAppTitle);
            this.panelHeader.Controls.Add(this.btnAlocacaoMassa);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1004, 50);
            this.panelHeader.TabIndex = 200;
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.lblAppTitle.Location = new System.Drawing.Point(20, 12);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(228, 25);
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "ALOCADOR DE PRODUTOS";
            //
            // btnAlocacaoMassa
            //
            this.btnAlocacaoMassa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlocacaoMassa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnAlocacaoMassa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlocacaoMassa.FlatAppearance.BorderSize = 0;
            this.btnAlocacaoMassa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlocacaoMassa.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnAlocacaoMassa.ForeColor = System.Drawing.Color.White;
            this.btnAlocacaoMassa.Location = new System.Drawing.Point(764, 8);
            this.btnAlocacaoMassa.Name = "btnAlocacaoMassa";
            this.btnAlocacaoMassa.Size = new System.Drawing.Size(220, 34);
            this.btnAlocacaoMassa.TabIndex = 1;
            this.btnAlocacaoMassa.TabStop = false;
            this.btnAlocacaoMassa.Text = "📦 ALOCAÇÃO EM MASSA";
            this.btnAlocacaoMassa.UseVisualStyleBackColor = false;
            this.btnAlocacaoMassa.Click += new System.EventHandler(this.btnAlocacaoMassa_Click);
            //
            // panelTop
            //
            this.panelTop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(54)))));
            this.panelTop.Controls.Add(this.btnLimpar);
            this.panelTop.Controls.Add(this.btnEditar);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.nAlocacao);
            this.panelTop.Controls.Add(this.BtnOk);
            this.panelTop.Controls.Add(this.label6);
            this.panelTop.Controls.Add(this.pAlocacaoAtual);
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.nMatricula);
            this.panelTop.Controls.Add(this.btnSalvar);
            this.panelTop.Location = new System.Drawing.Point(30, 60);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(10);
            this.panelTop.Size = new System.Drawing.Size(944, 220);
            this.panelTop.TabIndex = 100;
            //
            // panelInfo
            //
            this.panelInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(54)))));
            this.panelInfo.Controls.Add(this.label3);
            this.panelInfo.Controls.Add(this.pDescricao);
            this.panelInfo.Controls.Add(this.label4);
            this.panelInfo.Controls.Add(this.pReferencia);
            this.panelInfo.Controls.Add(this.label5);
            this.panelInfo.Controls.Add(this.pMarca);
            this.panelInfo.Location = new System.Drawing.Point(30, 290);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Padding = new System.Windows.Forms.Padding(10);
            this.panelInfo.Size = new System.Drawing.Size(944, 300);
            this.panelInfo.TabIndex = 101;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.label1.Location = new System.Drawing.Point(388, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 37);
            this.label1.TabIndex = 3;
            this.label1.Text = "ALOCAÇÃO:";
            //
            // nAlocacao
            //
            this.nAlocacao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.nAlocacao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nAlocacao.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.nAlocacao.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.nAlocacao.ForeColor = System.Drawing.Color.White;
            this.nAlocacao.Location = new System.Drawing.Point(342, 46);
            this.nAlocacao.MaxLength = 10;
            this.nAlocacao.Name = "nAlocacao";
            this.nAlocacao.Size = new System.Drawing.Size(260, 50);
            this.nAlocacao.TabIndex = 1;
            this.nAlocacao.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nAlocacao.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nAlocacao_KeyPress);
            //
            // btnLimpar
            //
            this.btnLimpar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnLimpar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpar.FlatAppearance.BorderSize = 0;
            this.btnLimpar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpar.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnLimpar.ForeColor = System.Drawing.Color.White;
            this.btnLimpar.Location = new System.Drawing.Point(15, 46);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(112, 46);
            this.btnLimpar.TabIndex = 63;
            this.btnLimpar.TabStop = false;
            this.btnLimpar.Text = "LIMPAR";
            this.btnLimpar.UseVisualStyleBackColor = false;
            this.btnLimpar.Click += new System.EventHandler(this.btnLimpar_Click);
            //
            // btnEditar
            //
            this.btnEditar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnEditar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.Location = new System.Drawing.Point(145, 46);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(112, 46);
            this.btnEditar.TabIndex = 49;
            this.btnEditar.TabStop = false;
            this.btnEditar.Text = "EDITAR";
            this.btnEditar.UseVisualStyleBackColor = false;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            //
            // BtnOk
            //
            this.BtnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.BtnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnOk.FlatAppearance.BorderSize = 0;
            this.BtnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnOk.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.BtnOk.ForeColor = System.Drawing.Color.White;
            this.BtnOk.Location = new System.Drawing.Point(620, 46);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.Size = new System.Drawing.Size(112, 46);
            this.BtnOk.TabIndex = 50;
            this.BtnOk.TabStop = false;
            this.BtnOk.Text = "\U0001f513";
            this.BtnOk.UseVisualStyleBackColor = false;
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.label6.Location = new System.Drawing.Point(30, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(178, 25);
            this.label6.TabIndex = 64;
            this.label6.Text = "ALOCAÇÃO ATUAL:";
            //
            // pAlocacaoAtual
            //
            this.pAlocacaoAtual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(54)))));
            this.pAlocacaoAtual.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pAlocacaoAtual.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.pAlocacaoAtual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.pAlocacaoAtual.Location = new System.Drawing.Point(15, 135);
            this.pAlocacaoAtual.MaxLength = 10;
            this.pAlocacaoAtual.Name = "pAlocacaoAtual";
            this.pAlocacaoAtual.ReadOnly = true;
            this.pAlocacaoAtual.Size = new System.Drawing.Size(260, 40);
            this.pAlocacaoAtual.TabIndex = 65;
            this.pAlocacaoAtual.TabStop = false;
            this.pAlocacaoAtual.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.label2.Location = new System.Drawing.Point(382, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 37);
            this.label2.TabIndex = 52;
            this.label2.Text = "MATRÍCULA:";
            //
            // nMatricula
            //
            this.nMatricula.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.nMatricula.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nMatricula.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.nMatricula.ForeColor = System.Drawing.Color.White;
            this.nMatricula.Location = new System.Drawing.Point(342, 148);
            this.nMatricula.MaxLength = 30;
            this.nMatricula.Name = "nMatricula";
            this.nMatricula.Size = new System.Drawing.Size(260, 50);
            this.nMatricula.TabIndex = 2;
            this.nMatricula.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nMatricula.TextChanged += new System.EventHandler(this.nMatricula_TextChanged);
            this.nMatricula.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nMatricula_KeyDown);
            this.nMatricula.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nMatricula_KeyPress);
            //
            // btnSalvar
            //
            this.btnSalvar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnSalvar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvar.FlatAppearance.BorderSize = 0;
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnSalvar.ForeColor = System.Drawing.Color.White;
            this.btnSalvar.Location = new System.Drawing.Point(620, 148);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(112, 46);
            this.btnSalvar.TabIndex = 3;
            this.btnSalvar.Text = "GRAVAR";
            this.btnSalvar.UseVisualStyleBackColor = false;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.label3.Location = new System.Drawing.Point(390, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 32);
            this.label3.TabIndex = 56;
            this.label3.Text = "DESCRIÇÃO:";
            //
            // pDescricao
            //
            this.pDescricao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(64)))));
            this.pDescricao.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pDescricao.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.pDescricao.ForeColor = System.Drawing.Color.White;
            this.pDescricao.Location = new System.Drawing.Point(20, 45);
            this.pDescricao.MaxLength = 200;
            this.pDescricao.Multiline = true;
            this.pDescricao.Name = "pDescricao";
            this.pDescricao.ReadOnly = true;
            this.pDescricao.Size = new System.Drawing.Size(904, 65);
            this.pDescricao.TabIndex = 55;
            this.pDescricao.TabStop = false;
            this.pDescricao.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.label4.Location = new System.Drawing.Point(378, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(178, 32);
            this.label4.TabIndex = 59;
            this.label4.Text = "REFERÊNCIA:";
            //
            // pReferencia
            //
            this.pReferencia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(64)))));
            this.pReferencia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pReferencia.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.pReferencia.ForeColor = System.Drawing.Color.White;
            this.pReferencia.Location = new System.Drawing.Point(20, 155);
            this.pReferencia.MaxLength = 50;
            this.pReferencia.Name = "pReferencia";
            this.pReferencia.ReadOnly = true;
            this.pReferencia.Size = new System.Drawing.Size(904, 40);
            this.pReferencia.TabIndex = 58;
            this.pReferencia.TabStop = false;
            this.pReferencia.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.label5.Location = new System.Drawing.Point(410, 205);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 32);
            this.label5.TabIndex = 62;
            this.label5.Text = "MARCA:";
            //
            // pMarca
            //
            this.pMarca.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(64)))));
            this.pMarca.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pMarca.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.pMarca.ForeColor = System.Drawing.Color.White;
            this.pMarca.Location = new System.Drawing.Point(20, 245);
            this.pMarca.MaxLength = 50;
            this.pMarca.Name = "pMarca";
            this.pMarca.ReadOnly = true;
            this.pMarca.Size = new System.Drawing.Size(904, 40);
            this.pMarca.TabIndex = 61;
            this.pMarca.TabStop = false;
            this.pMarca.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // lblBipe
            //
            this.lblBipe.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblBipe.AutoSize = true;
            this.lblBipe.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblBipe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.lblBipe.Location = new System.Drawing.Point(30, 603);
            this.lblBipe.Name = "lblBipe";
            this.lblBipe.Size = new System.Drawing.Size(279, 30);
            this.lblBipe.TabIndex = 66;
            this.lblBipe.Text = "LEITURA DE CODIGO DE BARRAS:";
            //
            // txtBipe
            //
            this.txtBipe.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBipe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.txtBipe.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBipe.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.txtBipe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.txtBipe.Location = new System.Drawing.Point(30, 638);
            this.txtBipe.MaxLength = 50;
            this.txtBipe.Name = "txtBipe";
            this.txtBipe.Size = new System.Drawing.Size(944, 50);
            this.txtBipe.TabIndex = 0;
            this.txtBipe.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBipe.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBipe_KeyDown);
            //
            // btnConfig
            //
            this.btnConfig.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(72)))));
            this.btnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfig.FlatAppearance.BorderSize = 0;
            this.btnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.btnConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(185)))));
            this.btnConfig.Location = new System.Drawing.Point(940, 603);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(34, 34);
            this.btnConfig.TabIndex = 70;
            this.btnConfig.TabStop = false;
            this.btnConfig.Text = "⚙";
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(145)))));
            this.lblStatus.Location = new System.Drawing.Point(30, 693);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(944, 20);
            this.lblStatus.TabIndex = 71;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblFila
            //
            this.lblFila.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblFila.AutoSize = true;
            this.lblFila.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblFila.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.lblFila.Location = new System.Drawing.Point(450, 607);
            this.lblFila.Name = "lblFila";
            this.lblFila.Size = new System.Drawing.Size(0, 21);
            this.lblFila.TabIndex = 72;
            this.lblFila.Visible = false;
            //
            // FormMain
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(1004, 720);
            this.Controls.Add(this.lblFila);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtBipe);
            this.Controls.Add(this.lblBipe);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelHeader);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1020, 750);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alocador de Produtos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Button btnAlocacaoMassa;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nAlocacao;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button BtnOk;
        private System.Windows.Forms.TextBox nMatricula;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pReferencia;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pMarca;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox pDescricao;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox pAlocacaoAtual;
        private System.Windows.Forms.Label lblBipe;
        private System.Windows.Forms.TextBox txtBipe;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblFila;
    }
}
