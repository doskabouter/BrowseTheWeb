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
using System.Drawing;
using System.Windows.Forms;

using Gecko;

namespace BrowseTheWeb.Setup
{
    public partial class GetThumb : Form
    {
        public string SelectedUrl = string.Empty;
        private GeckoWebBrowser browser;

        private bool received = false;
        private int time = 0;
        private int cancel = 0;

        public GetThumb()
        {
            InitializeComponent();

            browser = new GeckoWebBrowser();
            this.Controls.Add(browser);
        }

        private void GetThumb_Load(object sender, EventArgs e)
        {
            browser.Visible = false;
            chkUrl.Checked = true;

            txtUrl.Text = SelectedUrl;

            browser.Size = new Size(800, 1024);
            browser.DocumentCompleted += new EventHandler<Gecko.Events.GeckoDocumentCompletedEventArgs>(browser_DocumentCompleted);
            browser.Navigate(SelectedUrl);
        }

        private void browser_DocumentCompleted(object sender, EventArgs e)
        {
            if (Bookmark.GetAndSaveSnap(browser, SelectedUrl))
            {
                received = true;
                chkGetThumb.Checked = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cancel++;
            if (cancel > 100) this.Close();

            if (received)
            {
                time++;
                // implemented maybe later to get flash loaded...
                if (time > 0) this.Close();
            }
        }
    }
}
