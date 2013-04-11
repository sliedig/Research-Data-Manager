using System.Collections.Generic;
using System;
using Urdms.Approvals.VivoPublisher.Database.Enums;

namespace Urdms.Approvals.VivoPublisher.Database.Entities
{
    public class DataCollection
    {
        public int Id { get; set; }
        public IList<DataCollectionParty> Parties { get; private set; }
        public string Title { get; set; }
        public string ResearchDataDescription { get; set; }
        public DataCollectionType Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DataLicensingType DataLicensingRights { get; set; }
        public ShareAccess ShareAccess { get; set; }
        public string ShareAccessDescription { get; set; }
        public string Keywords { get; set; }
        public IList<DataCollectionSocioEconomicObjective> SocioEconomicObjectives { get; private set; }
        public IList<DataCollectionFieldOfResearch> FieldsOfResearch { get; private set; }
        public DataSharingAvailability Availability { get; set; }
        public DateTime? AvailabilityDate { get; set; }
        public int ProjectId { get; set; }
        public DataCollectionIdentifier DataCollectionIdentifier { get; set; }
        public string DataCollectionIdentifierValue { get; set; }

        public DataCollection()
        {
            Parties = new List<DataCollectionParty>();
            SocioEconomicObjectives = new List<DataCollectionSocioEconomicObjective>();
            FieldsOfResearch = new List<DataCollectionFieldOfResearch>();
        }
    }
}