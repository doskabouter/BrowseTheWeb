namespace BrowseTheWeb
{
  partial class ImportIE
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
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.btnImport = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.btnSelect = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.prgState = new System.Windows.Forms.ProgressBar();
      this.chkThumbs = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // listBox1
      // 
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new System.Drawing.Point(12, 35);
      this.listBox1.Name = "listBox1";
      this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.listBox1.Size = new System.Drawing.Size(502, 225);
      this.listBox1.TabIndex = 0;
      // 
      // btnImport
      // 
      this.btnImport.Location = new System.Drawing.Point(12, 341);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(139, 31);
      this.btnImport.TabIndex = 1;
      this.btnImport.Text = "Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(135, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Select bookmarks to import";
      // 
      // btnSelect
      // 
      this.btnSelect.Location = new System.Drawing.Point(12, 266);
      this.btnSelect.Name = "btnSelect";
      this.btnSelect.Size = new System.Drawing.Size(139, 23);
      this.btnSelect.TabIndex = 3;
      this.btnSelect.Text = "select / deselect all";
      this.btnSelect.UseVisualStyleBackColor = true;
      this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(375, 341);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(139, 31);
      this.btnCancel.TabIndex = 4;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // prgState
      // 
      this.prgState.Location = new System.Drawing.Point(12, 302);
      this.prgState.Name = "prgState";
      this.prgState.Size = new System.Drawing.Size(502, 23);
      this.prgState.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.prgState.TabIndex = 5;
      this.prgState.Visible = false;
      // 
      // chkThumbs
      // 
      this.chkThumbs.AutoSize = true;
      this.chkThumbs.Location = new System.Drawing.Point(187, 270);
      this.chkThumbs.Name = "chkThumbs";
      this.chkThumbs.Size = new System.Drawing.Size(164, 17);
      this.chkThumbs.TabIndex = 6;
      this.chkThumbs.Text = "make thumbs (take long time)";
      this.chkThumbs.UseVisualStyleBackColor = true;
      // 
      // ImportIE
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(526, 384);
      this.Controls.Add(this.chkThumbs);
      this.Controls.Add(this.prgState);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnSelect);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnImport);
      this.Controls.Add(this.listBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ImportIE";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Import Internet Explorer";
      this.Load += new System.EventHandler(this.ImportIE_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnSelect;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ProgressBar prgState;
    private System.Windows.Forms.CheckBox chkThumbs;
  }
}