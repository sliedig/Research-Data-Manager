using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models.DataCollectionModels
{
    public class DataCollectionItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RecordCreationDate { get; set; }
        public DataCollectionStatus Status { get; set; }
        public bool IsUserSubmitted { get; set; }
    }
}