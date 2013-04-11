using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using Microsoft.SharePoint.Client;
using NUnit.Framework;

namespace SharePointClientApiTest
{
    [TestFixture]
    public class SharePointClientObjectModelShould
    {
        private readonly string _adDomainName = ConfigurationManager.AppSettings["SpDomain"];
        private readonly string _sysUserName = ConfigurationManager.AppSettings["SpServiceAccount"];
        private readonly string _sysPassword = ConfigurationManager.AppSettings["SpServiceAccountPassword"];
        private readonly string _spManagedPath = ConfigurationManager.AppSettings["SpManagedPath"];
        private static readonly string _root = ConfigurationManager.AppSettings["SpRoot"];
        private ClientContext _clientContext;
        private Web _rootWeb;
        private NetworkCredential _credentials;
        private string _rootWebUrl;

        private string _projectTitle;
        private string _siteName;


        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _credentials = new NetworkCredential
                               {UserName = _sysUserName, Password = _sysPassword, Domain = _adDomainName};
            _rootWebUrl = string.Format("{0}/{1}", _root, _spManagedPath);
        }


        [SetUp]
        public void TestSetup()
        {
            _projectTitle = string.Format("Project {0}", new Random().Next(100));
            _siteName = _projectTitle.Replace(" ", "");


            _clientContext = new ClientContext(_rootWebUrl) {Credentials =  _credentials};
            _rootWeb = _clientContext.Web;
        }

        [TearDown]
        public void TestTearDown()
        {
            _clientContext.Dispose();
            _rootWeb = null;
        }

		[Test]
		[Ignore("Should be run manually.")]
		public void GetListItem()
        {
            var clientContext = new ClientContext("https://sharepoint.yourdomain.edu.au/research") {Credentials =  _credentials};
            var rootWeb = clientContext.Web;

            var siteRequestList = rootWeb.Lists.GetByTitle("URDMS - Site Creation Request");

            var listItem = siteRequestList.GetItemById(1);
            clientContext.Load(listItem);
            clientContext.ExecuteQuery();
        }

        [Test]
        [Ignore("Should be run manually.")]
        public void Create_new_list_item_in_site_creation_request_list_with_status_set_to_in_progress()
        {
            var siteRequestList = _rootWeb.Lists.GetByTitle("URDMS - Site Creation Request");

            var itemCreateInfo = new ListItemCreationInformation();
            var listItem = siteRequestList.AddItem(itemCreateInfo);

            listItem["Title"] = _siteName;
            listItem["UrdmsSiteTitle"] = _projectTitle;
            listItem["UrdmsClassification"] = null;
            listItem["Body"] = "This is a description of the site.";
            listItem["UrdmsSiteProvisioningRequestStatus"] = "In Progress";
            listItem.Update();
            _clientContext.ExecuteQuery();

            Console.WriteLine("Created list item with ID: {0}", listItem.Id);
        }


        [Test]
        [Ignore("Should be run manually.")]
        public void Create_new_list_item_in_site_creation_request_list_with_members_visitors_and_owners()
        {
            var siteRequestList = _rootWeb.Lists.GetByTitle("URDMS - Site Creation Request");

            var itemCreateInfo = new ListItemCreationInformation();
            var listItem = siteRequestList.AddItem(itemCreateInfo);

            listItem["Title"] = _siteName;
            listItem["UrdmsSiteTitle"] = _projectTitle;
            listItem["Body"] = "This is a description of the site.";
            listItem["UrdmsSiteProvisioningRequestStatus"] = "In Progress";

            string[] urdmsOwners = {""};
            string[] urdmsMembers = {""};
            string[] urdmsVisitors = {"", ""};

            listItem["UrdmsSiteOwners"] = SetUserValues(urdmsOwners);
            listItem["UrdmsSiteMembers"] = SetUserValues(urdmsMembers);
            listItem["UrdmsSiteVisitors"] = SetUserValues(urdmsVisitors);

            listItem.Update();
            _clientContext.ExecuteQuery();

            Console.WriteLine("Created list item with ID: {0}", listItem.Id);
        }


        [Test]
        [Ignore("Should be run manually.")]
        public void Create_new_list_item_in_site_creation_request_list_with_taxonomy_field_set_to_research()
        {
            var siteRequestList = _rootWeb.Lists.GetByTitle("URDMS - Site Creation Request");
            
            var termfield = siteRequestList.Fields.GetByInternalNameOrTitle("UrdmsClassification");
            _clientContext.Load(termfield);
            _clientContext.ExecuteQuery();

            const string term = "Research";

            string hiddentextFieldId = string.Empty;
            string termId = SharePointTaxonomyHelper.GetTermInfo(_rootWebUrl, termfield, term, ref hiddentextFieldId);

            Field hiddenTextField = siteRequestList.Fields.GetById(new Guid(hiddentextFieldId));
            _clientContext.Load(hiddenTextField);
            _clientContext.ExecuteQuery();

            var itemCreateInfo = new ListItemCreationInformation();
            var listItem = siteRequestList.AddItem(itemCreateInfo);

            listItem["Title"] = _siteName;
            listItem["UrdmsSiteTitle"] = _projectTitle;
            listItem["Body"] = "This is a description of the site.";
            listItem["UrdmsSiteProvisioningRequestStatus"] = "In Progress";
            listItem["UrdmsClassification"] = String.Format("-1;#{0}|{1}", term, termId);
            listItem[hiddenTextField.InternalName] = String.Format("-1;#{0}|{1}", term, termId);

            listItem.Update();
            _clientContext.ExecuteQuery();

            Console.WriteLine("Created list item with ID: {0}", listItem.Id);
            Console.WriteLine("Urdms Classification: {0}", listItem["UrdmsClassification"]);

        }




        private List<FieldUserValue> SetUserValues(IList<string> users)
        {
            var clientContext = new ClientContext(_root);

            var filteredUsers = new List<string>();

            foreach (var user in users)
            {
                try
                {
                    var ensureUser = _rootWeb.EnsureUser(user);
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