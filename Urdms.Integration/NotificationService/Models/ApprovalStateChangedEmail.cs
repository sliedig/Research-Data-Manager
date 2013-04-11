namespace Urdms.NotificationService.Models
{
    public class ApprovalStateChangedEmail : BaseEmail
    {
        public string DataCollectionTitle { get; set; }
        public string ProjectTitle { get; set; }
        public string DataCollectionOwner { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
    }
}