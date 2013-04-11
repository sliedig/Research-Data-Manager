using System.Collections.Generic;

namespace Urdms.Dmp.Models
{
    public class SubmittedDataCollectionsViewModel
    {
        public int ProjectId { get; set; }
        public IEnumerable<DataCollectionsForApprovalItemViewModel> PublishedDataCollectionItems { get; set; }
    }
}