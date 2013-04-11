using System.Collections.Generic;

namespace Urdms.Dmp.Models.DataCollectionModels
{
    public class DataCollectionListViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public IEnumerable<DataCollectionItemViewModel> DataCollectionItems { get; set; }
    }
}