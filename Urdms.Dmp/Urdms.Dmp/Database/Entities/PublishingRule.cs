using System;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities
{
    public class PublishingRule
    {
        public virtual int Id { get; set; }
        public virtual bool PublishToAnds { get; set; }
        public virtual bool AwareOfEthics { get; set; }
        public virtual DataSharingAvailability Availability { get; set; }
        public virtual DateTime? AvailabilityDate { get; set; }
    }
}