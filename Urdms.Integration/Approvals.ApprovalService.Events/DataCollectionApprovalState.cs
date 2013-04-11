using System;

namespace Urdms.Approvals.ApprovalService.Events
{
    [Serializable]
    public enum DataCollectionApprovalState
    {

        /// <summary>
        /// Initial state of the Data Collection bvefore the Managers submit for approval.
        /// </summary>
        Draft,
        
        /// <summary>
        /// State of the Data Collection once Manager has submitted for approval. The initial approver then needs to action this data collection.
        /// </summary>
        Submitted,
        
        /// <summary>
        /// State of the Data Collection once initial approver has verified the quality integrity of the Data Collection. 
        /// </summary>
        QaApproved,
        
        /// <summary>
        /// State of the Data Collection once secondary approver has confirmed contract, ethics and confidentiality checks for the Data Collection.
        /// At this stage the Data Collection is ready for final approval.
        /// </summary>
		SecondaryApproved,

        /// <summary>
        /// State of the Data Collection if the initial approver modifies the Data Collection. 
        /// If modifications are made, the secondary approver needs to reconfirm the integrity of the Data Collection.
        /// </summary>
        RecordAmended,
        
        /// <summary>
        /// State of the Data Collection once the Library has completed its final checks but before the record has been written to Vivo.
        /// </summary>
        Publishing,
        
        /// <summary>
        /// State of the Data Collection once the Library has completed its final checks and submission to Vivo has been successful.
        /// </summary>
        Published
    }
}