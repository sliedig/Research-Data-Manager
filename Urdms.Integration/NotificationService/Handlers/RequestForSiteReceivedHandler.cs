using System.Linq;
using NServiceBus;
using Urdms.NotificationService.Messages;
using Urdms.NotificationService.Models;
using Urdms.NotificationService.UserSearch;
using log4net;

namespace Urdms.NotificationService.Handlers
{
    public class NotifyRequestForSiteReceivedHandler : IHandleMessages<NotifyRequestForSiteReceived>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IMailer _mailer;
        private readonly IUrdmsUserService _userService;

        public NotifyRequestForSiteReceivedHandler() : this (new Mailer(), new UrdmsUserService())
        {
        }

        public NotifyRequestForSiteReceivedHandler(IMailer mailer, IUrdmsUserService userService)
        {
            _mailer = mailer;
            _userService = userService;
        }

        public void Handle(NotifyRequestForSiteReceived message)
        {
            var userEmails = message.UserIds.Select(u => _userService.GetUser(u).EmailAddress).ToList();
            
            if (userEmails.Count == 0)
            {
                return;
            }
            
            var siteProvisionEmail = new RequestForSiteReceivedEmail
            {
                To = userEmails, 
                ProjectId = message.ProjectId.ToString(),
                ProjectName = message.ProjectName,
                Subject = EmailTemplateSubject.RequestForSiteReceivedSubject
            };

            Log.InfoFormat("[URDMS] Sending Request For Site Received email. {0}.", siteProvisionEmail);
            _mailer.SendEmail(siteProvisionEmail, "RequestForSiteReceived");
        }
    }
}
