namespace Urdms.NotificationService.Models
{
    public class SiteProvisionedEmail : BaseEmail
    {
        public string SiteUrl { get; set; }
        public string ProjectName { get; set; }
    }
}