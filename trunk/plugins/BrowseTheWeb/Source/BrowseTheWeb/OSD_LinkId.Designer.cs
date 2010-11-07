namespace BrowseTheWeb
{
  partial class OSD_LinkId
  {
    /// <summary> 
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Komponenten-Designer generierter Code

    /// <summary> 
    /// Erforderliche Methode für die Designerunterstützung. 
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.txtId = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // txtId
      // 
      this.txtId.BackColor = System.Drawing.Color.White;
      this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtId.Location = new System.Drawing.Point(15, 16);
      this.txtId.Name = "txtId";
      this.txtId.Size = new System.Drawing.Size(273, 121);
      this.txtId.TabIndex = 0;
      this.txtId.Text = "0000";
      this.txtId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // OSD_LinkId
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Black;
      this.Controls.Add(this.txtId);
      this.Name = "OSD_LinkId";
      this.Size = new System.Drawing.Size(305, 154);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label txtId;
  }
}
