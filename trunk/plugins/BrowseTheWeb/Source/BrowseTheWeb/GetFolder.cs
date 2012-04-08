﻿#region Copyright (C) 2005-2010 Team MediaPortal

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
    public partial class GetFolder : Form
    {
        public string SelectedFolderName = string.Empty;

        public GetFolder()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
        }
        private void GetFolder_Load(object sender, EventArgs e)
        {
            txtName.Text = SelectedFolderName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtName.Text != string.Empty)
            {
                this.DialogResult = DialogResult.OK;
                SelectedFolderName = txtName.Text;
                Close();
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(this, new EventArgs());
            }
        }


    }
}
