using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.PathManager;

namespace BrowseTheWeb
{
    public static class MyLog
    {
        public const string BrowserThreadName = "BrowserThread";

        private static string GetPrefix()
        {
            if (Thread.CurrentThread.Name != BrowserThreadName)
                return "BrowseTheWeb | ";
            else
                return String.Empty;
        }
        public static void debug(string str)
        {
            ServiceRegistration.Get<ILogger>().Debug(GetPrefix() + str, new object[0]);
        }

        public static void error(string str)
        {
            ServiceRegistration.Get<ILogger>().Error(GetPrefix() + str, new object[0]);
        }

    }

    public static class VersionSpecific
    {
        public static string ThumbDir = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\BrowseTheWeb\Thumbs");

        public static Image Resize(Bitmap image)
        {
            return MediaPortal.Utilities.Graphics.ImageUtilities.ResizeImage(image, 300, 400, true);
        }

    }

}
