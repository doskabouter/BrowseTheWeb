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
using System.Text;
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
    private static XmlDocument xmlDocument;

    // main
    public string Name = string.Empty;
    public string Url = string.Empty;
    // statistics
    public int Visited = 0;
    public DateTime LastVisited;
    public DateTime Created = DateTime.Now;
    // helper for folder / sub-folder
    public bool isFolder = false;
    public bool isSubFolder = false;

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
          Bookmark bkm = (Bookmark)t.Tag;
          if (bkm != null)
          {
            WriteOneEntry(bkm);

            foreach (TreeNode sub in t.Nodes)
            {
              Bookmark bkm2 = (Bookmark)sub.Tag;
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
        xmlDocument = new XmlDocument();
        xmlDocument.Load(Path);

        TreeNode akt = new TreeNode();

        XmlNodeList col = xmlDocument.GetElementsByTagName("Entry");
        foreach (XmlNode node in col)
        {
          Bookmark bkm = GetData(node);

          if (bkm.isFolder)
          {
            akt = main.Nodes.Add(bkm.Name);
            akt.Tag = bkm;
            akt.ImageIndex = 1;
            akt.SelectedImageIndex = 1;
          }
          if (bkm.isSubFolder)
          {
            string name = bkm.Name.Replace("\0", "");
            TreeNode sub = akt.Nodes.Add(name);
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

    public static void AddSavedFolder(string Path)
    {
      if (!File.Exists(Path))
      {
        string s = "<?xml version=\"1.0\"?>\n<Bookmarks />";
        StreamWriter sr = new StreamWriter(Path);
        sr.Write(s);
        sr.Close();
      }
      try
      {
        xmlDocument = new XmlDocument();
        xmlDocument.Load(Path);

        bool found = false;

        foreach (XmlNode r in xmlDocument.ChildNodes)
        {
          foreach (XmlNode one in r.ChildNodes)
          {
            if (one.FirstChild.InnerText == "Saved by MP") found = true;
          }
        }

        if (!found)
        {
          XmlElement childElement = xmlDocument.CreateElement("Entry");

          XmlElement sub1 = xmlDocument.CreateElement("Name"); sub1.InnerText = "Saved by MP";
          childElement.AppendChild(sub1);
          XmlElement sub2 = xmlDocument.CreateElement("URL"); sub2.InnerText = string.Empty;
          childElement.AppendChild(sub2);
          XmlElement sub3 = xmlDocument.CreateElement("Visited"); sub3.InnerText = "0";
          childElement.AppendChild(sub3);
          XmlElement sub4 = xmlDocument.CreateElement("LastVisited"); sub4.InnerText = "0001-01-01T00:00:00";
          childElement.AppendChild(sub4);
          XmlElement sub5 = xmlDocument.CreateElement("Created"); sub5.InnerText = "0001-01-01T00:00:00";
          childElement.AppendChild(sub5);
          XmlElement sub6 = xmlDocument.CreateElement("isFolder"); sub6.InnerText = "true";
          childElement.AppendChild(sub6);
          XmlElement sub7 = xmlDocument.CreateElement("isSubFolder"); sub7.InnerText = "false";
          childElement.AppendChild(sub7);

          XmlNode parentNode = xmlDocument.SelectSingleNode("Bookmarks");
          parentNode.InsertBefore(childElement, parentNode.FirstChild);
        }
        xmlDocument.Save(Path);
      }
      catch
      { }
    }
    public static bool SavaBookmark(string Title, string Url, string Path)
    {
      try
      {
        xmlDocument = new XmlDocument();
        xmlDocument.Load(Path);

        bool found = false;
        XmlNode folder = null;

        foreach (XmlNode r in xmlDocument.ChildNodes)
        {

          foreach (XmlNode one in r.ChildNodes)
          {
            if (one.FirstChild.InnerText == Title) found = true;
            if (one.FirstChild.InnerText == "Saved by MP") folder = one;
          }
        }

        if ((!found) && (folder != null))
        {
          XmlElement childElement = xmlDocument.CreateElement("Entry");

          XmlElement sub1 = xmlDocument.CreateElement("Name"); sub1.InnerText = Title;
          childElement.AppendChild(sub1);
          XmlElement sub2 = xmlDocument.CreateElement("URL"); sub2.InnerText = Url;
          childElement.AppendChild(sub2);
          XmlElement sub3 = xmlDocument.CreateElement("Visited"); sub3.InnerText = "0";
          childElement.AppendChild(sub3);
          XmlElement sub4 = xmlDocument.CreateElement("LastVisited"); sub4.InnerText = "0001-01-01T00:00:00";
          childElement.AppendChild(sub4);
          XmlElement sub5 = xmlDocument.CreateElement("Created"); sub5.InnerText = "0001-01-01T00:00:00";
          childElement.AppendChild(sub5);
          XmlElement sub6 = xmlDocument.CreateElement("isFolder"); sub6.InnerText = "false";
          childElement.AppendChild(sub6);
          XmlElement sub7 = xmlDocument.CreateElement("isSubFolder"); sub7.InnerText = "true";
          childElement.AppendChild(sub7);

          XmlNode parentNode = xmlDocument.SelectSingleNode("Bookmarks");
          parentNode.InsertAfter(childElement, folder);

          xmlDocument.Save(Path);
          return true;
        }

      }
      catch
      { }
      return false;
    }

    private static void WriteOneEntry(Bookmark bkm)
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
    private static Bookmark GetData(XmlNode Node)
    {
      Bookmark result = new Bookmark();

      result.Name = Node.SelectSingleNode("Name").InnerText;
      result.Url = Node.SelectSingleNode("URL").InnerText;

      result.Visited = Convert.ToInt32(Node.SelectSingleNode("Visited").InnerText);
      result.LastVisited = Convert.ToDateTime(Node.SelectSingleNode("LastVisited").InnerText);
      result.Created = Convert.ToDateTime(Node.SelectSingleNode("Created").InnerText);

      result.isFolder = Convert.ToBoolean(Node.SelectSingleNode("isFolder").InnerText);
      result.isSubFolder = Convert.ToBoolean(Node.SelectSingleNode("isSubFolder").InnerText);

      return result;
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
        string filename = Url;

        if (filename.EndsWith("/")) filename = filename.Substring(0, filename.Length - 1);

        int x = filename.IndexOf("//");
        if (x > 0)
        {
          filename = filename.Substring(x + 2);
          filename = filename.Replace("/", "_");
          filename = filename.Replace(".", "_");
          filename = filename + ".png";

          filename = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache) + "\\BrowseTheWeb\\" + filename;
          Snap.Save(filename);
        }
      }
      catch { }
    }
    public static Bitmap GetSnap(string Url)
    {
      Bitmap snap = null;

      try
      {
        string filename = Url;

        if (filename.EndsWith("/")) filename = filename.Substring(0, filename.Length - 1);

        int x = filename.IndexOf("//");
        if (x > 0)
        {
          filename = filename.Substring(x + 2);
          filename = filename.Replace("/", "_");
          filename = filename.Replace(".", "_");
          filename = filename + ".png";

          filename = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache) + "\\BrowseTheWeb\\" + filename;
          if (File.Exists(filename))
            snap = (Bitmap)Bitmap.FromFile(filename);

          return snap;
        }
      }
      catch { }

      return snap;
    }

    public static string GetSnapPath(string Url)
    {
      string filename = Url;
      if (filename.EndsWith("/")) filename = filename.Substring(0, filename.Length - 1);

      int x = filename.IndexOf("//");
      if (x > 0)
      {
        filename = filename.Substring(x + 2);
      }

      filename = filename.Replace("/", "_");
      filename = filename.Replace(".", "_");
      filename = filename + ".png";

      filename = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache) + "\\BrowseTheWeb\\" + filename;

      return filename;
    }
    public static void InitCachePath()
    {
      if(!Directory.Exists(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache) + "\\BrowseTheWeb"))
        Directory.CreateDirectory(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Cache) + "\\BrowseTheWeb");
    }
  }
}
