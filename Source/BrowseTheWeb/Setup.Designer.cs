﻿namespace BrowseTheWeb
{
  partial class Setup
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setup));
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.btnSave = new System.Windows.Forms.Button();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.titelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
      this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.btnCancel = new System.Windows.Forms.Button();
      this.txtLink = new System.Windows.Forms.Label();
      this.txtHome = new System.Windows.Forms.TextBox();
      this.chkHome = new System.Windows.Forms.CheckBox();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.chkOSD = new System.Windows.Forms.CheckBox();
      this.chkStatus = new System.Windows.Forms.CheckBox();
      this.chkBlank = new System.Windows.Forms.CheckBox();
      this.txtName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.txtRemote = new System.Windows.Forms.Label();
      this.trkRemote = new System.Windows.Forms.TrackBar();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.groupBox6 = new System.Windows.Forms.GroupBox();
      this.optNoZoom = new System.Windows.Forms.RadioButton();
      this.optZoomDomain = new System.Windows.Forms.RadioButton();
      this.optZoomPage = new System.Windows.Forms.RadioButton();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.txtFont = new System.Windows.Forms.Label();
      this.trkFont = new System.Windows.Forms.TrackBar();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txtZoom = new System.Windows.Forms.Label();
      this.trkZoom = new System.Windows.Forms.TrackBar();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.tabPage4 = new System.Windows.Forms.TabPage();
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.contextMenuStrip1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.groupBox5.SuspendLayout();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trkRemote)).BeginInit();
      this.groupBox3.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.groupBox6.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trkFont)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trkZoom)).BeginInit();
      this.tabPage3.SuspendLayout();
      this.tabPage4.SuspendLayout();
      this.SuspendLayout();
      // 
      // treeView1
      // 
      this.treeView1.AllowDrop = true;
      this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.treeView1.ImageIndex = 0;
      this.treeView1.ImageList = this.imageList1;
      this.treeView1.Location = new System.Drawing.Point(6, 17);
      this.treeView1.Name = "treeView1";
      this.treeView1.SelectedImageIndex = 0;
      this.treeView1.Size = new System.Drawing.Size(539, 378);
      this.treeView1.TabIndex = 9;
      this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
      this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
      this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
      this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
      this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
      this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "file-icon.png");
      this.imageList1.Images.SetKeyName(1, "folder-icon.png");
      this.imageList1.Images.SetKeyName(2, "fav-b-icon.png");
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(12, 523);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(104, 38);
      this.btnSave.TabIndex = 10;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titelToolStripMenuItem,
            this.toolStripMenuItem4,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.editToolStripMenuItem,
            this.toolStripMenuItem5,
            this.deleteToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(154, 142);
      this.contextMenuStrip1.MouseLeave += new System.EventHandler(this.contextMenuStrip1_MouseLeave);
      // 
      // titelToolStripMenuItem
      // 
      this.titelToolStripMenuItem.BackColor = System.Drawing.Color.Silver;
      this.titelToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.titelToolStripMenuItem.Name = "titelToolStripMenuItem";
      this.titelToolStripMenuItem.Size = new System.Drawing.Size(153, 24);
      this.titelToolStripMenuItem.Text = "Titel";
      // 
      // toolStripMenuItem4
      // 
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(150, 6);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Image = global::BrowseTheWeb.Properties.Resources.file_icon;
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(153, 24);
      this.toolStripMenuItem1.Text = "Add bookmark";
      this.toolStripMenuItem1.Click += new System.EventHandler(this.onAddBookmark);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Image = global::BrowseTheWeb.Properties.Resources.folder_icon;
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 24);
      this.toolStripMenuItem2.Text = "Add folder";
      this.toolStripMenuItem2.Click += new System.EventHandler(this.onAddFolder);
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(150, 6);
      // 
      // editToolStripMenuItem
      // 
      this.editToolStripMenuItem.Image = global::BrowseTheWeb.Properties.Resources.edit_icon;
      this.editToolStripMenuItem.Name = "editToolStripMenuItem";
      this.editToolStripMenuItem.Size = new System.Drawing.Size(153, 24);
      this.editToolStripMenuItem.Text = "Edit";
      this.editToolStripMenuItem.Click += new System.EventHandler(this.onEditEntry_Click);
      // 
      // toolStripMenuItem5
      // 
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new System.Drawing.Size(150, 6);
      // 
      // deleteToolStripMenuItem
      // 
      this.deleteToolStripMenuItem.Image = global::BrowseTheWeb.Properties.Resources.delete_icon;
      this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
      this.deleteToolStripMenuItem.Size = new System.Drawing.Size(153, 24);
      this.deleteToolStripMenuItem.Text = "Delete";
      this.deleteToolStripMenuItem.Click += new System.EventHandler(this.onRemoveEntry);
      // 
      // btnCancel
      // 
      this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.Location = new System.Drawing.Point(478, 523);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(104, 38);
      this.btnCancel.TabIndex = 12;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // txtLink
      // 
      this.txtLink.AutoSize = true;
      this.txtLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtLink.Location = new System.Drawing.Point(6, 398);
      this.txtLink.Name = "txtLink";
      this.txtLink.Size = new System.Drawing.Size(89, 20);
      this.txtLink.TabIndex = 13;
      this.txtLink.Text = "Bookmarks";
      // 
      // txtHome
      // 
      this.txtHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtHome.Location = new System.Drawing.Point(19, 55);
      this.txtHome.Name = "txtHome";
      this.txtHome.Size = new System.Drawing.Size(270, 22);
      this.txtHome.TabIndex = 15;
      // 
      // chkHome
      // 
      this.chkHome.AutoSize = true;
      this.chkHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkHome.Location = new System.Drawing.Point(19, 29);
      this.chkHome.Name = "chkHome";
      this.chkHome.Size = new System.Drawing.Size(118, 20);
      this.chkHome.TabIndex = 16;
      this.chkHome.Text = "use homepage";
      this.chkHome.UseVisualStyleBackColor = true;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Controls.Add(this.tabPage3);
      this.tabControl1.Controls.Add(this.tabPage4);
      this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tabControl1.Location = new System.Drawing.Point(12, 12);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(570, 505);
      this.tabControl1.TabIndex = 18;
      // 
      // tabPage1
      // 
      this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
      this.tabPage1.Controls.Add(this.groupBox5);
      this.tabPage1.Controls.Add(this.groupBox4);
      this.tabPage1.Controls.Add(this.groupBox3);
      this.tabPage1.Location = new System.Drawing.Point(4, 25);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(562, 476);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Settings";
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.chkOSD);
      this.groupBox5.Controls.Add(this.chkStatus);
      this.groupBox5.Controls.Add(this.chkBlank);
      this.groupBox5.Controls.Add(this.txtName);
      this.groupBox5.Controls.Add(this.label2);
      this.groupBox5.Location = new System.Drawing.Point(16, 218);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(533, 241);
      this.groupBox5.TabIndex = 19;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Other settings";
      // 
      // chkOSD
      // 
      this.chkOSD.AutoSize = true;
      this.chkOSD.Location = new System.Drawing.Point(19, 129);
      this.chkOSD.Name = "chkOSD";
      this.chkOSD.Size = new System.Drawing.Size(149, 20);
      this.chkOSD.TabIndex = 4;
      this.chkOSD.Text = "Show OSD for link ID";
      this.chkOSD.UseVisualStyleBackColor = true;
      // 
      // chkStatus
      // 
      this.chkStatus.AutoSize = true;
      this.chkStatus.Location = new System.Drawing.Point(19, 103);
      this.chkStatus.Name = "chkStatus";
      this.chkStatus.Size = new System.Drawing.Size(189, 20);
      this.chkStatus.TabIndex = 3;
      this.chkStatus.Text = "Enable statusbar as default";
      this.chkStatus.UseVisualStyleBackColor = true;
      // 
      // chkBlank
      // 
      this.chkBlank.AutoSize = true;
      this.chkBlank.Location = new System.Drawing.Point(19, 77);
      this.chkBlank.Name = "chkBlank";
      this.chkBlank.Size = new System.Drawing.Size(225, 20);
      this.chkBlank.TabIndex = 2;
      this.chkBlank.Text = "Blank browser on window change";
      this.chkBlank.UseVisualStyleBackColor = true;
      // 
      // txtName
      // 
      this.txtName.Location = new System.Drawing.Point(19, 49);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(136, 22);
      this.txtName.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(16, 30);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(82, 16);
      this.label2.TabIndex = 0;
      this.label2.Text = "Plugin name";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.txtRemote);
      this.groupBox4.Controls.Add(this.trkRemote);
      this.groupBox4.Location = new System.Drawing.Point(16, 111);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(534, 92);
      this.groupBox4.TabIndex = 18;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Remote";
      // 
      // txtRemote
      // 
      this.txtRemote.AutoSize = true;
      this.txtRemote.Location = new System.Drawing.Point(16, 53);
      this.txtRemote.Name = "txtRemote";
      this.txtRemote.Size = new System.Drawing.Size(139, 16);
      this.txtRemote.TabIndex = 2;
      this.txtRemote.Text = "Reset link ID after 1,5s";
      // 
      // trkRemote
      // 
      this.trkRemote.Location = new System.Drawing.Point(5, 24);
      this.trkRemote.Maximum = 100;
      this.trkRemote.Minimum = 1;
      this.trkRemote.Name = "trkRemote";
      this.trkRemote.Size = new System.Drawing.Size(525, 45);
      this.trkRemote.SmallChange = 5;
      this.trkRemote.TabIndex = 1;
      this.trkRemote.TickFrequency = 5;
      this.trkRemote.Value = 15;
      this.trkRemote.ValueChanged += new System.EventHandler(this.trkRemote_ValueChanged);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.txtHome);
      this.groupBox3.Controls.Add(this.chkHome);
      this.groupBox3.Location = new System.Drawing.Point(16, 13);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(534, 92);
      this.groupBox3.TabIndex = 17;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Homepage";
      // 
      // tabPage2
      // 
      this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
      this.tabPage2.Controls.Add(this.groupBox6);
      this.tabPage2.Controls.Add(this.groupBox2);
      this.tabPage2.Controls.Add(this.groupBox1);
      this.tabPage2.Location = new System.Drawing.Point(4, 25);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(562, 476);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Appeareance";
      // 
      // groupBox6
      // 
      this.groupBox6.Controls.Add(this.optNoZoom);
      this.groupBox6.Controls.Add(this.optZoomDomain);
      this.groupBox6.Controls.Add(this.optZoomPage);
      this.groupBox6.Location = new System.Drawing.Point(22, 239);
      this.groupBox6.Name = "groupBox6";
      this.groupBox6.Size = new System.Drawing.Size(537, 137);
      this.groupBox6.TabIndex = 20;
      this.groupBox6.TabStop = false;
      this.groupBox6.Text = "Zoom settings";
      // 
      // optNoZoom
      // 
      this.optNoZoom.AutoSize = true;
      this.optNoZoom.Checked = true;
      this.optNoZoom.Location = new System.Drawing.Point(26, 95);
      this.optNoZoom.Name = "optNoZoom";
      this.optNoZoom.Size = new System.Drawing.Size(132, 20);
      this.optNoZoom.TabIndex = 2;
      this.optNoZoom.TabStop = true;
      this.optNoZoom.Text = "do not reset zoom";
      this.optNoZoom.UseVisualStyleBackColor = true;
      // 
      // optZoomDomain
      // 
      this.optZoomDomain.AutoSize = true;
      this.optZoomDomain.Location = new System.Drawing.Point(26, 69);
      this.optZoomDomain.Name = "optZoomDomain";
      this.optZoomDomain.Size = new System.Drawing.Size(206, 20);
      this.optZoomDomain.TabIndex = 1;
      this.optZoomDomain.Text = "reset zoom on domain change";
      this.optZoomDomain.UseVisualStyleBackColor = true;
      // 
      // optZoomPage
      // 
      this.optZoomPage.AutoSize = true;
      this.optZoomPage.Location = new System.Drawing.Point(26, 43);
      this.optZoomPage.Name = "optZoomPage";
      this.optZoomPage.Size = new System.Drawing.Size(193, 20);
      this.optZoomPage.TabIndex = 0;
      this.optZoomPage.Text = "reset zoom on page change";
      this.optZoomPage.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.txtFont);
      this.groupBox2.Controls.Add(this.trkFont);
      this.groupBox2.Location = new System.Drawing.Point(22, 130);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(537, 97);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Font";
      // 
      // txtFont
      // 
      this.txtFont.AutoSize = true;
      this.txtFont.Location = new System.Drawing.Point(26, 60);
      this.txtFont.Name = "txtFont";
      this.txtFont.Size = new System.Drawing.Size(110, 16);
      this.txtFont.TabIndex = 1;
      this.txtFont.Text = "Default font 100%";
      // 
      // trkFont
      // 
      this.trkFont.LargeChange = 10;
      this.trkFont.Location = new System.Drawing.Point(6, 21);
      this.trkFont.Maximum = 200;
      this.trkFont.Minimum = 10;
      this.trkFont.Name = "trkFont";
      this.trkFont.Size = new System.Drawing.Size(525, 45);
      this.trkFont.SmallChange = 10;
      this.trkFont.TabIndex = 0;
      this.trkFont.TickFrequency = 10;
      this.trkFont.Value = 100;
      this.trkFont.ValueChanged += new System.EventHandler(this.trkFont_ValueChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.txtZoom);
      this.groupBox1.Controls.Add(this.trkZoom);
      this.groupBox1.Location = new System.Drawing.Point(19, 27);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(537, 97);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Zoom";
      // 
      // txtZoom
      // 
      this.txtZoom.AutoSize = true;
      this.txtZoom.Location = new System.Drawing.Point(26, 60);
      this.txtZoom.Name = "txtZoom";
      this.txtZoom.Size = new System.Drawing.Size(122, 16);
      this.txtZoom.TabIndex = 1;
      this.txtZoom.Text = "Default zoom 100%";
      // 
      // trkZoom
      // 
      this.trkZoom.LargeChange = 10;
      this.trkZoom.Location = new System.Drawing.Point(6, 21);
      this.trkZoom.Maximum = 200;
      this.trkZoom.Minimum = 10;
      this.trkZoom.Name = "trkZoom";
      this.trkZoom.Size = new System.Drawing.Size(525, 45);
      this.trkZoom.SmallChange = 10;
      this.trkZoom.TabIndex = 0;
      this.trkZoom.TickFrequency = 10;
      this.trkZoom.Value = 100;
      this.trkZoom.ValueChanged += new System.EventHandler(this.trkZoom_ValueChanged);
      // 
      // tabPage3
      // 
      this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
      this.tabPage3.Controls.Add(this.treeView1);
      this.tabPage3.Controls.Add(this.txtLink);
      this.tabPage3.Location = new System.Drawing.Point(4, 25);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage3.Size = new System.Drawing.Size(562, 476);
      this.tabPage3.TabIndex = 2;
      this.tabPage3.Text = "Bookmarks";
      // 
      // tabPage4
      // 
      this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
      this.tabPage4.Controls.Add(this.listBox1);
      this.tabPage4.Location = new System.Drawing.Point(4, 25);
      this.tabPage4.Name = "tabPage4";
      this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage4.Size = new System.Drawing.Size(562, 476);
      this.tabPage4.TabIndex = 3;
      this.tabPage4.Text = "Keyboard";
      // 
      // listBox1
      // 
      this.listBox1.FormattingEnabled = true;
      this.listBox1.ItemHeight = 16;
      this.listBox1.Location = new System.Drawing.Point(6, 15);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(550, 452);
      this.listBox1.TabIndex = 0;
      // 
      // Setup
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(593, 573);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnSave);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Setup";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Setup BrowseTheWeb";
      this.Load += new System.EventHandler(this.Setup_Load);
      this.contextMenuStrip1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trkRemote)).EndInit();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.tabPage2.ResumeLayout(false);
      this.groupBox6.ResumeLayout(false);
      this.groupBox6.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trkFont)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trkZoom)).EndInit();
      this.tabPage3.ResumeLayout(false);
      this.tabPage3.PerformLayout();
      this.tabPage4.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView treeView1;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem titelToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Label txtLink;
    private System.Windows.Forms.TextBox txtHome;
    private System.Windows.Forms.CheckBox chkHome;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TrackBar trkZoom;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label txtFont;
    private System.Windows.Forms.TrackBar trkFont;
    private System.Windows.Forms.Label txtZoom;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.TrackBar trkRemote;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label txtRemote;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.GroupBox groupBox6;
    private System.Windows.Forms.RadioButton optZoomDomain;
    private System.Windows.Forms.RadioButton optZoomPage;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox chkBlank;
    private System.Windows.Forms.CheckBox chkStatus;
    private System.Windows.Forms.TabPage tabPage4;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
    private System.Windows.Forms.CheckBox chkOSD;
    private System.Windows.Forms.RadioButton optNoZoom;
  }
}