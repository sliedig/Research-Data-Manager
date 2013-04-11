using System;
using System.Collections.Generic;
using System.Linq;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Database.Entities
{
    public class Project
    {
        public virtual int Id { get; set; }
        public virtual IList<ProjectParty> Parties { get; set; }
        public virtual DataManagementPlan DataManagementPlan { get; set; }
        public virtual DataDeposit DataDeposit { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual SourceProjectType SourceProjectType { get; set; }
        public virtual string SourceId { get; set; }
        public virtual IList<ProjectSocioEconomicObjective> SocioEconomicObjectives { get; set; }
        public virtual IList<ProjectFieldOfResearch> FieldsOfResearch { get; set; }
        public virtual string Keywords { get; set; }
        public virtual ProjectStatus Status { get; set; }
        public virtual IList<ProjectFunder> Funders { get; set; }
        public virtual IList<DataCollection> DataCollections { get; set; }
        
        public virtual ProvisioningStatus ProvisioningStatus { get; set; }
        public virtual DateTime? InitialProvisioningRequestDate { get; set; }    
        private int _provisioningRequestId;
        public virtual int ProvisioningRequestId
        {
            get { return _provisioningRequestId; }
        }
        private string _siteUrl;
        public virtual string SiteUrl
        {
            get { return _siteUrl; }
        }

        /// <summary>
        /// Initializes a new instance of the Project class.
        /// </summary>
        public Project()
        {
            SetupValues();
        }

        public virtual void SetupValues()
        {
            Parties = new List<ProjectParty>();
            SocioEconomicObjectives = new List<ProjectSocioEconomicObjective>();
            FieldsOfResearch = new List<ProjectFieldOfResearch>();
            Funders = new List<ProjectFunder>();
            DataCollections = new List<DataCollection>();
        }


        public virtual DataCollection CreateInitialDataCollection()
        {
            var dataCollection = new DataCollection
                                            {
                                                Title = "Research data for " + this.Title,
                                                Keywords = this.Keywords,
                                                ProjectId = this.Id,
                                                IsFirstCollection = true,
                                                DataStoreLocationName = "Project Storage Space",
                                                DataStoreLocationUrl = SiteUrl
                                            };
            if (this.SourceProjectType == SourceProjectType.DEPOSIT && this.DataDeposit != null)
            {
                var deposit = this.DataDeposit;
                dataCollection.ResearchDataDescription = deposit.ResearchDataDescription;
                dataCollection.DataLicensingRights = deposit.LicensingArrangement;
                dataCollection.ShareAccess = deposit.ShareAccess;
                dataCollection.ShareAccessDescription = deposit.ShareAccessDescription;
                dataCollection.AvailabilityDate = deposit.AvailabilityDate;
                dataCollection.Availability = deposit.Availability;
            }
            else if (this.SourceProjectType != SourceProjectType.DEPOSIT && this.DataManagementPlan != null)
            {
                var dmp = this.DataManagementPlan;
                if (dmp.NewDataDetail != null)
                {
                    dataCollection.ResearchDataDescription = dmp.NewDataDetail.ResearchDataDescription;
                }
                if (dmp.DataSharing != null)
                {
                    dataCollection.DataLicensingRights = dmp.DataSharing.DataLicensingType;
                    dataCollection.ShareAccess = dmp.DataSharing.ShareAccess;
                    dataCollection.ShareAccessDescription = dmp.DataSharing.ShareAccessDescription;
                    dataCollection.AvailabilityDate = dmp.DataSharing.DataSharingAvailabilityDate;
                    dataCollection.Availability = dmp.DataSharing.DataSharingAvailability;
                }
            }
            var parties = this.GetDataCollectionParties(dataCollection);

            var socioEconomicObjectives = from o in (this.SocioEconomicObjectives ?? new List<ProjectSocioEconomicObjective>())
                                          select new DataCollectionSocioEconomicObjective { SocioEconomicObjective = o.SocioEconomicObjective };
            var fieldsOfResearch = from o in (this.FieldsOfResearch ?? new List<ProjectFieldOfResearch>())
                                   select new DataCollectionFieldOfResearch { FieldOfResearch = o.FieldOfResearch };

            dataCollection.Parties.AddRange(parties);
            dataCollection.SocioEconomicObjectives.AddRange(socioEconomicObjectives);
            dataCollection.FieldsOfResearch.AddRange(fieldsOfResearch);

            return dataCollection;
        }

        public virtual IEnumerable<DataCollectionParty> GetDataCollectionParties(DataCollection collection)
        {
            return Parties
                    .Where(p => p.Role != AccessRole.None 
                        && p.Id > 0 
                        && collection.Parties.All(cp => cp.Party.UserId != p.Party.UserId))
                    .Select(n => new DataCollectionParty
                                     {
                                         Party = n.Party,
                                         DataCollection = collection,
                                         Relationship = n.Relationship == ProjectRelationship.PrincipalInvestigator
                                            ? DataCollectionRelationshipType.Manager
                                            : DataCollectionRelationshipType.AssociatedResearcher
                                     });
        }
    }
}
