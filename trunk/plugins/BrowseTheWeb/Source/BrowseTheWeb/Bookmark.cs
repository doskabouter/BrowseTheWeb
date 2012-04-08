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
using MediaPortal.Configuration;

using System.Drawing;

namespace BrowseTheWeb
{
    public class Bookmark
    {
        private static XmlTextWriter textWriter;

        public static bool Save(TreeView Treeview, string Path)
        {
            bool result = false;

            try
            {
                textWriter = new XmlTextWriter(Path, null);
                textWriter.Formatting = Formatting.Indented;

                textWriter.WriteStartDocument();
                textWriter.WriteStartElement("Bookmarks");

                foreach (TreeNode t in Treeview.Nodes[0].Nodes)
                {
                    BookmarkElement bkm = (BookmarkElement)t.Tag;
                    if (bkm != null)
                    {
                        WriteOneEntry(bkm);

                        foreach (TreeNode sub in t.Nodes)
                        {
                            BookmarkElement bkm2 = (BookmarkElement)sub.Tag;
                            WriteOneEntry(bkm2);
                        }
                    }
                }

                textWriter.WriteEndElement();

                textWriter.WriteEndDocument();
                textWriter.Close();

                result = true;
            }
            catch
            {
                // error
            }
            finally
            {
                if (textWriter != null) textWriter.Close();
            }

            return result;
        }
        public static void Load(TreeView Treeview, string Path)
        {
            Treeview.Nodes.Clear();

            TreeNode main = Treeview.Nodes.Add("Bookmarks", "Bookmarks");
            main.ImageIndex = 2;
            main.SelectedImageIndex = 2;

            try
            {
                BookmarkXml.LoadBookmarks(Path);
                TreeNode akt = new TreeNode();

                foreach (BookmarkElement bkm in BookmarkXml.BookmarkItems)
                {
                    if (bkm.isFolder)
                    {
                        akt = main.Nodes.Add(bkm.Name);
                        akt.Tag = bkm;
                        akt.ImageIndex = 1;
                        akt.SelectedImageIndex = 1;
                    }
                    if (bkm.isSubFolder)
                    {
                        TreeNode sub = akt.Nodes.Add(bkm.Name);
                        sub.Tag = bkm;
                    }
                    if ((!bkm.isFolder) && (!bkm.isSubFolder))
                    {
                        TreeNode add = main.Nodes.Add(bkm.Name);
                        add.Tag = bkm;
                    }
                }

                Treeview.Invalidate();

            }
            catch { }
        }

        private static void WriteOneEntry(BookmarkElement bkm)
        {
            textWriter.WriteStartElement("Entry");

            textWriter.WriteStartElement("Name");
            textWriter.WriteValue(bkm.Name);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("URL");
            textWriter.WriteValue(bkm.Url);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Visited");
            textWriter.WriteValue(bkm.Visited);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("LastVisited");
            textWriter.WriteValue(bkm.LastVisited);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Created");
            textWriter.WriteValue(bkm.Created);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("isFolder");
            textWriter.WriteValue(bkm.isFolder);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("isSubFolder");
            textWriter.WriteValue(bkm.isSubFolder);
            textWriter.WriteEndElement();

            textWriter.WriteEndElement();

        }

        public static bool Exists(TreeView Treeview, string Name)
        {
            foreach (TreeNode t in Treeview.Nodes[0].Nodes)
            {
                if (Name == t.Text)
                    return true;
                foreach (TreeNode sub in t.Nodes)
                {
                    if (Name == sub.Text)
                        return true;
                }
            }
            return false;
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
            catch { }
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
            }
            catch { }

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
