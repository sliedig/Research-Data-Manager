using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;

namespace Urdms.Dmp.Models.Interfaces
{
    public interface IUserManagementViewModel
    {
        IList<UrdmsUserViewModel> UrdmsUsers { get; set; }
        IList<NonUrdmsUserViewModel> NonUrdmsUsers { get; set; }

		[Display(Name = "Institutional members (enter Institutional User ID)")]
        [NotRequired]
        string FindUserId { get; set; }

		[Display(Name = "Non-Institutional members (enter member's name)")]
        [NotRequired]
        string NonUrdmsNewUserName { get; set; }

        void AddUrdmsUser<T>(ICurtinUser user, T role);
        void AddNonUrdmsUser<T>(string fullName, T role);
    }

}
