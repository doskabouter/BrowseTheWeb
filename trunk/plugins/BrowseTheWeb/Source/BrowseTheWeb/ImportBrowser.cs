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

namespace BrowseTheWeb.Setup
{
    public partial class ImportBrowser : Form
    {
        private TreeNode importRootNode;
        private BrowserType browserType;

        public enum BrowserType { IE, FireFox, Chrome };

        public ImportBrowser(TreeNode importRootNode, ImageList imageList, BrowserType browserType)
        {
            InitializeComponent();
            this.importRootNode = importRootNode;
            this.browserType = browserType;
            treeView1.ImageList = imageList;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            btnImport.Visible = false;
            prgState.Visible = true;
            chkThumbs.Enabled = false;

            int max = GetCheckedCount(treeView1.Nodes);
            int counter = 0;

            AddBookmarks(treeView1.Nodes, importRootNode, ref counter, max);

            MessageBox.Show("Import is done. Imported " + counter.ToString() + " links.");
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode childNode in e.Node.Nodes)
                childNode.Checked = e.Node.Checked;
        }

        private void buttonSelAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode childNode in treeView1.Nodes)
                childNode.Checked = true;
        }

        private void buttonDeselAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode childNode in treeView1.Nodes)
                childNode.Checked = false;
        }


        private int GetCheckedCount(TreeNodeCollection nodes)
        {
            int res = 0;
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is BookmarkItem && node.Checked)
                    res++;
                res += GetCheckedCount(node.Nodes);
            }
            return res;
        }

        private void AddBookmarks(TreeNodeCollection src, TreeNode dest, ref int counter, int max)
        {
            foreach (TreeNode node in src)
            {
                if (node.Tag is BookmarkFolder)
                {
                    if (node.Checked)
                    {
                        TreeNode newNode = new TreeNode(node.Text) { Tag = node.Tag };
                        dest.Nodes.Add(newNode);
                        AddBookmarks(node.Nodes, newNode, ref counter, max);
                    }
                    else
                        AddBookmarks(node.Nodes, dest, ref counter, max);
                }
                else
                {
                    if (node.Checked)
                    {
                        dest.Nodes.Add((TreeNode)node.Clone());
                        if (chkThumbs.Checked)
                        {
                            GetThumb thumb = new GetThumb();
                            thumb.SelectedUrl = ((BookmarkItem)node.Tag).Url;
                            thumb.ShowDialog();
                        }

                        counter++;
                        prgState.Value = (counter * 100 / max);
                        Application.DoEvents();
                    }
                }

            }
        }

        private void ImportBase_Load(object sender, EventArgs e)
        {
            try
            {
                switch (browserType)
                {
                    case BrowserType.FireFox: ImportFF(); break;
                    case BrowserType.IE: ImportIE(); break;
                    case BrowserType.Chrome: ImportChrome(); break;
                    default: MyLog.debug("Unsupported browser: " + browserType.ToString()); break;
                }
            }
            catch (Exception ex)
            {
                MyLog.debug("Exception import " + browserType.ToString() + " : " + ex.Message);
                MyLog.debug("StackTrace : " + ex.StackTrace);
            }
        }

        #region FireFox
        private void ImportFFFolder(SQLiteClient client, string parent, List<BookmarkBase> rootNodes)
        {
            SQLiteResultSet result = client.Execute(String.Format("SELECT moz_bookmarks.title,moz_bookmarks.id,moz_places.url FROM moz_bookmarks " +
                "LEFT JOIN moz_places ON moz_bookmarks.fk = moz_places.id " +
                "WHERE moz_bookmarks.parent={0} AND moz_bookmarks.title IS NOT null AND TRIM(moz_bookmarks.title) != '' ORDER BY moz_bookmarks.position;", parent));
            foreach (SQLiteResultSet.Row row in result.Rows)
            {
                string title = row.fields[0].ToString();
                string id = row.fields[1].ToString();
                string url = row.fields[2].ToString();

                if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    BookmarkItem bm = new BookmarkItem() { Name = title, Url = url };
                    rootNodes.Add(bm);
                }
                else
                {
                    BookmarkFolder bmf = new BookmarkFolder() { Name = title };
                    rootNodes.Add(bmf);
                    ImportFFFolder(client, id, bmf.Items);
                }
            }
        }

        private void ImportFF()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\Profiles");

            MyLog.debug("Mozilla folder is " + path);

            if (Directory.Exists(path))
            {
                string[] dir = Directory.GetDirectories(path);
                if (dir.Length == 1)
                {
                    MyLog.debug("Mozilla profile under " + dir[0]);
                    path = Path.Combine(dir[0], "places.sqlite");

                    MyLog.debug("open database");
                    SQLiteClient client = new SQLiteClient(path);

                    BookmarkFolder dummy = new BookmarkFolder();
                    ImportFFFolder(client, "2", dummy.Items);
                    Setup.FillTreeview(dummy, treeView1.Nodes);
                    MyLog.debug("close database");
                    client.Close();
                }
            }
        }
        #endregion

        #region IE
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

        private void ImportIEFolder(string parentDir, List<BookmarkBase> rootNodes)
        {
            if (Directory.Exists(parentDir))
            {

                string[] favDirs = Directory.GetDirectories(parentDir);
                foreach (string dir in favDirs)
                {
                    BookmarkFolder bmf = new BookmarkFolder() { Name = Path.GetFileName(dir) };
                    rootNodes.Add(bmf);
                    ImportIEFolder(dir, bmf.Items);
                }


                string[] favFiles = Directory.GetFiles(parentDir, "*.url", SearchOption.TopDirectoryOnly);
                foreach (string s in favFiles)
                {
                    string url = GetUrlFile(s);

                    if (url != null)
                    {
                        BookmarkItem bm = new BookmarkItem() { Name = Path.GetFileNameWithoutExtension(s), Url = url };
                        rootNodes.Add(bm);
                    }
                }
            }
        }

        private void ImportIE()
        {
            BookmarkFolder dummy = new BookmarkFolder();
            string favPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            if (Directory.Exists(favPath))
            {
                ImportIEFolder(favPath, dummy.Items);
                Setup.FillTreeview(dummy, treeView1.Nodes);
            }
            else
                MyLog.debug("Directory " + favPath + " does not exist.");
        }
        #endregion

        #region Chrome

        private void ImportChromeFolder(List<JSONNode> srcNodes, List<BookmarkBase> rootNodes)
        {
            foreach (JSONNode node in srcNodes)
            {
                switch (node.GetValue("type"))
                {
                    case "folder":
                        {
                            BookmarkFolder bmf = new BookmarkFolder() { Name = node.GetValue("name") };
                            rootNodes.Add(bmf);
                            ImportChromeFolder(node.GetNodes("children", ""), bmf.Items);
                            break;
                        }
                    case "url":
                        {
                            BookmarkItem bm = new BookmarkItem() { Name = node.GetValue("name"), Url = node.GetValue("url") };
                            rootNodes.Add(bm);
                            break;
                        }
                }
            }
        }

        private void ImportChrome()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path += "\\Google\\Chrome\\User Data\\Default\\Bookmarks";

            MyLog.debug("Chrome folder is " + path);

            if (File.Exists(path))
            {
                string s = File.ReadAllText(path);
                JSONNode node = JSONNode.LoadJSON(s);
                List<JSONNode> res = node.GetNodes("roots/bookmark_bar/children", null);
                BookmarkFolder dummy = new BookmarkFolder();
                ImportChromeFolder(res, dummy.Items);
                Setup.FillTreeview(dummy, treeView1.Nodes);
            }
            else
                MyLog.debug("File " + path + " does not exist.");

        }
        #endregion

    }
}
