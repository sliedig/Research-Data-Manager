using System;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models.ApprovalModels
{
    public class DataCollectionApprovalItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string SubmittedBy { get; set; }
        public DataCollectionStatus Status { get; set; }
    }
}