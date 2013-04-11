using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Models.Interfaces
{
    public interface IResearchCodesViewModel
    {
        [NotRequired]
        IList<ClassificationBase> FieldsOfResearch { get; set; }

        [Display(Name = "Field of Research (FoR) code")]
        string FieldOfResearchCode { get; set; }

        [NotRequired]
        IList<ClassificationBase> SocioEconomicObjectives { get; set; }

        [Display(Name = "Socio Economic Objective (SEO) code")]
        string SocioEconomicObjectiveCode { get; set; }
    }
}
