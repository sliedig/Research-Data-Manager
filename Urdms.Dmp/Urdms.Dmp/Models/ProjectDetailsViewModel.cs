using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Controllers.Validators;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Models.Interfaces;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Models
{
    public class ProjectDetailsViewModel : IResearchCodesViewModel
    {
        public ProjectDetailsViewModel()
        {
            ArcFunder = new ArcProjectFunderViewModel();
            NmhrcFunder = new NmhrcProjectFunderViewModel();
            FieldsOfResearch = new List<ClassificationBase>();
            SocioEconomicObjectives = new List<ClassificationBase>();
        }

        public int Id { get; set; }

        [NotRequired]
        public ArcProjectFunderViewModel ArcFunder { get; set; }
        [NotRequired]
        public NmhrcProjectFunderViewModel NmhrcFunder { get; set; }

        public IEnumerable<ProjectFunderViewModel> Funders
        {
            get
            {
                if (ArcFunder == null)
                {
                    ArcFunder = new ArcProjectFunderViewModel();
                }
                if (NmhrcFunder == null)
                {
                    NmhrcFunder = new NmhrcProjectFunderViewModel();
                }
                yield return ArcFunder;
                yield return NmhrcFunder;
            }

        }

        [NotRequired]
        [Display(Name = ProjectTitles.PrincipalInvestigator)]
        public Party PrincipalInvestigator { get; set; }

        [NotRequired]
        [Display(Name = ProjectTitles.SourceId)]
        public string SourceId { get; set; }

        [Required]
        [Display(Name = ProjectTitles.Title)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = ProjectTitles.Description)]
        public string Description { get; set; }

        [NotRequired]
        [Display(Name = ProjectTitles.Status)]
        public ProjectStatus Status { get; set; }

        [NotRequired]
        public SourceProjectType SourceProjectType { get; set; }
        
        [Required]
        [DateTimeValidation]
        [Display(Name = ProjectTitles.Edit.StartDate)]
        public string StartDate { get; set; }

        [Required]
        [DateTimeValidation]
        [Display(Name = ProjectTitles.Edit.EndDate)]
        [GreaterThanDate("StartDate")]
        public string EndDate { get; set; }

        [NotRequired]
        [Display(Name = ProjectTitles.Keywords)]
        public string Keywords { get; set; }

        [NotRequired]
        [Display(Name = ProjectTitles.FieldsOfResearch)]
        public IList<ClassificationBase> FieldsOfResearch { get; set; }

        [NotRequired]
        [Display(Name = ProjectTitles.FieldOfResearchCode)]
        public string FieldOfResearchCode { get; set; }

        [NotRequired]
        [Display(Name = ProjectTitles.SocioEconomicObjectives)]
        public IList<ClassificationBase> SocioEconomicObjectives { get; set; }

        [NotRequired]
        [Display(Name = ProjectTitles.SocioEconomicObjectiveCode)]
        public string SocioEconomicObjectiveCode { get; set; }
    }
}
