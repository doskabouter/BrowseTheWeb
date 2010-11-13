#region Copyright (C) 2005-2010 Team MediaPortal

/* 
 *	Copyright (C) 2005-2010 Team MediaPortal
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.IO;

using Skybound.Gecko;
using MediaPortal.Configuration;

namespace BrowseTheWeb
{
  public partial class Setup : Form
  {
    #region declare vars
    private TreeNode sourceNode;

    private string remote_1 = string.Empty;
    private string remote_2 = string.Empty;
    private string remote_3 = string.Empty;
    private string remote_4 = string.Empty;

    #endregion

    public Setup()
    {
      InitializeComponent();

      #region create xulrunner if needed
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\xulrunner";
      string dirCache = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache);

      try
      {
        if (!Directory.Exists(dir))
        {
          System.Diagnostics.ProcessStartInfo procStartInfo =
            new System.Diagnostics.ProcessStartInfo("cmd", "/c unzip.exe -o xulrunner -d ../");
          procStartInfo.WorkingDirectory = dirCache;

          procStartInfo.RedirectStandardOutput = true;
          procStartInfo.UseShellExecute = false;

          procStartInfo.CreateNoWindow = true;

          System.Diagnostics.Process proc = new System.Diagnostics.Process();
          proc.StartInfo = procStartInfo;
          proc.Start();

          string result = proc.StandardOutput.ReadToEnd();
        }
      }
      catch { }
      #endregion

      Xpcom.Initialize(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\xulrunner");
    }

    private void Setup_Load(object sender, EventArgs e)
    {
      Bookmark.InitCachePath();
      Bookmark.Load(treeView1, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");
      treeView1.ExpandAll();

      LoadSettings();

      #region test for remote setup
      foreach (MediaPortal.GUI.Library.Action.ActionType
              val in Enum.GetValues(typeof(MediaPortal.GUI.Library.Action.ActionType)))
      {
        comboBox1.Items.Add(val);
      }

      foreach (Object obj in comboBox1.Items)
      {
        string act = obj.ToString();
        if (act == remote_1)
        {
          comboBox1.SelectedItem = obj;
          break;
        }
      }
      #endregion

      #region add info for keyboard
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
      #endregion
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool result = Bookmark.Save(treeView1, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");
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

      SaveSettings();
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
      Bookmark bkm = (Bookmark)n.Tag;

      if (bkm != null)
      {
        if (bkm.isFolder)
        {
          pictureBox1.Image = null;
          txtLink.Text = bkm.Name;
        }
        else
        {
          txtLink.Text = bkm.Url;
          pictureBox1.Image = Bookmark.GetSnap(bkm.Url);
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
          if (treeView1.SelectedNode.Level == 2)
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

      TreeNode nodeCopy = new TreeNode(sourceNode.Text, sourceNode.ImageIndex, sourceNode.SelectedImageIndex);
      Bookmark sourceBkm = (Bookmark)sourceNode.Tag;
      nodeCopy.Tag = sourceBkm;

      foreach (TreeNode n in sourceNode.Nodes)
      {
        nodeCopy.Nodes.Add((TreeNode)n.Clone());
      }
      #endregion

      if (targetNode != null)
      {
        int level = targetNode.Level;
        Bookmark targetBkm = (Bookmark)targetNode.Tag;
        Bookmark bkm;

        switch (level)
        {
          case 0: // copy to root
            bkm = (Bookmark)nodeCopy.Tag;
            bkm.isSubFolder = false;

            treeView1.Nodes[0].Nodes.Add(nodeCopy);

            sourceNode.Remove();
            treeView1.Invalidate();
            break;
          case 1: // main level      
            if (targetBkm.isFolder) // if target folder
            {
              bkm = (Bookmark)nodeCopy.Tag;
              bkm.isSubFolder = false;

              if (sourceBkm.isFolder) // move folder
              {
                if (sourceNode.Index > targetNode.Index)
                {
                  targetNode.Parent.Nodes.Insert(targetNode.Index, nodeCopy);
                  sourceNode.Remove();
                  treeView1.Invalidate();
                }
                else
                {
                  targetNode.Parent.Nodes.Insert(targetNode.Index + 1, nodeCopy);
                  sourceNode.Remove();
                  treeView1.Invalidate();
                }
              }
              else
              { // move item
                bkm = (Bookmark)nodeCopy.Tag;
                bkm.isSubFolder = true;

                targetNode.Nodes.Add(nodeCopy);
                sourceNode.Remove();

                targetNode.ExpandAll();

                treeView1.Invalidate();
              }
            }
            else
            { // if target item
              if (sourceNode.Index > targetNode.Index)
              {
                bkm = (Bookmark)nodeCopy.Tag;
                bkm.isSubFolder = false;

                targetNode.Parent.Nodes.Insert(targetNode.Index, nodeCopy);
                sourceNode.Remove();
                treeView1.Invalidate();
              }
              else
              {
                bkm = (Bookmark)nodeCopy.Tag;
                bkm.isSubFolder = false;

                targetNode.Parent.Nodes.Insert(targetNode.Index + 1, nodeCopy);
                sourceNode.Remove();
                treeView1.Invalidate();
              }
            }
            break;
          case 2: // sub level (in folder)
            {
              if (!sourceBkm.isFolder) // no folder
              {
                if (sourceNode.Index > targetNode.Index)
                {
                  bkm = (Bookmark)nodeCopy.Tag;
                  bkm.isSubFolder = true;

                  targetNode.Parent.Nodes.Insert(targetNode.Index, nodeCopy);

                  sourceNode.Remove();
                  treeView1.Invalidate();
                }
                else
                {
                  bkm = (Bookmark)nodeCopy.Tag;
                  bkm.isSubFolder = true;

                  targetNode.Parent.Nodes.Insert(targetNode.Index + 1, nodeCopy);
                  sourceNode.Remove();
                  treeView1.Invalidate();
                }
              }
            }
            break;
        }
      }
      else
      {
        // no target
        if (!sourceBkm.isFolder) // no folder
        {
          Bookmark bkm = (Bookmark)nodeCopy.Tag;
          bkm.isSubFolder = false;

          treeView1.Nodes[0].Nodes.Add(nodeCopy);

          sourceNode.Remove();
          treeView1.Invalidate();
        }
      }
    }

    private void contextMenuStrip1_MouseLeave(object sender, EventArgs e)
    {
      contextMenuStrip1.Close();
    }
    #endregion

    #region tree view action add / remove..
    private void onAddFolder(object sender, EventArgs e)
    {
      TreeNode node = treeView1.SelectedNode;

      if (node != null)
      {
        GetFolder get = new GetFolder();
        DialogResult result = get.ShowDialog();

        if (result == DialogResult.OK)
        {
          if (Bookmark.Exists(treeView1, get.SelectedFolderName))
          {
            MessageBox.Show("Name is already exisiting", "Error name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }

          if (node.Level == 0)
          {
            TreeNode newNode = treeView1.Nodes[0].Nodes.Add(get.SelectedFolderName);
            newNode.ImageIndex = 1;
            newNode.SelectedImageIndex = 1;

            Bookmark bkm = new Bookmark();
            bkm.Name = get.SelectedFolderName;
            bkm.isFolder = true;
            newNode.Tag = bkm;

            treeView1.Nodes[0].ExpandAll();
          }
          if (node.Level == 1)
          {
            int x = node.Parent.Nodes.IndexOf(node);
            if (x >= 0)
            {
              TreeNode newNode = node.Parent.Nodes.Insert(x + 1, get.SelectedFolderName);
              newNode.ImageIndex = 1;
              newNode.SelectedImageIndex = 1;

              Bookmark bkm = new Bookmark();
              bkm.Name = get.SelectedFolderName;
              bkm.isFolder = true;
              newNode.Tag = bkm;

              node.Parent.ExpandAll();
            }
          }
        }
      }

    }
    private void onAddBookmark(object sender, EventArgs e)
    {
      TreeNode node = treeView1.SelectedNode;
      if (node != null)
      {
        Bookmark bkm = (Bookmark)node.Tag;

        GetUrl get = new GetUrl();
        get.SelectedName = "new bookmark";
        get.SelectedUrl = @"http://";
        DialogResult result = get.ShowDialog();

        if (result == DialogResult.OK)
        {
          if (Bookmark.Exists(treeView1, get.SelectedName))
          {
            MessageBox.Show("Name is already exisiting", "Error name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
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

          if (bkm != null)
          {
            if (bkm.isFolder)
            {
              TreeNode add = node.Nodes.Add(get.SelectedUrl, get.SelectedName);

              Bookmark addBkm = new Bookmark();
              addBkm.Name = get.SelectedName;
              addBkm.Url = get.SelectedUrl;
              addBkm.isSubFolder = true;
              addBkm.Created = DateTime.Now;
              add.Tag = addBkm;

              node.ExpandAll();
            }
            else
            {
              TreeNode add = node.Parent.Nodes.Add(get.SelectedUrl, get.SelectedName);

              Bookmark addBkm = new Bookmark();
              addBkm.Name = get.SelectedName;
              addBkm.Url = get.SelectedUrl;
              addBkm.Created = DateTime.Now;
              add.Tag = addBkm;

              node.Parent.ExpandAll();
            }
          }
          else
          { // root
            TreeNode add = treeView1.Nodes[0].Nodes.Add(get.SelectedUrl, get.SelectedName);

            Bookmark addBkm = new Bookmark();
            addBkm.Name = get.SelectedName;
            addBkm.Url = get.SelectedUrl;
            addBkm.Created = DateTime.Now;
            add.Tag = addBkm;

            treeView1.Nodes[0].ExpandAll();
          }
        }
      }
    }
    private void onRemoveEntry(object sender, EventArgs e)
    {
      TreeNode node = treeView1.SelectedNode;
      if (node != null)
      {
        Bookmark bkm = (Bookmark)node.Tag;
        if (!bkm.isFolder)
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
        Bookmark bkm = (Bookmark)node.Tag;
        if (bkm.isFolder)
        {
          GetFolder get = new GetFolder();
          get.SelectedFolderName = bkm.Name;
          DialogResult result = get.ShowDialog();

          if (result == DialogResult.OK)
          {
            if ((get.SelectedFolderName != string.Empty) &&
                (get.SelectedFolderName != bkm.Name))
            {
              if (!Bookmark.Exists(treeView1, get.SelectedFolderName))
              {
                bkm.Name = get.SelectedFolderName;
                node.Text = get.SelectedFolderName;

                treeView1.Invalidate();
              }
              else
              {
                MessageBox.Show("Name is already exisiting", "Error name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
              }
            }
          }
        }
        else
        {
          GetUrl get = new GetUrl();
          get.SelectedName = bkm.Name;
          get.SelectedUrl = bkm.Url;
          DialogResult result = get.ShowDialog();

          if (result == DialogResult.OK)
          {
            if (get.SelectedName != bkm.Name)
            {
              if (Bookmark.Exists(treeView1, get.SelectedName))
              {
                MessageBox.Show("Name is already exisiting", "Error name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
              }
            }
            if (get.SelectedUrl != bkm.Url)
            {
              if (!Bookmark.isValidUrl(get.SelectedUrl))
              {
                DialogResult res = MessageBox.Show("The url seems not to be valid !\nContinue anyway ?", "Error home page address", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes) return;
              }
            }

            node.Name = get.SelectedName;

            bkm.Name = get.SelectedName;
            bkm.Url = get.SelectedUrl;

            treeView1.Invalidate();
          }
        }
      }
    }
    #endregion

    private void LoadSettings()
    {
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
      {
        chkHome.Checked = xmlreader.GetValueAsBool("btWeb", "usehome", true);
        txtHome.Text = xmlreader.GetValueAsString("btWeb", "homepage", "http://team-mediaportal.com");
        trkRemote.Value = xmlreader.GetValueAsInt("btWeb", "remotetime", 25);
        txtName.Text = xmlreader.GetValueAsString("btWeb", "name", "Browse Web");
        chkBlank.Checked = xmlreader.GetValueAsBool("btWeb", "blank", false);
        chkStatus.Checked = xmlreader.GetValueAsBool("btWeb", "status", false);
        chkOSD.Checked = xmlreader.GetValueAsBool("btWeb", "osd", true);
        chkWindowed.Checked = xmlreader.GetValueAsBool("btWeb", "window", false);

        trkZoom.Value = xmlreader.GetValueAsInt("btWeb", "zoom", 100);
        trkFont.Value = xmlreader.GetValueAsInt("btWeb", "font", 100);
        optZoomPage.Checked = xmlreader.GetValueAsBool("btWeb", "page", true);
        optZoomDomain.Checked = xmlreader.GetValueAsBool("btWeb", "domain", false);

        chkUseThumbs.Checked = xmlreader.GetValueAsBool("btWeb", "usethumbs", true);
        chkThumbsOnVisit.Checked = xmlreader.GetValueAsBool("btWeb", "cachethumbs", false);

        chkRemote.Checked = xmlreader.GetValueAsBool("btWeb", "remote", false);
        remote_1 = xmlreader.GetValueAsString("btWeb", "key_1", "REMOTE_1");

        chkProxy.Checked = xmlreader.GetValueAsBool("btWeb", "proxy", false);
        txtHttpServer.Text = xmlreader.GetValueAsString("btWeb", "proxy_server", "127.0.0.1");
        txtHttpPort.Text = xmlreader.GetValueAsInt("btWeb", "proxy_port", 8888).ToString();

        TrySetProxy();
      }
    }
    private void SaveSettings()
    {
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
      using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
      {
        xmlwriter.SetValueAsBool("btWeb", "usehome", chkHome.Checked);
        xmlwriter.SetValue("btWeb", "homepage", txtHome.Text);
        xmlwriter.SetValue("btWeb", "remotetime", trkRemote.Value);
        xmlwriter.SetValue("btWeb", "name", txtName.Text);
        xmlwriter.SetValueAsBool("btWeb", "blank", chkBlank.Checked);
        xmlwriter.SetValueAsBool("btWeb", "status", chkStatus.Checked);
        xmlwriter.SetValueAsBool("btWeb", "osd", chkOSD.Checked);
        xmlwriter.SetValueAsBool("btWeb", "window", chkWindowed.Checked);

        xmlwriter.SetValue("btWeb", "zoom", trkZoom.Value);
        xmlwriter.SetValue("btWeb", "font", trkFont.Value);
        xmlwriter.SetValueAsBool("btWeb", "page", optZoomPage.Checked);
        xmlwriter.SetValueAsBool("btWeb", "domain", optZoomDomain.Checked);

        xmlwriter.SetValueAsBool("btWeb", "usethumbs", chkUseThumbs.Checked);
        xmlwriter.SetValueAsBool("btWeb", "cachethumbs", chkThumbsOnVisit.Checked);

        xmlwriter.SetValueAsBool("btWeb", "remote", chkRemote.Checked);
        xmlwriter.SetValue("btWeb", "key_1", comboBox1.SelectedItem.ToString());

        xmlwriter.SetValueAsBool("btWeb", "proxy", chkProxy.Checked);
        xmlwriter.SetValue("btWeb", "proxy_server", txtHttpServer.Text);
        xmlwriter.SetValue("btWeb", "proxy_port", txtHttpPort.Text);
      }
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
      Bookmark bkm = (Bookmark)n.Tag;

      if (bkm != null)
      {
        if (!bkm.isFolder)
        {
          GetThumb thumb = new GetThumb();
          thumb.SelectedUrl = bkm.Url;

          thumb.ShowDialog();
          pictureBox1.Image = Bookmark.GetSnap(bkm.Url);
        }
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
      int ena = 0; if (useProxy) ena = 1;
      GeckoPreferences.User["network.proxy.type"] = ena;

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

    private void btnImportIE_Click(object sender, EventArgs e)
    {
      if (!Bookmark.Exists(treeView1, "Import IE"))
      {
        TreeNode newNode = treeView1.Nodes[0].Nodes.Add("Import IE");
        newNode.ImageIndex = 1;
        newNode.SelectedImageIndex = 1;

        Bookmark bkm = new Bookmark();
        bkm.Name = "Import IE";
        bkm.isFolder = true;
        newNode.Tag = bkm;

        treeView1.Nodes[0].ExpandAll();
      }

      ImportIE import = new ImportIE();
      import.ShowDialog();

      int max = import.EntryList.Count;
      int imported = 0;

      TreeNode node = null;
      foreach (TreeNode n in treeView1.Nodes[0].Nodes)
      {
        if (n.Text == "Import IE")
        {
          node = n;
          break;
        }
      }

      if (node != null)
      {
        foreach (Bookmark bkm in import.EntryList)
        {
          if (!Bookmark.Exists(treeView1, bkm.Name))
          {
            if (!Bookmark.isValidUrl(bkm.Url))
            {
              DialogResult res = MessageBox.Show("The url seems not to be valid !\nContinue anyway ?", "Error home page address", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
              //if (res != DialogResult.Yes) break;
            }

            if (chkUseThumbs.Checked)
            {
              GetThumb thumb = new GetThumb();
              thumb.SelectedUrl = bkm.Url;
              thumb.ShowDialog();
            }

            TreeNode add = node.Nodes.Add(bkm.Url, bkm.Name);

            Bookmark addBkm = new Bookmark();
            addBkm.Name = bkm.Name;
            addBkm.Url = bkm.Url;
            addBkm.isSubFolder = true;
            add.Tag = addBkm;

            node.ExpandAll();

          }
        }
      }
    }
    private void btnImportFF_Click(object sender, EventArgs e)
    {
      SQLite db = new SQLite();

      string path = @"C:\Users\mka\AppData\Roaming\Mozilla\Firefox\Profiles\wyhhe7f5.default\places.sqlite";

      db.OpenDatabase(path);
      //db.OpenDatabase(path);"Data Source=" + path + ";Version=3;New=True;Compress=True;"

      //DataTable table = db.ExecuteQuery("select * from moz_places");
      DataTable table = db.ExecuteQuery("SELECT moz_bookmarks.title,moz_places.url,moz_bookmarks.type FROM moz_bookmarks LEFT JOIN moz_places " +
                                        "WHERE moz_bookmarks.fk = moz_places.id AND moz_bookmarks.title != 'null' AND moz_places.url LIKE '%http%';");


      foreach (DataRow row in table.Rows)
      {
        string t = Convert.ToString(row["title"]);
        string u = Convert.ToString(row["url"]);

        System.Diagnostics.Debug.WriteLine("--------");
        System.Diagnostics.Debug.WriteLine(t);
        System.Diagnostics.Debug.WriteLine(u);
      }

      db.CloseDatabase();
    }
  }
}
