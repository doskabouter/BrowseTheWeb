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

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.Configuration;

using System.Xml;

namespace BrowseTheWeb
{
  public class GUIBookmark : GUIWindow
  {
    [SkinControlAttribute(50)]
    private GUIListControl facade = null;

    public override int GetID
    {
      get
      {
        return 54537688;
      }
      set
      {
        base.GetID = value;
      }
    }
    public override bool Init()
    {
      bool result = Load(GUIGraphicsContext.Skin + @"\BrowseTheWebBook.xml");
      return result;
    }

    protected override void OnPageLoad()
    {
      LoadFacade(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml", "");
      base.OnPageLoad();
    }
    protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
    {
      if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
      {
        GUIListItem item = facade.SelectedListItem;
        if (item != null)
        {
          if (item.IsFolder)
          {
            if (item.Label == "..")
              LoadFacade(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml", "");
            else
              LoadFacade(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml", item.Label);
          }
          else
          {
            GUIPlugin.loadFav = item.Path;
            GUIWindowManager.ActivateWindow(54537689);
          }
        }
      }
    }

    public void LoadFacade(string Path, string Folder)
    {
      facade.Clear();
      GUIListItem item = new GUIListItem();

      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(Path);

        if (Folder == string.Empty)
        {
          XmlNodeList col = xmlDocument.GetElementsByTagName("Entry");
          foreach (XmlNode node in col)
          {
            Bookmark bkm = GetData(node);

            if ((bkm.isFolder) ||
                (!bkm.isFolder) && (!bkm.isSubFolder))
            {
              item = new GUIListItem();
              item.IsFolder = bkm.isFolder;
              item.Label = bkm.Name;
              item.Path = bkm.Url;
              if (item.IsFolder)
                item.IconImage = "defaultFolder.png";

              facade.Add(item);
            }
          }
        }

        if (Folder != string.Empty)
        {
          item = new GUIListItem();
          item.IsFolder = true;
          item.Label = "..";
          item.Path = "..";
          item.IconImage = "defaultFolderBack.png";
          facade.Add(item);

          bool found = false;

          XmlNodeList col = xmlDocument.GetElementsByTagName("Entry");
          foreach (XmlNode node in col)
          {
            Bookmark bkm = GetData(node);

            if ((bkm.isFolder) || ((!bkm.isSubFolder && !bkm.isFolder))) found = false;

            if (Folder == bkm.Name)
            {
              found = true;
            }
            if (found)
            {
              if (bkm.isSubFolder)
              {
                item = new GUIListItem();
                item.IsFolder = bkm.isFolder;
                item.Label = bkm.Name;
                item.Path = bkm.Url;

                facade.Add(item);
              }
            }
          }
        }


      }
      catch { }
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
  }
}
