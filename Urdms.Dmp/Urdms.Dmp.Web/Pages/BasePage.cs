using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Urdms.Dmp.Web.Pages
{
    public abstract class BasePage<T> : WebViewPage<T>
    {
        public PageSettings PageSettings { get { return PageSettings.GetPageSettings(); } }
        
        public void AddScript(string script)
        {
            Regex localPath = new Regex(@"^\w");
            if (localPath.Match(script).Success)
            {
                script = "~/Scripts/" + script;
                if (!script.EndsWith(".js"))
                {
                    script += ".js";
                }
            }

            if (ViewBag.Scripts == null)
            {
                ViewBag.Scripts = new List<string>();
            }

            ViewBag.Scripts.Add(script);
        }

        public void AddStylesheet(string cssFile)
        {
            Regex localPath = new Regex(@"^\w");
            if (localPath.Match(cssFile).Success)
            {
                cssFile = "~/Content/Css/" + cssFile;
                if (!cssFile.EndsWith(".css"))
                {
                    cssFile += ".css";
                }
            }

            if (ViewBag.Stylesheets == null)
            {
                ViewBag.Stylesheets = new List<string>();
            }

            ViewBag.Stylesheets.Add(cssFile);
        }
    }

    public abstract class BasePage : BasePage<object>
    {
        
    }
}