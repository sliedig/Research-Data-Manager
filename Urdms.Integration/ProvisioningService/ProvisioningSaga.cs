using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Saga;
using Urdms.DocumentBuilderService.Commands;
using Urdms.NotificationService.Messages;
using Urdms.ProvisioningService.Commands;
using Urdms.ProvisioningService.Events;
using Urdms.ProvisioningService.Messages;

namespace Urdms.ProvisioningService
{
    public class ProvisioningSaga : Saga<ProvisioningSagaData>,
        IAmStartedByMessages<SiteRequestCommand>,
        IHandleMessages<CreateSiteResponse>,
        IHandleMessages<ForceProvisioningCompletionCommand>
    {

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Configuartion for finding the saga instances
        /// </summary>
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<CreateSiteResponse>(approval => approval.ProjectId).ToSaga(data => data.ProjectId);
            ConfigureMapping<ForceProvisioningCompletionCommand>(approval => approval.ProjectId).ToSaga(data => data.ProjectId);
        }

        #region Implementation of IMessageHandler<SiteRequestCommand>

        /// <summary>
        /// This is the message that initiates the saga.
        /// </summary>
        /// <param name="message">SiteRequestCommand message.</param>
        public void Handle(SiteRequestCommand message)
        {
            Data.ProjectId = message.ProjectId;
            Data.RequestId = -1;
            Data.SiteUrl = null;
            Data.ProvisioningRequestStatus = ProvisioningRequestStatus.Pending.ToString();

            Bus.Publish<ProvisioningStatusChanged>(m =>
                {
                    m.ProjectId = Data.ProjectId;
                    m.RequestId = Data.RequestId;
                    m.ProvisioningRequestStatusId = (int)ProvisioningRequestStatus.Pending;
                    m.SiteUrl = Data.SiteUrl;
                });

            Log.InfoFormat("[URDMS] Sending request to SharePoint integration endpoint to create site for Project {0}", Data.ProjectId);
            Bus.Send<CreateSiteRequest>(m =>
                {
                    m.ProjectId = Data.ProjectId;
                    m.SiteTitle = message.ProjectTitle;
                    m.SiteDescription = message.ProjectDescription;
                    m.UsersInRoles = CreateSiteUserList(message.UserRoles);
                });

            // Send notifocatioon to Principal Investigator that their request has been received.
            Bus.Send<NotifyRequestForSiteReceived>(m =>
                {
                    m.ProjectId = Data.ProjectId;
                    m.ProjectName = message.ProjectTitle;
                    m.UserIds = new List<string> { message.UserRoles["Owners"] };
                });
        }

        private static List<SiteUser> CreateSiteUserList(Dictionary<string, string> userRoles)
        {
            var siteUsers = new List<SiteUser>();

            foreach (var userRole in userRoles)
            {
                var ids = userRole.Value.Split(',');
                siteUsers.AddRange(ids.Select(id => new SiteUser() { Role = userRole.Key, UserId = id.Trim() }));
            }

            return siteUsers;
        }

        #endregion

        #region Implementation of IMessageHandler<CreateSiteResponse>

        /// <summary>
        /// Handles the response from the SharePoint integration endpoint that is responsible for
        /// the provisioning.
        /// </summary>
        /// <param name="message">CreateSiteResponse message.</param>
        public void Handle(CreateSiteResponse message)
        {
            Data.RequestId = message.RequestId;
            Data.SiteUrl = message.SiteUrl;
            Data.ProvisioningRequestStatus = message.ProvisioningRequestStatus.ToString();

            Bus.Publish<ProvisioningStatusChanged>(m =>
                {
                    m.ProjectId = Data.ProjectId;
                    m.SiteUrl = Data.SiteUrl;
                    m.RequestId = Data.RequestId;
                    m.ProvisioningRequestStatusId = (int)message.ProvisioningRequestStatus;
                });

            if (message.ProvisioningRequestStatus == ProvisioningRequestStatus.Provisioned)
            {
                GenerateDmp(Data.ProjectId, Data.SiteUrl);
                Complete();
            }
        }

        #endregion

        #region Implementation of IMessageHandler<ForceProvisioningCompletionCommand>

        /// <summary>
        /// Provides an override for provisioning and a way to complete the saga instance in case 
        /// the provisioning fails or times out. Works on the assumption that the resource that is 
        /// passed to it has been created manually and exists.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ForceProvisioningCompletionCommand message)
        {
            Data.SiteUrl = message.SiteUrl;
            Data.ProvisioningRequestStatus = ProvisioningRequestStatus.Provisioned.ToString();

            Bus.Publish<ProvisioningStatusChanged>(m =>
                {
                    m.ProjectId = Data.ProjectId;
                    m.SiteUrl = Data.SiteUrl;
                    m.RequestId = Data.RequestId;
                    m.ProvisioningRequestStatusId = (int)ProvisioningRequestStatus.Provisioned;
                });

            GenerateDmp(Data.ProjectId, Data.SiteUrl);
            Complete();
        }

        #endregion

        /// <summary>
        /// Sends the cpommand to the document builder endpoint to generate the Data Management Plan (DMP).
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="siteUrl">The site URL to which to send the document.</param>
        private void GenerateDmp(int projectId, string siteUrl)
        {
            Bus.Send<GenerateDmpCommand>(m =>
            {
                m.ProjectId = projectId;
                m.SiteUrl = siteUrl;
            });
        }

        /// <summary>
        /// Marks the Saga as complete
        /// </summary>
        private void Complete()
        {
            Log.InfoFormat("[URDMS] Provisioning for Project {0} ({1}) has completed.", Data.ProjectId, Data.SiteUrl);
            MarkAsComplete();
        }


    }
}
