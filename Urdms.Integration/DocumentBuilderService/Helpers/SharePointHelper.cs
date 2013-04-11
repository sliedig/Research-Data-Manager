using System.Configuration;
using System.IO;
using System.Net;
using Microsoft.SharePoint.Client;
using File = System.IO.File;

namespace Urdms.DocumentBuilderService.Helpers
{
    public interface ISharePointHelper
    {
        void UploadDocumentToSharePoint(string filePath, string rootSiteUrl);
    }

    public class SharePointHelper : ISharePointHelper
    {
        public void UploadDocumentToSharePoint(string filePath, string rootSiteUrl)
        {
            var context = new ClientContext(rootSiteUrl)
                              {
                                  AuthenticationMode = ClientAuthenticationMode.Default,
                                  Credentials =
                                      new NetworkCredential(ConfigurationManager.AppSettings["SharePointAdmin"],
                                                            ConfigurationManager.AppSettings["SharePointAdminPassword"],
                                                            ConfigurationManager.AppSettings["SharePointAdminDomain"])
                              };

            var docLibrary = context.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["SharePointProjectDocumentLibrary"]);
            var uploadFile = docLibrary.RootFolder.Files.Add(new FileCreationInformation
                                                                 {
                                                                     Content = File.ReadAllBytes(filePath),
                                                                     Overwrite = true,
                                                                     Url = Path.GetFileName(filePath)
                                                                 });
            
            context.Load(uploadFile);
            context.ExecuteQuery();
        }
    }
}