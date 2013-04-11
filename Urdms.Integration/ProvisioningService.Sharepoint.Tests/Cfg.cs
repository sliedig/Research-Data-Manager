using System;
using System.Configuration;

namespace ProvisioningService.SharePoint.Tests
{
    public static class Cfg
    {
      public static string SpRoot = ConfigurationManager.AppSettings["SpRoot"];
      public static string SpSysAccount = ConfigurationManager.AppSettings["SpServiceAccount"];
      public static string SpSysAccountPass = ConfigurationManager.AppSettings["SpServiceAccountPassword"];
      public static string SpDomain = ConfigurationManager.AppSettings["SpDomain"];
      public static string SpSiteNamePrefix = ConfigurationManager.AppSettings["SiteNamePrefix"];
      public static string SpSiteManagedPath = ConfigurationManager.AppSettings["SpManagedPath"];
      public static string SpTemplateName = ConfigurationManager.AppSettings["SpTemplateName"];
                 
    }
}
