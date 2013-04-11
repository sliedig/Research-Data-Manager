using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Models.Interfaces;

namespace Urdms.Dmp.Models
{
    public class DataManagementPlanViewModel : IUserManagementViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        [DataType(DataType.MultilineText)]
        public string ProjectTitle { get; set; }
        [DataType(DataType.MultilineText)]
        public string ProjectDescription { get; set; }

        public DataStorageViewModel DataStorage { get; set; }
        public NewDataDetailViewModel NewDataDetail { get; set; }
        public ExistingDataDetailViewModel ExistingDataDetail { get; set; }
        public DataDocumentationViewModel DataDocumentation { get; set; }
        public EthicViewModel Ethic { get; set; }
        public ConfidentialityViewModel Confidentiality { get; set; }
        public BackupPolicyViewModel BackupPolicy { get; set; }
        public DataRetentionViewModel DataRetention { get; set; }
        public DataSharingViewModel DataSharing { get; set; }
        public DataRelationshipDetailViewModel DataRelationshipDetail { get; set; }
        public DataOrganisationViewModel DataOrganisation { get; set; }

        public IList<UrdmsUserViewModel> UrdmsUsers { get; set; }
        public IList<NonUrdmsUserViewModel> NonUrdmsUsers { get; set; }

        [Display(Name = DataManagementViewModelTitles.PrincipalInvestigator)]
        public Party PrincipalInvestigator { get; set; }
        public ProvisioningStatus Status { get; set; }

        public int Step { get; set; }
        public DateTime Start { get; set; }
        [Display(Name = DataManagementViewModelTitles.FindUserId)]
        [NotRequired]
        public string FindUserId { get; set; }
        [Display(Name = DataManagementViewModelTitles.NonUrdmsNewUserName)]
        [NotRequired]
        public string NonUrdmsNewUserName { get; set; }

        public DataManagementPlanViewModel()
        {
            UrdmsUsers = new List<UrdmsUserViewModel>();
            NonUrdmsUsers = new List<NonUrdmsUserViewModel>();
        }

        public static int GetAllowableRole(AccessRole role)
        {
            if (!Enum.IsDefined(typeof(AccessRole), role))
            {
                return (int)AccessRole.Visitors;
            }
            return (int)role;
        }

        public void AddUrdmsUser<T>(ICurtinUser user, T role)
        {
            if (PrincipalInvestigator == null || user.CurtinId != PrincipalInvestigator.UserId)
            {
                UrdmsUsers.AddUrdmsUser(user, role);
            }
        }

        public void AddNonUrdmsUser<T>(string fullName, T role)
        {
            NonUrdmsUsers.AddNonUrdmsUser(fullName, role);
        }
    }
}
