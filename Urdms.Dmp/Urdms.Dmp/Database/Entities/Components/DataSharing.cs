using System;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities.Components
{
    public class DataSharing
    {
        public virtual DataSharingAvailability DataSharingAvailability { get; set; }
        public virtual DateTime? DataSharingAvailabilityDate { get; set; }
        public virtual ShareAccess ShareAccess { get; set; }
        public virtual string ShareAccessDescription { get; set; }
        public virtual DataLicensingType DataLicensingType { get; set; }
        public virtual string ReuseByOrganisations { get; set; }
    }
}