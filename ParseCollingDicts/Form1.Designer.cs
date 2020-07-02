namespace ParseCollingDicts
  {
  partial class Form1
    {
    /// <summary>
    /// Variable del diseñador necesaria.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Limpiar los recursos que se estén usando.
    /// </summary>
    /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
    protected override void Dispose( bool disposing )
      {
      if( disposing && ( components != null ) )
        {
        components.Dispose();
        }
      base.Dispose( disposing );
      }

    #region Código generado por el Diseñador de Windows Forms

    /// <summary>
    /// Método necesario para admitir el Diseñador. No se puede modificar
    /// el contenido de este método con el editor de código.
    /// </summary>
    private void InitializeComponent()
      {
      this.btnParse = new System.Windows.Forms.Button();
      this.txtMsgBox = new System.Windows.Forms.TextBox();
      this.lbPath1 = new System.Windows.Forms.Label();
      this.txtPagesDir = new System.Windows.Forms.TextBox();
      this.btnSelDir = new System.Windows.Forms.Button();
      this.SelFolderDlg = new System.Windows.Forms.FolderBrowserDialog();
      this.lbNowPage = new System.Windows.Forms.Label();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.txtSaveName = new System.Windows.Forms.TextBox();
      this.lbSave = new System.Windows.Forms.Label();
      this.tabParse = new System.Windows.Forms.TabControl();
      this.tabDir = new System.Windows.Forms.TabPage();
      this.tabPage = new System.Windows.Forms.TabPage();
      this.label1 = new System.Windows.Forms.Label();
      this.txtPageName = new System.Windows.Forms.TextBox();
      this.btnSelPage = new System.Windows.Forms.Button();
      this.btnVerPage = new System.Windows.Forms.Button();
      this.SelFileDlg = new System.Windows.Forms.OpenFileDialog();
      this.tabParse.SuspendLayout();
      this.tabDir.SuspendLayout();
      this.tabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnParse
      // 
      this.btnParse.Location = new System.Drawing.Point(8, 81);
      this.btnParse.Margin = new System.Windows.Forms.Padding(4);
      this.btnParse.Name = "btnParse";
      this.btnParse.Size = new System.Drawing.Size(102, 28);
      this.btnParse.TabIndex = 11;
      this.btnParse.Text = "Analizar";
      this.btnParse.UseVisualStyleBackColor = true;
      this.btnParse.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // txtMsgBox
      // 
      this.txtMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMsgBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMsgBox.Location = new System.Drawing.Point(4, 117);
      this.txtMsgBox.Margin = new System.Windows.Forms.Padding(4);
      this.txtMsgBox.Multiline = true;
      this.txtMsgBox.Name = "txtMsgBox";
      this.txtMsgBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtMsgBox.Size = new System.Drawing.Size(902, 373);
      this.txtMsgBox.TabIndex = 12;
      // 
      // lbPath1
      // 
      this.lbPath1.AutoSize = true;
      this.lbPath1.Location = new System.Drawing.Point(3, 13);
      this.lbPath1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbPath1.Name = "lbPath1";
      this.lbPath1.Size = new System.Drawing.Size(41, 17);
      this.lbPath1.TabIndex = 0;
      this.lbPath1.Text = "Path:";
      // 
      // txtPagesDir
      // 
      this.txtPagesDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPagesDir.Location = new System.Drawing.Point(45, 10);
      this.txtPagesDir.Margin = new System.Windows.Forms.Padding(4);
      this.txtPagesDir.Name = "txtPagesDir";
      this.txtPagesDir.Size = new System.Drawing.Size(804, 22);
      this.txtPagesDir.TabIndex = 1;
      // 
      // btnSelDir
      // 
      this.btnSelDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelDir.Location = new System.Drawing.Point(856, 7);
      this.btnSelDir.Margin = new System.Windows.Forms.Padding(4);
      this.btnSelDir.Name = "btnSelDir";
      this.btnSelDir.Size = new System.Drawing.Size(33, 28);
      this.btnSelDir.TabIndex = 2;
      this.btnSelDir.Text = "...";
      this.btnSelDir.UseVisualStyleBackColor = true;
      this.btnSelDir.Click += new System.EventHandler(this.btnPathIni_Click);
      // 
      // lbNowPage
      // 
      this.lbNowPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lbNowPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbNowPage.Location = new System.Drawing.Point(414, 89);
      this.lbNowPage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbNowPage.Name = "lbNowPage";
      this.lbNowPage.Size = new System.Drawing.Size(492, 24);
      this.lbNowPage.TabIndex = 0;
      this.lbNowPage.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(120, 81);
      this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(94, 28);
      this.btnCancel.TabIndex = 11;
      this.btnCancel.Text = "Cancelar";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Visible = false;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(252, 500);
      this.btnSave.Margin = new System.Windows.Forms.Padding(4);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(94, 28);
      this.btnSave.TabIndex = 13;
      this.btnSave.Text = "Guardar";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Visible = false;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // txtSaveName
      // 
      this.txtSaveName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSaveName.Location = new System.Drawing.Point(70, 503);
      this.txtSaveName.Margin = new System.Windows.Forms.Padding(4);
      this.txtSaveName.Name = "txtSaveName";
      this.txtSaveName.Size = new System.Drawing.Size(174, 22);
      this.txtSaveName.TabIndex = 1;
      this.txtSaveName.Text = "..\\Datos.txt";
      // 
      // lbSave
      // 
      this.lbSave.AutoSize = true;
      this.lbSave.Location = new System.Drawing.Point(9, 506);
      this.lbSave.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lbSave.Name = "lbSave";
      this.lbSave.Size = new System.Drawing.Size(62, 17);
      this.lbSave.TabIndex = 0;
      this.lbSave.Text = "Nombre:";
      // 
      // tabParse
      // 
      this.tabParse.Controls.Add(this.tabDir);
      this.tabParse.Controls.Add(this.tabPage);
      this.tabParse.Location = new System.Drawing.Point(4, 6);
      this.tabParse.Name = "tabParse";
      this.tabParse.SelectedIndex = 0;
      this.tabParse.Size = new System.Drawing.Size(903, 71);
      this.tabParse.TabIndex = 14;
      // 
      // tabDir
      // 
      this.tabDir.BackColor = System.Drawing.SystemColors.Control;
      this.tabDir.Controls.Add(this.lbPath1);
      this.tabDir.Controls.Add(this.txtPagesDir);
      this.tabDir.Controls.Add(this.btnSelDir);
      this.tabDir.Location = new System.Drawing.Point(4, 25);
      this.tabDir.Name = "tabDir";
      this.tabDir.Padding = new System.Windows.Forms.Padding(3);
      this.tabDir.Size = new System.Drawing.Size(895, 42);
      this.tabDir.TabIndex = 0;
      this.tabDir.Text = "Análisis de un Directorio";
      // 
      // tabPage
      // 
      this.tabPage.BackColor = System.Drawing.SystemColors.Control;
      this.tabPage.Controls.Add(this.label1);
      this.tabPage.Controls.Add(this.txtPageName);
      this.tabPage.Controls.Add(this.btnVerPage);
      this.tabPage.Controls.Add(this.btnSelPage);
      this.tabPage.Location = new System.Drawing.Point(4, 25);
      this.tabPage.Name = "tabPage";
      this.tabPage.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage.Size = new System.Drawing.Size(895, 42);
      this.tabPage.TabIndex = 1;
      this.tabPage.Text = "Análisis de una Página";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(4, 13);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(145, 17);
      this.label1.TabIndex = 3;
      this.label1.Text = "Nombre de la Página:";
      // 
      // txtPageName
      // 
      this.txtPageName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPageName.Location = new System.Drawing.Point(152, 10);
      this.txtPageName.Margin = new System.Windows.Forms.Padding(4);
      this.txtPageName.Name = "txtPageName";
      this.txtPageName.Size = new System.Drawing.Size(321, 22);
      this.txtPageName.TabIndex = 4;
      // 
      // btnSelPage
      // 
      this.btnSelPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelPage.Location = new System.Drawing.Point(481, 7);
      this.btnSelPage.Margin = new System.Windows.Forms.Padding(4);
      this.btnSelPage.Name = "btnSelPage";
      this.btnSelPage.Size = new System.Drawing.Size(33, 28);
      this.btnSelPage.TabIndex = 5;
      this.btnSelPage.Text = "...";
      this.btnSelPage.UseVisualStyleBackColor = true;
      this.btnSelPage.Click += new System.EventHandler(this.btnSelPage_Click);
      // 
      // btnVerPage
      // 
      this.btnVerPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnVerPage.Location = new System.Drawing.Point(547, 7);
      this.btnVerPage.Margin = new System.Windows.Forms.Padding(4);
      this.btnVerPage.Name = "btnVerPage";
      this.btnVerPage.Size = new System.Drawing.Size(56, 28);
      this.btnVerPage.TabIndex = 5;
      this.btnVerPage.Text = "Ver";
      this.btnVerPage.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(919, 540);
      this.Controls.Add(this.txtSaveName);
      this.Controls.Add(this.tabParse);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.lbNowPage);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnParse);
      this.Controls.Add(this.txtMsgBox);
      this.Controls.Add(this.lbSave);
      this.Name = "Form1";
      this.Text = "Analisis de diccionarios Colling";
      this.tabParse.ResumeLayout(false);
      this.tabDir.ResumeLayout(false);
      this.tabDir.PerformLayout();
      this.tabPage.ResumeLayout(false);
      this.tabPage.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

    #endregion
    private System.Windows.Forms.Button btnParse;
    private System.Windows.Forms.TextBox txtMsgBox;
    private System.Windows.Forms.Label lbPath1;
    private System.Windows.Forms.TextBox txtPagesDir;
    private System.Windows.Forms.Button btnSelDir;
    private System.Windows.Forms.FolderBrowserDialog SelFolderDlg;
    private System.Windows.Forms.Label lbNowPage;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TextBox txtSaveName;
    private System.Windows.Forms.Label lbSave;
    private System.Windows.Forms.TabControl tabParse;
    private System.Windows.Forms.TabPage tabDir;
    private System.Windows.Forms.TabPage tabPage;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtPageName;
    private System.Windows.Forms.Button btnVerPage;
    private System.Windows.Forms.Button btnSelPage;
    private System.Windows.Forms.OpenFileDialog SelFileDlg;
    }
  }

