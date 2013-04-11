using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using Curtin.Framework.Common.Classes;
using Urdms.Dmp.Web.Menu;

namespace Urdms.Dmp.Web.Pages {
    /// <summary>
    /// Class to allow the setting of metadata and settings for the current page
    /// </summary>
    public class PageSettings {
        /// <summary>
        /// Static cache of the base PageSettings so that the config file values
        ///     don't need to be parsed for every page request
        /// </summary>
        private static PageSettings _pagesettingsCache;
        private static NameValueCollection _metaDataSection;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Creator { get; set; }
        public string SiteTitle { get; set; }
        public bool TopNavDisplay { get; set; }
        public bool MenuNavDisplay { get; set; }
        public bool GlobalNavDisplay { get; set; }
        public bool SideContentDisplay { get; set; }
        public MenuHelper Menu { get; set; }
        public List<Classification> Classifications { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Clone constructor for the PageSettings class
        /// </summary>
        /// <param name="pageSettings">The PageSettings object to copy the properties of</param>
        private PageSettings(PageSettings pageSettings) {
            Title = pageSettings.Title;
            Description = pageSettings.Description;
            Keywords = pageSettings.Keywords;
            Creator = pageSettings.Creator;
            SiteTitle = pageSettings.SiteTitle;
            TopNavDisplay = pageSettings.TopNavDisplay;
            MenuNavDisplay = pageSettings.MenuNavDisplay;
            GlobalNavDisplay = pageSettings.GlobalNavDisplay;
            SideContentDisplay = pageSettings.SideContentDisplay;
            Menu = pageSettings.Menu;
            #if DEBUG
            Menu = new MenuHelper("~/Menu.json", ConfigurationManager.AppSettings);
            #endif
            Classifications = new List<Classification>(pageSettings.Classifications);
            DateCreated = pageSettings.DateCreated;
            DateModified = pageSettings.DateModified;
        }

        public static void SetMetaData(NameValueCollection metadata)
        {
            _metaDataSection = metadata;
        }

        /// <summary>
        /// Constructor for the PageSettings class; loads data from the Metadata section of
        ///     the config file
        /// </summary>
        private PageSettings() {
            Classifications = new List<Classification>();

            if (_metaDataSection == null)
            {
                _metaDataSection = (NameValueCollection)ConfigurationManager.GetSection("Metadata") ??
                    new NameValueCollection();
            }

            // Set string metadata from the config
            Title = GetConfigItem(ref _metaDataSection, "Title");
            Description = GetConfigItem(ref _metaDataSection, "Description");
            Keywords = GetConfigItem(ref _metaDataSection, "Keywords");
            Creator = GetConfigItem(ref _metaDataSection, "Creator");
            SiteTitle = GetConfigItem(ref _metaDataSection, "SiteTitle");
            TopNavDisplay = GetConfigItem(ref _metaDataSection, "TopNavDisplay").Equals("true");
            MenuNavDisplay = GetConfigItem(ref _metaDataSection, "MenuNavDisplay").Equals("true");
            GlobalNavDisplay = GetConfigItem(ref _metaDataSection, "GlobalNavDisplay").Equals("true");
            SideContentDisplay = GetConfigItem(ref _metaDataSection, "SideContentDisplay").Equals("true");

            // Parse any date created string in the config
            var dateCreated = GetConfigItem(ref _metaDataSection, "DateCreated");
            if (!string.IsNullOrEmpty(dateCreated)) {
                DateCreated = DateTime.Parse(dateCreated);
            }

            Menu = new MenuHelper("~/Menu.json", ConfigurationManager.AppSettings);

            // Find any classifications
            foreach (String s in _metaDataSection.AllKeys) {
                if (String.Compare("Classification", 0, s, 0, 14) == 0)
                {
                    Classifications.Add((Classification) StringEnum.Parse(typeof (Classification), _metaDataSection[s]));
                }
            }

            // Set date modified as the creation date of the assembly by default
            if (Assembly.GetExecutingAssembly().Location != null) {
                try {
// ReSharper disable AssignNullToNotNullAttribute
                    DateModified = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
// ReSharper restore AssignNullToNotNullAttribute
// ReSharper disable EmptyGeneralCatchClause
                } catch (Exception) { } // Suppress
// ReSharper restore EmptyGeneralCatchClause
            }
        }

        private static string GetConfigItem(ref NameValueCollection section, string key)
        {
            if (section[key] != null) {
                return section[key];
            }
            return "";
        }

// ReSharper disable FieldCanBeMadeReadOnly.Local
        private static object _locker = new object();
// ReSharper restore FieldCanBeMadeReadOnly.Local

        /// <summary>
        /// Returns a reference to the PageSettings object for the current request and
        ///     creates one if it doesn't exist yet
        /// </summary>
        /// <returns>The PageSettings object for the current request</returns>
        public static PageSettings GetPageSettings() {
            InitializeCache();

            if (HttpContext.Current != null)
            {
                if (!HttpContext.Current.Items.Contains("PageSettings"))
                    HttpContext.Current.Items["PageSettings"] = new PageSettings(_pagesettingsCache);
                return (PageSettings) HttpContext.Current.Items["PageSettings"];
            }

            return _pagesettingsCache;
        }

        private static void InitializeCache()
        {
            if (_pagesettingsCache != null)
                return;

            lock (_locker)
            {
                if (_pagesettingsCache == null)
                {
                    _pagesettingsCache = new PageSettings();
                }
            }
        }

        public static void ResetCache()
        {
            _pagesettingsCache = null;
            _metaDataSection = null;
        }
    }
}
