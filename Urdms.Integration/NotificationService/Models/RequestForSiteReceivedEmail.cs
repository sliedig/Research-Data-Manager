namespace Urdms.NotificationService.Models
{
    public class RequestForSiteReceivedEmail : BaseEmail
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
