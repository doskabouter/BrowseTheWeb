using System.Collections.Generic;
using System.Linq;
using MediaPortal.Common.Settings;
using MediaPortal.Common.Configuration.ConfigurationClasses;

namespace BrowseTheWeb
{
    public class Settings
    {
        public bool UseHomePage { get; set; }
        public string[] PreviousTags { get; set; }
        public string[] NextTags { get; set; }
        public bool UseMouse { get; set; }
        public string HomePage { get; set; }
        public double OSDTime { get; set; }
        public bool StatusVisible { get; set; }

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
        private Settings() { }
        #endregion

    }
}
namespace BrowseTheWeb.Configuration
{
    public class Settings
    {
        [Setting(SettingScope.User, true)]
        public bool UseHomePage { get; set; }
        [Setting(SettingScope.User, new string[] { })]
        public string[] PreviousTags { get; set; }
        [Setting(SettingScope.User, new string[] { })]
        public string[] NextTags { get; set; }
        [Setting(SettingScope.User, true)]
        public bool UseMouse { get; set; }
        [Setting(SettingScope.User, "about:blank")]
        public string HomePage { get; set; }
        [Setting(SettingScope.User, 1.5)]
        public double OSDTime { get; set; }
        [Setting(SettingScope.User, false)]
        public bool StatusVisible { get; set; }

        public Settings()
        {

        }

        public void SetValuesToApi()
        {
            BrowseTheWeb.Settings.Instance.UseHomePage = UseHomePage;
            BrowseTheWeb.Settings.Instance.PreviousTags = PreviousTags;
            BrowseTheWeb.Settings.Instance.NextTags = NextTags;
            BrowseTheWeb.Settings.Instance.UseMouse = UseMouse;
            BrowseTheWeb.Settings.Instance.HomePage = HomePage;
            BrowseTheWeb.Settings.Instance.OSDTime = OSDTime;
            BrowseTheWeb.Settings.Instance.StatusVisible = StatusVisible;
        }
    }

    public class UseHomePage : YesNo
    {
        public override void Load()
        {
            Yes = SettingsManager.Load<Settings>().UseHomePage;
        }

        public override void Save()
        {
            Settings settings = SettingsManager.Load<Settings>();
            settings.UseHomePage = Yes;
            SettingsManager.Save(settings);
        }
    }

    public class HomePage : Entry
    {
        public override void Load()
        {
            Value = SettingsManager.Load<Settings>().HomePage;
        }

        public override void Save()
        {
            Settings settings = SettingsManager.Load<Settings>();
            settings.HomePage = Value;
            SettingsManager.Save(settings);
        }

        public override int DisplayLength { get { return 255; } }
    }

    public class UseMouse : YesNo
    {
        public override void Load()
        {
            Yes = SettingsManager.Load<Settings>().UseMouse;
        }

        public override void Save()
        {
            Settings settings = SettingsManager.Load<Settings>();
            settings.UseMouse = Yes;
            SettingsManager.Save(settings);
        }
    }

    public class Tags : MultipleEntryList
    {
        public override int DisplayHeight
        {
            get { return 12; }
        }

        public override int DisplayLength
        {
            get { return 20; }
        }
    }

    public class PreviousTags : Tags
    {
        public override void Load()
        {
            Lines = new List<string>(SettingsManager.Load<Settings>().PreviousTags);
        }

        public override void Save()
        {
            Settings settings = SettingsManager.Load<Settings>();
            settings.PreviousTags = Lines.ToArray();
            SettingsManager.Save(settings);
        }
    }

    public class NextTags : Tags
    {
        public override void Load()
        {
            Lines = new List<string>(SettingsManager.Load<Settings>().NextTags);
        }

        public override void Save()
        {
            Settings settings = SettingsManager.Load<Settings>();
            settings.NextTags = Lines.ToArray();
            SettingsManager.Save(settings);
        }
    }

    public class OSDTime : LimitedNumberSelect
    {
        public override void Load()
        {
            _type = NumberType.FloatingPoint;
            LowerLimit = 0;
            UpperLimit = 60;
            Step = 0.1;
            Value = SettingsManager.Load<Settings>().OSDTime;
        }

        public override void Save()
        {
            Settings settings = SettingsManager.Load<Settings>();
            settings.OSDTime = Value;
            SettingsManager.Save(settings);
        }
    }

    public class StatusVisible : YesNo
    {
        public override void Load()
        {
            Yes = SettingsManager.Load<Settings>().StatusVisible;
        }

        public override void Save()
        {
            Settings settings = SettingsManager.Load<Settings>();
            settings.StatusVisible = Yes;
            SettingsManager.Save(settings);
        }
    }
}
