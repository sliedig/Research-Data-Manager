using System;
using System.Configuration;
using System.Linq.Expressions;
using System.Net;
using Microsoft.SharePoint.Client;
using log4net;

namespace Urdms.ProvisioningService.SharePoint.Helpers
{
    public class SharePointHelper : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _root = ConfigurationManager.AppSettings["SpRoot"];
        private readonly string _spManagedPath = ConfigurationManager.AppSettings["SpManagedPath"];
        private readonly ClientContext _clientContext;
        private readonly Web _rootWeb;
        private readonly string _rootWebUrl;

        public ClientContext ClientContext
        {
            get { return _clientContext; }
        }

        public string Root
        {
            get { return _root; }
        }

        public Web RootWeb
        {
            get { return _rootWeb; }
        }

        public string AdDomainNameUrl
        {
            get { return ConfigurationManager.AppSettings["SpDomain"]; }
        }

        public string RootWebUrl
        {
            get { return _rootWebUrl; }
        }

        public SharePointHelper()
        {
            _rootWebUrl = string.Format("{0}/{1}", _root, _spManagedPath);
            _clientContext = NewClientContext(RootWebUrl);
            _rootWeb = _clientContext.Web;
        }

        public SharePointHelper(string siteUrl)
        {
            _rootWebUrl = siteUrl;
            _clientContext = NewClientContext(RootWebUrl);
            _rootWeb = _clientContext.Web;
        }

        internal bool TryLoadRootWeb(params Expression<Func<Web, object>>[] retrievals)
        {
            try
            {
                _clientContext.Load(_rootWeb, retrievals);
                _clientContext.ExecuteQuery();
                return _rootWeb.Id != Guid.Empty;
            }
            catch
            {
                throw new Exception(string.Format("[URDMS] Unable to access SharePoint root {0}. ", RootWebUrl));
            }
        }

        internal string CreateWeb(string siteName, string siteTitle, string siteDescription, string templateName)
        {

            var webCreationInformation = new WebCreationInformation
             {
                 Title = siteTitle,
                 Description = siteDescription,
                 WebTemplate = templateName,
                 Language = 1033,
                 Url = siteName
             };

            var newSite = _rootWeb.Webs.Add(webCreationInformation);

            _clientContext.Load(newSite);
            _clientContext.ExecuteQuery();

            return string.Format("{0}{1}", _root, newSite.ServerRelativeUrl);
        }

        private static ClientContext NewClientContext(string url)
        {
            var context = new ClientContext(url)
                {
                    Credentials = GetAdminCredentials()
                };
            
            return context;
        }

        private static NetworkCredential GetAdminCredentials()
        {
            return new NetworkCredential
                       {
                           UserName = ConfigurationManager.AppSettings["SpServiceAccount"],
                           Password = ConfigurationManager.AppSettings["SpServiceAccountPassword"],
                           Domain = ConfigurationManager.AppSettings["SpDomain"]
                       };
        }

        public void Dispose()
        {
            if (_clientContext != null)
                _clientContext.Dispose();
        }
    }
}
