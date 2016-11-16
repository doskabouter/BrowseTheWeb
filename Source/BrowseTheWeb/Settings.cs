using System;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using Action = MediaPortal.GUI.Library.Action;

namespace BrowseTheWeb
{
    public class Settings
    {
        public bool UseHome { get; set; }
        public string HomePage { get; set; }
        public int RemoteTime { get; set; }
        public string PluginName { get; set; }
        public bool BlankBrowser { get; set; }
        public bool StatusBar { get; set; }
        public bool OSD { get; set; }
        public bool Windowed { get; set; }
        public bool UseMouse { get; set; }
        public bool DisableAero { get; set; }

        public int DefaultZoom_percentage { get; set; }
        public int FontZoom_percentage { get; set; }
        public float FontZoom { get { return (float)FontZoom_percentage / 100; } }
        public float DefaultZoom { get { return (float)DefaultZoom_percentage / 100; } }

        public bool ZoomPage { get; set; }
        public bool ZoomDomain { get; set; }
        public bool UseThumbs { get; set; }
        public bool CacheThumbs { get; set; }
        public bool Remote { get; set; }
        public Action.ActionType Remote_Confirm { get; set; }
        public Action.ActionType Remote_Bookmark { get; set; }
        public Action.ActionType Remote_Zoom_In { get; set; }
        public Action.ActionType Remote_Zoom_Out { get; set; }
        public Action.ActionType Remote_Status { get; set; }
        public Action.ActionType Remote_PageUp { get; set; }
        public Action.ActionType Remote_PageDown { get; set; }

        public static Action.ActionType Default_Remote_Confirm = Action.ActionType.ACTION_SELECT_ITEM;
        public static Action.ActionType Default_Remote_Bookmark = Action.ActionType.ACTION_SHOW_INFO;
        public static Action.ActionType Default_Remote_Zoom_In = Action.ActionType.ACTION_ZOOM_IN;
        public static Action.ActionType Default_Remote_Zoom_Out = Action.ActionType.ACTION_ZOOM_OUT;
        public static Action.ActionType Default_Remote_Status = Action.ActionType.ACTION_SHOW_GUI;
        public static Action.ActionType Default_Remote_PageUp = Action.ActionType.ACTION_PAGE_UP;
        public static Action.ActionType Default_Remote_PageDown = Action.ActionType.ACTION_PAGE_DOWN;

        public string LastUrl { get; set; }
        public bool UseProxy { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public GUIFacadeControl.Layout View { get; set; }
        public string UserAgent { get; set; }

        public string[] PreviousTags;
        public string[] NextTags;
        private const string section = "btWeb";

        #region Singleton
        private static Settings _Instance = null;
        public static Settings Instance
        {
            get
            {
                if (_Instance == null) _Instance = new Settings();
                return _Instance;
            }
        }
        private Settings() { LoadFromXml(); }
        #endregion

        public static string XulRunnerPath()
        {
            return Path.Combine(Config.GetFolder(MediaPortal.Configuration.Config.Dir.Plugins),
                         Path.Combine("Windows", "Firefox"));
        }

        public static string TagsToString(string[] tags)
        {
            return String.Join(";", tags);
        }

        public static string[] StringToTags(string tags)
        {
            return tags.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void LoadFromXml()
        {
            string dir = Config.GetFolder(Config.Dir.Config);
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
            {
                UseHome = xmlreader.GetValueAsBool(section, "usehome", true);
                HomePage = xmlreader.GetValueAsString(section, "homepage", "http://team-mediaportal.com");
                RemoteTime = xmlreader.GetValueAsInt(section, "remotetime", 15);
                PluginName = xmlreader.GetValueAsString(section, "name", "WebBrowser");
                BlankBrowser = xmlreader.GetValueAsBool(section, "blank", false);
                StatusBar = xmlreader.GetValueAsBool(section, "status", true);
                OSD = xmlreader.GetValueAsBool(section, "osd", true);
                Windowed = xmlreader.GetValueAsBool(section, "window", false);
                UseMouse = xmlreader.GetValueAsBool(section, "mouse", false);
                DisableAero = xmlreader.GetValueAsBool(section, "disableAero", false);

                DefaultZoom_percentage = xmlreader.GetValueAsInt(section, "zoom", 100);
                FontZoom_percentage = xmlreader.GetValueAsInt(section, "font", 100);
                ZoomPage = xmlreader.GetValueAsBool(section, "page", true);
                ZoomDomain = xmlreader.GetValueAsBool(section, "domain", false);

                UseThumbs = xmlreader.GetValueAsBool(section, "usethumbs", true);
                CacheThumbs = xmlreader.GetValueAsBool(section, "cachethumbs", false);

                Remote = xmlreader.GetValueAsBool(section, "remote", false);

                Remote_Confirm = GetActionFromString(xmlreader, "remote_confirm", Default_Remote_Confirm);
                Remote_Bookmark = GetActionFromString(xmlreader, "remote_bookmark", Default_Remote_Bookmark);
                Remote_Zoom_In = GetActionFromString(xmlreader, "remote_zoom_in", Default_Remote_Zoom_In);
                Remote_Zoom_Out = GetActionFromString(xmlreader, "remote_zoom_out", Default_Remote_Zoom_Out);
                Remote_Status = GetActionFromString(xmlreader, "remote_status", Default_Remote_Status);
                Remote_PageUp = GetActionFromString(xmlreader, "remote_pageup", Default_Remote_PageUp);
                Remote_PageDown = GetActionFromString(xmlreader, "remote_pagedown", Default_Remote_PageDown);

                LastUrl = xmlreader.GetValueAsString(section, "lastUrl", string.Empty);
                UserAgent = xmlreader.GetValueAsString(section, "useragent", string.Empty);

                PreviousTags = StringToTags(xmlreader.GetValueAsString(section, "previousTags", string.Empty));
                NextTags = StringToTags(xmlreader.GetValueAsString(section, "nextTags", string.Empty));

                UseProxy = xmlreader.GetValueAsBool(section, "proxy", false);
                Server = xmlreader.GetValueAsString(section, "proxy_server", "127.0.0.1");
                Port = xmlreader.GetValueAsInt(section, "proxy_port", 8888);
                string tmp = xmlreader.GetValueAsString(section, "bookmark", GUIFacadeControl.Layout.LargeIcons.ToString());
                if (tmp == "List view") // for backwards compatibility
                    View = GUIFacadeControl.Layout.List;
                else
                {
                    tmp = tmp.Replace(" ", String.Empty);// for backwards compatibility
                    try
                    {
                        View = (GUIFacadeControl.Layout)Enum.Parse(typeof(GUIFacadeControl.Layout), tmp, true);
                    }
                    catch
                    {
                        View = GUIFacadeControl.Layout.List;
                    }
                }
            }
        }

        private Action.ActionType GetActionFromString(MediaPortal.Profile.Settings xmlReader, string entry, Action.ActionType defaultAction)
        {
            return (Action.ActionType)Enum.Parse(typeof(Action.ActionType), xmlReader.GetValueAsString(section, entry, defaultAction.ToString()), true);
        }

        public void SaveToXml(bool fromSetup)
        {
            string dir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Config);
            using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(dir + "\\MediaPortal.xml"))
            {
                if (fromSetup)
                {
                    xmlwriter.SetValueAsBool(section, "usehome", UseHome);
                    xmlwriter.SetValue(section, "homepage", HomePage);
                    xmlwriter.SetValue(section, "remotetime", RemoteTime);
                    xmlwriter.SetValue(section, "name", PluginName);
                    xmlwriter.SetValueAsBool(section, "blank", BlankBrowser);
                    xmlwriter.SetValueAsBool(section, "status", StatusBar);
                    xmlwriter.SetValueAsBool(section, "osd", OSD);
                    xmlwriter.SetValueAsBool(section, "window", Windowed);
                    xmlwriter.SetValueAsBool(section, "mouse", UseMouse);
                    xmlwriter.SetValueAsBool(section, "disableAero", DisableAero);

                    xmlwriter.SetValue(section, "zoom", DefaultZoom_percentage);
                    xmlwriter.SetValue(section, "font", FontZoom_percentage);
                    xmlwriter.SetValueAsBool(section, "page", ZoomPage);
                    xmlwriter.SetValueAsBool(section, "domain", ZoomDomain);

                    xmlwriter.SetValueAsBool(section, "usethumbs", UseThumbs);
                    xmlwriter.SetValueAsBool(section, "cachethumbs", CacheThumbs);

                    xmlwriter.SetValueAsBool(section, "remote", Remote);
                    xmlwriter.SetValue(section, "remote_confirm", Remote_Confirm);
                    xmlwriter.SetValue(section, "remote_bookmark", Remote_Bookmark);
                    xmlwriter.SetValue(section, "remote_zoom_in", Remote_Zoom_In);
                    xmlwriter.SetValue(section, "remote_zoom_out", Remote_Zoom_Out);
                    xmlwriter.SetValue(section, "remote_status", Remote_Status);
                    xmlwriter.SetValue(section, "remote_pageup", Remote_PageUp);
                    xmlwriter.SetValue(section, "remote_pagedown", Remote_PageDown);

                    xmlwriter.SetValue(section, "bookmark", View);
                    xmlwriter.SetValue(section, "useragent", UserAgent);

                    xmlwriter.SetValue(section, "previousTags", TagsToString(PreviousTags));
                    xmlwriter.SetValue(section, "nextTags", TagsToString(NextTags));

                    xmlwriter.SetValueAsBool(section, "proxy", UseProxy);
                    xmlwriter.SetValue(section, "proxy_server", Server);
                    xmlwriter.SetValue(section, "proxy_port", Port);
                }
                else
                {
                    xmlwriter.SetValue(section, "bookmark", View);
                    xmlwriter.SetValue(section, "lastUrl", LastUrl);
                }
            }
        }

    }
}
