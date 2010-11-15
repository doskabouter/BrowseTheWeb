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
using System.IO;
using MediaPortal.Configuration;

namespace BrowseTheWeb
{
  public partial class ImportIE : Form
  {
    private List<BookmarkElement> EntryList = new List<BookmarkElement>();
    private TreeView tree;
    private bool select = true;

    public ImportIE(TreeView SetupTreeview)
    {
      InitializeComponent();
      tree = SetupTreeview;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      btnImport.Visible = false;
      btnSelect.Visible = false;
      prgState.Visible = true;
      chkThumbs.Enabled = false;

      int max = listBox1.SelectedItems.Count;
      int imported = 0;
      int counter = 1;

      #region get parent
      TreeNode node = null;
      foreach (TreeNode n in tree.Nodes[0].Nodes)
      {
        if (n.Text == "Import IE")
        {
          node = n;
          break;
        }
      }
      #endregion

      if (node != null)
      {
        foreach (Object item in listBox1.SelectedItems)
        {
          Application.DoEvents();
          prgState.Value = (counter * 100 / max);

          string name = (string)item;
          BookmarkElement bkm = GetBookmark(name);

          if (bkm != null)
          {
            if (!Bookmark.Exists(tree, bkm.Name))
            {
              imported++;

              long id = Setup.actualID;
              Setup.IncAndSaveID();

              TreeNode add = node.Nodes.Add(bkm.Url, bkm.Name);

              BookmarkElement addBkm = new BookmarkElement();
              addBkm.Name = bkm.Name;
              addBkm.Url = bkm.Url;
              addBkm.isSubFolder = true;
              addBkm.Id = id;
              add.Tag = addBkm;

              if (chkThumbs.Checked)
              {
                GetThumb thumb = new GetThumb(id);
                thumb.SelectedUrl = bkm.Url;
                thumb.ShowDialog();
              }

              node.ExpandAll();
            }
          }
          counter++;
        }
      }

      MessageBox.Show("Import is done. Imported " + imported.ToString() + " links.");
      this.Close();
    }
    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ImportIE_Load(object sender, EventArgs e)
    {
      string favPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
      string[] favFiles;

      MyLog.debug("Import folder is " + favPath);

      if (Directory.Exists(favPath))
      {
        string[] favDirs = Directory.GetDirectories(favPath);
        MyLog.debug("Found " + favDirs.Length.ToString() + " folder");

        foreach (string folder in favDirs)
        {
          MyLog.debug("Work on folder '" + Path.GetFileName(folder) + "'");

          favFiles = Directory.GetFiles(folder, "*.url", SearchOption.TopDirectoryOnly);
          MyLog.debug(favFiles.Length.ToString() + " files to import");

          foreach (string s in favFiles)
          {
            FileInfo f = new FileInfo(s);
            string name = Path.GetFileNameWithoutExtension(f.Name);

            string url = GetUrlFile(s);

            if (url != null)
            {
              BookmarkElement bkm = new BookmarkElement();
              bkm.Url = url;
              bkm.Name = name;

              EntryList.Add(bkm);
              listBox1.Items.Add(bkm.Name);
            }
          }
        }

        MyLog.debug("Reading root folder");

        favFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Favorites), "*.url", SearchOption.TopDirectoryOnly);
        MyLog.debug(favFiles.Length.ToString() + " files to import");

        foreach (string s in favFiles)
        {
          FileInfo f = new FileInfo(s);
          string name = Path.GetFileNameWithoutExtension(f.Name);

          string url = GetUrlFile(s);

          if (url != null)
          {
            BookmarkElement bkm = new BookmarkElement();
            bkm.Url = url;
            bkm.Name = name;

            EntryList.Add(bkm);
            listBox1.Items.Add(bkm.Name);
          }
        }
        MyLog.debug("Reading finished. Found " + EntryList.Count + " bookmarks");
      }
      else
      {
        MyLog.debug("Directory does not exist.");
      }
    }
    private string GetUrlFile(string File)
    {
      using (StreamReader sr = new StreamReader(File))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();
          if (line.StartsWith("URL="))
            return line.Substring(4);
        }
      }
      return null;
    }
    private BookmarkElement GetBookmark(string Name)
    {
      foreach (BookmarkElement bkm in EntryList)
      {
        if (bkm.Name == Name) return bkm;
      }
      return null;
    }

    private void btnSelect_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < listBox1.Items.Count; i++)
      {
        listBox1.SetSelected(i, select);
      }
      select = !select;
    }
  }
}
