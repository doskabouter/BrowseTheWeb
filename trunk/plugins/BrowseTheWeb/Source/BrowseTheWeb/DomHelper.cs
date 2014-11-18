using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Gecko;
using Gecko.Collections;
using Gecko.DOM;

namespace BrowseTheWeb
{
    public class DomHelper
    {
        private const string _baseSpanStyle =
            "font-family: arial,sans-serif; " +
            "font-size: 12px ! important; " +
            "line-height: 130% ! important; " +
            "border-width: 1px ! important; " +
            "border-style: solid ! important; " +
            "border-radius: 2px 2px 2px 2px ! important; " +
            "max-height: 10px ! important; " +
            "overflow: visible ! important; " +
            "float: none ! important; " +
            "display: inline;";

        private const string _spanstyle = _baseSpanStyle +
            "padding: 0px 2px ! important; " +
            "margin-right: 2px; " +
            "max-width: 20px; " +
            "position: relative ! important; " +
            "top: 0; " +
            "left: 0; ";

        private const string _spanStyleObject = _baseSpanStyle +
            "padding: 0px 2px 4px 2px ! important; " +
            "margin-right: 8px; " +
            "max-width: 30px; " +
            "position: absolute ! important;" +
            "z-index-100; ";

        private const string btwebId = "btweb_id";

        private static string previousUrl;
        private static string nextUrl;

        public static Tuple<string, string> AddLinksToPage(GeckoDocument document, Settings settings)
        {
            int maxId = GetMaxId(document);
            if (maxId == 0)
            {
                //new page, so reset prev and next
                MyLog.debug("Reset prev and next url");
                previousUrl = null;
                nextUrl = null;
            }
            AddLinksToPage(document, maxId + 1, settings);
            return new Tuple<string, string>(previousUrl, nextUrl);
        }

        public static GeckoHtmlElement GetElement(string linkId, GeckoDocument document)
        {
            GeckoHtmlElement ge = document.GetElements(String.Format("//*[@{0}='{1}']", btwebId, linkId)).FirstOrDefault();
            if (ge != null)
                return ge;

            GeckoElementCollection iframes = document.GetElementsByTagName("iframe");
            foreach (GeckoIFrameElement element in iframes)
            {
                ge = GetElement(linkId, element.ContentDocument);
                if (ge != null)
                    return ge;
            }
            return null;
        }

        public static Point GetCenterCoordinate(GeckoDocument root, GeckoHtmlElement element)
        {
            Point documentOffset = DocumentOffset(root, element.OwnerDocument);
            Rectangle rect = element.GetBoundingClientRect();
            Point p = new Point(Convert.ToInt32(rect.Left + rect.Width / 2), Convert.ToInt32(rect.Top + rect.Height / 2));
            p.X += documentOffset.X;
            p.Y += documentOffset.Y;
            return p;
        }

        public static int NrOfChildElementsDone(GeckoHtmlElement element)
        {
            return element.GetElements(".//*[@" + btwebId + "]").Count();
        }

        private static Point DocumentOffset(GeckoDocument root, GeckoDocument current)
        {
            Point result = new Point(0, 0);
            if (root.Equals(current))
                return result;

            GeckoElementCollection iframes = root.GetElementsByTagName("iframe");
            foreach (GeckoIFrameElement element in iframes)
            {
                if (element.ContentDocument.Equals(current))
                {
                    Point tmp = DocumentOffset(root, element.OwnerDocument);
                    result.X += element.GetBoundingClientRect().Left + tmp.X;
                    result.Y += element.GetBoundingClientRect().Top + tmp.Y;
                    return result;
                }
            }
            return new Point(0, 0);
        }

        private static void Check(GeckoAnchorElement element, string[] tags, ref string url)
        {
            if (String.IsNullOrEmpty(url))
                foreach (string tag in tags)
                    if (element.TextContent.ToUpperInvariant().Contains(tag.ToUpperInvariant()))
                        url = element.Href;
        }

        private static int AddLinksToPage(GeckoDocument document, int id, Settings settings)
        {
            Dictionary<string, int> hrefs = new Dictionary<string, int>();
            GeckoElementCollection links = document.Links;
            MyLog.debug("page links cnt : " + links.Count<GeckoHtmlElement>());
            foreach (GeckoHtmlElement element in links) // no casting to GeckoAnchorElement, because document.links also returns GeckoAreaElemenets
                if (!element.GetAttribute("href").StartsWith("javascript:"))
                {
                    GeckoHtmlElement lastSpan = element;
                    bool ready = false;
                    while (!ready)
                    {
                        GeckoHtmlElement ls = lastSpan.LastChild as GeckoHtmlElement;
                        if (ls == null || ls.TagName != "SPAN")
                            ready = true;
                        else
                            lastSpan = ls;
                    };
                    if (!elementDone(element))
                    {
                        GeckoElement ls = element;
                        while (ls.LastChild != null && ls.LastChild is GeckoElement && !String.IsNullOrEmpty(ls.LastChild.TextContent))
                            ls = (GeckoElement)ls.LastChild;

                        int newId;
                        string url = element.GetAttribute("href");
                        if (!element.HasAttribute("onclick"))
                        {
                            if (hrefs.ContainsKey(url))
                                newId = hrefs[url];
                            else
                            {
                                newId = id++;
                                hrefs.Add(url, newId);
                            }

                            if (!String.IsNullOrEmpty(url) && element is GeckoAnchorElement)
                            {
                                Check((GeckoAnchorElement)element, settings.PreviousTags, ref previousUrl);
                                Check((GeckoAnchorElement)element, settings.NextTags, ref nextUrl);
                            }
                        }
                        else
                            newId = id++;

                        insertSpanAfter(newId, lastSpan.ClassName, ls);
                        SetLinkAttributes(element, newId);
                    }
                }

            foreach (GeckoHtmlElement element in links) // no casting to GeckoAnchorElement, because document.links also returns GeckoAreaElemenets
                if (!element.GetAttribute("href").StartsWith("javascript:") && element.ClientRects.Length == 0)
                //invisible, so find visible previousSibling/parent and put a number on that
                {
                    GeckoNode el = element;
                    while (el != null && !ElementVisible(el))
                    {
                        GeckoNode ps = el.PreviousSibling;
                        if (ps != null)
                            el = ps;
                        else
                            el = el.ParentNode;
                    }

                    if (el != null)// -> ElementVisible(el)=true, and thus el is a GeckoHtmlElement
                    {
                        GeckoHtmlElement geckoEl = (GeckoHtmlElement)el;

                        GeckoHtmlElement lastSpan = geckoEl;
                        bool ready = false;
                        while (!ready)
                        {
                            GeckoHtmlElement ls = lastSpan.LastChild as GeckoHtmlElement;
                            if (ls == null || ls.TagName != "SPAN")
                                ready = true;
                            else
                                lastSpan = ls;
                        };

                        if (!elementDone(geckoEl))
                        {

                            GeckoElement ls = geckoEl;
                            while (ls.LastChild != null && ls.LastChild is GeckoElement && !String.IsNullOrEmpty(ls.LastChild.TextContent))
                                ls = (GeckoElement)ls.LastChild;
                            insertSpanAfter(id, lastSpan.ClassName, ls);
                            SetLinkAttributes(geckoEl, id);
                            id++;
                        }
                    }
                }

            GeckoElementCollection objects = document.GetElementsByTagName("object");
            MyLog.debug("page objects cnt : " + objects.Count<GeckoHtmlElement>());
            foreach (GeckoObjectElement element in objects)
                if (element.Type == "application/x-shockwave-flash")
                {
                    if (!elementDone(element))
                    {
                        insertSpanAfter(id, null, element.Parent, "color:black;background-color:white", true);
                        SetLinkAttributes(element, id);
                        id++;
                    }
                }

            GeckoElementCollection embeds = document.GetElementsByTagName("embed");
            MyLog.debug("page embeds cnt : " + embeds.Count<GeckoHtmlElement>());
            foreach (GeckoEmbedElement element in embeds)
                if (element.Type == "application/x-shockwave-flash")
                {
                    if (!elementDone(element))
                    {
                        insertSpanAfter(id, null, element.Parent, "color:black;background-color:white", true);
                        SetLinkAttributes(element, id);
                        id++;
                    }
                }

            GeckoElementCollection forms = document.GetElementsByTagName("form");
            MyLog.debug("page forms cnt : " + forms.Count<GeckoHtmlElement>());
            foreach (GeckoHtmlElement element in forms)
            {
                IDomHtmlCollection<GeckoElement> inps = element.GetElementsByTagName("input");
                foreach (GeckoInputElement inp in inps)
                    if (!elementDone(inp))
                    {
                        string linkType = inp.Type;
                        if (!String.IsNullOrEmpty(linkType))
                        {
                            if (linkType != "hidden")
                            {
                                SetLinkAttributes(inp, id);

                                GeckoNode ps = inp.PreviousSibling;
                                while (ps != null && !(ps is GeckoHtmlElement))
                                    ps = ps.PreviousSibling;

                                if (inp.PreviousSibling != null)
                                    insertSpanBefore(id, null, inp);
                                else
                                    insertSpanAfter(id, null, inp.Parent);
                                id++;
                            }
                        }
                        else
                        {
                            SetLinkAttributes(inp, id);
                            insertSpanAfter(id, null, inp.Parent);
                            id++;
                        }
                    }

                IDomHtmlCollection<GeckoElement> buttons = element.GetElementsByTagName("button");
                foreach (GeckoHtmlElement button in buttons)
                    if (!elementDone(button) && button.ClientRects.Length != 0)
                    {
                        SetLinkAttributes(button, id);
                        insertSpanBefore(id, null, button);
                        id++;
                    }

                IDomHtmlCollection<GeckoElement> selects = element.GetElementsByTagName("select");
                foreach (GeckoHtmlElement select in selects)
                    if (!elementDone(select) && select.ClientRects.Length != 0)
                    {
                        SetLinkAttributes(select, id);
                        insertSpanBefore(id, null, select);
                        id++;
                    }
            }

            GeckoElementCollection iframes = document.GetElementsByTagName("iframe");
            MyLog.debug("page iframes cnt : " + iframes.Count<GeckoHtmlElement>());
            foreach (GeckoIFrameElement element in iframes)
                id = AddLinksToPage(element.ContentDocument, id, settings);
            return id;
        }

        private static int GetMaxId(GeckoDocument document)
        {
            int maxId = 0;
            try
            {
                foreach (GeckoHtmlElement ge in document.GetElements("//*[@" + btwebId + "]"))
                {
                    int j;
                    if (Int32.TryParse(ge.Attributes[btwebId].NodeValue, out j))
                        maxId = Math.Max(maxId, j);
                }
            }
            catch
            {
                // sometimes this causes an exception...
            };

            GeckoElementCollection iframes = document.GetElementsByTagName("iframe");
            foreach (GeckoIFrameElement element in iframes)
                maxId = Math.Max(maxId, GetMaxId(element.ContentDocument));
            return maxId;
        }

        private static GeckoHtmlElement CreateSpan(GeckoDocument owner, int geckoId, string className, string extra, bool isObject = false)
        {
            GeckoHtmlElement result = owner.CreateHtmlElement("span");
            if (isObject)
                result.SetAttribute("style", _spanStyleObject + extra);
            else
                result.SetAttribute("style", _spanstyle + extra);
            result.InnerHtml = geckoId.ToString();
            if (!String.IsNullOrEmpty(className))
                result.SetAttribute("class", className);
            return result;
        }

        private static GeckoElement insertSpanAfter(int geckoId, string className, GeckoNode after, string extra = "", bool isObject = false)
        {
            if (after == null)
                throw new ArgumentNullException("after");
            GeckoHtmlElement newChild = CreateSpan(after.OwnerDocument, geckoId, className, extra, isObject);
            if (after.FirstChild == null)
                after.AppendChild(newChild);
            else
                after.InsertBefore(newChild, after.FirstChild);
            return newChild;
        }

        private static GeckoElement insertSpanBefore(int geckoId, string className, GeckoNode before, string extra = "")
        {
            if (before == null)
                throw new ArgumentNullException("after");
            GeckoHtmlElement newElement = CreateSpan(before.OwnerDocument, geckoId, className, extra);
            before.ParentNode.InsertBefore(newElement, before);
            return newElement;
        }

        private static void SetLinkAttributes(GeckoElement link, int linkNumber)
        {
            link.SetAttribute(btwebId, linkNumber.ToString());
        }

        private static bool elementDone(GeckoElement element)
        {
            return !String.IsNullOrEmpty(element.GetAttribute(btwebId));
        }

        private static bool ElementVisible(GeckoNode el)
        {
            GeckoHtmlElement ge = el as GeckoHtmlElement;
            return ge != null && ge.OffsetHeight > 0;
        }


    }
}
