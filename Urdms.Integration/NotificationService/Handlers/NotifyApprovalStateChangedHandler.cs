using System.Collections.Generic;
using System.Configuration;
using NServiceBus;
using Urdms.NotificationService.Database.Repositories;
using Urdms.NotificationService.Messages;
using Urdms.NotificationService.Models;
using Urdms.NotificationService.UserSearch;
using log4net;
using NServiceBus.Logging;

namespace Urdms.NotificationService.Handlers
{
    public class NotifyApprovalStateChangedHandler : IHandleMessages<NotifyApprovalStateChanged>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _qaApprovalGroupEmail;
        private readonly string _secondaryApprovalGroupEmail;
        private readonly IMailer _mailer;
        private readonly IApprovalStateChangedRepository _approvalStateChangedRepository;
        private readonly IUrdmsUserService _userService;

        public NotifyApprovalStateChangedHandler()
            : this(new Mailer(), new ApprovalStateChangedRepository(), new UrdmsUserService())
        {
        }

        public NotifyApprovalStateChangedHandler(IMailer mailer, IApprovalStateChangedRepository approvalStateChangedRepository, IUrdmsUserService userService)
        {
            _mailer = mailer;
            _approvalStateChangedRepository = approvalStateChangedRepository;
            _userService = userService;

            _qaApprovalGroupEmail = ConfigurationManager.AppSettings["QaApprovalGroupEmailAddress"];
			_secondaryApprovalGroupEmail = ConfigurationManager.AppSettings["SecondaryApprovalGroupEmailAddress"];

        }

        /// <summary>
        /// Handles the ApprovalStateChanged message.
        /// </summary>
        /// <param name="message">ApprovalStateChanged message.</param>
        public void Handle(NotifyApprovalStateChanged message)
        {
            var emailData = _approvalStateChangedRepository.Get(message.DataCollectionId);

            switch (message.ApprovalState)
            {
                case "Submitted":
                    var submittedStateEmail = new ApprovalStateChangedEmail
                                    {
                                        To = new List<string> { _qaApprovalGroupEmail },
                                        Subject = EmailTemplateSubject.ApprovalStateChangedSubmittedSubject,
                                        ProjectTitle = emailData.ProjectTitle,
                                        DataCollectionTitle = emailData.DataCollectionTitle,
                                        DataCollectionOwner = emailData.Manager,
                                        ApproverId = message.Approver,
                                        ApproverName = GetApproverName(message.Approver)
                                    };

                    Log.InfoFormat("[URDMS] Sending Approval State Changed email. {0}.", submittedStateEmail);
                    _mailer.SendEmail(submittedStateEmail, "ApprovalStateChangedSubmitted");
                    break;

                case "QaApproved":
                    var qaApprovedStateEmail = new ApprovalStateChangedEmail
                                    {
                                        To = new List<string> { _secondaryApprovalGroupEmail },
                                        Subject = EmailTemplateSubject.ApprovalStateChangedQaApprovedSubject,
                                        ProjectTitle = emailData.ProjectTitle,
                                        DataCollectionTitle = emailData.DataCollectionTitle,
                                        DataCollectionOwner = emailData.Manager,
                                        ApproverId = message.Approver,
                                        ApproverName = GetApproverName(message.Approver)
                                    };
                    Log.InfoFormat("[URDMS] Sending Approval State Changed email. {0}.", qaApprovedStateEmail);
                    _mailer.SendEmail(qaApprovedStateEmail, "ApprovalStateChangedQaApproved");
                    break;

				case "SecondaryApproved":
                    var secondaryApprovedStateEmail = new ApprovalStateChangedEmail
                                    {
                                        To = new List<string> { _qaApprovalGroupEmail },
                                        Subject = EmailTemplateSubject.ApprovalStateChangedSecondaryApprovedSubject,
                                        ProjectTitle = emailData.ProjectTitle,
                                        DataCollectionTitle = emailData.DataCollectionTitle,
                                        DataCollectionOwner = emailData.Manager,
                                        ApproverId = message.Approver,
                                        ApproverName = GetApproverName(message.Approver)
                                    };

                    Log.InfoFormat("[URDMS] Sending Approval State Changed email. {0}.", secondaryApprovedStateEmail);
                    _mailer.SendEmail(secondaryApprovedStateEmail, "ApprovalStateChangedSecondaryApproved");
                    break;

                case "RecordAmended":
                    var recordAmendedStateEmail = new ApprovalStateChangedEmail
                                    {
                                        To = new List<string> { _secondaryApprovalGroupEmail },
                                        Subject = EmailTemplateSubject.ApprovalStateChangedRecordAmendedSubject,
                                        ProjectTitle = emailData.ProjectTitle,
                                        DataCollectionTitle = emailData.DataCollectionTitle,
                                        DataCollectionOwner = emailData.Manager,
                                        ApproverId = message.Approver,
                                        ApproverName = GetApproverName(message.Approver)
                                    };

                    Log.InfoFormat("[URDMS] Sending Approval State Changed email. {0}.", recordAmendedStateEmail);
                    _mailer.SendEmail(recordAmendedStateEmail, "ApprovalStateChangedRecordAmended");
                    break;

                case "Published":
                    var managerEmailAddress = _userService.GetUser(emailData.ManagerId).EmailAddress;
                    var publishedStateEmail = new ApprovalStateChangedEmail
                                    {
                                        To = new List<string> { managerEmailAddress },
                                        Subject = EmailTemplateSubject.ApprovalStateChangedPublishedSubject,
                                        ProjectTitle = emailData.ProjectTitle,
                                        DataCollectionTitle = emailData.DataCollectionTitle
                                    };

                    Log.InfoFormat("[URDMS] Sending Approval State Changed email. {0}.", publishedStateEmail);
                    _mailer.SendEmail(publishedStateEmail, "ApprovalStateChangedPublished");
                    break;
            }
        }

        private string GetApproverName(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return string.Empty;

            return _userService.GetUser(userId).FullName;
        }

    }
}