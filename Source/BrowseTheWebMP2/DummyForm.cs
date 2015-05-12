using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Gecko;
using Gecko.Events;

namespace BrowseTheWeb
{
    public partial class DummyForm : Form
    {
        public GeckoWebBrowser webBrowser;

        public DummyForm(string xulRunnerPath, EventHandler<GeckoDocumentCompletedEventArgs> documentCompleted, Point location, Size size)
        {
            InitializeComponent();
            this.Location = location;
            this.Size = size;
            statusLabel.Visible = Settings.Instance.StatusVisible;

            webBrowser = new Gecko.GeckoWebBrowser();
            webBrowser.Name = "BrowseTheWeb";
            webBrowser.NoDefaultContextMenu = true;
            webBrowser.Enabled = Settings.Instance.UseMouse;
            this.Enabled = Settings.Instance.UseMouse;
            webBrowser.Visible = true;
            webBrowser.Dock = DockStyle.Fill;

            MyLog.debug("xpcom init");

            try
            {
                Gecko.Xpcom.Initialize(xulRunnerPath);
            }
            catch (Exception ex)
            {
                MyLog.debug("Could not find xulrunner under : " + xulRunnerPath);
                MyLog.debug("Reason : " + ex.Message);
            }
            MyLog.debug("xpcom inited");

            webBrowser.DocumentCompleted += documentCompleted;
            webBrowser.CreateWindow += webBrowser_CreateWindow;
            webBrowser.DomClick += webBrowser_DomClick;
            Closed += (sender2, e2) => Dispatcher.CurrentDispatcher.InvokeShutdown();
            Controls.Add(webBrowser);
            Controls.SetChildIndex(webBrowser, 0);//webbrowser.Dock only works correctly if it's before the statuslabel
        }

        void webBrowser_DomClick(object sender, DomMouseEventArgs e)
        {
            Activate();
        }

        void webBrowser_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        {
            e.Cancel = true;
        }

        public new void Resize(Point location, Size size)
        {
            this.Location = location;
            this.Size = size;
        }

        public void ToggleStatus()
        {
            ExecuteSafely(delegate
            {
                statusLabel.Visible = !statusLabel.Visible;
            });
        }

        public void SetStatus(string status)
        {
            ExecuteSafely(delegate
            {
                statusLabel.Text = status;
            });
        }

        public void ExecuteSafely(Action a)
        {
            if (InvokeRequired)
                Invoke(a);
            else
                a();
        }

        public void ExecuteSafely(Action<string> a, string p)
        {
            if (InvokeRequired)
                Invoke(a, p);
            else
                a(p);
        }

        public string ExecuteSafely(Func<string> a)
        {
            if (InvokeRequired)
                return (string)Invoke(a);
            else
                return a();
        }

        public void SafeDispose()
        {
            ExecuteSafely(delegate { Dispose(); });
        }

        private enum ExtendedWindowStyles : uint
        {
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_NOACTIVATE = 0x08000000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_WINDOWEDGE = 0x00000100
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                baseParams.ExStyle |= (int)(
                    ExtendedWindowStyles.WS_EX_NOACTIVATE |
                    ExtendedWindowStyles.WS_EX_TOOLWINDOW);

                return baseParams;
            }
        }

    }
}
