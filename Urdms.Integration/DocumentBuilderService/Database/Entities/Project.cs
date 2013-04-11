using Urdms.DocumentBuilderService.Models.Enums;

namespace Urdms.DocumentBuilderService.Database.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public SourceProjectType SourceProjectType { get; set; }
    }
}
