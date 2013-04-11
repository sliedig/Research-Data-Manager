using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.DataDeposit;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Models.Interfaces;

namespace Urdms.Dmp.Models
{
    public class ProjectViewModel : IResearchCodesViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project Title")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string StartDate { get; set; }

        [NotRequired]
        public Party PrincipalInvestigator { get; set; }

        public string EndDate { get; set; }

        [Display(Name = "Project Source")]
        public SourceProjectType SourceProjectType { get; set; }

        [DataType(DataType.Text)]
        public int ScriptId { get; set; }

        [Display(Name = "External Id")]
        public string SourceId { get; set; }

        public string FieldOfResearchCode { get; set; }
        
        [Display(Name = "Socio Economic Research (SEO)")]
        [NotRequired]
        public IList<ClassificationBase> SocioEconomicObjectives { get; set; }

        public string SocioEconomicObjectiveCode  { get; set; }

        [Display(Name = "Field of Research (FoR)")]
        [NotRequired]
        public IList<ClassificationBase> FieldsOfResearch { get; set; }

        [DataType(DataType.MultilineText)]
        public string Keywords { get; set; }

        public ProjectStatus Status { get; set; }

        [NotRequired]
        public DataManagementPlanViewModel DataManagementPlan { get; set; }

        [NotRequired]
        public DataDepositViewModel DataDeposit { get; set; }
        
        public ProjectViewModel()
        {
            SocioEconomicObjectives = new List<ClassificationBase>();
            FieldsOfResearch = new List<ClassificationBase>();
        }

        public static ProjectViewModel NewFullViewModel()
        {
            return new ProjectViewModel
            {
                DataManagementPlan = new DataManagementPlanViewModel
                {
                    BackupPolicy = new BackupPolicyViewModel(),
                    Confidentiality = new ConfidentialityViewModel(),
                    UrdmsUsers = new List<UrdmsUserViewModel>(),
                    DataDocumentation = new DataDocumentationViewModel(),
                    DataRelationshipDetail = new DataRelationshipDetailViewModel(),
                    DataRetention = new DataRetentionViewModel(),
                    DataSharing = new DataSharingViewModel(),
                    DataStorage = new DataStorageViewModel(),
                    DataOrganisation = new DataOrganisationViewModel(),
                    Ethic = new EthicViewModel(),
                    ExistingDataDetail = new ExistingDataDetailViewModel(),
                    NewDataDetail = new NewDataDetailViewModel(),
                    NonUrdmsUsers = new List<NonUrdmsUserViewModel>(),
                }

            };
        }
    }
}
