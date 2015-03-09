using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Gecko;
using Gecko.Collections;
using Gecko.DOM;

namespace BrowseTheWeb
{
    public class LinkHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            public uint Type;
            public MOUSEINPUT Mouse;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private bool clickFromPlugin = false;
        private Timer restoreClickTimer = new Timer();

        private GeckoWebBrowser webBrowser;
        private Action<GeckoSelectElement> showSelect;
        private ShowKeyboard showKeyboard;

        public delegate void ShowKeyboard(string title, string value, bool isPassword, Action<string> action);
        public delegate void ShowSelect(GeckoElement select);

        public LinkHelper(GeckoWebBrowser webBrowser, Action<GeckoSelectElement> showSelect, ShowKeyboard showKeyboard)
        {
            this.webBrowser = webBrowser;
            this.showSelect = showSelect;
            this.showKeyboard = showKeyboard;
        }

        public void Init()
        {
            restoreClickTimer.Enabled = false;
            restoreClickTimer.Interval = 500;
            restoreClickTimer.Tick += new EventHandler(restoreClickTimer_Tick);

            webBrowser.DomClick += new EventHandler<DomMouseEventArgs>(webBrowser_DomClick);
        }

        public void Done()
        {
            webBrowser.DomClick -= webBrowser_DomClick;

            restoreClickTimer.Stop();
            restoreClickTimer.Tick -= restoreClickTimer_Tick;
        }

        public void OnLinkId(string LinkId, float zoom)
        {
            GeckoHtmlElement ge = DomHelper.GetElement(LinkId, webBrowser.Document);

            if (ge == null)
            {
                MyLog.debug(String.Format("LinkId {0} not found in _htmlLinkNumbers", LinkId));
                return;
            }

            if (ge is GeckoAnchorElement && !ge.HasAttribute("onclick"))
            {
                string link = ((GeckoAnchorElement)ge).Href;
                webBrowser.Navigate(link);
                MyLog.debug("navigate to linkid=" + LinkId + " URL=" + link);
            }
            else
                if (ge is GeckoButtonElement)
                {
                    ge.Click();
                }
                else
                    if (ge is GeckoSelectElement)
                    {
                        showSelect(ge as GeckoSelectElement);
                    }
                    else
                        if (ge is GeckoInputElement)
                        {
                            string linkType = ((GeckoInputElement)ge).Type;
                            if (!String.IsNullOrEmpty(linkType))
                            {
                                switch (linkType)
                                {
                                    case "password": ShowInputDialog(true, ge as GeckoInputElement); break;
                                    case "submit":
                                    case "reset":
                                    case "radio":
                                    case "image":
                                    case "checkbox":
                                        ge.Click();
                                        MyLog.debug("action linkid=" + LinkId);
                                        break;
                                    case "hidden": break;
                                    default: ShowInputDialog(false, ge as GeckoInputElement); break;
                                }
                            }
                        }
                        else
                        //if (ge is GeckoObjectElement)
                        // some items just need a mousehover, and a ge.Click won't do that
                        {
                            Point p = DomHelper.GetCenterCoordinate(webBrowser.Document, ge);
                            p.X = Convert.ToInt32(p.X * zoom);
                            p.Y = Convert.ToInt32(p.Y * zoom);
                            ClickOn(p);
                        }
            //else
            // ge.Click();
        }

        private int delta = 5;
        public void ClickOn(Point? p)
        {
            webBrowser.Enabled = true;
            if (p.HasValue)
            {
                Point newP = new Point(p.Value.X + delta, p.Value.Y);
                delta = -delta;// some flash vids don't react to clicking on previous coordinate
                MyLog.debug("perform click on " + newP.ToString());
                Cursor.Position = webBrowser.PointToScreen(newP);
            }

            INPUT[] ips = new INPUT[] { new INPUT { Type = 0 }, new INPUT { Type = 0 } };
            ips[0].Mouse.Flags = MOUSEEVENTF_LEFTDOWN;
            ips[1].Mouse.Flags = MOUSEEVENTF_LEFTUP;

            clickFromPlugin = true;
            restoreClickTimer.Start();
            if (SendInput(2, ips, Marshal.SizeOf(typeof(INPUT))) == 0)
                MyLog.debug("Error sendinput");
        }


        private void ResetFocus()
        {
            clickFromPlugin = false;
            restoreClickTimer.Stop();

            webBrowser.Enabled = false;
            webBrowser.Parent.Focus();
        }

        private void webBrowser_DomClick(object sender, DomEventArgs e)
        {
            if (clickFromPlugin) // click succeeded, so focus can safely be reset
                ResetFocus();
        }

        private void restoreClickTimer_Tick(object sender, EventArgs e)
        {
            ResetFocus();
        }

        public void ShowInputDialog(bool isPassword, GeckoInputElement element)
        {
            showKeyboard(element.Name, element.Value, isPassword, delegate(string result)
            {
                if (element != null)
                    element.SetAttribute("value", result);
                GeckoFormElement form = element.Form;
                if (form != null)
                {
                    IDomHtmlCollection<GeckoElement> inps = form.GetElementsByTagName("input");
                    if (DomHelper.NrOfChildElementsDone(form) == 1)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (GeckoInputElement inp in inps)
                        {
                            if (sb.Length != 0)
                                sb.Append('&');
                            sb.Append(inp.Name);
                            sb.Append('=');
                            sb.Append(inp.Value);
                        }

                        if (form.Method == "get")
                            webBrowser.Navigate(form.Action + '?' + sb.ToString());
                        else
                        {
                            using (Gecko.IO.MimeInputStream stream = Gecko.IO.MimeInputStream.Create())
                            {
                                stream.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                stream.AddContentLength = true;
                                stream.SetData(sb.ToString());
                                webBrowser.Navigate(form.Action, 0, null, stream);
                            }
                        }
                    }
                }

            });
        }
    }
}