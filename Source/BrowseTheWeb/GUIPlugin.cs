#region Copyright (C) 2005-2011 Team MediaPortal

/* 
 *	Copyright (C) 2005-2011 Team MediaPortal
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
using System.Drawing;
using System.IO;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;

using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Gecko;
using Gecko.DOM;

using Action = MediaPortal.GUI.Library.Action;

namespace BrowseTheWeb
{
    [PluginIcons("BrowseTheWeb.xulrunner.png", "BrowseTheWeb.xulrunnerOff.png")]

    public class GUIPlugin : GUIWindow, ISetupForm
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int ShowCursor(bool bShow);
        [DllImport("dwmapi.dll", EntryPoint = "DwmEnableComposition")]
        protected extern static uint Win32DwmEnableComposition(uint uCompositionAction);

        private const bool logHtml = false;
        private bool mouseVisible = false;
        private bool aeroDisabled = false;

        private LinkHelper linkHelper;

        private Tuple<string, string> prevNextUrls;

        #region Constants
        public const int PluginWindowId = 54537689;
        #endregion

        #region declare vars
        private GeckoWebBrowser webBrowser;
        private OSD_LinkId osd_linkID;

        private string lastDomain = string.Empty;
        private float zoom = Settings.Instance.DefaultZoom;
        private Settings settings = Settings.Instance;

        public static string Parameter = string.Empty;

        public static string loadFav = string.Empty;
        private bool originalMouseSupport;
        private bool originalMouseAutoHide;
        private bool formsAdded = false;

        #endregion

        #region ISetupForm Member

        public string Author()
        {
            return "Doskabouter";
        }
        public bool CanEnable()
        {
            return true;
        }
        public bool DefaultEnabled()
        {
            return true;
        }
        public string Description()
        {
            return "Browse the web and have fun. Webbrowser based on XULrunner.";
        }
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = settings.PluginName;
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = @"hover_browsetheweb.png";
            return true;

        }
        public int GetWindowId()
        {
            return GetID;
        }
        public bool HasSetup()
        {
            return true;
        }
        public string PluginName()
        {
            return "BrowseTheWeb";
        }
        public void ShowPlugin()
        {
            Setup.Setup setup = new Setup.Setup();
            setup.ShowDialog();
        }

        #endregion

        public override int GetID
        {
            get
            {
                return PluginWindowId;
            }
            set
            {
                base.GetID = value;
            }
        }

        public static string StartupLink
        {
            get
            {
                string value = Parameter;
                if (null != value && !string.IsNullOrEmpty(value.Trim()))
                {
                    return value.Trim();
                }
                else
                {
                    value = GUIPropertyManager.GetProperty("#btWeb.startup.link");
                    if (null != value && !string.IsNullOrEmpty(value.Trim()))
                        return value.Trim();
                }
                return string.Empty;
            }
            set { GUIPropertyManager.SetProperty("#btWeb.startup.link", string.IsNullOrEmpty(value) ? " " : value.Trim()); }
        }

        public override bool Init()
        {
            MyLog.debug("Init Browse the web");
            return Load(GUIGraphicsContext.Skin + @"\BrowseTheWeb.xml");
        }

        private void AddForms()
        {
            if (formsAdded)
                return;
            MyLog.debug("Start AddForms");
            try
            {
                Xpcom.Initialize(Settings.XulRunnerPath());
            }
            catch (Exception ex)
            {
                MyLog.debug("Could not find xulrunner under : " + Settings.XulRunnerPath());
                MyLog.debug("Reason : " + ex.Message);
            }

            #region add forms
            webBrowser = new GeckoWebBrowser();
            webBrowser.Name = "BrowseTheWeb";
            webBrowser.NoDefaultContextMenu = true;
            webBrowser.Enabled = false;
            webBrowser.Visible = false;

            GUIGraphicsContext.form.Controls.Add(webBrowser);

            osd_linkID = new OSD_LinkId();
            osd_linkID.VisibleTime = settings.RemoteTime * 100;
            GUIGraphicsContext.form.Controls.Add(osd_linkID);

            string preferenceFile = Path.Combine(Config.GetFolder(Config.Dir.Config), "btwebprefs.js");
            if (File.Exists(preferenceFile))
                GeckoPreferences.Load(preferenceFile);

            #endregion
            TrySetProxy();
            if (!String.IsNullOrEmpty(settings.UserAgent))
                GeckoPreferences.User["general.useragent.override"] = settings.UserAgent;

            formsAdded = true;
            linkHelper = new LinkHelper(webBrowser, ShowSelect, ShowKeyboard);
            MyLog.debug("Finish AddForms");
        }

        private void SetBrowserWindow()
        {
            GUIControl cntrol = GetControl(545376890);
            GUIControl statusBar = GetControl(545376891);
            GUIPropertyManager.SetProperty("#btWeb.statusvisible", settings.StatusBar.ToString());

            Point loc;
            int w, h;
            if (cntrol != null && statusBar != null)
            {
                loc = new Point(cntrol.XPosition, cntrol.YPosition);
                w = cntrol.Width;

                if (settings.StatusBar)
                    h = cntrol.Height - statusBar.Height;
                else
                    h = cntrol.Height;
            }
            else
            {
                loc = new Point(0, 0);
                w = GUIGraphicsContext.form.Width;
                if (settings.StatusBar)
                    h = GUIGraphicsContext.form.Height - 100;
                else
                    h = GUIGraphicsContext.form.Height;
            }

            webBrowser.Location = loc;
            webBrowser.Size = new Size(w, h);
        }

        protected override void OnPageLoad()
        {
            AddForms();
            GUIPropertyManager.SetProperty("#currentmodule", settings.PluginName);

            if (settings.DisableAero && !aeroDisabled)
            {
                uint res = Win32DwmEnableComposition(0);
                MyLog.debug("disable aero result: " + res.ToString());
                webBrowser.Select();
                aeroDisabled = true;
            }

            try
            {
                linkHelper.Init();
                MyLog.debug("Init browser");

                GUIPropertyManager.SetProperty("#btWeb.status", "Init browser");
                GUIPropertyManager.SetProperty("#btWeb.linkid", "");

                if (settings.UseMouse)
                {
                    MyLog.debug("Mouse support is enabled");
                    originalMouseSupport = GUIGraphicsContext.MouseSupport;
                    GUIGraphicsContext.MouseSupport = true;
                    while (ShowCursor(true) < 0) ;
                    FieldInfo fi = GUIGraphicsContext.form.GetType().GetField("AutoHideMouse", BindingFlags.NonPublic | BindingFlags.Instance);
                    originalMouseAutoHide = (bool)fi.GetValue(GUIGraphicsContext.form);
                    fi.SetValue(GUIGraphicsContext.form, false);
                }

                Parameter = _loadParameter;

                #region init browser

                webBrowser.Dock = DockStyle.None;
                SetBrowserWindow();

                webBrowser.Visible = true;

                webBrowser.Enabled = settings.UseMouse;

                MyLog.debug("Create eventhandler");

                webBrowser.DocumentCompleted += new EventHandler<Gecko.Events.GeckoDocumentCompletedEventArgs>(webBrowser_DocumentCompleted);
                webBrowser.StatusTextChanged += new EventHandler(webBrowser_StatusTextChanged);
                webBrowser.CreateWindow += new EventHandler<GeckoCreateWindowEventArgs>(webBrowser_CreateWindow2);

                MyLog.debug("Create dom eventhandler");
                webBrowser.DomKeyDown += new EventHandler<DomKeyEventArgs>(webBrowser_DomKeyDown);

                MyLog.debug("set zoom size to " + settings.FontZoom + "/" + zoom);

                webBrowser.Window.TextZoom = settings.FontZoom;
                webBrowser.GetMarkupDocumentViewer().SetFullZoomAttribute(zoom);

                if (settings.Windowed)
                {
                    MyLog.debug("switch to windowed fullscreen mode");
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SWITCH_FULL_WINDOWED, 0, 0, 0, 0, 0, null);
                    GUIWindowManager.SendMessage(msg);
                }

                string loadFav = StartupLink;
                if (!String.IsNullOrEmpty(loadFav))
                {
                    webBrowser.Navigate(loadFav);
                    MyLog.debug("load favorite " + loadFav);
                    StartupLink = string.Empty;
                }
                else
                {
                    if (String.IsNullOrEmpty(webBrowser.Document.Domain))
                    {
                        if (settings.UseHome)
                        {
                            webBrowser.Navigate(settings.HomePage);
                            MyLog.debug("load home page " + settings.HomePage);
                        }
                        else
                        {
                            webBrowser.Navigate("about:blank");
                            MyLog.debug("load about:blank");
                        }
                    }

                }

                #endregion

                osd_linkID.Location = new System.Drawing.Point((GUIGraphicsContext.form.Width / 2) - (osd_linkID.Width / 2),
                                                               (GUIGraphicsContext.form.Height / 2) - (osd_linkID.Height / 2));
                osd_linkID.Enabled = settings.OSD;

                if (settings.UseMouse)
                    webBrowser.Select();
            }
            catch (Exception ex)
            {
                MyLog.debug("Exception OnLoad : " + ex.Message);
                MyLog.debug("Exception OnLoad : " + ex.StackTrace);
            }

            base.OnPageLoad();
        }

        void webBrowser_CreateWindow2(object sender, GeckoCreateWindowEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            MyLog.debug("page destroy " + new_windowId.ToString());
            if (new_windowId != GUIBookmark.BookmarkWindowId)
            { // not if you got favs
                if (settings.BlankBrowser)
                {
                    webBrowser.Navigate("about:blank");
                    MyLog.debug("blank on destroy");
                }
            }
            settings.SaveToXml(false);
            bool result = Bookmarks.Instance.SaveToXml(Config.GetFolder(Config.Dir.Config) + "\\bookmarks.xml");
            if (!result)
                MyLog.error("Bookmarks could not be saved !");

            webBrowser.Visible = false;
            GUIGraphicsContext.form.Focus();

            osd_linkID.HideOSD();

            webBrowser.DocumentCompleted -= webBrowser_DocumentCompleted;
            webBrowser.StatusTextChanged -= webBrowser_StatusTextChanged;
            webBrowser.DomKeyDown -= webBrowser_DomKeyDown;
            linkHelper.Done();
            if (settings.UseMouse)
            {
                Cursor.Hide();
                GUIGraphicsContext.MouseSupport = originalMouseSupport;
                FieldInfo fi = GUIGraphicsContext.form.GetType().GetField("AutoHideMouse", BindingFlags.NonPublic | BindingFlags.Instance);
                fi.SetValue(GUIGraphicsContext.form, originalMouseAutoHide);
            }
            base.OnPageDestroy(new_windowId);
        }

        private void TrySetProxy()
        {
            try
            {
                if (settings.UseProxy)
                    MyLog.debug("use proxy settings");
                else
                    MyLog.debug("no proxy selected");

                SetProxy(settings.Server, settings.Port, settings.UseProxy);
            }
            catch (Exception ex)
            {
                MyLog.debug("proxy exception : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void SetProxy(string Server, int Port, bool useProxy)
        {
            // http://geckofx.org/viewtopic.php?id=832
            GeckoPreferences.User["network.proxy.http"] = Server;
            GeckoPreferences.User["network.proxy.http_port"] = Port;
            GeckoPreferences.User["network.proxy.type"] = useProxy ? 1 : 0;

            // maybe possible... not sure...
            // network.proxy.login
            // network.proxy.password
        }

        public override bool OnMessage(GUIMessage message)
        {
            //Console.WriteLine("message :" + message.Label);
            return base.OnMessage(message);
        }

        protected override void OnPreviousWindow()
        {
            if (aeroDisabled)
            {
                uint res = Win32DwmEnableComposition(1);
                MyLog.debug("enable aero result: " + res.ToString());
            }
            aeroDisabled = false;
            base.OnPreviousWindow();
        }

        public override void OnAction(Action action)
        {
            GUIPropertyManager.SetProperty("#btWeb.linkid", String.IsNullOrEmpty(osd_linkID.ID) ? String.Empty : "Link ID = " + osd_linkID.ID);
            #region remote diagnostic
            if (settings.Remote)
            {
                if (action.wID != Action.ActionType.ACTION_KEY_PRESSED)
                    GUIPropertyManager.SetProperty("#btWeb.status", DateTime.Now.ToLongTimeString() + " : " +
                                                    action.wID.ToString());
                else
                    GUIPropertyManager.SetProperty("#btWeb.status", DateTime.Now.ToLongTimeString() + " : " +
                                                    action.wID.ToString() + " / " + action.m_key.KeyChar.ToString());
            }
            #endregion

            #region selectable buttons
            if (action.wID == settings.Remote_Confirm)
            {
                if (!mouseVisible)
                {
                    if (!settings.UseMouse)
                    {

                        if (osd_linkID.ID != string.Empty)
                        {
                            MyLog.debug("confirm link pressed");
                            linkHelper.OnLinkId(osd_linkID.ID);
                            osd_linkID.HideOSD();
                        }
                        else
                        {
                            MyLog.debug("confirm2 link pressed, no link present");
                        }
                    }
                }
                else
                {
                    linkHelper.ClickOn(null);
                }
            }
            if (action.wID == settings.Remote_Bookmark)
            {
                GUIWindowManager.ActivateWindow(GUIBookmark.BookmarkWindowId);
                return;
            }
            if ((action.wID == settings.Remote_Zoom_In) ||
                (action.wID == Action.ActionType.ACTION_MUSIC_FORWARD))
            {
                OnZoomIn();
            }
            if ((action.wID == settings.Remote_Zoom_Out) ||
                (action.wID == Action.ActionType.ACTION_MUSIC_REWIND))
            {
                OnZoomOut();
            }
            if (action.wID == settings.Remote_Status)
            {
                settings.StatusBar = !settings.StatusBar;
                SetBrowserWindow();
            }
            if (action.wID == settings.Remote_PageUp)
            {
                OnPageUp();
            }
            if (action.wID == settings.Remote_PageDown)
            {
                OnPageDown();
            }
            #endregion

            switch (action.wID)
            {
                case Action.ActionType.ACTION_MOUSE_CLICK:
                    {
                        break;
                    }
                case Action.ActionType.ACTION_MOUSE_MOVE:
                    if (settings.UseMouse)
                    {

                    }
                    break;
                case Action.ActionType.ACTION_NEXT_SUBTITLE:
                    if (!settings.UseMouse)
                    {
                        if (mouseVisible)
                        {
                            mouseVisible = false;
                            Cursor.Hide();
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream(Properties.Resources.MouseCursor))
                            {
                                GUIGraphicsContext.form.Cursor = new Cursor(memoryStream);
                            }
                            mouseVisible = true;
                            while (ShowCursor(true) < 0) ;
                        }
                    }
                    break;
                case Action.ActionType.ACTION_KEY_PRESSED:
                    if (!settings.UseMouse)
                    {
                        MyLog.debug("action key press=" + action.m_key.KeyChar);
                        if (action.m_key.KeyChar == 27)
                        {
                            // escape
                            if (!osd_linkID.Visible)
                            {
                                //GUIWindowManager.ShowPreviousWindow();
                            }
                            else
                            {
                                osd_linkID.HideOSD();
                            }
                        }
                        else
                            if (action.m_key.KeyChar >= '0' && action.m_key.KeyChar <= '9')
                                osd_linkID.AddChar((char)action.m_key.KeyChar);
                    }
                    break;
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    osd_linkID.HideOSD();
                    break;
                case Action.ActionType.ACTION_PLAY:
                case Action.ActionType.ACTION_MUSIC_PLAY:
                    OnEnterNewLink();
                    return;
                case Action.ActionType.ACTION_PAUSE:
                    webBrowser.Navigate(settings.HomePage);
                    MyLog.debug("load home page " + settings.HomePage);
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "go to homepage");
                    return;
                case Action.ActionType.ACTION_STOP:
                    webBrowser.Navigate("about:blank");
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "Stop");
                    return;
                case Action.ActionType.ACTION_PREV_ITEM:
                case Action.ActionType.ACTION_REWIND:
                    webBrowser.GoBack();
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "go backward");
                    MyLog.debug("navigate go back");
                    return;
                case Action.ActionType.ACTION_NEXT_ITEM:
                case Action.ActionType.ACTION_FORWARD:
                    webBrowser.GoForward();
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "go forward");
                    MyLog.debug("navigate go forward");
                    return;
                case Action.ActionType.ACTION_RECORD:
                    OnAddBookmark();
                    return;

                #region move
                case Action.ActionType.ACTION_MOVE_RIGHT:
                    OnMoveRight();
                    return;
                case Action.ActionType.ACTION_MOVE_LEFT:
                    OnMoveLeft();
                    return;
                case Action.ActionType.ACTION_MOVE_UP:
                    OnMoveUp();
                    return;
                case Action.ActionType.ACTION_MOVE_DOWN:
                    OnMoveDown();
                    return;
                case Action.ActionType.ACTION_SELECT_ITEM:
                    return;
                #endregion
            }

            if (prevNextUrls != null)
                switch (action.wID)
                {
                    case Action.ActionType.ACTION_PREV_CHAPTER:
                        if (!String.IsNullOrEmpty(prevNextUrls.Item1))
                            webBrowser.Navigate(prevNextUrls.Item1);
                        return;
                    case Action.ActionType.ACTION_NEXT_CHAPTER:
                        if (!String.IsNullOrEmpty(prevNextUrls.Item2))
                            webBrowser.Navigate(prevNextUrls.Item2);
                        return;
                }
            base.OnAction(action);
        }

        private void webBrowser_DomKeyDown(object sender, DomKeyEventArgs e)
        {
            if (settings.UseMouse || mouseVisible)
            {
                GeckoHtmlElement element = webBrowser.Document.ActiveElement;
                bool keyIsChar = e.KeyCode >= 65 && (e.KeyCode <= 90);
                if (keyIsChar && ((element is GeckoInputElement) || (element is GeckoTextAreaElement)))
                { }//user is typing text, so don't convert to MediaPortal.GUI.Library.Action
                else
                {
                    Action action = new Action();
                    Key key;
                    //Uppercase keys (f.e. Record=R) isn't recognized, so hack to get mp to find the correct action
                    if (keyIsChar)
                        key = new Key((int)e.KeyCode + 32, 0);
                    else
                        key = new Key((int)e.KeyChar, (int)e.KeyCode);
                    if (ActionTranslator.GetAction(-1, key, ref action) && action.wID != Action.ActionType.ACTION_INVALID)
                        OnAction(action);
                }
            }
        }

        private void OnEnterNewLink()
        {
            GUIGraphicsContext.form.Focus();

            string selectedUrl = "http://";
            if (settings.LastUrl != string.Empty)
            {
                selectedUrl = settings.LastUrl;
            }

            ShowKeyboard("", selectedUrl, false, delegate(string result)
            {
                if (Bookmark.isValidUrl(result))
                {
                    webBrowser.Navigate(result);
                    MyLog.debug("navigate to " + result);

                    settings.LastUrl = result;
                }
                else
                    ShowAlert("Wrong link ?", " The link you entered seems to be not valid.", "Input:", result);
            });

            if (settings.UseMouse)
                webBrowser.Select();

        }
        private void OnAddBookmark()
        {
            GUIGraphicsContext.form.Focus();

            string title = webBrowser.Document.Title;
            string actualUrl = webBrowser.Document.Url.ToString();

            title = title.Replace("\0", "");

            ShowKeyboard("", title, false, delegate(string result)
             {
                 bool hasSaved = Bookmarks.Instance.AddBookmark(title, actualUrl, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");
                 if (hasSaved)
                 {
                     ShowAlert("Bookmark has been saved !", "Title : " + title, "URL : " + actualUrl, "");
                     Bookmark.GetAndSaveSnap(webBrowser, actualUrl);
                 }
                 else
                     ShowAlert("Bookmark could not be saved !", "Title : " + title, "URL : " + actualUrl, "");
             });

            if (settings.UseMouse)
                webBrowser.Select();
        }
        private void OnZoomIn()
        {
            if (zoom < 3) zoom += 0.1f;
            webBrowser.GetMarkupDocumentViewer().SetFullZoomAttribute(zoom);
            if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
        }
        private void OnZoomOut()
        {
            if (zoom > 0.1f) zoom -= 0.1f;
            webBrowser.GetMarkupDocumentViewer().SetFullZoomAttribute(zoom);
            if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
        }
        private void OnMoveLeft()
        {
            if (!mouseVisible)
            {
                if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX - 100, webBrowser.Window.ScrollY);
            }
            else
            {
                Cursor.Position = new Point(Cursor.Position.X - 20, Cursor.Position.Y);
            }
        }
        private void OnMoveRight()
        {
            if (!mouseVisible)
            {
                if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX + 100, webBrowser.Window.ScrollY);
            }
            else
            {
                Cursor.Position = new Point(Cursor.Position.X + 20, Cursor.Position.Y);
            }
        }
        private void OnMoveUp()
        {
            if (!mouseVisible)
            {
                if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX, webBrowser.Window.ScrollY - 100);
            }
            else
            {
                Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - 20);
            }
        }
        private void OnMoveDown()
        {
            if (!mouseVisible)
            {
                if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX, webBrowser.Window.ScrollY + 100);
            }
            else
            {
                Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + 20);
            }
        }
        private void OnPageUp()
        {
            if (!mouseVisible)
            {
                int height = Convert.ToInt32((float)webBrowser.Size.Height / zoom);
                if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX, webBrowser.Window.ScrollY - height + 100);
            }
            else
            {
                //not yet tested
                Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - 20);
            }
        }
        private void OnPageDown()
        {
            if (!mouseVisible)
            {
                int height = Convert.ToInt32((float)webBrowser.Size.Height / zoom);
                if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX, webBrowser.Window.ScrollY + height - 100);
            }
            else
            {
                //not yet tested
                Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + 20);
            }
        }

        private Bitmap CopyBitmap(Bitmap srcBitmap, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);
            g.Dispose();

            return bmp;
        }

        private void ScrollTo(int x, int y)
        {
            webBrowser.Window.ScrollTo(x, y);
        }

        private void webBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            GeckoWebBrowser web = (GeckoWebBrowser)sender;
            if (web.StatusText != string.Empty)
            {
                string str = DateTime.Now.ToLongTimeString();
                str += " ";

                int l = web.StatusText.Length;
                if (l > 50) l = 47;

                str += web.StatusText.Substring(0, l);
                if (l > 50) str += "...";

                GUIPropertyManager.SetProperty("#btWeb.status", str);
            }
        }

        private void webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            MyLog.debug("page completed : " + webBrowser.Url.ToString());

            GUIPropertyManager.SetProperty("#btWeb.linkid", webBrowser.Url.ToString());
            try
            {
                #region MP gui stuff
                string str = DateTime.Now.ToLongTimeString();
                str += " Completed";

                GUIPropertyManager.SetProperty("#btWeb.status", str);
                #endregion

                if (!settings.UseMouse)
                    prevNextUrls = DomHelper.AddLinksToPage(webBrowser.Document, settings);

                #region reset zoom
                if (settings.ZoomPage)
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
                }
                #endregion
                if (logHtml)
                {
                    using (System.IO.StreamWriter tw = new System.IO.StreamWriter(@"d:\last.html"))
                    {
                        tw.WriteLine(((GeckoHtmlElement)webBrowser.Document.DocumentElement).OuterHtml);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.debug("on completed exception : " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void ShowKeyboard(string title, string value, bool PasswordInput, Action<string> action)
        {
            webBrowser.Visible = false;
            VirtualKeyboard vk = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            vk.Reset();
            vk.Password = PasswordInput;
            vk.Text = value;
            vk.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
            vk.DoModal(GUIWindowManager.ActiveWindow);

            if (vk.IsConfirmed)
                action(vk.Text);
            webBrowser.Visible = true;
        }

        public static void ShowAlert(String headline, String line1, String line2, String line3)
        {
            GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            dlg.SetHeading(headline);
            dlg.SetLine(1, line1);
            dlg.SetLine(2, line2);
            dlg.SetLine(3, line3);
            dlg.DoModal(GUIWindowManager.ActiveWindow);
        }

        public void ShowSelect(GeckoSelectElement select)
        {
            webBrowser.Visible = false;

            GUIDialogSelect2 dlgMenu = (GUIDialogSelect2)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT2);
            dlgMenu.Reset();
            dlgMenu.SetHeading(select.Name);
            dlgMenu.SelectedLabel = select.SelectedIndex;
            GeckoOptionsCollection options = select.Options;
            for (uint i = 0; i < options.Length; i++)
            {
                GeckoOptionElement option = options.item(i);
                dlgMenu.Add(option.Label);
            }
            dlgMenu.DoModal(GUIWindowManager.ActiveWindow);
            webBrowser.Visible = true;

            if (dlgMenu.SelectedLabel == -1)
                return;
            select.SelectedIndex = dlgMenu.SelectedLabel;
            return;

        }

        private void OnRenderSound(string strFilePath)
        {
            MediaPortal.Util.Utils.PlaySound(strFilePath, false, true);
        }
    }
}
