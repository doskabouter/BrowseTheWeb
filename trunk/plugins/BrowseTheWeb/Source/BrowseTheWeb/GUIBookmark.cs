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
using System.IO;
using System.Drawing;

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
    private GUIFacadeControl facade = null;
    [SkinControlAttribute(2)]
    protected GUIButtonControl btnViewAs = null;
    [SkinControlAttribute(3)]
    protected GUISortButtonControl btnSortBy = null;

    private static string view = string.Empty;

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
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);

      view = "Large icons";
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
      {
        view = xmlreader.GetValueAsString("btWeb", "bookmark", "Large icons");
      }

      LoadFacade(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml", "");
      Bookmark.InitCachePath();
      base.OnPageLoad();
    }
    protected override void OnPageDestroy(int new_windowId)
    {
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
      using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
      {
        xmlwriter.SetValue("btWeb", "bookmark", view);
      }
      base.OnPageDestroy(new_windowId);
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
            GUIPlugin.StartupLink = item.Path;
            GUIWindowManager.ActivateWindow(54537689);
          }
        }
      }

      if (control == btnViewAs)
      {
        switch (view)
        {
          case "Small icons":
            view = "Large icons";
            break;
          case "Large icons":
            view = "List view";
            break;
          case "List view":
            view = "Small icons";
            break;
        }

        string strLine = string.Empty;
        switch (view)
        {
          case "Small icons":
            facade.View = GUIFacadeControl.ViewMode.SmallIcons;
            strLine = GUILocalizeStrings.Get(100);
            break;
          case "Large icons":
            facade.View = GUIFacadeControl.ViewMode.LargeIcons;
            strLine = GUILocalizeStrings.Get(417);
            break;
          case "List view":
            facade.View = GUIFacadeControl.ViewMode.List;
            strLine = GUILocalizeStrings.Get(101);
            break;
        }
        btnViewAs.Label = strLine;
      }
    }

    public void LoadFacade(string Path, string Folder)
    {
      switch (view)
      {
        case "Small icons":
          facade.View = GUIFacadeControl.ViewMode.SmallIcons;
          break;
        case "Large icons":
          facade.View = GUIFacadeControl.ViewMode.LargeIcons;
          break;
        case "List view":
          facade.View = GUIFacadeControl.ViewMode.List;
          break;
      }
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
            BookmarkElement bkm = BookmarkXml.GetData(node);

            string name = bkm.Name.Replace(" ", "_");
            name = name.Replace(".", "_");

            if ((bkm.isFolder) ||
                (!bkm.isFolder) && (!bkm.isSubFolder))
            {
              item = new GUIListItem();
              item.IsFolder = bkm.isFolder;
              item.Label = bkm.Name;
              item.Path = bkm.Url;
              if (item.IsFolder)
              {
                item.IconImage = "defaultFolder.png";
                item.IconImageBig = "defaultFolderBig.png";
              }
              else
              {
                string file = Bookmark.GetSnapPath(bkm.Url);
                item.IconImage = file;
                item.IconImageBig = file;
              }

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
          item.IconImageBig = "defaultFolderBackBig.png";
          facade.Add(item);

          bool found = false;

          XmlNodeList col = xmlDocument.GetElementsByTagName("Entry");
          foreach (XmlNode node in col)
          {
            BookmarkElement bkm = BookmarkXml.GetData(node);

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

                string file = Bookmark.GetSnapPath(bkm.Url);
                item.IconImage = file;
                item.IconImageBig = file;

                facade.Add(item);
              }
            }
          }
        }

        GUIPropertyManager.SetProperty("#itemcount", facade.Count.ToString());
      }
      catch { }
    }
  }
}
