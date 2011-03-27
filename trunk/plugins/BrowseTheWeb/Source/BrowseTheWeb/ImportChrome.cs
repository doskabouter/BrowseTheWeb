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
  public partial class ImportChrome : Form
  {
    private List<BookmarkElement> EntryList = new List<BookmarkElement>();
    private TreeView tree;
    private bool select = true;

    public ImportChrome(TreeView SetupTreeview)
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
        if (n.Text == "Import Chrome")
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

              TreeNode add = node.Nodes.Add(bkm.Url, bkm.Name);

              BookmarkElement addBkm = new BookmarkElement();
              addBkm.Name = bkm.Name;
              addBkm.Url = bkm.Url;
              addBkm.isSubFolder = true;
              add.Tag = addBkm;

              if (chkThumbs.Checked)
              {
                GetThumb thumb = new GetThumb();
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

    private void ImportChr_Load(object sender, EventArgs e)
    {
      try
      {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        path += "\\Google\\Chrome\\User Data\\Default";

        MyLog.debug("Chrome folder is " + path);

        if (Directory.Exists(path))
        {
          if (File.Exists(path + "\\Bookmarks"))
          {
            MyLog.debug("Open bookmarks");

            StreamReader sr = new StreamReader(path + "\\Bookmarks");
            string line = sr.ReadToEnd();
            sr.Close();

            MyLog.debug("Read " + line.Length +  " bytes");

            int x = 0;
            int secNo = 1;

            while ((x = line.IndexOf("children\": [", x + 1)) > 0)
            {
              int y = line.IndexOf("]", x + 12);
              string section = line.Substring(x + 12, y - x - 12);

              MyLog.debug("Read section " + secNo);
              secNo++;

              int start = 0;
              while ((start = section.IndexOf("{", start + 1)) > 0)
              {
                int stop = section.IndexOf("}", start + 1);
                string entry = section.Substring(start + 1, stop - start - 3);

                string[] sep = new string[1];
                sep[0] = "\r\n";
                string[] lines = entry.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length; i++)
                {
                  lines[i] = lines[i].Replace("\"", "");
                  lines[i] = lines[i].Trim();
                }

                string name = string.Empty;
                string url = string.Empty;
                string typ = string.Empty;

                for (int i = 0; i < lines.Length; i++)
                {
                  if (lines[i].StartsWith("type:"))
                  {
                    typ = lines[i].Replace("type:", "").Trim();
                    if (typ.EndsWith(",")) typ = typ.Substring(0, typ.Length - 1);
                  }
                  if (lines[i].StartsWith("name:"))
                  {
                    name = lines[i].Replace("name:", "").Trim();
                    if (name.EndsWith(",")) name = name.Substring(0, name.Length - 1);
                  }
                  if (lines[i].StartsWith("url:"))
                  { // with /
                    url = lines[i].Replace("url:", "").Trim();
                  }
                }

                if ((name != "") && (typ == "url") && (url != ""))
                {
                  MyLog.debug("Found bookmark " + name);

                  BookmarkElement bkm = new BookmarkElement();
                  bkm.Url = url;
                  bkm.Name = name;

                  EntryList.Add(bkm);
                  listBox1.Items.Add(bkm.Name);
                }
              }
            }




          }
          
        }
      }
      catch (Exception ex)
      {
        MyLog.debug("Exception import chrome : " + ex.Message);
        MyLog.debug("Exception import chrome : " + ex.StackTrace);
      }
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
