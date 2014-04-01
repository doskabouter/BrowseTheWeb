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


using MediaPortal.GUI.Library;
using MediaPortal.Configuration;


namespace BrowseTheWeb
{
    public class BtwebGuiListItem : GUIListItem
    {
        public BookmarkBase bookmark;
    }

    public class GUIBookmark : GUIWindow
    {
        [SkinControlAttribute(50)]
        private GUIFacadeControl facade = null;
        [SkinControlAttribute(2)]
        protected GUIButtonControl btnViewAs = null;
        [SkinControlAttribute(3)]
        protected GUISortButtonControl btnSortBy = null;

        public const int BookmarkWindowId = 54537688;

        public override int GetID
        {
            get
            {
                return BookmarkWindowId;
            }
            set
            {
                base.GetID = value;
            }
        }

        public override bool Init()
        {
            MyLog.debug("Init Browse the web bookmarks");
            bool result = Load(GUIGraphicsContext.Skin + @"\BrowseTheWebBook.xml");
            Bookmarks.Instance.LoadFromXml(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");

            return result;
        }

        protected override void OnPageLoad()
        {
            LoadFacade(null);
            Bookmark.InitCachePath();
            base.OnPageLoad();
        }
        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM)
            {
                BtwebGuiListItem item = facade.SelectedListItem as BtwebGuiListItem;
                if (item != null)
                {
                    if (item.IsFolder)
                    {
                        LoadFacade((BookmarkFolder)item.bookmark);
                    }
                    else
                    {
                        GUIPlugin.StartupLink = item.Path;
                        if (GUIWindowManager.GetPreviousActiveWindow() == GUIPlugin.PluginWindowId)
                            GUIWindowManager.ShowPreviousWindow();
                        else
                            GUIWindowManager.ActivateWindow(GUIPlugin.PluginWindowId);
                    }
                }
            }

            if (control == btnViewAs)
            {
                Settings.Instance.View++;
                if (Settings.Instance.View > GUIFacadeControl.Layout.LargeIcons)
                    Settings.Instance.View = GUIFacadeControl.Layout.List;

                string strLine = string.Empty;
                facade.CurrentLayout = Settings.Instance.View;
                switch (Settings.Instance.View)
                {
                    case GUIFacadeControl.Layout.SmallIcons:
                        strLine = GUILocalizeStrings.Get(100);
                        break;
                    case GUIFacadeControl.Layout.LargeIcons:
                        strLine = GUILocalizeStrings.Get(417);
                        break;
                    case GUIFacadeControl.Layout.List:
                        strLine = GUILocalizeStrings.Get(101);
                        break;
                }
                btnViewAs.Label = strLine;
            }
        }

        public void LoadFacade(BookmarkFolder parent)
        {
            facade.CurrentLayout = Settings.Instance.View;
            facade.Clear();
            if (parent != null)
            {
                BtwebGuiListItem item = new BtwebGuiListItem();
                item.bookmark = parent.Parent;
                item.IsFolder = true;
                item.Label = "..";
                item.Path = "..";
                item.IconImage = "defaultFolderBack.png";
                item.IconImageBig = "defaultFolderBackBig.png";
                facade.Add(item);
            }

            BookmarkFolder bmf = parent == null ? Bookmarks.Instance.root : parent;

            foreach (BookmarkBase bookmark in bmf.Items)
            {
                BtwebGuiListItem item = new BtwebGuiListItem();
                item.IsFolder = bookmark is BookmarkFolder;
                item.Label = bookmark.Name;
                item.bookmark = bookmark;

                if (item.IsFolder)
                {
                    item.IconImage = "defaultFolder.png";
                    item.IconImageBig = "defaultFolderBig.png";
                }
                else
                {
                    item.Path = ((BookmarkItem)bookmark).Url;
                    string file = Bookmark.GetSnapPath(item.Path);
                    item.IconImage = file;
                    item.IconImageBig = file;
                }

                facade.Add(item);
            }
            GUIPropertyManager.SetProperty("#itemcount", facade.Count.ToString());
            facade.SelectedListItemIndex = 0;

        }
    }
}
