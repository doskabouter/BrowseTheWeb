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
using System.Windows.Forms;

namespace BrowseTheWeb
{
    public partial class OSD_LinkId : UserControl
    {
        private Timer timer = new Timer();
        private string linkId = String.Empty;
        private bool enabled;

        public OSD_LinkId()
        {
            InitializeComponent();
            Visible = false;
            timer.Tick += new EventHandler(timer_Tick);
        }

        public int VisibleTime
        {
            set { timer.Interval = value; }
        }

        public new bool Enabled
        {
            set { enabled = value; }
        }

        public string ID
        {
            get { return linkId; }
            set
            {
                txtId.Text = value;
                linkId = value;
                if (enabled)
                {
                    Visible = true;
                    timer.Stop();
                    timer.Start();
                    BringToFront();
                }
                this.Invalidate();
            }
        }

        public void AddChar(char c)
        {
            string newId = linkId + c;
            if (newId.Length > 4) newId = newId.Substring(1);
            ID = newId;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            HideOSD();
        }

        public void HideOSD()
        {
            timer.Stop();
            Visible = false;
            linkId = String.Empty;
        }

    }
}
