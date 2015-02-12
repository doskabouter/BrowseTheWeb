#region Copyright (C) 2005-2011 Team MediaPortal

/* 
 *	Copyright (C) 2005-2011 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Gecko;

using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using Action = MediaPortal.GUI.Library.Action;
using TreeView = System.Windows.Forms.TreeView;

namespace BrowseTheWeb.Setup
{
    public partial class Setup : Form
    {
        #region declare vars
        private TreeNode sourceNode;

        private Settings settings = null;
        #endregion

        public Setup()
        {
            InitializeComponent();

            #region remove old xulrunner folder

            string unpackDirectory = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
            string oldXulPath = Path.Combine(unpackDirectory, "xulrunner");
            if (Directory.Exists(oldXulPath))
                Directory.Delete(oldXulPath, true);

            #endregion

            settings = Settings.Instance;

            try
            {
                Xpcom.Initialize(Settings.XulRunnerPath());
            }
            catch (Exception ex)
            {
                Log.Debug("BrowseTheWeb | Exception on init Xpcom : " + ex.Message, new object[0]);
            }
        }


        public static void FillTreeview(BookmarkFolder bmf, TreeNodeCollection nodes)
        {
            foreach (BookmarkBase item in bmf.Items)
            {
                TreeNode newNode = nodes.Add(item.Name);
                newNode.Tag = item;
                if (item is BookmarkFolder)
                {
                    newNode.ImageIndex = 1;
                    newNode.SelectedImageIndex = 1;
                    FillTreeview((BookmarkFolder)item, newNode.Nodes);
                }
            }
        }

        private void FillTreeview(TreeView treeview)
        {
            treeview.Nodes.Clear();
            TreeNode main = treeview.Nodes.Add("Bookmarks", "Bookmarks");
            main.ImageIndex = 2;
            main.SelectedImageIndex = 2;
            FillTreeview(Bookmarks.Instance.root, main.Nodes);
            treeview.Invalidate();
        }

        private void FillBookmarks(TreeNodeCollection nodes, BookmarkFolder bms)
        {
            foreach (TreeNode node in nodes)
            {
                BookmarkBase bkm = (BookmarkBase)node.Tag;
                bms.Items.Add(bkm);

                BookmarkFolder bmf = node.Tag as BookmarkFolder;
                if (bmf != null)
                {
                    bmf.Items.Clear();
                    FillBookmarks(node.Nodes, bmf);
                }
            }
        }

        private void Setup_Load(object sender, EventArgs e)
        {
            Bookmark.InitCachePath();
            Bookmarks.Instance.LoadFromXml(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");
            FillTreeview(treeView1);

            treeView1.ExpandAll();

            for (GUIFacadeControl.Layout l = GUIFacadeControl.Layout.List; l <= GUIFacadeControl.Layout.LargeIcons; l++)
                cmbBookmarkView.Items.Add(l);

            #region prepare remote setup
            foreach (Action.ActionType val in Enum.GetValues(typeof(Action.ActionType)))
            {
                if (val > Action.ActionType.ACTION_MOVE_DOWN && val != Action.ActionType.ACTION_PAUSE && val != Action.ActionType.ACTION_STOP &&
                    val != Action.ActionType.ACTION_NEXT_ITEM && val != Action.ActionType.ACTION_PREV_ITEM &&
                    val != Action.ActionType.ACTION_FORWARD && val != Action.ActionType.ACTION_REWIND &&
                    val != Action.ActionType.ACTION_PLAY && val != Action.ActionType.ACTION_MUSIC_PLAY &&
                    val != Action.ActionType.ACTION_RECORD)
                {
                    cmbShowBookmarks.Items.Add(val);
                    cmbConfirmLink.Items.Add(val);
                    cmbStatusBar.Items.Add(val);
                    cmbZoomIn.Items.Add(val);
                    cmbZoomOut.Items.Add(val);
                    cmbPageUp.Items.Add(val);
                    cmbPageDown.Items.Add(val);
                }
            }

            SettingsToUI();
            #endregion

            #region add info for keyboard

            /*
      listBox1.Items.Add("Keyboard\t\tRemote\t\tFunction");
      listBox1.Items.Add("--------------------------------------------------------------------------------------------------");
      listBox1.Items.Add("P\t\tPlay\t\tselect a url");
      listBox1.Items.Add("B\t\tStop\t\tblank page");
      listBox1.Items.Add("Space\t\tPause\t\thome page");
      listBox1.Items.Add("0-9\t\t0-9\t\tSelect a link ID");
      listBox1.Items.Add("U\t\t#\t\tconfirm link ID");
      listBox1.Items.Add("Page up\t\tP+\t\tzoom out");
      listBox1.Items.Add("Page down\tP-\t\tzoom in");
      listBox1.Items.Add("F7\t\tStep backw\tgo backward");
      listBox1.Items.Add("F8\t\tStep forw.\t\tgo forward");
      listBox1.Items.Add("F9\t\tYellow\t\tshow favorites");
      listBox1.Items.Add("R\t\tRec\t\tadd bookmark");
      listBox1.Items.Add("up\t\tup\t\tmove up");
      listBox1.Items.Add("down\t\tdown\t\tmove down");
      listBox1.Items.Add("left\t\tleft\t\tmove left");
      listBox1.Items.Add("right\t\tright\t\tmove right");
      listBox1.Items.Add("X\t\tRed\t\ttoggle statusbar");
      */
            #endregion
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Bookmarks.Instance.Clear();
            FillBookmarks(treeView1.Nodes[0].Nodes, Bookmarks.Instance.root);

            bool result = Bookmarks.Instance.SaveToXml(Config.GetFolder(Config.Dir.Config) + "\\bookmarks.xml");
            if (!result)
            {
                MessageBox.Show("Bookmarks could not be saved !");
                return; // should never happen
            }

            if ((chkHome.Checked) && (!Bookmark.isValidUrl(txtHome.Text)))
            {
                DialogResult res = MessageBox.Show("The homepage adress seems not to be valid !\nContinue anyway ?", "Error home page address", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes) return;
            }

            UIToSettings();
            settings.SaveToXml(true);
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region treeview / drag drop
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode n = (TreeNode)e.Node;
            BookmarkBase bkm = (BookmarkBase)n.Tag;

            if (bkm != null)
            {
                if (bkm is BookmarkFolder)
                {
                    pictureBox1.Image = null;
                    txtLink.Text = bkm.Name;
                }
                else
                {
                    txtLink.Text = ((BookmarkItem)bkm).Url;
                    pictureBox1.Image = Bookmark.GetSnap(txtLink.Text);
                }
            }
            else
            {
                txtLink.Text = n.Text;
            }
        }
        private void treeView1_Click(object sender, EventArgs e)
        {
            MouseEventArgs arg = (MouseEventArgs)e;
            if (arg.Button == MouseButtons.Right)
            {
                TreeNode node = treeView1.SelectedNode;

                Point pos = new Point(arg.X, arg.Y);
                contextMenuStrip1.Show(treeView1, pos);

                if (treeView1.SelectedNode != null)
                {
                    contextMenuStrip1.Items[0].Text = treeView1.SelectedNode.Text;

                    contextMenuStrip1.Items[2].Enabled = true; // bookmark
                    contextMenuStrip1.Items[3].Enabled = true; // folder
                    contextMenuStrip1.Items[5].Enabled = true; // edit
                    contextMenuStrip1.Items[7].Enabled = true; // delete

                    if (treeView1.SelectedNode.Level == 0)
                    {
                        contextMenuStrip1.Items[5].Enabled = false;
                        contextMenuStrip1.Items[7].Enabled = false;
                    }
                    if (node.Tag is BookmarkItem)
                    {
                        contextMenuStrip1.Items[2].Enabled = false;
                        contextMenuStrip1.Items[3].Enabled = false;
                    }
                }
                else
                {
                    contextMenuStrip1.Close();
                }
            }
            if (arg.Button == MouseButtons.Left)
            {
                contextMenuStrip1.Close();
            }
        }
        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            sourceNode = (TreeNode)e.Item;
            DoDragDrop(e.Item.ToString(), DragDropEffects.Move | DragDropEffects.Copy);
        }
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            #region get source and target node
            Point pos = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeView1.GetNodeAt(pos);
            TreeNode tmp = targetNode;
            while (tmp != null && tmp != sourceNode)
                tmp = tmp.Parent;
            if (tmp != null)
            {
                MessageBox.Show("The destination folder is a subfolder of the source folder.", "Error moving folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            if (targetNode != null)
            {
                if (targetNode.Tag is BookmarkFolder)
                {
                    sourceNode.Remove();
                    targetNode.Nodes.Insert(0, sourceNode);
                }
                else
                {
                    sourceNode.Remove();
                    if (targetNode.Parent != null)
                        targetNode.Parent.Nodes.Insert(targetNode.Index, sourceNode);
                    else
                        targetNode.Nodes.Insert(targetNode.Index, sourceNode);
                }
            }

        }

        private void contextMenuStrip1_MouseLeave(object sender, EventArgs e)
        {
            contextMenuStrip1.Close();
        }
        #endregion

        #region tree view action add / remove..

        private TreeNode addNode(TreeNode node, string name)
        {
            TreeNode newNode;
            if (node.Parent == null)//root node "Bookmarks"
                newNode = node.Nodes.Insert(node.Index + 1, name);
            else
                if (node.Tag is BookmarkFolder)
                    newNode = node.Nodes.Add(name);
                else
                    newNode = node.Parent.Nodes.Insert(node.Index + 1, name);

            newNode.ImageIndex = 1;
            newNode.SelectedImageIndex = 1;
            return newNode;
        }

        private void onAddFolder(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            if (node != null)
            {
                GetFolder get = new GetFolder();
                DialogResult result = get.ShowDialog();

                if (result == DialogResult.OK)
                {
                    TreeNode newNode = addNode(node, get.SelectedFolderName);

                    BookmarkFolder bmf = new BookmarkFolder();
                    bmf.Name = get.SelectedFolderName;
                    newNode.Tag = bmf;

                    newNode.Parent.ExpandAll();
                }
            }

        }

        private void onAddBookmark(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                GetUrl get = new GetUrl();
                get.SelectedName = "new bookmark";
                get.SelectedUrl = @"http://";
                DialogResult result = get.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (!Bookmark.isValidUrl(get.SelectedUrl))
                    {
                        DialogResult res = MessageBox.Show("The url seems not to be valid !\nContinue anyway ?", "Error home page address", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (res != DialogResult.Yes) return;
                    }

                    if (chkUseThumbs.Checked)
                    {
                        GetThumb thumb = new GetThumb();
                        thumb.SelectedUrl = get.SelectedUrl;
                        thumb.ShowDialog();
                    }

                    TreeNode newNode = addNode(node, get.SelectedName);

                    BookmarkItem bmi = new BookmarkItem();
                    bmi.Name = get.SelectedName;
                    bmi.Url = get.SelectedUrl;
                    newNode.Tag = bmi;

                    newNode.Parent.ExpandAll();

                }
            }
        }
        private void onRemoveEntry(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                BookmarkBase bkm = (BookmarkBase)node.Tag;
                if (bkm is BookmarkItem)
                {
                    DialogResult res = MessageBox.Show("Do you really want to remove entry\n" + node.Text + "?", "Confirm ?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.OK)
                    {
                        node.Remove();
                        treeView1.Invalidate();
                    }
                }
                else
                {
                    DialogResult res = MessageBox.Show("Do you really want to remove folder\n" + node.Text + "\nand its content ?", "Confirm ?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.OK)
                    {
                        node.Remove();
                        treeView1.Invalidate();
                    }
                }
            }
        }
        private void onEditEntry_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                BookmarkBase bkm = (BookmarkBase)node.Tag;
                if (bkm is BookmarkFolder)
                {
                    GetFolder get = new GetFolder();
                    get.SelectedFolderName = bkm.Name;
                    DialogResult result = get.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        if ((get.SelectedFolderName != string.Empty) &&
                            (get.SelectedFolderName != bkm.Name))
                        {
                            bkm.Name = get.SelectedFolderName;
                            node.Text = get.SelectedFolderName;

                            treeView1.Invalidate();
                        }
                    }
                }
                else
                {
                    GetUrl get = new GetUrl();
                    get.SelectedName = bkm.Name;
                    BookmarkItem bmi = (BookmarkItem)bkm;
                    get.SelectedUrl = bmi.Url;
                    DialogResult result = get.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        if (get.SelectedName != bkm.Name)
                        {
                        }
                        if (get.SelectedUrl != bmi.Url)
                        {
                            if (!Bookmark.isValidUrl(get.SelectedUrl))
                            {
                                DialogResult res = MessageBox.Show("The url seems not to be valid !\nContinue anyway ?", "Error home page address", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (res != DialogResult.Yes) return;
                            }
                        }

                        node.Text = get.SelectedName;

                        bmi.Name = get.SelectedName;
                        bmi.Url = get.SelectedUrl;

                        treeView1.Invalidate();
                    }
                }
            }
        }
        #endregion

        private void SettingsToUI()
        {
            chkHome.Checked = settings.UseHome;
            txtHome.Text = settings.HomePage;
            trkRemote.Value = settings.RemoteTime;
            txtName.Text = settings.PluginName;
            chkBlank.Checked = settings.BlankBrowser;
            chkStatus.Checked = settings.StatusBar;
            chkOSD.Checked = settings.OSD;
            chkWindowed.Checked = settings.Windowed;
            chkMouse.Checked = settings.UseMouse;
            chkDisableAero.Checked = settings.DisableAero;

            trkZoom.Value = settings.DefaultZoom_percentage;
            trkFont.Value = settings.FontZoom_percentage;
            optZoomPage.Checked = settings.ZoomPage;
            optZoomDomain.Checked = settings.ZoomDomain;

            chkUseThumbs.Checked = settings.UseThumbs;
            chkThumbsOnVisit.Checked = settings.CacheThumbs;

            chkRemote.Checked = settings.Remote;
            cmbShowBookmarks.SelectedItem = settings.Remote_Bookmark;
            cmbConfirmLink.SelectedItem = settings.Remote_Confirm;
            cmbZoomIn.SelectedItem = settings.Remote_Zoom_In;
            cmbZoomOut.SelectedItem = settings.Remote_Zoom_Out;
            cmbStatusBar.SelectedItem = settings.Remote_Status;
            cmbPageUp.SelectedItem = settings.Remote_PageUp;
            cmbPageDown.SelectedItem = settings.Remote_PageDown;
            cmbBookmarkView.SelectedItem = settings.View;

            cmbUserAgent.Text = settings.UserAgent;
            cbOverrideUserAgent.Checked = !String.IsNullOrEmpty(cmbUserAgent.Text);

            textBoxPrevious.Text = Settings.TagsToString(settings.PreviousTags);
            textBoxNext.Text = Settings.TagsToString(settings.NextTags);

            chkProxy.Checked = settings.UseProxy;
            txtHttpServer.Text = settings.Server;
            txtHttpPort.Text = settings.Port.ToString();
            TrySetProxy();
        }

        private void UIToSettings()
        {
            settings.UseHome = chkHome.Checked;

            settings.HomePage = txtHome.Text;
            settings.RemoteTime = trkRemote.Value;
            settings.PluginName = txtName.Text;
            settings.BlankBrowser = chkBlank.Checked;
            settings.StatusBar = chkStatus.Checked;
            settings.OSD = chkOSD.Checked;
            settings.Windowed = chkWindowed.Checked;
            settings.UseMouse = chkMouse.Checked;
            settings.DisableAero = chkDisableAero.Checked;

            settings.DefaultZoom_percentage = trkZoom.Value;
            settings.FontZoom_percentage = trkFont.Value;
            settings.ZoomPage = optZoomPage.Checked;
            settings.ZoomDomain = optZoomDomain.Checked;

            settings.UseThumbs = chkUseThumbs.Checked;
            settings.CacheThumbs = chkThumbsOnVisit.Checked;

            settings.Remote = chkRemote.Checked;
            settings.Remote_Confirm = (Action.ActionType)cmbConfirmLink.SelectedItem;
            settings.Remote_Bookmark = (Action.ActionType)cmbShowBookmarks.SelectedItem;
            settings.Remote_Zoom_In = (Action.ActionType)cmbZoomIn.SelectedItem;
            settings.Remote_Zoom_Out = (Action.ActionType)cmbZoomOut.SelectedItem;
            settings.Remote_Status = (Action.ActionType)cmbStatusBar.SelectedItem;

            settings.View = (GUIFacadeControl.Layout)cmbBookmarkView.SelectedItem;
            if (cbOverrideUserAgent.Checked)
                settings.UserAgent = cmbUserAgent.Text;
            else
                settings.UserAgent = String.Empty;

            settings.PreviousTags = Settings.StringToTags(textBoxPrevious.Text);
            settings.NextTags = Settings.StringToTags(textBoxNext.Text);

            settings.UseProxy = chkProxy.Checked;
            settings.Server = txtHttpServer.Text;
            settings.Port = Int32.Parse(txtHttpPort.Text);
        }

        #region zoom & font
        private void trkRemote_ValueChanged(object sender, EventArgs e)
        {
            txtRemote.Text = "Reset link ID after " + (((decimal)trkRemote.Value) / 10) + " s";
        }
        private void trkZoom_ValueChanged(object sender, EventArgs e)
        {
            txtZoom.Text = "Default zoom " + trkZoom.Value + "%";
        }
        private void trkFont_ValueChanged(object sender, EventArgs e)
        {
            txtFont.Text = "Default font " + trkFont.Value + "%";
        }
        #endregion

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            TreeNode n = treeView1.SelectedNode;
            BookmarkItem bkm = n.Tag as BookmarkItem;

            if (bkm != null)
            {
                pictureBox1.Image = null;
                GetThumb thumb = new GetThumb();
                thumb.SelectedUrl = bkm.Url;

                thumb.ShowDialog();

                pictureBox1.Image = Bookmark.GetSnap(bkm.Url);
            }
        }

        private void TrySetProxy()
        {
            try
            {
                int port = Convert.ToInt32(txtHttpPort.Text);
                SetProxy(txtHttpServer.Text, port, chkProxy.Checked);
            }
            catch { }
        }
        private void SetProxy(string Server, int Port, bool useProxy)
        {
            // http://geckofx.org/viewtopic.php?id=832
            GeckoPreferences.User["network.proxy.http"] = Server;
            GeckoPreferences.User["network.proxy.http_port"] = Port;
            GeckoPreferences.User["network.proxy.type"] = useProxy ? 1 : 0;

            // maybe possible... not sure...
            // network.proxy.login
            // network.proxy.password
        }

        private void txtHttpServer_TextChanged(object sender, EventArgs e)
        {
            TrySetProxy();
        }
        private void txtHttpPort_TextChanged(object sender, EventArgs e)
        {
            TrySetProxy();
        }
        private void chkProxy_CheckedChanged(object sender, EventArgs e)
        {
            TrySetProxy();
        }

        private TreeNode FindNode(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == name)
                    return node;
                TreeNode tmp = FindNode(node.Nodes, name);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        private void DoImport(string nodeName, ImportBrowser.BrowserType browserType)
        {
            #region generate folder
            TreeNode importRootNode = FindNode(treeView1.Nodes, nodeName);
            if (importRootNode == null)
            {
                importRootNode = treeView1.Nodes[0].Nodes.Add(nodeName);
                importRootNode.ImageIndex = 1;
                importRootNode.SelectedImageIndex = 1;

                BookmarkFolder bmf = new BookmarkFolder();
                bmf.Name = nodeName;
                importRootNode.Tag = bmf;

                treeView1.Nodes[0].ExpandAll();
            }
            #endregion

            ImportBrowser import = new ImportBrowser(importRootNode, imageList1, browserType);
            import.ShowDialog();

            treeView1.Invalidate();
        }


        private void btnImportIE_Click(object sender, EventArgs e)
        {
            DoImport("Import IE", ImportBrowser.BrowserType.IE);
        }

        private void btnImportFF_Click(object sender, EventArgs e)
        {
            DoImport("Import FF", ImportBrowser.BrowserType.FireFox);
        }

        private void btnImportChr_Click(object sender, EventArgs e)
        {
            DoImport("Import Chrome", ImportBrowser.BrowserType.Chrome);
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            cmbConfirmLink.SelectedItem = Settings.Default_Remote_Confirm;
            cmbShowBookmarks.SelectedItem = Settings.Default_Remote_Bookmark;
            cmbZoomIn.SelectedItem = Settings.Default_Remote_Zoom_In;
            cmbZoomOut.SelectedItem = Settings.Default_Remote_Zoom_Out;
            cmbStatusBar.SelectedItem = Settings.Default_Remote_Status;
            cmbPageUp.SelectedItem = Settings.Default_Remote_PageUp;
            cmbPageDown.SelectedItem = Settings.Default_Remote_PageDown;
        }

        private void cbOverrideUserAgent_CheckedChanged(object sender, EventArgs e)
        {
            cmbUserAgent.Enabled = cbOverrideUserAgent.Checked;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

    }
}
