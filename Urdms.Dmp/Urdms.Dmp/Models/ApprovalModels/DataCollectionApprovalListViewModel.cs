using System.Collections.Generic;

namespace Urdms.Dmp.Models.ApprovalModels
{
    public class DataCollectionApprovalListViewModel
    {
        public IEnumerable<DataCollectionApprovalItemViewModel> DataCollectionItems { get; set; }
    }
}