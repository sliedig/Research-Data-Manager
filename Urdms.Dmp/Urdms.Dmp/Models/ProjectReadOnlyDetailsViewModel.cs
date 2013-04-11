using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class ProjectReadOnlyDetailsViewModel
    {
        public ProjectReadOnlyDetailsViewModel()
        {
            this.FieldsOfResearch = new List<ProjectFieldOfResearch>();
            this.SocioEconomicObjectives = new List<ProjectSocioEconomicObjective>();
        }

        public int Id { get; set; }
        
        [Display(Name = ProjectTitles.Title)]
        public string Title { get; set; }

        [Display(Name = ProjectTitles.Description)]
        public string Description { get; set; }
        
        [Display(Name = ProjectTitles.PrincipalInvestigator)]
        public Party PrincipalInvestigator { get; set; }
        
        [Display(Name = ProjectTitles.Status)]
        public ProjectStatus Status { get; set; }
        
        [Display(Name = ProjectTitles.FieldsOfResearch)]
        public IList<ProjectFieldOfResearch> FieldsOfResearch { get; private set; }
        
        [Display(Name = ProjectTitles.SocioEconomicObjectives)]
        public IList<ProjectSocioEconomicObjective> SocioEconomicObjectives { get; private set; }
        
        [Display(Name = ProjectTitles.View.StartDate)]
        public DateTime? StartDate { get; set; }
        
        [Display(Name = ProjectTitles.View.EndDate)]
        public DateTime? EndDate { get; set; }

        [Display(Name = ProjectTitles.Keywords)]
        public string Keywords { get; set; }
    }
}