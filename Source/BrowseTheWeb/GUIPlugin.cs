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

using System.Windows.Forms;
using System.Runtime.InteropServices;

using Skybound.Gecko;

namespace BrowseTheWeb
{
  [PluginIcons("BrowseTheWeb.xulrunner.png", "BrowseTheWeb.xulrunnerOff.png")]

  public class GUIPlugin : GUIWindow, ISetupForm
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    #region Links
    private static GeckoElementCollection _links;
    private static GeckoElementCollection _forms;
    private static List<HtmlLinkNumber> _htmlLinkNumbers;
    #endregion

    #region Constants
    private const string _span = "<span style=\"font-family: arial,sans-serif; font-size: 12px ! important; line-height: 130% ! important; border-width: 1px ! important; border-style: solid ! important; -moz-border-radius: 2px 2px 2px 2px ! important; padding: 0px 2px ! important; margin-left: 2px; max-width: 20px; max-height: 10px ! important; overflow: visible ! important; float: none ! important; display: inline;\" gecko_id=\"{0}\" gecko_action=\"{1}\" gecko_type=\"{2}\">{0}</span>";
    #endregion

    #region declare vars
    private GeckoWebBrowser webBrowser;
    private OSD_LinkId osd_linkID;
    private Mouse mouse;
    private string linkId = string.Empty;
    private int linkTime = 0;
    private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

    private bool usehome = false;
    private string homepage = string.Empty;
    private int remoteTime = 0;
    private string pluginName = "Browse Web";
    private bool blankBrowser = false;
    private bool statusBar = true;
    private bool osd = false;
    private bool windowed = false;
    private bool zoomPage = false;
    private bool zoomDomain = false;
    private string lastDomain = string.Empty;
    private bool cacheThumbs = false;
    private bool remote = false;
    private string remote_1 = string.Empty;

    private bool useProxy = false;
    private string Server = string.Empty;
    private int Port = 8080;

    private float defaultZoom = 1.0f;
    private float zoom = 1.0f;
    private float font = 1.0f;

    public static string loadFav = string.Empty;

    #endregion

    #region ISetupForm Member

    public string Author()
    {
      return "Mark Koenig (kroko) 2010";
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
      strButtonText = pluginName;
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
        return 54537689;
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
        string value = GUIPropertyManager.GetProperty("#btWeb.startup.link");
        if (null != value && !string.IsNullOrEmpty(value.Trim()))
          return value.Trim();
        return string.Empty;
      }
      set { GUIPropertyManager.SetProperty("#btWeb.startup.link", string.IsNullOrEmpty(value) ? " " : value.Trim()); }
    }

    public override bool Init()
    {
      Xpcom.Initialize(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\xulrunner");

      #region add forms
      webBrowser = new GeckoWebBrowser();
      GUIGraphicsContext.form.Controls.Add(webBrowser);
      webBrowser.Visible = false;

      osd_linkID = new OSD_LinkId();
      GUIGraphicsContext.form.Controls.Add(osd_linkID);
      osd_linkID.Visible = false;

      mouse = new Mouse();
      GUIGraphicsContext.form.Controls.Add(mouse);
      mouse.Visible = false;
      #endregion

      LoadSettings();
      Bookmark.AddFolder(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml", "Saved by MP");

      return Load(GUIGraphicsContext.Skin + @"\BrowseTheWeb.xml");
    }

    protected override void OnPageLoad()
    {
      try
      {
        MyLog.debug("Init browser");

        GUIPropertyManager.SetProperty("#btWeb.status", "Init browser");
        GUIPropertyManager.SetProperty("#btWeb.linkid", "");
        linkId = string.Empty;

        #region init browser
        webBrowser.Visible = true;
        webBrowser.Enabled = false;

        webBrowser.Dock = System.Windows.Forms.DockStyle.None;
        webBrowser.Location = new System.Drawing.Point(0, 0);

        MyLog.debug("Create eventhandler");

        webBrowser.DocumentCompleted += new EventHandler(webBrowser_DocumentCompleted);
        webBrowser.StatusTextChanged += new EventHandler(webBrowser_StatusTextChanged);

        if (statusBar)
          webBrowser.Size = new System.Drawing.Size(GUIGraphicsContext.form.Width, GUIGraphicsContext.form.Height - 100);
        else
          webBrowser.Size = new System.Drawing.Size(GUIGraphicsContext.form.Width, GUIGraphicsContext.form.Height);

        webBrowser.Window.TextZoom = font;
        webBrowser.Zoom = zoom;

        if (windowed)
        {
          MyLog.debug("switch to windowed fullscreen mode");
          GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SWITCH_FULL_WINDOWED, 0, 0, 0, 0, 0, null);
          GUIWindowManager.SendMessage(msg);
        }

        string loadFav = StartupLink;

        if (webBrowser.Document.Domain == string.Empty)
        {
          if ((usehome) && (string.IsNullOrEmpty(loadFav)))
          {
            webBrowser.Navigate(homepage);
            MyLog.debug("load home page " + homepage);
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
      }
      catch (Exception ex)
      {
        MyLog.debug("Exception OnLoad : " + ex.Message);
        MyLog.debug("Exception OnLoad : " + ex.StackTrace);
      }

      base.OnPageLoad();
    }

    private void LoadSettings()
    {
      string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
      {
        usehome = xmlreader.GetValueAsBool("btWeb", "usehome", true);
        homepage = xmlreader.GetValueAsString("btWeb", "homepage", "http://team-mediaportal.com");
        remoteTime = xmlreader.GetValueAsInt("btWeb", "remotetime", 15);
        pluginName = xmlreader.GetValueAsString("btWeb", "name", "Browse Web");
        blankBrowser = xmlreader.GetValueAsBool("btWeb", "blank", false);
        statusBar = xmlreader.GetValueAsBool("btWeb", "status", true);
        osd = xmlreader.GetValueAsBool("btWeb", "osd", true);
        windowed = xmlreader.GetValueAsBool("btWeb", "window", false);

        defaultZoom = (float)xmlreader.GetValueAsInt("btWeb", "zoom", 100) / 100;
        zoom = defaultZoom;
        font = (float)xmlreader.GetValueAsInt("btWeb", "font", 100) / 100;
        zoomPage = xmlreader.GetValueAsBool("btWeb", "page", true);
        zoomDomain = xmlreader.GetValueAsBool("btWeb", "domain", false);

        cacheThumbs = xmlreader.GetValueAsBool("btWeb", "cachethumbs", false);

        remote = xmlreader.GetValueAsBool("btWeb", "remote", false);
        remote_1 = xmlreader.GetValueAsString("btWeb", "key_1", "REMOTE_1");

        useProxy = xmlreader.GetValueAsBool("btWeb", "proxy", false);
        Server = xmlreader.GetValueAsString("btWeb", "proxy_server", "127.0.0.1");
        Port = xmlreader.GetValueAsInt("btWeb", "proxy_port", 8888);
        TrySetProxy();
      }
    }

    private void TrySetProxy()
    {
      try
      {
        if (useProxy)
          MyLog.debug("use proxy settings");
        else
          MyLog.debug("no proxy selected");

        SetProxy(Server, Port, useProxy);
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
      int ena = 0; if (useProxy) ena = 1;
      GeckoPreferences.User["network.proxy.type"] = ena;

      // maybe possible... not sure...
      // network.proxy.login
      // network.proxy.password
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      if (linkId != string.Empty)
      {
        if (osd)
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

      if (linkTime > remoteTime)
      {
        linkId = string.Empty;
        linkTime = 0;

        GUIPropertyManager.SetProperty("#btWeb.linkid", linkId);
      }
    }
    protected override void OnPageDestroy(int new_windowId)
    {
      if (new_windowId != 54537688)
      { // not if you got favs
        if (blankBrowser)
        {
          webBrowser.Navigate("about:blank");
          MyLog.debug("blank on destroy");
        }
      }
      webBrowser.Visible = false;
      osd_linkID.Visible = false;

      webBrowser.DocumentCompleted -= new EventHandler(webBrowser_DocumentCompleted);
      webBrowser.StatusTextChanged -= new EventHandler(webBrowser_StatusTextChanged);

      timer.Tick -= new EventHandler(timer_Tick);
      timer.Stop();

      base.OnPageDestroy(new_windowId);
    }

    public override void OnAction(Action action)
    {
      if (remote)
      {
        if (action.wID != Action.ActionType.ACTION_KEY_PRESSED)
          GUIPropertyManager.SetProperty("#btWeb.status", action.wID.ToString());
        else
          GUIPropertyManager.SetProperty("#btWeb.status", action.wID.ToString() + " / " + action.m_key.KeyChar.ToString());
      }

      string strAction = action.wID.ToString();
      if (strAction == remote_1)
      {
        if (linkId != string.Empty)
        {
          MyLog.debug("confirm2 link pressed");
          OnLinkId(linkId);
        }
        else
        {
          MyLog.debug("confirm2 link pressed, no link present");
        }
      }

      switch (action.wID)
      {
        case Action.ActionType.ACTION_VOLUME_MUTE:
          /*
           * test *
          Cursor.Position = new Point(250, 350);

          Cursor.Show();
          mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
          Cursor.Hide();
          */
          break;
        case Action.ActionType.ACTION_KEY_PRESSED:
          linkTime = 0;
          MyLog.debug("action key press=" + action.m_key.KeyChar);
          switch (action.m_key.KeyChar)
          {
            #region 0..9
            case '1':
              linkId += "1";
              break;
            case '2':
              linkId += "2";
              break;
            case '3':
              linkId += "3";
              break;
            case '4':
              linkId += "4";
              break;
            case '5':
              linkId += "5";
              break;
            case '6':
              linkId += "6";
              break;
            case '7':
              linkId += "7";
              break;
            case '8':
              linkId += "8";
              break;
            case '9':
              linkId += "9";
              break;
            case '0':
              linkId += "0";
              break;
            #endregion
          }
          break;
        case Action.ActionType.ACTION_PLAY:
        case Action.ActionType.ACTION_MUSIC_PLAY:
          webBrowser.Visible = false;

          string selectedUrl = "http://";
          if (ShowKeyboard(ref selectedUrl, false) == System.Windows.Forms.DialogResult.OK)
          {
            if (Bookmark.isValidUrl(selectedUrl))
            {
              webBrowser.Navigate(selectedUrl);
              MyLog.debug("navigate to " + selectedUrl);
            }
            else
              ShowAlert("Wrong link ?", " The link you entered seems to be not valid.", "Input:", selectedUrl);
          }
          webBrowser.Visible = true;
          break;
        case Action.ActionType.ACTION_PAUSE:
          webBrowser.Navigate(homepage);
          MyLog.debug("load home page " + homepage);
          GUIPropertyManager.SetProperty("#btWeb.status", "go to homepage");
          break;
        case Action.ActionType.ACTION_STOP:
          webBrowser.Navigate("about:blank");
          GUIPropertyManager.SetProperty("#btWeb.status", "Stop");
          break;
        case Action.ActionType.ACTION_PARENT_DIR:
        case Action.ActionType.ACTION_ASPECT_RATIO:
          if (linkId != string.Empty)
          {
            MyLog.debug("confirm link pressed");
            OnLinkId(linkId);
          }
          else
          {
            MyLog.debug("confirm link pressed, no link present");
          }
          break;
        case Action.ActionType.ACTION_CONTEXT_MENU:
        case Action.ActionType.ACTION_SHOW_INFO:
          GUIWindowManager.ActivateWindow(54537688);
          return;
        case Action.ActionType.ACTION_PREV_ITEM:
          webBrowser.GoBack();
          GUIPropertyManager.SetProperty("#btWeb.status", "go backward");
          MyLog.debug("navigate go back");
          break;
        case Action.ActionType.ACTION_NEXT_ITEM:
          webBrowser.GoForward();
          GUIPropertyManager.SetProperty("#btWeb.status", "go forward");
          MyLog.debug("navigate go forward");
          break;
        case Action.ActionType.ACTION_RECORD:
          string title = webBrowser.Document.Title;
          string actualUrl = webBrowser.Document.Url.ToString();
          title = title.Replace("\0", "");

          webBrowser.Visible = false;

          System.Windows.Forms.DialogResult result = ShowKeyboard(ref title, false);
          if (result == System.Windows.Forms.DialogResult.OK)
          {
            bool hasSaved = Bookmark.SavaBookmark(title, actualUrl, Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config) + "\\bookmarks.xml");
            if (hasSaved)
              ShowAlert("Bookmark has been saved !", "Title : " + title, "URL : " + actualUrl, "");
            else
              ShowAlert("Bookmark could not been saved !", "Title : " + title, "URL : " + actualUrl, "");
          }

          webBrowser.Visible = true;
          break;
        case Action.ActionType.ACTION_SHOW_GUI:
          statusBar = !statusBar;
          if (statusBar)
            webBrowser.Size = new System.Drawing.Size(GUIGraphicsContext.form.Width, GUIGraphicsContext.form.Height - 100);
          else
            webBrowser.Size = new System.Drawing.Size(GUIGraphicsContext.form.Width, GUIGraphicsContext.form.Height);
          break;

        #region zoom & move
        case Action.ActionType.ACTION_ZOOM_IN:
        case Action.ActionType.ACTION_PAGE_UP:
          if (zoom < 2) zoom += 0.1f;
          webBrowser.Zoom = zoom;
          GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
          break;
        case Action.ActionType.ACTION_ZOOM_OUT:
        case Action.ActionType.ACTION_PAGE_DOWN:
          if (zoom > 0.1f) zoom -= 0.1f;
          webBrowser.Zoom = zoom;
          GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
          break;
        case Action.ActionType.ACTION_MOVE_RIGHT:
          if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX + 100, webBrowser.Window.ScrollY);
          break;
        case Action.ActionType.ACTION_MOVE_LEFT:
          if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX - 100, webBrowser.Window.ScrollY);
          break;
        case Action.ActionType.ACTION_MOVE_UP:
          if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX, webBrowser.Window.ScrollY - 100);
          break;
        case Action.ActionType.ACTION_MOVE_DOWN:
          if (webBrowser.Window != null) ScrollTo(webBrowser.Window.ScrollX, webBrowser.Window.ScrollY + 100);
          break;
        #endregion
      }
      if (linkId != string.Empty)
        GUIPropertyManager.SetProperty("#btWeb.linkid", "Link ID = " + linkId);
      else
        GUIPropertyManager.SetProperty("#btWeb.linkid", linkId);

      base.OnAction(action);
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
        str += web.StatusText.Substring(0, 50);
        if (web.StatusText.Length > 50) str += "...";

        GUIPropertyManager.SetProperty("#btWeb.status", str);
      }
    }
    private void webBrowser_DocumentCompleted(object sender, EventArgs e)
    {
      MyLog.debug("page completetd : " + webBrowser.Url.ToString());

      try
      {
        #region MP gui stuff
        string str = DateTime.Now.ToLongTimeString();
        str += " Completed";

        GUIPropertyManager.SetProperty("#btWeb.status", str);
        #endregion

        #region add links to page
        _htmlLinkNumbers = new List<HtmlLinkNumber>();

        _links = webBrowser.Document.Links;
        int i = 1;

        MyLog.debug("page links cnt : " + _links.Count);

        foreach (GeckoElement element in _links)
        {
          string link = element.GetAttribute("href");

          if (!link.StartsWith("javascript:"))
          {
            if (!element.InnerHtml.Contains("gecko_id"))
            {
              element.InnerHtml += string.Format(_span, i, "", "LINK");
            }

            string gb = element.GetAttribute("gb");
            string id = element.GetAttribute("id");
            string name = element.GetAttribute("name");
            if (string.IsNullOrEmpty(gb))
            {
              element.SetAttribute("gb", "gecko_link" + i);
            }
            if (string.IsNullOrEmpty(id))
            {
              element.SetAttribute("id", "gb" + i);
              id = "gb" + i;
            }
            _htmlLinkNumbers.Add(new HtmlLinkNumber(i, id, name, link, HtmlInputType.Link));
            i++;
          }
        }

        _forms = webBrowser.Document.GetElementsByTagName("form");
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

        MyLog.debug("page forms cnt : " + _forms.Count);

        foreach (GeckoElement element in _forms)
        {
          string action = element.GetAttribute("action");
          doc.LoadHtml(element.InnerHtml);
          foreach (HtmlAgilityPack.HtmlNode link in doc.DocumentNode.SelectNodes("//*"))
          {
            if (link.OriginalName == "input")
            {
              if (link.Attributes["type"] != null)
              {
                if (link.Attributes["type"].Value != "hidden")
                {

                  string gb = link.GetAttributeValue("gb", "");
                  string id = link.GetAttributeValue("id", "");
                  string name = link.GetAttributeValue("name", "");
                  string outerHtml = link.OuterHtml;
                  if (string.IsNullOrEmpty(gb))
                  {
                    link.SetAttributeValue("gb", "gecko_link" + i);
                  }
                  if (string.IsNullOrEmpty(id))
                  {
                    link.SetAttributeValue("id", "gb" + i);
                    id = "gb" + i;
                  }

                  if (!element.InnerHtml.Contains("gecko_id=\"" + i + "\""))
                  {
                    string newLink = link.OuterHtml + string.Format(_span, i, action, "INPUT");
                    element.InnerHtml = element.InnerHtml.Replace(outerHtml, newLink);
                  }
                  if (link.Attributes["type"].Value == "submit" ||
                      link.Attributes["type"].Value == "reset" ||
                      link.Attributes["type"].Value == "radio" ||
                      link.Attributes["type"].Value == "image" ||
                      link.Attributes["type"].Value == "checkbox")
                  {
                    _htmlLinkNumbers.Add(new HtmlLinkNumber(i, id, name, action, HtmlInputType.Action));
                  }
                  else
                  {
                    _htmlLinkNumbers.Add(new HtmlLinkNumber(i, id, name, action, HtmlInputType.Input));
                  }
                  i++;
                }
              }
              else
              {
                string gb = link.GetAttributeValue("gb", "");
                string id = link.GetAttributeValue("id", "");
                string name = link.GetAttributeValue("name", "");
                string outerHtml = link.OuterHtml;
                if (string.IsNullOrEmpty(gb))
                {
                  link.SetAttributeValue("gb", "gecko_link" + i);
                }
                if (string.IsNullOrEmpty(id))
                {
                  link.SetAttributeValue("id", "gb" + i);
                  id = "gb" + i;
                }

                if (!element.InnerHtml.Contains("gecko_id=\"" + i + "\""))
                {
                  string newLink = link.OuterHtml + string.Format(_span, i, action, "INPUT");
                  element.InnerHtml = element.InnerHtml.Replace(outerHtml, newLink);
                }

                _htmlLinkNumbers.Add(new HtmlLinkNumber(i, id, name, action, HtmlInputType.Input));
                i++;
              }
            }
          }
        }
        #endregion

        #region reset zoom
        if (zoomPage)
        {
          webBrowser.Zoom = defaultZoom;
          zoom = defaultZoom;
          GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
        }
        if (zoomDomain)
        {
          if (lastDomain != webBrowser.Document.Domain)
          {
            {
              webBrowser.Zoom = defaultZoom;
              zoom = defaultZoom;
              GUIPropertyManager.SetProperty("#btWeb.status", "Zoom set to " + (int)(zoom * 100));
            }
          }
          lastDomain = webBrowser.Document.Domain;
        }
        #endregion

        #region save snapshot

        if (webBrowser.Url.ToString() != "about:blank")
        {
          if (cacheThumbs)
          {
            Bitmap snap = new Bitmap(webBrowser.Width, webBrowser.Height);
            webBrowser.DrawToBitmap(snap, new Rectangle(0, 0, webBrowser.Width, webBrowser.Height));

            snap = MediaPortal.Util.BitmapResize.Resize(ref snap, 300, 400, false, true);

            Graphics g = Graphics.FromImage((Image)snap);
            g.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(1, 1, snap.Width - 2, snap.Height - 2));

            Bookmark.SaveSnap(snap, webBrowser.Url.ToString());
          }
        }
        #endregion
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

      HtmlLinkNumber hln = null;
      if (GetLinkById(Convert.ToInt32(LinkId), out hln))
      {
        switch (hln.Type)
        {
          case HtmlInputType.Link:
            webBrowser.Navigate(hln.Link);
            MyLog.debug("navigate to linkid=" + LinkId + " URL=" + hln.Link);
            break;
          case HtmlInputType.Input:
            ShowInputDialog(hln);
            break;
          case HtmlInputType.Action:
            webBrowser.Navigate("javascript:document.getElementById(\"" + hln.Id + "\").click()");
            MyLog.debug("action linkid=" + LinkId);
            break;
        }
      }
    }
    private bool GetLinkById(int value, out HtmlLinkNumber hln)
    {
      if (_htmlLinkNumbers != null && _htmlLinkNumbers.Count > 0)
      {
        foreach (HtmlLinkNumber id in _htmlLinkNumbers)
        {
          if (id.Number == value)
          {
            switch (id.Type)
            {
              case HtmlInputType.Link:
                {
                  Uri uri;
                  if (Uri.TryCreate(webBrowser.Url, id.Link, out uri))
                  {
                    id.Link = uri.AbsoluteUri;
                    hln = id;
                    return true;
                  }
                }
                break;
              case HtmlInputType.Input:
              case HtmlInputType.Action:
                hln = id;
                return true;
              //return "javascript:document.getElementById(\"" + id.Name + "\").click()";
            }
          }
        }
      }
      hln = null;
      return false;
    }

    public void ShowInputDialog(HtmlLinkNumber id)
    {
      webBrowser.Visible = false;

      string result = string.Empty;
      if (ShowKeyboard(ref result, false) == System.Windows.Forms.DialogResult.OK)
      {
        SetInputFieldText(id.Number, result);
      }
      webBrowser.Visible = true;
    }
    public void SetInputFieldText(int id, string text)
    {
      HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
      doc.LoadHtml(webBrowser.Document.Body.InnerHtml);

      foreach (HtmlAgilityPack.HtmlNode element in doc.DocumentNode.SelectNodes("//input"))
      {
        string name = element.GetAttributeValue("gb", "");
        if (!string.IsNullOrEmpty(name))
        {
          if (name == "gecko_link" + id)
          {
            element.SetAttributeValue("value", text);
            webBrowser.Document.Body.InnerHtml = doc.DocumentNode.InnerHtml;
            break;
          }
        }
      }
    }

    public static System.Windows.Forms.DialogResult ShowKeyboard(ref string DefaultText, bool PasswordInput)
    {
      VirtualKeyboard vk = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);

      vk.Reset();
      vk.Password = PasswordInput;
      vk.Text = DefaultText;
      vk.Name = "";
      vk.DoModal(GUIWindowManager.ActiveWindow);

      if (vk.IsConfirmed)
      {
        DefaultText = vk.Text;
        return System.Windows.Forms.DialogResult.OK;
      }
      else
        return System.Windows.Forms.DialogResult.Cancel;
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
    private void OnRenderSound(string strFilePath)
    {
      MediaPortal.Util.Utils.PlaySound(strFilePath, false, true);
    }
  }
}
