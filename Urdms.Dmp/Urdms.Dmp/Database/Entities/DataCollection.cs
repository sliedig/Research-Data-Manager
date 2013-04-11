using System;
using System.Collections.Generic;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities
{
    public class DataCollection
    {
        public virtual int Id { get; set; }
        public virtual IList<DataCollectionParty> Parties { get; set; }
        public virtual string Title { get; set; }
        public virtual string ResearchDataDescription { get; set; }
        public virtual DataCollectionType Type { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DataLicensingType DataLicensingRights { get; set; }

        public virtual ShareAccess ShareAccess { get; set; }
        public virtual string ShareAccessDescription { get; set; }
        public virtual string Keywords { get; set; }
        public virtual IList<DataCollectionSocioEconomicObjective> SocioEconomicObjectives { get; set; }
        public virtual IList<DataCollectionFieldOfResearch> FieldsOfResearch { get; set; }
        public virtual bool AwareOfEthics { get; set; }
        public virtual string EthicsApprovalNumber { get; set; }
        public virtual DataSharingAvailability Availability { get; set; }
        public virtual DateTime? AvailabilityDate { get; set; }
        public virtual int ProjectId { get; set; }
        public virtual DateTime RecordCreationDate { get; set; }
        public virtual DataCollectionState CurrentState { get; set; }
        public virtual string DataStoreLocationName { get; set; }
        public virtual string DataStoreLocationUrl { get; set; }
        public virtual string DataStoreAdditionalDetails { get; set; }
        public virtual DataCollectionIdentifier DataCollectionIdentifier { get; set; }
        public virtual string DataCollectionIdentifierValue { get; set; }
        public virtual bool IsFirstCollection { get; set; }
        public DataCollection()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            RecordCreationDate = DateTime.UtcNow;
            Parties = new List<DataCollectionParty>();
            SocioEconomicObjectives = new List<DataCollectionSocioEconomicObjective>();
            FieldsOfResearch = new List<DataCollectionFieldOfResearch>();
            CurrentState = new DataCollectionState(DataCollectionStatus.Draft, RecordCreationDate);
        }
    }
}