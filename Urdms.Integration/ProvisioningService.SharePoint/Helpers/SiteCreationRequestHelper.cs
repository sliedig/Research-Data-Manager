using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using log4net;
using NServiceBus.Logging;

namespace Urdms.ProvisioningService.SharePoint.Helpers
{
    public struct SiteCreationRequestArgs
    {
        public string SiteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Owners { get; set; }
        public string[] Members { get; set; }
        public string[] Visitors { get; set; }

    }

    public interface ISiteCreationRequestHelper
    {
        int CreateSiteRequest(SiteCreationRequestArgs args, string listName);
    }

    public class SiteCreationRequestHelper : ISiteCreationRequestHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SharePointHelper _sharePoint;
        private List _siteRequestList;


        public SiteCreationRequestHelper()
        {
            _sharePoint = new SharePointHelper();
        }

        public int CreateSiteRequest(SiteCreationRequestArgs args, string listName)
        {
            _siteRequestList = _sharePoint.RootWeb.Lists.GetByTitle(listName);

            // Check to see we haven't already created a list item for this project.
            var query = string.Format(@"<View><Query><Where><Eq><FieldRef Name='UrdmsSiteId'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", args.SiteId);
            var camlQuery = new CamlQuery { ViewXml = query };

            var existingRequests = _siteRequestList.GetItems(camlQuery);
            _sharePoint.ClientContext.Load(existingRequests);
            _sharePoint.ClientContext.ExecuteQuery();

            int id;

            if (existingRequests.Count == 0)
            {
                id = CreateNewRequest(args.SiteId, args.Title, args.Description, args.Owners, args.Members, args.Visitors);
                Log.InfoFormat("Created new site request with ID: {0}", id);
            }
            else
            {
                id = existingRequests[0].Id;
            }

            return id;
        }

        private int CreateNewRequest(string siteName, string siteTitle, string siteDescription, string[] owners, string[] members, string[] visitors)
        {
            // Get the term foeld for the Reasearch taxonomy
            var termfield = _siteRequestList.Fields.GetByInternalNameOrTitle("InstitutionalSiteClassification");
            _sharePoint.ClientContext.Load(termfield);
            _sharePoint.ClientContext.ExecuteQuery();

            const string term = "Research";

            string hiddentextFieldId = string.Empty;
            string termId = SharePointTaxonomyHelper.GetTermInfo(_sharePoint.Root, termfield, term, ref hiddentextFieldId);

            Field hiddenTextField = _siteRequestList.Fields.GetById(new Guid(hiddentextFieldId));
            _sharePoint.ClientContext.Load(hiddenTextField);
            _sharePoint.ClientContext.ExecuteQuery();

            var itemCreateInfo = new ListItemCreationInformation();
            var listItem = _siteRequestList.AddItem(itemCreateInfo);
            listItem["UrdmsSiteTitle"] = siteTitle;
            listItem["UrdmsSiteId"] = siteName;
            listItem["Body"] = siteDescription;
            listItem["UrdmsSiteProvisioningRequestStatus"] = "Approved";
            listItem["UrdmsClassification"] = String.Format("-1;#{0}|{1}", term, termId);
            listItem[hiddenTextField.InternalName] = String.Format("-1;#{0}|{1}", term, termId);
            // Set Site Users
            listItem["UrdmsSiteOwners"] = SetUserValues(owners);
            listItem["UrdmsSiteMembers"] = SetUserValues(members);
            listItem["UrdmsSiteVisitors"] = SetUserValues(visitors);

            listItem.Update();
            _sharePoint.ClientContext.ExecuteQuery();

            return listItem.Id;
        }

        private List<FieldUserValue> SetUserValues(IList<string> users)
        {
            var clientContext = new ClientContext(_sharePoint.Root);
            var rootWeb = clientContext.Web;

            var filteredUsers = new List<string>();

            foreach (var user in users)
            {
                try
                {
                    var ensureUser = rootWeb.EnsureUser(user);
                    clientContext.Load(ensureUser);
                    clientContext.ExecuteQuery();

                    filteredUsers.Add(user);
                }
                catch (Exception)
                {
                    // Do nothing, user does not exists. 
                }
            }

            clientContext.Dispose();

            if (filteredUsers.Count > 0)
            {
                return filteredUsers.Select(loginName => FieldUserValue.FromUser(loginName)).ToList();
            }

            return new List<FieldUserValue>();
        }

    }
}