using Urdms.Dmp.Models.DataCollectionModels;

namespace Urdms.Dmp.Models.ApprovalModels
{
    public class DataCollectionApprovalViewModelStep2 : DataCollectionViewModelStep2
    {
        public override bool IsForApproval { get { return true; } }
    }
}