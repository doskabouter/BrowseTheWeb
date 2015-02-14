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

namespace BrowseTheWeb
{
    public class BookmarkBase
    {
        public string Name = String.Empty;
        public DateTime Created = DateTime.Now;

        virtual public void ToXml(XmlTextWriter textWriter)
        {
            textWriter.WriteStartElement("Name");
            textWriter.WriteValue(Name);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Created");
            textWriter.WriteValue(Created);
            textWriter.WriteEndElement();
        }

        virtual public void FromXml(XmlNode node)
        {
            XmlNode nameNode = node.SelectSingleNode("Name");
            if (nameNode != null)
            {
                Name = nameNode.InnerText;
                Created = Convert.ToDateTime(node.SelectSingleNode("Created").InnerText);
            }
        }
    }

    public class BookmarkItem : BookmarkBase
    {
        public string Url = string.Empty;

        public int Visited = 0;
        public DateTime LastVisited;

        public override void ToXml(XmlTextWriter textWriter)
        {
            textWriter.WriteStartElement("Entry");
            base.ToXml(textWriter);

            textWriter.WriteStartElement("URL");
            textWriter.WriteValue(Url);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Visited");
            textWriter.WriteValue(Visited);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("LastVisited");
            textWriter.WriteValue(LastVisited);
            textWriter.WriteEndElement();

            textWriter.WriteEndElement();
        }

        public override void FromXml(XmlNode node)
        {
            base.FromXml(node);
            Url = node.SelectSingleNode("URL").InnerText;

            Visited = Convert.ToInt32(node.SelectSingleNode("Visited").InnerText);
            LastVisited = Convert.ToDateTime(node.SelectSingleNode("LastVisited").InnerText);
        }

    }

    public class BookmarkFolder : BookmarkBase
    {
        public List<BookmarkBase> Items;
        public BookmarkFolder Parent;

        public BookmarkFolder()
        {
            Items = new List<BookmarkBase>();
        }

        public override void ToXml(XmlTextWriter textWriter)
        {
            textWriter.WriteStartElement("Folder");
            base.ToXml(textWriter);
            foreach (BookmarkBase item in Items)
                item.ToXml(textWriter);


            textWriter.WriteEndElement();
        }

        public override void FromXml(XmlNode node)
        {
            base.FromXml(node);
            foreach (XmlNode subNode in node.SelectNodes("Entry|Folder"))
            {
                if (subNode.Name == "Folder")
                {
                    BookmarkFolder item = new BookmarkFolder();
                    item.FromXml(subNode);
                    Items.Add(item);
                }
                else
                {
                    BookmarkItem bmItem = new BookmarkItem();
                    bmItem.FromXml(subNode);
                    Items.Add(bmItem);
                }

            }
        }
    }

}
