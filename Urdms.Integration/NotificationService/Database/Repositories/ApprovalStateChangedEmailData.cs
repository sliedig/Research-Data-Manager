namespace Urdms.NotificationService.Database.Repositories
{
    public struct ApprovalStateChangedEmailData
    {
        public string DataCollectionTitle { get; set; }
        public string ProjectTitle { get; set; }
        public string Manager { get; set; }
        public string ManagerId { get; set; }
    }
}