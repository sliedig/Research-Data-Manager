using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Controllers.Validators;

namespace Urdms.Dmp.Models
{
    public class CreateProjectViewModel
    {
        public CreateProjectViewModel()
        {
            ResearchInitiatedProjects = new List<ResearchInitiatedProjectsViewModel>();
        }

        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Principal Investigator")]
        public string Principalinvestigator { get; set; }

        [ExistsIn("ResearchInitiatedProjects", ErrorMessage = "Please select a previously created project")]
        [Display(Name = "Research-Initiated project")]
        [Required(ErrorMessage = "Please select a previously created project")]
        public string ResearchInitiatedProject { get; set; }
        public IList<ResearchInitiatedProjectsViewModel> ResearchInitiatedProjects { get; set; }

        [BooleanRequiredToBeTrue(ErrorMessage = "You must confirm this field before proceeding")]
        public bool Overwrite { get; set; }
    }
}
