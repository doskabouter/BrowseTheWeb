Deployment new version:

1: Check assemblyinfo for version
2: do a release build
3: rename resulting mpe1 to include version number
4: edit BrowseTheWeb.xmp2 (and Change url of OnlineLocation of old files)
5: Edit http://www.team-mediaportal.com/extensions/news-info/browse-the-web-webbrowser
   - Update version
   - Update changelog
   - Upload new MPE1
6: Notify on forum

Change history (not maintained)
V0.1 inital release

V0.1.1 add diagnose remote / logs

V0.2
add thumbs for webpages
bookmark add / edit can be set even url is not valid
Ready for Info Service, Thanks to SilentExeption

V0.2.1 extract xulrunner first in setup

V0.2.2 add proxy support

V0.2.3 add force windowed mode

v0.2.4 import from IE and FF, some bug fix
v0.2.4.2 fixed remote link id time

v0.2.5
remote is now configurable
zoom max 300%
add mouse support
start work on alternate mouse support
save thumb if a bookmark is added inside the MP

0.2.6
add #currentmodule on pageload
add selector for view bookmarks

todo:
mouseless / mouse browsing

planed:
better alternate OSD
speed up (if possible), partly done
add statistics and sort functions
