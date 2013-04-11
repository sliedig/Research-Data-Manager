namespace Urdms.NotificationService.Models
{
    class EmailTemplateSubject
    {
        public const string SiteProvisionedSubject = "Project Storage Space Provisioned Notification";
        public const string RequestForSiteReceivedSubject = "Project Storage Space Request Confirmation";
        public const string ApprovalStateChangedSecondaryApprovedSubject = "Dataset/collection pending final public release";
        public const string ApprovalStateChangedQaApprovedSubject = "New dataset/collection requires secondary Approval";
        public const string ApprovalStateChangedRecordAmendedSubject = "Dataset/collection requires secondary Re-aproval";
        public const string ApprovalStateChangedSubmittedSubject = "New dataset/collection requires quality check";
        public const string ApprovalStateChangedPublishedSubject = "Your dataset/collection has been published";
    }
}
