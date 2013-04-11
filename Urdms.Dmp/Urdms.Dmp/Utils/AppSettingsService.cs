using System;
using System.Configuration;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Utils
{
    public interface IAppSettingsService
    {
        LibGuideSource LibGuideSource { get; }
        string LdapUser { get; }
        string LdapPassword { get; }
        string LdapUri { get; }
        string CsvSeparator { get; }
    }

    public class AppSettingsService : IAppSettingsService
    {
        public LibGuideSource LibGuideSource
        {
            get
            {
                var text = ConfigurationManager.AppSettings["LibGuideSource"] ?? "";
                LibGuideSource result;
                Enum.TryParse(text, true, out result);
                if (result == LibGuideSource.None)
                {
                    return LibGuideSource.Client;
                }
                return result;
            }
        }
        
        public string LdapUser
        {
            get { return ConfigurationManager.AppSettings["LdapUser"] ?? ""; }
        }

        public string LdapPassword
        {
            get { return ConfigurationManager.AppSettings["LdapPassword"] ?? ""; }
        }

        public string LdapUri
        {
            get { return ConfigurationManager.AppSettings["LdapURI"] ?? ""; }
        }
       
        public string CsvSeparator
        {
            get { return ConfigurationManager.AppSettings["CsvSeparator"] ?? ""; }
        }
    }
}