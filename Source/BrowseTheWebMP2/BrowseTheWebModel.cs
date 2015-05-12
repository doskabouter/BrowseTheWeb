//at shutdown: ****** Missing Dispose() call for Gecko.ChromeContext. *******
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using MediaPortal.Common;
using MediaPortal.Common.PathManager;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.SkinEngine.SkinManagement;

using Gecko.DOM;

namespace BrowseTheWeb
{
    public class BrowseTheWebModel : IWorkflowModel, IDisposable
    {
        public static readonly Guid MainModelId = new Guid("1E26DD5F-2630-4647-921F-9CC4376A7CC4");

        private Gecko.GeckoWebBrowser webBrowser;
        private Tuple<string, string> prevNextUrls = null;
        private OSD_LinkId osd_linkID = null;
        private LinkHelper linkHelper;
        private bool dialogOpen;
        private DummyForm dummyForm;
        private MessageHandler messageHandler;

        public BrowseTheWebModel()
        {
            string xulRunnerPath = ServiceRegistration.Get<IPathManager>().GetPath(@"<PLUGINS>\BrowseTheWeb\xulrunner");

            var frm = SkinContext.Form;
            var loc = frm.PointToScreen(frm.ClientRectangle.Location);
            var size = frm.ClientSize;

            ServiceRegistration.Get<ISettingsManager>().Load<Configuration.Settings>().SetValuesToApi();

            Thread thread = new Thread(() =>
            {
                dummyForm = new DummyForm(xulRunnerPath, webBrowser_DocumentCompleted, loc, size);
                dummyForm.Show();
                lock (this)
                    Monitor.Pulse(this);
                Dispatcher.Run();
            });
            thread.Name = MyLog.BrowserThreadName;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            lock (this)
                Monitor.Wait(this);
            webBrowser = dummyForm.webBrowser;

            dummyForm.ExecuteSafely(delegate
            {
                osd_linkID = new OSD_LinkId();
                osd_linkID.Visible = false;
                osd_linkID.Enabled = !Settings.Instance.UseMouse;
                osd_linkID.Location = new System.Drawing.Point((frm.Width / 2) - (osd_linkID.Width / 2),
                                                               (frm.Height / 2) - (osd_linkID.Height / 2));
                osd_linkID.VisibleTime = (int)(Settings.Instance.OSDTime * 1000);
                dummyForm.Controls.Add(osd_linkID);
            });

            linkHelper = new LinkHelper(webBrowser, ShowSelect, ShowKeyboard);
            MyLog.debug("main");

            messageHandler = new MessageHandler(OnResize, OnActivate);

            //NavigateTo("http://google.com");
            //NavigateTo("http://xkcd.com");
            if (Settings.Instance.UseHomePage)
                NavigateTo(Settings.Instance.HomePage);
            else
                NavigateTo("about:blank");
            //NavigateTo(@"http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/18_Contribute/4_Development/GITHub_%28GIT%29");


        }

        private void ShowSelect(GeckoSelectElement select)
        {
            dummyForm.ExecuteSafely(delegate { dummyForm.Visible = false; });
            GeckoOptionsCollection options = select.Options;
            var items = new ItemsList();
            for (uint i = 0; i < options.Length; i++)
            {
                GeckoOptionElement option = options.item(i);
                var listItem = new SelectItem(option.Label, i, select);
                listItem.Selected = i == select.SelectedIndex;
                items.Add(listItem);
            }

            ServiceRegistration.Get<IWorkflowManager>().NavigatePushTransient(
                WorkflowState.CreateTransientState("Select options", select.Name, true, "BTWeb-dialogSelectItems", false, WorkflowType.Dialog),
                new NavigationContextConfig()
                {
                    AdditionalContextVariables = new Dictionary<string, object>
                    {
                        { "Items", items },
                        { "Command", new CommandContainer<SelectItem>(ExecuteSiteOption) }
                   }
                });
            dialogOpen = true;
        }

        private void ExecuteSiteOption(SelectItem option)
        {
            dialogOpen = false;
            dummyForm.ExecuteSafely(delegate
            {
                option.Owner.SelectedIndex = option.Index;
                dummyForm.Visible = true;
            });
        }

        private void OnResize()
        {
            var frm = SkinContext.Form;
            var loc = frm.PointToScreen(frm.ClientRectangle.Location);
            var size = frm.ClientSize;
            dummyForm.ExecuteSafely(delegate
            {
                dummyForm.Resize(loc, size);
            });
        }

        private void OnActivate()
        {
            if (Settings.Instance.UseMouse)
                dummyForm.ExecuteSafely(delegate { dummyForm.Activate(); });
        }

        public string KeyboardText { get; set; }

        public void ShowKeyboard(string title, string value, bool isPassword, Action<string> action)
        {
            dummyForm.ExecuteSafely(delegate { dummyForm.Visible = false; });
            KeyboardText = value;
            ServiceRegistration.Get<IWorkflowManager>().NavigatePushTransient(
                WorkflowState.CreateTransientState("Enter text", title, true, "BTWeb-dialogEnterText", false, WorkflowType.Dialog),
                new NavigationContextConfig()
                {
                    AdditionalContextVariables = new Dictionary<string, object>
                    {
                        { "Command", new CommandContainer(delegate {dummyForm.ExecuteSafely(action,KeyboardText); })}
                    }
                });
            dialogOpen = true;
        }

        #region Keybindings
        public void NumberPressed(char n)
        {
            dummyForm.ExecuteSafely(delegate { osd_linkID.AddChar(n); });
        }

        public void OkPressed()
        {
            var linkId = osd_linkID.ID;
            dummyForm.ExecuteSafely(delegate { osd_linkID.HideOSD(); });
            if (!String.IsNullOrEmpty(linkId))
            {
                dummyForm.ExecuteSafely(delegate
                {
                    linkHelper.OnLinkId(linkId);
                });
            }
        }
        public void ScrollVertical(int dy) { ScrollBy(0, dy); }
        public void ScrollHorizontal(int dx) { ScrollBy(dx, 0); }
        public void PageUp() { ScrollBy(0, 100 - SkinContext.Form.ClientSize.Height); }
        public void PageDown() { ScrollBy(0, SkinContext.Form.ClientSize.Height - 100); }

        public void GotoAboutBlank() { NavigateTo("about:blank"); }
        public void GotoHomePage() { NavigateTo(Settings.Instance.HomePage); }

        public void NextPage()
        {
            if (!Settings.Instance.UseMouse && !String.IsNullOrEmpty(prevNextUrls.Item2))
                webBrowser.Navigate(prevNextUrls.Item2);
        }

        public void PreviousPage()
        {
            if (!Settings.Instance.UseMouse && !String.IsNullOrEmpty(prevNextUrls.Item1))
                webBrowser.Navigate(prevNextUrls.Item1);
        }

        public void Forward() { dummyForm.ExecuteSafely(delegate { webBrowser.GoForward(); }); }
        public void Back() { dummyForm.ExecuteSafely(delegate { webBrowser.GoBack(); }); }

        public void AddBookmark()
        {
            string title = dummyForm.ExecuteSafely(delegate { return webBrowser.Document.Title; });
            ShowKeyboard("Name", title, false, DoAddBookmark);
        }

        public void EnterNewLink()
        {
            string url = @"http://";
            ShowKeyboard("Url", url, false, delegate { NavigateTo(KeyboardText); });
        }

        public void ToggleStatus()
        {
            dummyForm.ToggleStatus();
        }
        #endregion


        private void DoAddBookmark(string title)
        {
            string actualUrl = webBrowser.Document.Url.ToString();
            string bookmarksFileName = BookmarksModel.BookmarksFilename;
            Bookmarks.Instance.LoadFromXml(bookmarksFileName);

            bool isAdded = Bookmarks.Instance.AddBookmark(title, actualUrl);
            if (isAdded)
            {
                Bookmark.GetAndSaveSnap(webBrowser, actualUrl);
                Bookmarks.Instance.SaveToXml(bookmarksFileName);
                MyLog.debug("Bookmark has been saved !" + " Title : " + title + " URL : " + actualUrl);
            }
            else
                MyLog.debug("Bookmark could not be saved !" + " Title : " + title + " URL : " + actualUrl);
        }

        private void ScrollBy(int dx, int dy)
        {
            dummyForm.ExecuteSafely(delegate
            {
                webBrowser.Window.ScrollBy(dx, dy);
            });
        }

        public void NavigateTo(string url)
        {
            dummyForm.ExecuteSafely(delegate { webBrowser.Navigate(url); });
        }

        void webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            dummyForm.Opacity = 1;
            MyLog.debug("page completed : " + webBrowser.Url.ToString());
            dummyForm.SetStatus(webBrowser.Url.ToString());

            //GUIPropertyManager.SetProperty("#btWeb.linkid", webBrowser.Url.ToString());
            try
            {
                #region MP gui stuff
                string str = DateTime.Now.ToLongTimeString();
                str += " Completed";

                //GUIPropertyManager.SetProperty("#btWeb.status", str);
                #endregion

                if (!Settings.Instance.UseMouse)
                    prevNextUrls = DomHelper.AddLinksToPage(webBrowser.Document, Settings.Instance);

                #region reset zoom
                /*if (Settings.Instance.ZoomPage)
                {
                    webBrowser.GetMarkupDocumentViewer().SetFullZoomAttribute(settings.DefaultZoom);
                    zoom = settings.DefaultZoom;
                    GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
                }
                if (settings.ZoomDomain)
                {
                    if (lastDomain != webBrowser.Document.Domain)
                    {
                        {
                            webBrowser.GetMarkupDocumentViewer().SetFullZoomAttribute(settings.DefaultZoom);
                            zoom = settings.DefaultZoom;
                            GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
                        }
                    }
                    lastDomain = webBrowser.Document.Domain;
                }*/
                #endregion
            }
            catch (Exception ex)
            {
                MyLog.debug("on completed exception : " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        #region IWorkflowModel implementation

        public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
        {
            return true;
        }

        public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
        {
            // We could initialize some data here when changing the media navigation state
        }

        public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
        {
            dummyForm.ExecuteSafely(delegate { dummyForm.Visible = true; });
            /*ExecuteGUIThread(delegate
            {
                //InitBrowser();
                //SkinContext.Form.Controls.Add(dummyForm);
                //SkinContext.Form.Controls.Add(osd_linkID);
            });*/
        }

        public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
        {
            dummyForm.ExecuteSafely(delegate
            {
                dummyForm.Visible = false;
                //SkinContext.Form.Controls.Remove(dummyForm);
                //SkinContext.Form.Controls.Remove(osd_linkID);
                //webBrowser.Dispose();
                //webBrowser = null;
                //osd_linkID = null;
            });
        }

        public Guid ModelId
        {
            get { return MainModelId; }
        }

        public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
        {
            dummyForm.ExecuteSafely(delegate { dummyForm.Visible = true; });
        }

        public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
        {
            dummyForm.ExecuteSafely(delegate { dummyForm.Visible = false; });
        }

        public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
        {
        }

        public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
        {
            if (dialogOpen && context.WorkflowModelId == MainModelId)
            {
                dialogOpen = false;
                dummyForm.ExecuteSafely(delegate { dummyForm.Visible = true; });
            }
            return ScreenUpdateMode.AutoWorkflowManager;
        }
        #endregion


        public void Dispose()
        {
            messageHandler.Dispose();
            dummyForm.Dispose();
        }
    }

    class SelectItem : ListItem
    {
        private uint index;
        public int Index { get { return (int)index; } }
        private GeckoSelectElement owner;
        public GeckoSelectElement Owner { get { return owner; } }

        public SelectItem(string name, uint index, GeckoSelectElement owner)
            : base("Name", name)
        {
            this.index = index;
            this.owner = owner;
        }
    }

}
