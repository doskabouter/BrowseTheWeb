﻿#region Copyright (C) 2005-2010 Team MediaPortal

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


using MediaPortal.Configuration;

namespace BrowseTheWeb
{
  public partial class Setup : Form
  {
    private TreeNode sourceNode;

    public Setup()
    {
      InitializeComponent();
    }

    private void Setup_Load(object sender, EventArgs e)
    {
      Bookmark.Load(treeView1, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");
      treeView1.ExpandAll();

      LoadSettings();

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
      listBox1.Items.Add("ESC\t\tESC\t\tleave plugin");
      listBox1.Items.Add("M\t\tMute\t\ttoggle mute");
      listBox1.Items.Add("X\t\tRed\t\ttoggle statusbar");
      #endregion

      #region create xulrunner
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
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool res = Bookmark.Save(treeView1, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");
      if (!res)
        MessageBox.Show("Bookmarks could not be saved !");
      if ((chkHome.Checked) && (!Bookmark.isValidUrl(txtHome.Text)))
      {
        MessageBox.Show("Please correct homepage adress !");
        return;
      }

      SaveSettings();
      this.Close();
    }
    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
      TreeNode n = (TreeNode)e.Node;
      Bookmark bkm = (Bookmark)n.Tag;

      if (bkm != null)
      {
        if (bkm.isFolder) txtLink.Text = bkm.Name;
        else
          txtLink.Text = bkm.Url;
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
    private void treeView1_DoubleClick(object sender, EventArgs e)
    {
      // browse ???
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
            MessageBox.Show("URL is not valid", "Error name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
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
              add.Tag = addBkm;

              node.ExpandAll();
            }
            else
            {
              TreeNode add = node.Parent.Nodes.Add(get.SelectedUrl, get.SelectedName);

              Bookmark addBkm = new Bookmark();
              addBkm.Name = get.SelectedName;
              addBkm.Url = get.SelectedUrl;
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
                MessageBox.Show("URL is not valid", "Error name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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

    private void LoadSettings()
    {
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
      {
        chkHome.Checked = xmlreader.GetValueAsBool("btWeb", "usehome", true);
        txtHome.Text = xmlreader.GetValueAsString("btWeb", "homepage", "http://team-mediaportal.com");
        trkRemote.Value = xmlreader.GetValueAsInt("btWeb", "remote", 25);
        txtName.Text = xmlreader.GetValueAsString("btWeb", "name", "Browse Web");
        chkBlank.Checked = xmlreader.GetValueAsBool("btWeb", "blank", false);
        chkStatus.Checked = xmlreader.GetValueAsBool("btWeb", "status", false);
        chkOSD.Checked = xmlreader.GetValueAsBool("btWeb", "osd", true);

        trkZoom.Value = xmlreader.GetValueAsInt("btWeb", "zoom", 100);
        trkFont.Value = xmlreader.GetValueAsInt("btWeb", "font", 100);
        optZoomPage.Checked = xmlreader.GetValueAsBool("btWeb", "page", true);
        optZoomDomain.Checked = xmlreader.GetValueAsBool("btWeb", "domain", false);

        chkRemote.Checked = xmlreader.GetValueAsBool("btWeb", "remote", false);
      }
    }
    private void SaveSettings()
    {
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
      using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
      {
        xmlwriter.SetValueAsBool("btWeb", "usehome", chkHome.Checked);
        xmlwriter.SetValue("btWeb", "homepage", txtHome.Text);
        xmlwriter.SetValue("btWeb", "remote", trkRemote.Value);
        xmlwriter.SetValue("btWeb", "name", txtName.Text);
        xmlwriter.SetValueAsBool("btWeb", "blank", chkBlank.Checked);
        xmlwriter.SetValueAsBool("btWeb", "status", chkStatus.Checked);
        xmlwriter.SetValueAsBool("btWeb", "osd", chkOSD.Checked);

        xmlwriter.SetValue("btWeb", "zoom", trkZoom.Value);
        xmlwriter.SetValue("btWeb", "font", trkFont.Value);
        xmlwriter.SetValueAsBool("btWeb", "page", optZoomPage.Checked);
        xmlwriter.SetValueAsBool("btWeb", "domain", optZoomDomain.Checked);

        xmlwriter.SetValueAsBool("btWeb", "remote", chkRemote.Checked);
      }
    }

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
  }
}