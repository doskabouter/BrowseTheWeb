using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Skybound.Gecko;

namespace BrowseTheWeb
{
  public partial class GetThumb : Form
  {
    public string SelectedUrl = string.Empty;
    private GeckoWebBrowser browser;
    private Bitmap snap;
    private int value;

    private bool received = false;
    private int time = 0;

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

      browser.Size = new Size(600, 800);
      browser.DocumentCompleted += new EventHandler(browser_DocumentCompleted);
      browser.Navigate(SelectedUrl);
    }

    private void browser_DocumentCompleted(object sender, EventArgs e)
    {
      if (browser.Url.ToString() != "about:blank")
      {
        snap = new Bitmap(browser.Width, browser.Height);
        browser.DrawToBitmap(snap, new Rectangle(0, 0, browser.Width, browser.Height));

        snap = MediaPortal.Util.BitmapResize.Resize(ref snap, 300, 400, false, true);

        Graphics g = Graphics.FromImage((Image)snap);
        g.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(1, 1, snap.Width - 2, snap.Height - 2));

        Bookmark.SaveSnap(snap, SelectedUrl);
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
      value++;
      if (value > 100) value = 1;
      progressBar1.Value = value;

      if (received)
      {
        time++;
        if (time > 20) this.Close();
      }
    }
  }
}
