using System;
using System.Collections.Generic;
using MediaPortal.Common;
using MediaPortal.Common.PathManager;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace BrowseTheWeb
{
    public class BookmarksModel : IWorkflowModel
    {
        private const string modelId = "621186EE-F16C-475D-9D3C-BC2176D43C09";

        private ItemsList _tree = null;
        private string bookmarksFileName;
        private BookmarkFolder currentList;
        private BookmarkItem currentItem;

        public BookmarksModel()
        {
            bookmarksFileName = BookmarksFilename;
            InitializeTree();
        }

        public static string BookmarksFilename
        {
            get
            {
                return ServiceRegistration.Get<IPathManager>().GetPath(String.Format(@"<CONFIG>\{0}\BrowseTheWeb.Bookmarks.xml", Environment.UserName));
            }
        }

        protected void CreateChildren(ItemsList tree, BookmarkFolder bmf, BookmarkBase selected)
        {
            if (bmf.Parent != null)
            {
                tree.Add(new BookmarkViewModel(bmf.Parent, ".."));
            }
            foreach (BookmarkBase bm in bmf.Items)
            {
                var childItem = new BookmarkViewModel(bm);
                childItem.Selected = bm == selected;
                tree.Add(childItem);
                //if (bm is BookmarkFolder)
                //  CreateChildren(childItem.SubItems, bm as BookmarkFolder);
            }
        }

        public void Select(ListItem item)
        {
            BookmarkViewModel bmv = item as BookmarkViewModel;
            if (bmv == null)
                return;
            if (bmv.Bookmark is BookmarkFolder)
            {
                BookmarkFolder bmf = bmv.Bookmark as BookmarkFolder;
                _tree.Clear();
                CreateChildren(_tree, bmf, currentList);
                _tree.FireChange();
                currentList = bmf;
            }
            else
                if (bmv.Bookmark is BookmarkItem)
                {
                    IWorkflowManager workflowManager = ServiceRegistration.Get<IWorkflowManager>();
                    workflowManager.NavigatePop(1);
                    BookmarkItem bmi = bmv.Bookmark as BookmarkItem;
                    currentItem = bmi;
                    var btwebModel = ServiceRegistration.Get<IWorkflowManager>().GetModel(BrowseTheWebModel.MainModelId) as BrowseTheWebModel;
                    btwebModel.NavigateTo(bmi.Url);
                }
                else
                    MyLog.error("Unknown bookmark type " + bmv.Bookmark.ToString());
        }

        protected void InitializeTree()
        {
            Bookmarks.Instance.LoadFromXml(bookmarksFileName);
            _tree = new ItemsList();
            currentList = Bookmarks.Instance.root;
        }

        protected void DisposeTree()
        {
            _tree = null;
        }

        public ItemsList BookmarksList
        {
            get { return _tree; }
        }

        public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
        {
            return true;
        }

        public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
        {
            // throw new NotImplementedException();
        }

        public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
        {
            //throw new NotImplementedException();
        }

        public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
        {
            _tree.Clear();
            CreateChildren(_tree, currentList, currentItem);
        }

        public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
        {
        }

        public Guid ModelId
        {
            get { return new Guid(modelId); }
        }

        public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
        {
            //throw new NotImplementedException();
        }

        public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
        {
            //throw new NotImplementedException();
        }

        public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
        {
            return ScreenUpdateMode.AutoWorkflowManager;
        }
    }
}
