using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Codeplex.Data;
using Curtin.Framework.Common.Extensions;

namespace Urdms.Dmp.Web.Menu
{
    public static class HttpContextPathWrapper
    {
        public static Func<string, string> MapPath = (path) => HttpContext.Current.Server.MapPath(path);
    }

    public class MenuHelper
    {
        private readonly IList<MenuItem> _menu;

        public MenuHelper(string file, NameValueCollection appSettings)
        {
            _menu = LoadMenu(HttpContextPathWrapper.MapPath(file), appSettings);
        }

        private static IList<MenuItem> LoadMenu(string file, NameValueCollection appSettings)
        {
            if (!File.Exists(file)) 
            {
                throw new FileNotFoundException("Couldn't find menu file.", file);
            }
            string content = File.ReadAllText(file);
            content = content.ReReplace(@"\$\((.*?)\)", m =>
            {
                if (m.Groups.Count == 2)
                {
                    var key = m.Groups[1].Value;
                    if (appSettings.AllKeys.Contains(key))
                    {
                        return appSettings[(string) key];
                    }
                    throw new KeyNotFoundException(string.Format("{0} key, specified in menu, missing from appsettings (in Web.config)", (object) key));
                }
                return m.Value;
            });
            var json = DynamicJson.Parse(content);
            return BuildMenuFromJson(json);
        }

        public static List<MenuItem> BuildMenuFromJson(dynamic json)
        {
            if (!json.IsArray) throw new Exception("Menu not an array");
            var menu = new List<MenuItem>();
            foreach (var item in json)
            {
                if (!item.title()) throw new Exception("Menu item missing title");
                if (!item.paths() || !item.paths.IsArray) throw new Exception("Menu item missing array of paths");
                var paths = (List<string>)item.paths;
                if (paths.IsEmpty()) throw new Exception("At least one path is required");

                bool? auth = null;
                if (item.auth())
                    auth = (bool?)item.auth;

                var roles = new List<string>();
                if (item.roles())
                {
                    roles = (List<string>)item.roles;
                    auth = true;
                }

                var temp = new MenuItem { Title = item.title, Auth = auth, Roles = roles, AdditionalPaths = paths };
                temp.Path = temp.AdditionalPaths.FirstOrDefault();
                temp.AdditionalPaths.RemoveAt(0);
                if (item.submenu())
                {
                    temp.Children = BuildMenuFromJson(item.submenu);
                }
                menu.Add(temp);
            }
            return menu;
        }

        public Menus GetMenus(bool topNavDisplay = true)
        {
            var result = new Menus();
            var selected = GetSelected(_menu, HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.Replace("~/", "/"));
            var menuCopy = _menu.Select(i => i.CopyWithChildrenWhenSelected(selected)).ToList();

            if (topNavDisplay)
            {
                result.TopNav = menuCopy;
                var selectedTopNavMenuItem = menuCopy.Where(i => i.Selected).FirstOrDefault();
                if (selectedTopNavMenuItem != null)
                {
                    result.MenuNav = selectedTopNavMenuItem.Children;
                    selectedTopNavMenuItem.Children = new List<MenuItem>();
                }
                else
                {
                    result.MenuNav = new List<MenuItem>();
                }
            }
            else
            {
                result.MenuNav = menuCopy;
            }
            return result;
        }

        private static IList<int> GetSelected(IEnumerable<MenuItem> menu, string path)
        {
            var selected = new List<int>();
            foreach (var item in menu)
            {
                var subSelected = item.Children.IsNotEmpty() ? GetSelected(item.Children, path) : new List<int>();
                if (subSelected.IsNotEmpty())
                {
                    selected.Add(item.GetHashCode());
                    selected.AddRange(subSelected);
                    return selected;
                }
                else if (item.Path == path || item.AdditionalPaths.Any(p => PathsAreEquivalent(p, path)))
                {
                    selected.Add(item.GetHashCode());
                    return selected;
                }
            }
            return selected;
        }

        private static bool PathsAreEquivalent(string fromMenu, string fromUri)
        {
            if (fromMenu == fromUri) return true;
            if (fromMenu.Contains("*"))
            {
                var r = new Regex("^"+fromMenu.Replace("*", "[^/]*?")+"$");
                if (r.IsMatch(fromUri))
                {
                    return true;
                }
            }
            return false;
        }
    }
}