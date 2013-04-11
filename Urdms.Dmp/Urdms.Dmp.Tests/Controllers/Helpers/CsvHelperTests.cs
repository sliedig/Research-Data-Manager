using System;
using System.Linq;
using FizzWare.NBuilder;
using NUnit.Framework;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers.Helpers
{
    [TestFixture]
    public class CsvHelperShould
    {
        private ICsvHelper _csvHelper;
        private const int DataManagementColumnCount = 44;
        private const int DataCollectionColumnCount = 27;

        [SetUp]
        public void Setup()
        {
            _csvHelper = new CsvHelper();
        }

        [Test]
        public void Generate_Datatable_from_DataManagementPlan()
        {
            var project = SetUpFullProject("GA37493");
            var dmpTable = _csvHelper.DataManagementPlanToDataTable(project.DataManagementPlan, project.Parties);
            var dmp = project.DataManagementPlan;
            Assert.That(dmpTable, Is.Not.Null);
            Assert.That(dmpTable.Columns.Count, Is.EqualTo(DataManagementColumnCount), "Incorrect number of columns");
            Assert.That(dmpTable.Rows.Count, Is.EqualTo(1));
            var dataRow = dmpTable.Rows[0];
            Assert.That(dataRow["ExistingDataAccessTypes"], Is.EqualTo(dmp.ExistingDataDetail.ExistingDataAccessTypes.ToString()));
            Assert.That(dataRow["ExistingDataOwner"], Is.EqualTo(dmp.ExistingDataDetail.ExistingDataOwner));
            Assert.That(dataRow["InstitutionalOtherTypeDescription"], Is.EqualTo(dmp.DataStorage.InstitutionalOtherTypeDescription));
            Assert.That(dataRow["DataRetentionLocations"], Is.EqualTo(dmp.DataRetention.DataRetentionLocations.ToString()));
            Assert.That(dataRow["VersionControl"], Is.EqualTo(dmp.DataStorage.VersionControl.ToString()));
            Assert.That(dataRow["EthicRequiresClearance"], Is.EqualTo(dmp.Ethic.EthicRequiresClearance));
            Assert.That(dataRow["ConfidentialityComments"], Is.EqualTo(dmp.Confidentiality.ConfidentialityComments));
            Assert.That(dataRow["RelationshipBetweenExistingAndNewData"], Is.EqualTo(dmp.DataRelationshipDetail.RelationshipBetweenExistingAndNewData.ToString()));
            Assert.That(dataRow["DataSharingAvailabilityDate"], Is.EqualTo(dmp.DataSharing.DataSharingAvailabilityDate));

            var parties = dataRow["AccessRoles"].ToString().Split('%');
            Assert.That(parties.Length, Is.EqualTo(5));
        }

        [Test]
        public void Generate_Datatable_from_DataManagementPlans()
        {
            const string userId = "GA37493";
            var projectParties = Builder<ProjectParty>.CreateListOfSize(5)
              .TheFirst(1)
              .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
              .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
              .TheLast(4)
              .With(q => q.Party = Builder<Party>.CreateNew().Build())
              .And(q => q.Relationship = PickHelper.RandomEnumExcept(ProjectRelationship.None, ProjectRelationship.PrincipalInvestigator))
              .Build();
            var dataManagementPlan = Builder<DataManagementPlan>.CreateNew()
                  .With(o => o.ExistingDataDetail = Builder<ExistingDataDetail>.CreateNew()
                      .With(q => q.UseExistingData = PickHelper.RandomBoolean())
                      .And(q => q.ExistingDataAccessTypes = PickHelper.RandomEnumsExcept(ExistingDataAccessTypes.None))
                      .Build())
                  .And(o => o.BackupPolicy = Builder<BackupPolicy>.CreateNew().Build())
                  .And(o => o.NewDataDetail = Builder<NewDataDetail>.CreateNew()
                      .With(q => q.DataOwners = PickHelper.RandomEnumsExcept(DataOwners.None))
                      .Build())
                  .And(o => o.DataRelationshipDetail = Builder<DataRelationshipDetail>.CreateNew()
                      .With(q => q.RelationshipBetweenExistingAndNewData = PickHelper.RandomEnumExcept(DataRelationship.None))
                      .Build())
                  .And(o => o.DataDocumentation = Builder<DataDocumentation>.CreateNew()
                      .Build())
                  .And(o => o.Ethic = Builder<Ethic>.CreateNew()
                      .With(q => q.EthicRequiresClearance = PickHelper.RandomBoolean())
                      .Build())
                  .And(o => o.Confidentiality = Builder<Confidentiality>.CreateNew()
                      .With(q => q.IsSensitive = PickHelper.RandomBoolean())
                      .Build())
                  .And(o => o.DataStorage = Builder<DataStorage>.CreateNew()
                      .With(q => q.InstitutionalStorageTypes = PickHelper.RandomEnumExcept(InstitutionalStorageTypes.ProjectStorageSpace))
                      .And(q => q.ExternalStorageTypes = PickHelper.RandomEnumExcept(ExternalStorageTypes.None))
                      .And(q => q.PersonalStorageTypes = PickHelper.RandomEnumExcept(PersonalStorageTypes.None))
                      .And(q => q.MaxDataSize = PickHelper.RandomEnumExcept(MaxDataSize.None))
                      .And(q => q.VersionControl = PickHelper.RandomEnumsExcept(VersionControl.None))
                      .Build())
                  .And(o => o.DataRetention = Builder<DataRetention>.CreateNew()
                      .With(q => q.DataRetentionLocations = PickHelper.RandomEnumExcept(DataRetentionLocations.None))
                      .And(q => q.DataRetentionResponsibilities = PickHelper.RandomEnumExcept(DataResponsibilities.None))
                      .And(q => q.DataRetentionPeriod = PickHelper.RandomEnumExcept<DataRetentionPeriod>())
                      .Build())
                  .And(o => o.DataSharing = Builder<DataSharing>.CreateNew()
                      .With(q => q.DataSharingAvailability = PickHelper.RandomEnumExcept<DataSharingAvailability>())
                      .And(q => q.DataSharingAvailabilityDate = DateTime.Today.AddYears(1))
                      .And(q => q.ShareAccess = PickHelper.RandomEnumExcept(ShareAccess.NoAccess))
                      .And(q => q.DataLicensingType = PickHelper.RandomEnumExcept<DataLicensingType>())
                      .Build())
                 .Build();
            var projects = Builder<Project>.CreateListOfSize(5)
                .All()
                .With(o => o.DataManagementPlan = dataManagementPlan)
                .And(o => o.ProvisioningStatus = PickHelper.RandomEnumExcept<ProvisioningStatus>())
                .Do(o => o.Parties.AddRange(projectParties))
                .Build();
            var dataManagementPlansTable = _csvHelper.DataManagementPlansToDataTable(projects);

            Assert.That(dataManagementPlansTable, Is.Not.Null);
            Assert.That(dataManagementPlansTable.Columns.Count, Is.EqualTo(DataManagementColumnCount), "Incorrect number of columns");
            Assert.That(dataManagementPlansTable.Rows.Count, Is.EqualTo(5));

            for (var index = 0; index < projects.Count; index++)
            {
                var dmp = projects[index].DataManagementPlan;
                var dataRow = dataManagementPlansTable.Rows[index];

                Assert.That(dataRow["ExistingDataAccessTypes"], Is.EqualTo(dmp.ExistingDataDetail.ExistingDataAccessTypes.ToString()));
                Assert.That(dataRow["ExistingDataOwner"], Is.EqualTo(dmp.ExistingDataDetail.ExistingDataOwner));
                Assert.That(dataRow["InstitutionalOtherTypeDescription"], Is.EqualTo(dmp.DataStorage.InstitutionalOtherTypeDescription));
                Assert.That(dataRow["DataRetentionLocations"], Is.EqualTo(dmp.DataRetention.DataRetentionLocations.ToString()));
                Assert.That(dataRow["VersionControl"], Is.EqualTo(dmp.DataStorage.VersionControl.ToString()));
                Assert.That(dataRow["EthicRequiresClearance"], Is.EqualTo(dmp.Ethic.EthicRequiresClearance));
                Assert.That(dataRow["ConfidentialityComments"], Is.EqualTo(dmp.Confidentiality.ConfidentialityComments));
                Assert.That(dataRow["RelationshipBetweenExistingAndNewData"], Is.EqualTo(dmp.DataRelationshipDetail.RelationshipBetweenExistingAndNewData.ToString()));
                Assert.That(dataRow["DataSharingAvailabilityDate"], Is.EqualTo(dmp.DataSharing.DataSharingAvailabilityDate));

                var parties = dataRow["AccessRoles"].ToString().Split('%');
                Assert.That(parties.Length, Is.EqualTo(5));
            }
        }

        [Test]
        public void Generate_Datatable_from_DataCollection()
        {
            var dataCollection = SetUpDataCollection("GA37493");
            var dataCollectionTable = _csvHelper.DataCollectionToDataTable(dataCollection);

            Assert.That(dataCollectionTable, Is.Not.Null);
            Assert.That(dataCollectionTable.Columns.Count, Is.EqualTo(DataCollectionColumnCount));
            Assert.That(dataCollectionTable.Rows.Count, Is.EqualTo(1));

            var dataRow = dataCollectionTable.Rows[0];

            Assert.That(dataRow["Title"], Is.EqualTo(dataCollection.Title));
            Assert.That(dataRow["Type"], Is.EqualTo(dataCollection.Type.ToString()));
            Assert.That(dataRow["StartDate"], Is.EqualTo(dataCollection.StartDate));
            Assert.That(dataRow["DataLicensingRights"], Is.EqualTo(dataCollection.DataLicensingRights.ToString()));
            Assert.That(dataRow["ShareAccess"], Is.EqualTo(dataCollection.ShareAccess.ToString()));

            var parties = dataRow["Parties"].ToString().Split('%');
            Assert.That(parties.Length, Is.EqualTo(dataCollection.Parties.Count));

            var seoCodes = dataRow["SocioEconomicObjectives"].ToString().Split('%');
            Assert.That(seoCodes.Length, Is.EqualTo(dataCollection.SocioEconomicObjectives.Count));

            var forCodes = dataRow["FieldsOfResearch"].ToString().Split('%');
            Assert.That(forCodes.Length, Is.EqualTo(dataCollection.FieldsOfResearch.Count));
        }

        [Test]
        public void Generate_Datatable_from_DataCollections()
        {
            const string userId = "GA37493";
            var dataCollectionParties = Builder<DataCollectionParty>.CreateListOfSize(5)
                .TheFirst(1)
                .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                .And(q => q.Relationship = DataCollectionRelationshipType.Manager)
                .TheLast(4)
                .With(q => q.Party = Builder<Party>.CreateNew().Build())
                .And(q => q.Relationship = PickHelper.RandomEnumExcept(DataCollectionRelationshipType.None, DataCollectionRelationshipType.Manager))
                .Build();
            var forCodes = Builder<DataCollectionFieldOfResearch>.CreateListOfSize(5)
                .All()
                .With(f => f.FieldOfResearch = Builder<FieldOfResearch>.CreateNew().Build())
                .Build();
            var seoCodes = Builder<DataCollectionSocioEconomicObjective>.CreateListOfSize(5)
                .All()
                .With(f => f.SocioEconomicObjective = Builder<SocioEconomicObjective>.CreateNew().Build())
                .Build();
            var dataCollections = Builder<DataCollection>.CreateListOfSize(5)
                .All()
                .Do(d => d.Parties.AddRange(dataCollectionParties))
                .And(d => d.FieldsOfResearch.AddRange(forCodes))
                .And(d => d.SocioEconomicObjectives.AddRange(seoCodes))
                .Build();
            var dataCollectionTable = _csvHelper.DataCollectionsToDataTable(dataCollections);

            Assert.That(dataCollectionTable, Is.Not.Null);
            Assert.That(dataCollectionTable.Columns.Count, Is.EqualTo(DataCollectionColumnCount));
            Assert.That(dataCollectionTable.Rows.Count, Is.EqualTo(5));

            for (var index = 0; index < dataCollections.Count; index++ )
            {
                var dataRow = dataCollectionTable.Rows[index];

                Assert.That(dataRow["Title"], Is.EqualTo(dataCollections[index].Title));
                Assert.That(dataRow["Type"], Is.EqualTo(dataCollections[index].Type.ToString()));
                Assert.That(dataRow["StartDate"], Is.EqualTo(dataCollections[index].StartDate));
                Assert.That(dataRow["DataLicensingRights"], Is.EqualTo(dataCollections[index].DataLicensingRights.ToString()));
                Assert.That(dataRow["ShareAccess"], Is.EqualTo(dataCollections[index].ShareAccess.ToString()));

                var parties = dataRow["Parties"].ToString().Split('%');
                Assert.That(parties.Length, Is.EqualTo(dataCollections[index].Parties.Count));

                var seoCodeStrings = dataRow["SocioEconomicObjectives"].ToString().Split('%');
                Assert.That(seoCodeStrings.Length, Is.EqualTo(dataCollections[index].SocioEconomicObjectives.Count));

                var forCodeStrings = dataRow["FieldsOfResearch"].ToString().Split('%');
                Assert.That(forCodeStrings.Length, Is.EqualTo(dataCollections[index].FieldsOfResearch.Count));
            }
        }

        [Test]
        public void Render_CSV_from_DataManagementPlanTable()
        {
            var project = SetUpFullProject("GA37493");
            var dmpTable = _csvHelper.DataManagementPlanToDataTable(project.DataManagementPlan, project.Parties);
            var csv = _csvHelper.ExportToCsv(dmpTable);
            var csvRows = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Assert.That(csvRows.Length, Is.EqualTo(2));

            var csvHeaders = csvRows[0].Split('|');
            var csvValues = csvRows[1].Split('|');

            Assert.That(csvHeaders.Length, Is.EqualTo(DataManagementColumnCount), "Incorrect number of column headers");
            Assert.That(csvValues.Length, Is.EqualTo(DataManagementColumnCount), "Incorrect number of column values");
            
            var parties = csvValues[DataManagementColumnCount - 1].Split('%');
            Assert.That(parties.Length, Is.EqualTo(5));
        }

        [Test]
        public void CSV_from_DataCollection()
        {
            var dataCollection = SetUpDataCollection("GA37493");
            var dataCollectionTable = _csvHelper.DataCollectionToDataTable(dataCollection);
            var csv = _csvHelper.ExportToCsv(dataCollectionTable);
            var csvRows = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Assert.That(csvRows.Length, Is.EqualTo(2));

            var csvHeaders = csvRows[0].Split('|').ToList();
            var csvValues = csvRows[1].Split('|');

            Assert.That(csvHeaders.Count, Is.EqualTo(DataCollectionColumnCount));
            Assert.That(csvValues.Length, Is.EqualTo(DataCollectionColumnCount));

            var partyIndex = csvHeaders.IndexOf("Parties");
            var parties = csvValues[partyIndex].Split('%');
            Assert.That(parties.Length, Is.EqualTo(dataCollection.Parties.Count));
            
            var seoCodesIndex = csvHeaders.IndexOf("SocioEconomicObjectives");
            var seoCodes = csvValues[seoCodesIndex].Split('%');
            Assert.That(seoCodes.Length, Is.EqualTo(dataCollection.SocioEconomicObjectives.Count));

            var forCodesIndex = csvHeaders.IndexOf("FieldsOfResearch");
            var forCodes = csvValues[forCodesIndex].Split('%');
            Assert.That(forCodes.Length, Is.EqualTo(dataCollection.FieldsOfResearch.Count));
        }

        private Project SetUpFullProject(string userId)
        {
            var dmp = Builder<DataManagementPlan>.CreateNew()
                .With(o => o.ExistingDataDetail = Builder<ExistingDataDetail>.CreateNew()
                    .With(q => q.UseExistingData = PickHelper.RandomBoolean())
                    .And(q => q.ExistingDataAccessTypes = PickHelper.RandomEnumsExcept(ExistingDataAccessTypes.None))
                    .Build())
                .And(o => o.BackupPolicy = Builder<BackupPolicy>.CreateNew().Build())
                .And(o => o.NewDataDetail = Builder<NewDataDetail>.CreateNew()
                    .With(q => q.DataOwners = PickHelper.RandomEnumsExcept(DataOwners.None))
                    .Build())
                .And(o => o.DataRelationshipDetail = Builder<DataRelationshipDetail>.CreateNew()
                    .With(q => q.RelationshipBetweenExistingAndNewData = PickHelper.RandomEnumExcept(DataRelationship.None))
                    .Build())
                .And(o => o.DataDocumentation = Builder<DataDocumentation>.CreateNew()
                    .Build())
                .And(o => o.Ethic = Builder<Ethic>.CreateNew()
                    .With(q => q.EthicRequiresClearance = PickHelper.RandomBoolean())
                    .Build())
                .And(o => o.Confidentiality = Builder<Confidentiality>.CreateNew()
                    .With(q => q.IsSensitive = PickHelper.RandomBoolean())
                    .Build())
                .And(o => o.DataStorage = Builder<DataStorage>.CreateNew()
                    .With(q => q.InstitutionalStorageTypes = PickHelper.RandomEnumExcept(InstitutionalStorageTypes.ProjectStorageSpace))
                    .And(q => q.ExternalStorageTypes = PickHelper.RandomEnumExcept(ExternalStorageTypes.None))
                    .And(q => q.PersonalStorageTypes = PickHelper.RandomEnumExcept(PersonalStorageTypes.None))
                    .And(q => q.MaxDataSize = PickHelper.RandomEnumExcept(MaxDataSize.None))
                    .And(q => q.VersionControl = PickHelper.RandomEnumsExcept(VersionControl.None))
                    .Build())
                .And(o => o.DataRetention = Builder<DataRetention>.CreateNew()
                    .With(q => q.DataRetentionLocations = PickHelper.RandomEnumExcept(DataRetentionLocations.None))
                    .And(q => q.DataRetentionResponsibilities = PickHelper.RandomEnumExcept(DataResponsibilities.None))
                    .And(q => q.DataRetentionPeriod = PickHelper.RandomEnumExcept<DataRetentionPeriod>())
                    .Build())
                .And(o => o.DataSharing = Builder<DataSharing>.CreateNew()
                    .With(q => q.DataSharingAvailability = PickHelper.RandomEnumExcept<DataSharingAvailability>())
                    .And(q => q.DataSharingAvailabilityDate = DateTime.Today.AddYears(1))
                    .And(q => q.ShareAccess = PickHelper.RandomEnumExcept(ShareAccess.NoAccess))
                    .And(q => q.DataLicensingType = PickHelper.RandomEnumExcept<DataLicensingType>())
                    .Build())
               .Build();

            var projectParties = Builder<ProjectParty>.CreateListOfSize(5)
                .TheFirst(1)
                .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                .TheLast(4)
                .With(q => q.Party = Builder<Party>.CreateNew().Build())
                .And(q => q.Relationship = PickHelper.RandomEnumExcept(ProjectRelationship.None, ProjectRelationship.PrincipalInvestigator))
                .Build();

            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.ProvisioningStatus = PickHelper.RandomEnumExcept<ProvisioningStatus>())
                .Do(o => o.Parties.AddRange(projectParties))
                .Build();

            return project;
        }

        private DataCollection SetUpDataCollection(string userId)
        {
            var dataCollectionParties = Builder<DataCollectionParty>.CreateListOfSize(5)
                .TheFirst(1)
                .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                .And(q => q.Relationship = DataCollectionRelationshipType.Manager)
                .TheLast(4)
                .With(q => q.Party = Builder<Party>.CreateNew().Build())
                .And(q => q.Relationship = PickHelper.RandomEnumExcept(DataCollectionRelationshipType.None, DataCollectionRelationshipType.Manager))
                .Build();
            var forCodes = Builder<DataCollectionFieldOfResearch>.CreateListOfSize(5)
                .All()
                .With(f => f.FieldOfResearch = Builder<FieldOfResearch>.CreateNew().Build())
                .Build();
            var seoCodes = Builder<DataCollectionSocioEconomicObjective>.CreateListOfSize(5)
                .All()
                .With(f => f.SocioEconomicObjective= Builder<SocioEconomicObjective>.CreateNew().Build())
                .Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .Do(d => d.Parties.AddRange(dataCollectionParties))
                .And(d => d.FieldsOfResearch.AddRange(forCodes))
                .And(d => d.SocioEconomicObjectives.AddRange(seoCodes))
                .Build();
            return dataCollection;
        }

    }
}
