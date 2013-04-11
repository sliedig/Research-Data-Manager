using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Urdms.Dmp.Models
{
    public class CopyDataManagementPlanProjectViewModel
    {
        public int DestinationProjectId { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Do you wish to auto-populate the Data Management Plan with responses from exsting Data Management Plan?")]
        public bool? CopyFromExistingDmp { get; set; }
        public IList<ProjectListViewModel> AvailableProjects {get;set;}
    }
}