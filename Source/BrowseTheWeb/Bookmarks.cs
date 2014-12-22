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
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Net;

using System.Drawing;

using Gecko;
using Gecko.Utils;
using TreeView = System.Windows.Forms.TreeView;

namespace BrowseTheWeb
{

    public class Bookmarks
    {
        public static string SavedByMp = "Saved by MP";
        public BookmarkFolder root;

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
            root = new BookmarkFolder();
            root.Parent = null;
        }

        public void Clear()
        {
            root.Items.Clear();
        }

        private void LoadOldVersion(XmlDocument doc)
        {

            try
            {
                BookmarkFolder last = root;
                foreach (XmlNode node in doc.DocumentElement.SelectNodes("//Entry"))
                {
                    if (Convert.ToBoolean(node.SelectSingleNode("isFolder").InnerText))
                    {
                        BookmarkFolder bmf = new BookmarkFolder();
                        bmf.FromXml(node);
                        last = bmf;
                        root.Items.Add(bmf);
                    }
                    else
                    {
                        BookmarkItem item = new BookmarkItem();
                        item.FromXml(node);
                        if (Convert.ToBoolean(node.SelectSingleNode("isSubFolder").InnerText))
                            last.Items.Add(item);
                        else
                            root.Items.Add(item);
                    }
                }

            }
            catch (Exception e)
            {
                MyLog.debug("Exception: " + e.ToString());
            }
        }

        private void SetParent(BookmarkFolder item)
        {
            foreach (BookmarkBase sub in item.Items)
            {
                BookmarkFolder bmf = sub as BookmarkFolder;
                if (bmf != null)
                {
                    bmf.Parent = item;
                    SetParent(bmf);
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
                    File.Copy(fileName, Path.ChangeExtension(fileName, ".bak"));
                    LoadOldVersion(doc);
                }
                else
                {
                    root.FromXml(doc.DocumentElement);
                }

                SetParent(root);
            }
            else
            {
                BookmarkFolder bmf = new BookmarkFolder();
                bmf.Name = SavedByMp;
                root.Items.Add(bmf);
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
                    foreach (BookmarkBase item in root.Items)
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
            BookmarkFolder bmf = root.Items.Find(x => x.Name == SavedByMp && x is BookmarkFolder) as BookmarkFolder;
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
        private static string ThumbDir = VersionSpecific.ThumbDir;

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

        private static void SaveSnap(Image Snap, string Url)
        {
            try
            {
                string filename = GetSnapPath(Url);
                Snap.Save(filename);
            }
            catch (Exception e)
            {
                MyLog.debug("Exception: " + e.ToString());
            }
        }

        public static Bitmap GetSnap(string Url)
        {
            try
            {
                string filename = GetSnapPath(Url);

                if (File.Exists(filename))
                {
                    return (Bitmap)Bitmap.FromFile(filename);
                }
                else
                    MyLog.debug("Getsnap " + filename + " does not exist");

            }
            catch (Exception e)
            {
                MyLog.debug("Exception: " + e.ToString());
            }

            return null;
        }

        public static string GetSnapPath(string Url)
        {
            string filename = GetThumbString(Url);
            return Path.Combine(ThumbDir, filename);
        }

        public static void InitCachePath()
        {
            if (!Directory.Exists(ThumbDir))
                Directory.CreateDirectory(ThumbDir);
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

        public static bool GetAndSaveSnap(GeckoWebBrowser browser, string url)
        {
            if (browser.Url.ToString() != "about:blank")
            {
                using (Bitmap snap = browser.GetBitmap((uint)browser.Width, (uint)browser.Height))
                {
                    using (Image newImage = VersionSpecific.Resize(snap))
                    {
                        using (Graphics g = Graphics.FromImage(newImage))
                        {
                            g.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(1, 1, newImage.Width - 2, newImage.Height - 2));
                        }
                        Bookmark.SaveSnap(newImage, url);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
