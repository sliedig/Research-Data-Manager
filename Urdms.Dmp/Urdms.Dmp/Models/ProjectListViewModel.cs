using System.Web.Mvc;
using System.Web.Mvc.Html;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Models
{
    public class ProjectListViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public SourceProjectType ProjectType { get; set; }
        public string ProjectStatus { get; set; }
        public int DmpId { get; set; }
        public int DataDepositId { get; set; }
        public string DmpCreationDate { get; set; }
        public ProvisioningStatus ProvisioningStatus { get; set; }
        public string SiteUrl { get; set; }
        public bool HasFirstCollection { get; set; }
 
        public MvcHtmlString TitleUrl(HtmlHelper helper)
        {
            switch (ProjectType)
            {
                case SourceProjectType.DEPOSIT:
                    return helper.ActionLink(ProjectTitle, "Project", "DataDeposit", new {id = ProjectId}, new {});
                default:
                    return helper.ActionLink(ProjectTitle, "Project", "Project", new {id = ProjectId}, new {});
            }

        }

        public MvcHtmlString ProvisioningStatusHtml(HtmlHelper helper)
        {
            var text = ProjectType == SourceProjectType.DEPOSIT ? "Awaiting Data Deposit Submission" : "Awaiting DMP Submission";
            if (this.ProvisioningStatus == ProvisioningStatus.Provisioned)
            {
                var html = string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>", SiteUrl, "Provisioned");
                return MvcHtmlString.Create(html);
            }
            return MvcHtmlString.Create(text);
        }
    }
}