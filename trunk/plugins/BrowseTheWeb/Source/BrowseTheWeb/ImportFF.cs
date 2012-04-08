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
using System.Windows.Forms;
using System.IO;

using SQLite.NET;

namespace BrowseTheWeb
{
    public partial class ImportFF : Form
    {
        private List<BookmarkElement> EntryList = new List<BookmarkElement>();
        private TreeView tree;
        private bool select = true;

        public ImportFF(TreeView SetupTreeview)
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
                if (n.Text == "Import FF")
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

        private void ImportFF_Load(object sender, EventArgs e)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path += "\\Mozilla\\Firefox\\Profiles";

                MyLog.debug("Morzilla folder is " + path);

                if (Directory.Exists(path))
                {
                    string[] dir = Directory.GetDirectories(path);
                    if (dir.Length == 1)
                    {
                        MyLog.debug("Morzilla profile under " + dir[0]);
                        path = dir[0] + "\\places.sqlite";

                        MyLog.debug("open database");
                        SQLiteClient client = new SQLiteClient(path);


                        SQLiteResultSet result = client.Execute("SELECT moz_bookmarks.title,moz_places.url,moz_bookmarks.type FROM moz_bookmarks LEFT JOIN moz_places " +
                                                                "WHERE moz_bookmarks.fk = moz_places.id AND moz_bookmarks.title != 'null' AND moz_places.url LIKE '%http%';");

                        MyLog.debug("Morzilla bookmarks found : " + result.Rows.Count);

                        foreach (SQLite.NET.SQLiteResultSet.Row row in result.Rows)
                        {
                            string title = row.fields[0].ToString();
                            string url = row.fields[1].ToString();

                            BookmarkElement bkm = new BookmarkElement();
                            bkm.Url = url;
                            bkm.Name = title;

                            EntryList.Add(bkm);
                            listBox1.Items.Add(bkm.Name);
                        }

                        MyLog.debug("close database");
                        client.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.debug("Exception import ff : " + ex.Message);
                MyLog.debug("Exception import ff : " + ex.StackTrace);
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
