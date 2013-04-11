using System.ComponentModel.DataAnnotations;

namespace Urdms.Dmp.Models
{
    public class ProjectConfirmationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Does the Research involve importation of experimental organisms?")]
        public bool ResearchInvolvesExperimentalOrganisms { get; set; }

        [Display(Name = "Does the Research involve human participants?")]
        public bool ResearchInvolvesHumanParticipants { get; set; }

        [Display(Name = "Does the Research involve animals?")]
        public bool ResearchInvolvesAnimals { get; set; }

        [Display(Name = "Does the Research involve social science data sets?")]
        public bool ResearchInvolvesSocialScienceDatasets { get; set; }

        [Display(Name = "Does the Research involve genetic manipulation?")]
        public bool ResearchInvolvesGeneticManipulation { get; set; }

        [Display(Name = "Does the Research involve ionising radiation? ")]
        public bool ResearchInvolvesIonisingRadiation { get; set; }

        [Display(Name = "Does the Research involve deposition of biological materials?")]
        public bool ResearchInvolvesDepositionOfBiologicalMaterials { get; set; }

        [Display(Name = "Has the project received Occupational Health & Safety approval?")]
        public bool HasReceivedOhsApproval { get; set; }
    }
}