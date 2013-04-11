using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Models.Interfaces;

namespace Urdms.Dmp.Models.DataDeposit
{
    public class DataDepositViewModel : IUserManagementViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }

        [Display(Name = DataManagementViewModelTitles.NewDataDetail.ResearchDataDescription)]
        [DataType(DataType.MultilineText)]
        [NotRequired]
        public string ResearchDataDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataStorage.MaxDataSize)]
        [NotRequired]
        public MaxDataSize MaxDataSize { get; set; }

        public IList<UrdmsUserViewModel> UrdmsUsers { get; set; }
        public IList<NonUrdmsUserViewModel> NonUrdmsUsers { get; set; }

        public Party PrincipalInvestigator { get; set; }

        [Display(Name = DataManagementViewModelTitles.FindUserId)]
        [NotRequired]
        public string FindUserId { get; set; }

        [Display(Name = DataManagementViewModelTitles.NonUrdmsNewUserName)]
        [NotRequired]
        public string NonUrdmsNewUserName { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.DataSharingAvailability)]
        [NotRequired]
        public DataSharingAvailability Availability { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.DataSharingAvailabilityDate)]
        [NotRequired]
        [DataType(DataType.Date)]
        public DateTime? AvailabilityDate { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.ShareAccess)]
        [NotRequired]
        public ShareAccess ShareAccess { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.ShareAccessDescription)]
        [NotRequired]
        public string ShareAccessDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.DataLicensingType)]
        [NotRequired]
        public DataLicensingType DataLicensingType { get; set; }

        public DataDepositViewModel()
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
