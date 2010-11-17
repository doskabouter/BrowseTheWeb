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

namespace BrowseTheWeb
{
  public class BookmarkXml
  {
    public static List<BookmarkElement> BookmarkItems = new List<BookmarkElement>();

    private static void InitBookmarks(string Path)
    {
      if (!File.Exists(Path))
      {
        string s = "<?xml version=\"1.0\"?>\n<Bookmarks />";
        StreamWriter sr = new StreamWriter(Path);
        sr.Write(s);
        sr.Close();
      }
    }

    public static bool LoadBookmarks(string Path)
    {
      InitBookmarks(Path);

      BookmarkItems = new List<BookmarkElement>();
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(Path);

        XmlNodeList col = xmlDocument.GetElementsByTagName("Entry");
        foreach (XmlNode node in col)
        {
          BookmarkElement elem = GetData(node);
          BookmarkItems.Add(elem);
        }
      }
      catch
      {
        return false;
      }
      return true;
    }
    public static BookmarkElement GetData(XmlNode Node)
    {
      BookmarkElement result = new BookmarkElement();

      result.Name = Node.SelectSingleNode("Name").InnerText;
      result.Url = Node.SelectSingleNode("URL").InnerText;

      result.Visited = Convert.ToInt32(Node.SelectSingleNode("Visited").InnerText);
      result.LastVisited = Convert.ToDateTime(Node.SelectSingleNode("LastVisited").InnerText);
      result.Created = Convert.ToDateTime(Node.SelectSingleNode("Created").InnerText);

      result.isFolder = Convert.ToBoolean(Node.SelectSingleNode("isFolder").InnerText);
      result.isSubFolder = Convert.ToBoolean(Node.SelectSingleNode("isSubFolder").InnerText);

      return result;
    }

    public static void AddFolder(string Path, string FolderName)
    {
      InitBookmarks(Path);

      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(Path);

        bool found = false;

        foreach (XmlNode r in xmlDocument.ChildNodes)
        {
          foreach (XmlNode one in r.ChildNodes)
          {
            if (one.FirstChild.InnerText == FolderName) found = true;
          }
        }

        if (!found)
        {
          XmlElement childElement = xmlDocument.CreateElement("Entry");

          XmlElement sub1 = xmlDocument.CreateElement("Name"); sub1.InnerText = FolderName;
          childElement.AppendChild(sub1);
          XmlElement sub2 = xmlDocument.CreateElement("URL"); sub2.InnerText = string.Empty;
          childElement.AppendChild(sub2);
          XmlElement sub3 = xmlDocument.CreateElement("ID"); sub3.InnerText = "0";
          childElement.AppendChild(sub3);
          XmlElement sub4 = xmlDocument.CreateElement("Visited"); sub4.InnerText = "0";
          childElement.AppendChild(sub4);
          XmlElement sub5 = xmlDocument.CreateElement("LastVisited"); sub5.InnerText = "0001-01-01T00:00:00";
          childElement.AppendChild(sub5);
          XmlElement sub6 = xmlDocument.CreateElement("Created"); sub6.InnerText = DateTime.UtcNow.ToString("u", null);
          childElement.AppendChild(sub6);
          XmlElement sub7 = xmlDocument.CreateElement("isFolder"); sub7.InnerText = "true";
          childElement.AppendChild(sub7);
          XmlElement sub8 = xmlDocument.CreateElement("isSubFolder"); sub8.InnerText = "false";
          childElement.AppendChild(sub8);

          XmlNode parentNode = xmlDocument.SelectSingleNode("Bookmarks");
          parentNode.InsertBefore(childElement, parentNode.FirstChild);
        }
        xmlDocument.Save(Path);
      }
      catch
      { }
    }
    public static bool AddBookmark(string Title, string Url, string Path, long ID)
    {
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
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
          XmlElement sub3 = xmlDocument.CreateElement("ID"); sub3.InnerText = ID.ToString();
          childElement.AppendChild(sub3);
          XmlElement sub4 = xmlDocument.CreateElement("Visited"); sub4.InnerText = "0";
          childElement.AppendChild(sub4);
          XmlElement sub5 = xmlDocument.CreateElement("LastVisited"); sub5.InnerText = "0001-01-01T00:00:00";
          childElement.AppendChild(sub5);
          XmlElement sub6 = xmlDocument.CreateElement("Created"); sub6.InnerText = DateTime.UtcNow.ToString("u", null);
          childElement.AppendChild(sub6);
          XmlElement sub7 = xmlDocument.CreateElement("isFolder"); sub7.InnerText = "false";
          childElement.AppendChild(sub7);
          XmlElement sub8 = xmlDocument.CreateElement("isSubFolder"); sub8.InnerText = "true";
          childElement.AppendChild(sub8);

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
  }
}