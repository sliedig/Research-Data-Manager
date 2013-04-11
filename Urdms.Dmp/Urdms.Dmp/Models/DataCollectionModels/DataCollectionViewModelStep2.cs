using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Models.Interfaces;

namespace Urdms.Dmp.Models.DataCollectionModels
{
    // Note: Updates to this class may affect the read-only view DataCollectionReadOnlyViewModel also
    public class DataCollectionViewModelStep2 : IUserManagementViewModel, IResearchCodesViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        [NotRequired]
        public string Keywords { get; set; }

        [NotRequired]
        public IList<ClassificationBase> FieldsOfResearch { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.FieldOfResearchCode)]
        public string FieldOfResearchCode { get; set; }

        [NotRequired]
        public IList<ClassificationBase> SocioEconomicObjectives { get; set; }

        [NotRequired]
        public virtual bool IsForApproval { get { return false; } }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.SocioEconomicObjectiveCode)]
        public string SocioEconomicObjectiveCode { get; set; }

        public IList<UrdmsUserViewModel> UrdmsUsers { get; set; }
        public IList<NonUrdmsUserViewModel> NonUrdmsUsers { get; set; }

        [Display(Name = DataCollectionViewModelTitles.Manager)]
        public Party Manager { get; set; }

        [Display(Name = DataCollectionViewModelTitles.FindUserId)]
        [NotRequired]
        public string FindUserId { get; set; }


        [Display(Name = DataCollectionViewModelTitles.NonUrdmsNewUserName)]
        [NotRequired]
        public string NonUrdmsNewUserName { get; set; }


        

        public DataCollectionViewModelStep2()
        {
            UrdmsUsers = new List<UrdmsUserViewModel>();
            NonUrdmsUsers = new List<NonUrdmsUserViewModel>();
            FieldsOfResearch = new List<ClassificationBase>();
            SocioEconomicObjectives = new List<ClassificationBase>();
        }

        public static int GetAllowableRole(DataCollectionRelationshipType role)
        {
            if (!Enum.IsDefined(typeof(DataCollectionRelationshipType), role))
            {
                return (int)DataCollectionRelationshipType.AssociatedResearcher;
            }
            return (int)role;
        }

        public void AddUrdmsUser<TCollectionRelationship>(ICurtinUser user, TCollectionRelationship role)
        {
            if (user.CurtinId != Manager.UserId)
            {
                UrdmsUsers.AddUrdmsUser(user, role);
            }
        }

        public void AddNonUrdmsUser<TCollectionRelationship>(string fullName, TCollectionRelationship role)
        {
            NonUrdmsUsers.AddNonUrdmsUser(fullName, role);
        }
    }
}
