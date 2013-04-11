using System;
using System.Configuration;
using System.Linq;
using NServiceBus;
using Urdms.ProvisioningService.Messages;
using Urdms.ProvisioningService.SharePoint.Helpers;

namespace Urdms.ProvisioningService.SharePoint
{
    public class CreateSiteRequestHandler : IHandleMessages<CreateSiteRequest>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SharePointHelper _sharePoint;
        private readonly string _spSiteNamePrefix = ConfigurationManager.AppSettings["SiteNamePrefix"];

        public IBus Bus { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSiteRequestHandler"/> class.
        /// </summary>
        public CreateSiteRequestHandler()
        {
            _sharePoint = new SharePointHelper();
        }

        #region IMessageHandler<T> Members

        public void Handle(CreateSiteRequest message)
        {
            if (true /*_sharePoint.TryLoadRootWeb(o => o)*/)
            {
				//var siteName = _spSiteNamePrefix + message.ProjectId;
				//var siteCreationRequestHelper = new SiteCreationRequestHelper();


				//var args = new SiteCreationRequestArgs
				//            {
				//                SiteId = siteName,
				//                Title = message.SiteTitle,
				//                Description = message.SiteDescription,
				//                Owners = message.UsersInRoles.Where(u => u.Role == "Owners").Select(u => u.UserId).ToArray(),
				//                Members = message.UsersInRoles.Where(u => u.Role == "Members").Select(u => u.UserId).ToArray(),
				//                Visitors = message.UsersInRoles.Where(u => u.Role == "Visitors").Select(u => u.UserId).ToArray()
				//            };


				//const string listName = "URDMS - Site Creation Request";

				//// Generate the request 
				//var requestId = siteCreationRequestHelper.CreateSiteRequest(args, listName);

				//// Poll the list item for changes in status
				//var poller = new SiteCreationRequestListPoller(_sharePoint, requestId, listName);
				//var result = poller.Poll();

                var response = new CreateSiteResponse
                                   {
                                       ProjectId = message.ProjectId,
									   //RequestId = requestId,
									   //ProvisioningRequestStatus = result.ProvisioningRequestStatus
								   };

				// Create a dummy response for now.
            	response.RequestId = 2342;
            	response.ProvisioningRequestStatus = ProvisioningRequestStatus.Provisioned;
            	response.SiteUrl = "http://sharepoint.yourdomain.edu.au/project/research/2";

                // Handle the Polling result.
				//switch (result.ProvisioningRequestStatus)
				//{
				//    case ProvisioningRequestStatus.Provisioned:
				//        Log.InfoFormat("[URDMS] Site for Project {0} created ({1})", message.ProjectId, result.SiteUrl);
				//        response.SiteUrl = result.SiteUrl;
				//        break;
				//    case ProvisioningRequestStatus.Error:
				//        Log.ErrorFormat("[URDMS] Site provisioning for Project {0} failed.", message.ProjectId);
				//        break;
				//    case ProvisioningRequestStatus.TimeOut:
				//    case ProvisioningRequestStatus.Pending:
				//        Log.ErrorFormat("[URDMS] Site provisioning for Project {0} has timed out or is still pending.", message.ProjectId);
				//        break;
				//    default:
				//        throw new ArgumentOutOfRangeException();
				//}

                Bus.Reply(response);

            }

        }

        #endregion

    }
}
