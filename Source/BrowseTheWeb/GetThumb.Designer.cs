namespace BrowseTheWeb.Setup
{
  partial class GetThumb
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
      this.components = new System.ComponentModel.Container();
      this.chkUrl = new System.Windows.Forms.CheckBox();
      this.chkGetThumb = new System.Windows.Forms.CheckBox();
      this.button1 = new System.Windows.Forms.Button();
      this.txtUrl = new System.Windows.Forms.Label();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.chkWait = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // chkUrl
      // 
      this.chkUrl.AutoCheck = false;
      this.chkUrl.AutoSize = true;
      this.chkUrl.Location = new System.Drawing.Point(12, 34);
      this.chkUrl.Name = "chkUrl";
      this.chkUrl.Size = new System.Drawing.Size(82, 17);
      this.chkUrl.TabIndex = 0;
      this.chkUrl.Text = "Check URL";
      this.chkUrl.UseVisualStyleBackColor = true;
      // 
      // chkGetThumb
      // 
      this.chkGetThumb.AutoCheck = false;
      this.chkGetThumb.AutoSize = true;
      this.chkGetThumb.Location = new System.Drawing.Point(12, 57);
      this.chkGetThumb.Name = "chkGetThumb";
      this.chkGetThumb.Size = new System.Drawing.Size(89, 17);
      this.chkGetThumb.TabIndex = 1;
      this.chkGetThumb.Text = "Cache thumb";
      this.chkGetThumb.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(12, 103);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(295, 30);
      this.button1.TabIndex = 3;
      this.button1.Text = "Cancel";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // txtUrl
      // 
      this.txtUrl.AutoSize = true;
      this.txtUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtUrl.Location = new System.Drawing.Point(12, 9);
      this.txtUrl.Name = "txtUrl";
      this.txtUrl.Size = new System.Drawing.Size(45, 16);
      this.txtUrl.TabIndex = 4;
      this.txtUrl.Text = "label1";
      // 
      // timer1
      // 
      this.timer1.Enabled = true;
      this.timer1.Interval = 300;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // chkWait
      // 
      this.chkWait.AutoCheck = false;
      this.chkWait.AutoSize = true;
      this.chkWait.Location = new System.Drawing.Point(12, 80);
      this.chkWait.Name = "chkWait";
      this.chkWait.Size = new System.Drawing.Size(98, 17);
      this.chkWait.TabIndex = 5;
      this.chkWait.Text = "Wait some time";
      this.chkWait.UseVisualStyleBackColor = true;
      // 
      // GetThumb
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(319, 149);
      this.Controls.Add(this.chkWait);
      this.Controls.Add(this.txtUrl);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.chkGetThumb);
      this.Controls.Add(this.chkUrl);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "GetThumb";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Status";
      this.Load += new System.EventHandler(this.GetThumb_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox chkUrl;
    private System.Windows.Forms.CheckBox chkGetThumb;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label txtUrl;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.CheckBox chkWait;
  }
}