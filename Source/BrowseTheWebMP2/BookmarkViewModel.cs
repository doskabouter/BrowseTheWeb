using MediaPortal.Common.General;
using MediaPortal.UI.Presentation.DataObjects;

namespace BrowseTheWeb
{
    public class BookmarkViewModel : ListItem
    {

        protected AbstractProperty _nameProperty;
        public AbstractProperty NameProperty { get { return _nameProperty; } }
        public string Name
        {
            get { return (string)_nameProperty.GetValue(); }
            set { _nameProperty.SetValue(value); }
        }

        protected AbstractProperty _thumbProperty;
        public AbstractProperty ThumbProperty { get { return _thumbProperty; } }
        public string Thumb
        {
            get { return (string)_thumbProperty.GetValue(); }
            set { _thumbProperty.SetValue(value); }
        }

        protected BookmarkBase _Bookmark;
        public BookmarkBase Bookmark
        {
            get { return _Bookmark; }
        }

        public BookmarkViewModel(BookmarkBase Bookmark)
            : this(Bookmark, Bookmark.Name)
        { }

        public BookmarkViewModel(BookmarkBase Bookmark, string Name)
            : base("Name", Name)
        {
            _Bookmark = Bookmark;

            _nameProperty = new WProperty(typeof(string), Name);
            if (Bookmark is BookmarkFolder)
                _thumbProperty = new WProperty(typeof(string), "folder.png");
            else
                _thumbProperty = new WProperty(typeof(string), BrowseTheWeb.Bookmark.GetSnapPath(((BookmarkItem)Bookmark).Url));
        }
    }
}
