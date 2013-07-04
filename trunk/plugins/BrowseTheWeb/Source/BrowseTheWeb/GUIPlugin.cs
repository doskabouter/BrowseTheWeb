﻿#region Copyright (C) 2005-2011 Team MediaPortal

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
using System.Text;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;

using System.Windows.Forms;
using System.Runtime.InteropServices;

using Gecko;
using Gecko.DOM;

namespace BrowseTheWeb
{
    [PluginIcons("BrowseTheWeb.xulrunner.png", "BrowseTheWeb.xulrunnerOff.png")]

    public class GUIPlugin : GUIWindow, ISetupForm
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int ShowCursor(bool bShow);
        [DllImport("dwmapi.dll", EntryPoint = "DwmEnableComposition")]
        protected extern static uint Win32DwmEnableComposition(uint uCompositionAction);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const bool logHtml = false;
        private bool mouseVisible = false;
        private bool clickFromPlugin = false;
        private bool aeroDisabled = false;

        #region Constants
        public const int PluginWindowId = 54537689;
        #endregion

        #region declare vars
        private GeckoWebBrowser webBrowser;
        private OSD_LinkId osd_linkID;
        private string linkId = string.Empty;
        private int linkTime = 0;
        private Timer timer = new Timer();

        private string lastDomain = string.Empty;
        private float zoom = Settings.Instance.DefaultZoom;
        private Settings settings = Settings.Instance;

        public static string Parameter = string.Empty;

        public static string loadFav = string.Empty;
        private bool originalMouseSupport;
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
            Setup setup = new Setup();
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

            BookmarkXml.AddFolder(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) +
                                  "\\bookmarks.xml", "Saved by MP");

            MyLog.debug("Init Browse the web finished");

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
                MyLog.debug("Could not find xulrunner under : " + Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\xulrunner");
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
            osd_linkID.Visible = false;
            GUIGraphicsContext.form.Controls.Add(osd_linkID);
            string preferenceFile = Path.Combine(Config.GetFolder(Config.Dir.Config), "btwebprefs.js");
            if (File.Exists(preferenceFile))
                GeckoPreferences.Load(preferenceFile);

            #endregion
            TrySetProxy();
            if (!String.IsNullOrEmpty(settings.UserAgent))
                GeckoPreferences.User["general.useragent.override"] = settings.UserAgent;

            formsAdded = true;
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
                MyLog.debug("Init browser");

                GUIPropertyManager.SetProperty("#btWeb.status", "Init browser");
                GUIPropertyManager.SetProperty("#btWeb.linkid", "");
                linkId = string.Empty;

                if (settings.UseMouse)
                {
                    MyLog.debug("Mouse support is enabled");
                    originalMouseSupport = GUIGraphicsContext.MouseSupport;
                    GUIGraphicsContext.MouseSupport = true;
                    while (ShowCursor(true) < 0) ;
                }

                Parameter = _loadParameter;

                #region init browser

                webBrowser.Dock = DockStyle.None;
                SetBrowserWindow();

                webBrowser.Visible = true;

                webBrowser.Enabled = settings.UseMouse;
                webBrowser.ClearCachedCOMPtrs();

                MyLog.debug("Create eventhandler");

                webBrowser.DocumentCompleted += new EventHandler(webBrowser_DocumentCompleted);
                webBrowser.StatusTextChanged += new EventHandler(webBrowser_StatusTextChanged);

                MyLog.debug("Create dom eventhandler");
                webBrowser.DomKeyDown += new EventHandler<DomKeyEventArgs>(webBrowser_DomKeyDown);
                webBrowser.DomClick += new EventHandler<DomEventArgs>(webBrowser_DomClick);

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

                if (String.IsNullOrEmpty(webBrowser.Document.Domain))
                {
                    if ((settings.UseHome) && (string.IsNullOrEmpty(loadFav)))
                    {
                        webBrowser.Navigate(settings.HomePage);
                        MyLog.debug("load home page " + settings.HomePage);
                    }
                }

                if (!string.IsNullOrEmpty(loadFav))
                {
                    webBrowser.Navigate(loadFav);
                    MyLog.debug("load favorite " + loadFav);
                    StartupLink = string.Empty;
                }

                #endregion

                osd_linkID.Location = new System.Drawing.Point((GUIGraphicsContext.form.Width / 2) - (osd_linkID.Width / 2),
                                                               (GUIGraphicsContext.form.Height / 2) - (osd_linkID.Height / 2));

                timer.Interval = 100;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();

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

            webBrowser.Visible = false;
            GUIGraphicsContext.form.Focus();

            osd_linkID.Visible = false;

            webBrowser.DocumentCompleted -= new EventHandler(webBrowser_DocumentCompleted);
            webBrowser.StatusTextChanged -= new EventHandler(webBrowser_StatusTextChanged);
            webBrowser.DomKeyDown -= new EventHandler<DomKeyEventArgs>(webBrowser_DomKeyDown);
            webBrowser.DomClick -= new EventHandler<DomEventArgs>(webBrowser_DomClick);

            timer.Tick -= new EventHandler(timer_Tick);
            timer.Stop();
            if (settings.UseMouse)
            {
                Cursor.Hide();
                GUIGraphicsContext.MouseSupport = originalMouseSupport;
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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (linkId != string.Empty)
            {
                if (settings.OSD)
                {
                    osd_linkID.Visible = true;
                    osd_linkID.BringToFront();
                    osd_linkID.ID = linkId;
                }
                linkTime++;
            }
            else
            {
                osd_linkID.Visible = false;
            }

            if (linkTime > settings.RemoteTime)
            {
                linkId = string.Empty;
                linkTime = 0;

                GUIPropertyManager.SetProperty("#btWeb.linkid", linkId);
            }
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

        public override void OnAction(MediaPortal.GUI.Library.Action action)
        {
            GUIPropertyManager.SetProperty("#btWeb.linkid", String.IsNullOrEmpty(linkId) ? String.Empty : "Link ID = " + linkId);
            #region remote diagnostic
            if (settings.Remote)
            {
                if (action.wID != MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED)
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

                        if (linkId != string.Empty)
                        {
                            MyLog.debug("confirm link pressed");
                            OnLinkId(linkId);
                        }
                        else
                        {
                            MyLog.debug("confirm2 link pressed, no link present");
                        }
                    }
                }
                else
                {
                    webBrowser.Enabled = true;
                    System.Threading.Thread.Sleep(200);
                    clickFromPlugin = true;
                    int X = Cursor.Position.X;
                    int Y = Cursor.Position.Y;
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
            }
            if (action.wID == settings.Remote_Bookmark)
            {
                GUIWindowManager.ActivateWindow(GUIBookmark.BookmarkWindowId);
                return;
            }
            if ((action.wID == settings.Remote_Zoom_In) ||
                (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_FORWARD))
            {
                OnZoomIn();
            }
            if ((action.wID == settings.Remote_Zoom_Out) ||
                (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_REWIND))
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
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOUSE_CLICK:
                    {
                        break;
                    }
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOUSE_MOVE:
                    if (settings.UseMouse)
                    {

                    }
                    break;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_SUBTITLE:
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
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED:
                    if (!settings.UseMouse)
                    {
                        linkTime = 0;
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
                                linkId = string.Empty;
                                osd_linkID.Visible = false;
                                Application.DoEvents();
                            }
                        }
                        else
                            if (action.m_key.KeyChar >= '0' && action.m_key.KeyChar <= '9')
                                linkId += (char)action.m_key.KeyChar;
                        if (linkId.Length > 4) linkId = linkId.Substring(0, 1);
                    }
                    break;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU:
                    linkId = string.Empty;
                    break;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY:
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY:
                    OnEnterNewLink();
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PAUSE:
                    webBrowser.Navigate(settings.HomePage);
                    MyLog.debug("load home page " + settings.HomePage);
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "go to homepage");
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_STOP:
                    webBrowser.Navigate("about:blank");
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "Stop");
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_ITEM:
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_REWIND:
                    webBrowser.GoBack();
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "go backward");
                    MyLog.debug("navigate go back");
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_ITEM:
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_FORWARD:
                    webBrowser.GoForward();
                    if (!settings.Remote) GUIPropertyManager.SetProperty("#btWeb.status", "go forward");
                    MyLog.debug("navigate go forward");
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_RECORD:
                    OnAddBookmark();
                    return;

                #region move
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_RIGHT:
                    OnMoveRight();
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_LEFT:
                    OnMoveLeft();
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_UP:
                    OnMoveUp();
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_DOWN:
                    OnMoveDown();
                    return;
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM:
                    return;
                #endregion
            }
            base.OnAction(action);
        }

        private void webBrowser_DomKeyDown(object sender, DomKeyEventArgs e)
        {
            if (settings.UseMouse || mouseVisible)
            {
                //System.Diagnostics.Debug.WriteLine("DOM " + e.KeyCode.ToString());

                if (e.KeyCode == (uint)Keys.Escape)
                    GUIWindowManager.ShowPreviousWindow();

                if (e.KeyCode == (uint)Keys.PageUp) OnZoomIn();
                if (e.KeyCode == (uint)Keys.PageDown) OnZoomOut();

                if (e.KeyCode == (uint)Keys.Down) OnMoveDown();
                if (e.KeyCode == (uint)Keys.Up) OnMoveUp();
                if (e.KeyCode == (uint)Keys.Left) OnMoveLeft();
                if (e.KeyCode == (uint)Keys.Right) OnMoveRight();

                if (e.KeyCode == (uint)Keys.F3) GUIWindowManager.ActivateWindow(GUIBookmark.BookmarkWindowId);

                if (e.KeyCode == (uint)Keys.F7) webBrowser.GoBack();
                if (e.KeyCode == (uint)Keys.F8) webBrowser.GoForward();

                if (e.CtrlKey == true)
                {
                    if (e.KeyCode == (uint)Keys.R) OnAddBookmark();
                    if (e.KeyCode == (uint)Keys.P) OnEnterNewLink();
                    if (e.KeyCode == (uint)Keys.B) webBrowser.Navigate("about:blank");
                }

            }
        }
        void webBrowser_DomClick(object sender, DomEventArgs e)
        {
            if (settings.UseMouse)
            {
                // this is a workarround until i know what wrong on the links...
                GeckoWebBrowser g = (GeckoWebBrowser)sender;
                string dom = g.Document.Url.AbsoluteUri.ToString();
                string parent = e.Target.Parent.InnerHtml;

                if (!parent.Contains("shockwave"))
                {
                    int x = parent.IndexOf("a href=");
                    if (x >= 0)
                    {
                        int y = parent.IndexOf("\"", x + 8);
                        if (y >= 0)
                        {
                            string link = parent.Substring(x + 7, y - x - 6);
                            link = link.Replace("\"", "");
                            if (link.Contains("http"))
                                g.Navigate(link);
                        }
                    }
                }
            }

            if (clickFromPlugin) // click succeeded, so focus can safely be reset
            {
                clickFromPlugin = false;
                webBrowser.Enabled = false;
                GUIGraphicsContext.form.Focus();
            }
        }

        private void OnEnterNewLink()
        {
            webBrowser.Visible = false;
            GUIGraphicsContext.form.Focus();

            string selectedUrl = "http://";
            if (settings.LastUrl != string.Empty)
            {
                selectedUrl = settings.LastUrl;
            }

            if (ShowKeyboard(ref selectedUrl, false) == DialogResult.OK)
            {
                if (Bookmark.isValidUrl(selectedUrl))
                {
                    webBrowser.Navigate(selectedUrl);
                    MyLog.debug("navigate to " + selectedUrl);

                    settings.LastUrl = selectedUrl;
                }
                else
                    ShowAlert("Wrong link ?", " The link you entered seems to be not valid.", "Input:", selectedUrl);
            }

            webBrowser.Visible = true;
            if (settings.UseMouse)
                webBrowser.Select();

        }
        private void OnAddBookmark()
        {
            webBrowser.Visible = false;
            GUIGraphicsContext.form.Focus();

            string title = webBrowser.Document.Title;
            string actualUrl = webBrowser.Document.Url.ToString();

            title = title.Replace("\0", "");

            DialogResult result = ShowKeyboard(ref title, false);
            if (result == DialogResult.OK)
            {
                bool hasSaved = BookmarkXml.AddBookmark(title, actualUrl, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml", 0);
                if (hasSaved)
                {
                    ShowAlert("Bookmark has been saved !", "Title : " + title, "URL : " + actualUrl, "");
                    #region save snapshot

                    if (webBrowser.Url.ToString() != "about:blank")
                    {
                        int y = webBrowser.Height;
                        int x = y / 4 * 3;

                        int offset = (webBrowser.Width - x) / 2;

                        Bitmap snap = new Bitmap(webBrowser.Width, webBrowser.Height);
                        webBrowser.DrawToBitmap(snap, new Rectangle(0, 0, webBrowser.Width, webBrowser.Height));

                        snap = CopyBitmap(snap, new Rectangle(offset, 0, x, y));

                        snap = MediaPortal.Util.BitmapResize.Resize(ref snap, 300, 400, false, true);

                        Graphics g = Graphics.FromImage((Image)snap);
                        g.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(1, 1, snap.Width - 2, snap.Height - 2));

                        Bookmark.SaveSnap(snap, actualUrl);
                    }
                    #endregion

                }
                else
                    ShowAlert("Bookmark could not been saved !", "Title : " + title, "URL : " + actualUrl, "");
            }

            webBrowser.Visible = true;
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
                    DomHelper.AddLinksToPage(webBrowser.Document);

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
                    using (System.IO.StreamWriter tw = new System.IO.StreamWriter(@"e:\last.html"))
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

        private void OnLinkId(string LinkId)
        {
            linkId = string.Empty;
            osd_linkID.Visible = false;
            Application.DoEvents();

            GeckoHtmlElement ge = DomHelper.GetElement(LinkId, webBrowser.Document);

            if (ge == null)
            {
                MyLog.debug(String.Format("LinkId {0} not found in _htmlLinkNumbers", LinkId));
                return;
            }

            if (ge is GeckoAnchorElement && !ge.HasAttribute("onclick"))
            {
                string link = ((GeckoAnchorElement)ge).Href;
                webBrowser.Navigate(link);
                MyLog.debug("navigate to linkid=" + LinkId + " URL=" + link);
            }
            else
                if (ge is GeckoButtonElement)
                {
                    ge.Click();
                }
                else
                    if (ge is GeckoSelectElement)
                    {
                        ShowSelect(ge as GeckoSelectElement);
                    }
                    else
                        if (ge is GeckoInputElement)
                        {
                            string linkType = ((GeckoInputElement)ge).Type;
                            if (!String.IsNullOrEmpty(linkType))
                            {
                                switch (linkType)
                                {
                                    case "password": ShowInputDialog(true, ge as GeckoInputElement); break;
                                    case "submit":
                                    case "reset":
                                    case "radio":
                                    case "image":
                                    case "checkbox":
                                        ge.Click();
                                        MyLog.debug("action linkid=" + LinkId);
                                        break;
                                    case "hidden": break;
                                    default: ShowInputDialog(false, ge as GeckoInputElement); break;
                                }
                            }
                        }
                        else
                        //if (ge is GeckoObjectElement)
                        // some items just need a mousehover, and a ge.Click won't do that
                        {
                            Point p = DomHelper.GetCenterCoordinate(webBrowser.Document, ge);
                            MyLog.debug("perform click on " + p.ToString());
                            p.X = Convert.ToInt32(p.X * zoom);
                            p.Y = Convert.ToInt32(p.Y * zoom);
                            webBrowser.Enabled = true;
                            System.Threading.Thread.Sleep(200);
                            Cursor.Position = webBrowser.PointToScreen(p);
                            clickFromPlugin = true;
                            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        }
            //else
            // ge.Click();
        }

        public void ShowInputDialog(bool isPassword, GeckoInputElement element)
        {
            webBrowser.Visible = false;

            string result = element.Value;
            if (ShowKeyboard(ref result, isPassword) == DialogResult.OK)
            {
                if (element != null)
                    element.SetAttribute("value", result);
                GeckoFormElement form = element.Form;
                if (form != null)
                {
                    //List<GeckoHtmlElement> inps = DomHelper.GetElements(form, "input");
                    GeckoElementCollection inps = form.GetElementsByTagName("input");
                    if (DomHelper.NrOfChildElementsDone(form) == 1)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (GeckoInputElement inp in inps)
                        {
                            if (sb.Length != 0)
                                sb.Append('&');
                            sb.Append(inp.Name);
                            sb.Append('=');
                            sb.Append(inp.Value);
                        }

                        if (form.Method == "get")
                            webBrowser.Navigate(form.Action + '?' + sb.ToString());
                        else
                        {
                            using (GeckoMIMEInputStream stream = new GeckoMIMEInputStream())
                            {
                                stream.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                stream.AddContentLength = true;
                                stream.SetData(sb.ToString());
                                webBrowser.Navigate(form.Action, 0, null, stream);
                            }
                        }
                    }
                }
            }
            webBrowser.Visible = true;
        }

        public static DialogResult ShowKeyboard(ref string DefaultText, bool PasswordInput)
        {
            VirtualKeyboard vk = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            vk.Reset();
            vk.Password = PasswordInput;
            vk.Text = DefaultText;
            vk.SetLabelAsInitialText(false); // set to false, otherwise our intial text is cleared
            vk.DoModal(GUIWindowManager.ActiveWindow);

            if (vk.IsConfirmed)
            {
                DefaultText = vk.Text;
                return DialogResult.OK;
            }
            else
                return DialogResult.Cancel;
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
