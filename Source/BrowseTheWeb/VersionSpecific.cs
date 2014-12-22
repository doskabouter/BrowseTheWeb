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

using System.Drawing;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;

namespace BrowseTheWeb
{
    /// <summary>
    /// just some log if needed
    /// </summary>
    public static class MyLog
    {
        public static void debug(string str)
        {
            Log.Debug("BrowseTheWeb | " + str, new object[0]);
        }

        public static void error(string str)
        {
            Log.Error("BrowseTheWeb | " + str, new object[0]);
        }
    }

    public static class VersionSpecific
    {
        public static string ThumbDir = Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs) + "\\BrowseTheWeb";

        public static Image Resize(Bitmap image)
        {
          return MediaPortal.Util.BitmapResize.Resize(ref image, 300, 400, false, true);
        }

    }
}

