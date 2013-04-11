using System;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities
{
    public class DataDeposit
    {
        private readonly DateTime _creationDate;

        public virtual int Id { get; set; }
        public virtual string ResearchDataDescription { get; set; }
        public virtual MaxDataSize MaxDataSize { get; set; }
        public virtual DataSharingAvailability Availability { get; set; }
        public virtual DateTime? AvailabilityDate { get; set; }
        public virtual ShareAccess ShareAccess { get; set; }
        public virtual string ShareAccessDescription { get; set; }
        public virtual DataLicensingType LicensingArrangement { get; set; }

        public virtual DateTime CreationDate { get { return _creationDate; } }

        /// <summary>
        /// Initializes a new instance of the DataManagementPlan class.
        /// </summary>
        public DataDeposit()
        {
            _creationDate = DateTime.Now;
        }
    }
}