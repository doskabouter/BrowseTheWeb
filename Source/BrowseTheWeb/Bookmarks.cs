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
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Net;
using MediaPortal.Configuration;

using System.Drawing;

namespace BrowseTheWeb
{

    public class Bookmarks
    {
        public static string SavedByMp = "Saved by MP";
        public List<BookmarkBase> root;

        #region singleton
        private static Bookmarks instance;

        public static Bookmarks Instance
        {
            get
            {
                if (instance == null)
                    instance = new Bookmarks();
                return instance;
            }
        }
        #endregion

        private Bookmarks()
        {
            root = new List<BookmarkBase>();
        }

        public void Clear()
        {
            root.Clear();
        }

        private void LoadOldVersion(XmlDocument doc)
        {

            try
            {
                List<BookmarkBase> last = root;
                foreach (XmlNode node in doc.DocumentElement.SelectNodes("//Entry"))
                {
                    if (Convert.ToBoolean(node.SelectSingleNode("isFolder").InnerText))
                    {
                        BookmarkFolder bmf = new BookmarkFolder();
                        bmf.FromXml(node);
                        last = bmf.Items;
                        root.Add(bmf);
                    }
                    else
                    {
                        BookmarkItem item = new BookmarkItem();
                        item.FromXml(node);
                        if (Convert.ToBoolean(node.SelectSingleNode("isSubFolder").InnerText))
                            last.Add(item);
                        else
                            root.Add(item);
                    }
                }

            }
            catch (Exception e)
            {
                MyLog.debug("Exception: " + e.ToString());
            }
        }

        private void SetParent(List<BookmarkBase> items, BookmarkFolder parent)
        {
            foreach (BookmarkBase sub in items)
            {
                BookmarkFolder bmf = sub as BookmarkFolder;
                if (bmf != null)
                {
                    bmf.Parent = parent;
                    SetParent(bmf.Items, bmf);
                }
            }

        }

        public void LoadFromXml(string fileName)
        {
            Clear();
            XmlDocument doc = new XmlDocument();
            if (File.Exists(fileName))
            {
                doc.Load(fileName);

                if (doc.DocumentElement.SelectSingleNode("//isFolder") != null)
                {
                    LoadOldVersion(doc);
                }
                else
                {
                    BookmarkFolder dummy = new BookmarkFolder();
                    dummy.Items = root;
                    dummy.FromXml(doc.DocumentElement);
                }

                SetParent(root, null);
            }
            else
            {
                BookmarkFolder bmf = new BookmarkFolder();
                bmf.Name = SavedByMp;
                root.Add(bmf);
            }

        }

        public bool SaveToXml(string fileName)
        {
            try
            {
                using (XmlTextWriter textWriter = new XmlTextWriter(fileName, null))
                {
                    textWriter.Formatting = Formatting.Indented;
                    textWriter.WriteStartDocument();
                    textWriter.WriteStartElement("Bookmarks");
                    foreach (BookmarkBase item in root)
                    {
                        item.ToXml(textWriter);
                    }

                    textWriter.WriteEndElement();
                    textWriter.WriteEndDocument();
                }
                return true;
            }
            catch (Exception e)
            {
                MyLog.debug("Exception: " + e.ToString());
                return false;
            }
        }

        public bool AddBookmark(string title, string url, string path)
        {
            BookmarkFolder bmf = root.Find(x => x.Name == SavedByMp && x is BookmarkFolder) as BookmarkFolder;
            if (bmf != null)
            {
                bmf.Items.Add(new BookmarkItem() { Name = title, Url = url });
                return true;
            }
            return false;
        }

    }

    public class Bookmark
    {
        public static TreeNode FindNode(TreeView Treeview, string Name)
        {
            foreach (TreeNode t in Treeview.Nodes[0].Nodes)
            {
                if (Name == t.Text)
                    return t;
                foreach (TreeNode sub in t.Nodes)
                {
                    if (Name == sub.Text)
                        return sub;
                }
            }
            return null;
        }

        public static bool Exists(TreeView treeview, string name)
        {
            return FindNode(treeview, name) != null;
        }

        public static bool isValidUrl(string URL)
        {
            try
            {
                Uri urlCheck = new Uri(URL);
                WebRequest request = WebRequest.Create(urlCheck);
                request.Timeout = 10000;

                WebResponse response;

                response = request.GetResponse();
            }
            catch (Exception)
            {
                return false; //url does not exist
            }
            return true;
        }

        public static void SaveSnap(Bitmap Snap, string Url)
        {
            try
            {
                string filename = GetThumbString(Url);
                filename = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + "\\BrowseTheWeb\\" + filename;
                Snap.Save(filename);
            }
            catch (Exception e)
            {
                MyLog.debug("Exception: " + e.ToString());
            }
        }
        public static Bitmap GetSnap(string Url)
        {
            Bitmap snap = null;

            try
            {
                string filename = GetThumbString(Url);
                filename = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + "\\BrowseTheWeb\\" + filename;

                if (File.Exists(filename))
                {
                    snap = (Bitmap)Bitmap.FromFile(filename);
                    return snap;
                }
                else
                    MyLog.debug("Getsnap does not exist");

            }
            catch (Exception e)
            {
                MyLog.debug("Exception: " + e.ToString());
            }

            return snap;
        }

        public static string GetSnapPath(string Url)
        {
            string filename = GetThumbString(Url);
            filename = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + "\\BrowseTheWeb\\" + filename;

            return filename;
        }
        public static void InitCachePath()
        {
            if (!Directory.Exists(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + "\\BrowseTheWeb"))
                Directory.CreateDirectory(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + "\\BrowseTheWeb");


            if (Directory.Exists(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache) + "\\BrowseTheWeb"))
            {
                string[] files = Directory.GetFiles(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache) + "\\BrowseTheWeb", "*.*");
                foreach (string f in files)
                {
                    File.Move(f, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + "\\BrowseTheWeb\\" + Path.GetFileName(f));
                }
            }

        }

        private static string GetThumbString(string Name)
        {
            string result = Name;

            if (result.EndsWith("/")) result = result.Substring(0, result.Length - 1);

            int x = result.IndexOf("//");
            if (x > 0)
            {
                result = result.Substring(x + 2);
            }

            foreach (char c in Path.GetInvalidFileNameChars())
                result = result.Replace(c, '_');
            result = result + ".png";
            return result;
        }
    }
}
