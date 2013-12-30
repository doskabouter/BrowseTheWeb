namespace BrowseTheWeb.Setup
{
  partial class ImportBrowser
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
            this.btnImport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.prgState = new System.Windows.Forms.ProgressBar();
            this.chkThumbs = new System.Windows.Forms.CheckBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.buttonSelAll = new System.Windows.Forms.Button();
            this.buttonDeselAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
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
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(12, 35);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(502, 225);
            this.treeView1.TabIndex = 7;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // buttonSelAll
            // 
            this.buttonSelAll.Location = new System.Drawing.Point(15, 264);
            this.buttonSelAll.Name = "buttonSelAll";
            this.buttonSelAll.Size = new System.Drawing.Size(60, 23);
            this.buttonSelAll.TabIndex = 8;
            this.buttonSelAll.Text = "Select all";
            this.buttonSelAll.UseVisualStyleBackColor = true;
            this.buttonSelAll.Click += new System.EventHandler(this.buttonSelAll_Click);
            // 
            // buttonDeselAll
            // 
            this.buttonDeselAll.Location = new System.Drawing.Point(81, 264);
            this.buttonDeselAll.Name = "buttonDeselAll";
            this.buttonDeselAll.Size = new System.Drawing.Size(70, 23);
            this.buttonDeselAll.TabIndex = 9;
            this.buttonDeselAll.Text = "Deselect all";
            this.buttonDeselAll.UseVisualStyleBackColor = true;
            this.buttonDeselAll.Click += new System.EventHandler(this.buttonDeselAll_Click);
            // 
            // ImportBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 384);
            this.Controls.Add(this.buttonDeselAll);
            this.Controls.Add(this.buttonSelAll);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.chkThumbs);
            this.Controls.Add(this.prgState);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportBrowser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Browser";
            this.Load += new System.EventHandler(this.ImportBase_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ProgressBar prgState;
    private System.Windows.Forms.CheckBox chkThumbs;
    private System.Windows.Forms.TreeView treeView1;
    private System.Windows.Forms.Button buttonSelAll;
    private System.Windows.Forms.Button buttonDeselAll;
  }
}