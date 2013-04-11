using System;
using FizzWare.NBuilder;
using NUnit.Framework;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    [TestFixture]
    internal class DataManagementPlanRepositoryShould : DbTestBase
    {
        private IProjectRepository _repository;
        private const int InvalidRepoId = 21;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _repository = new ProjectRepository(CreateSession());
        }

        [Test]
        public void Save_an_incomplete_data_management_plan()
        {
            var entity = Builder<Project>.CreateNew().With(p => p.Id = 0).Build();
            entity.DataManagementPlan = new DataManagementPlan();
                            
            _repository.Save(entity);
            Assert.That(entity.Id, Is.GreaterThan(0), "Data Management Plan not created");
            Assert.That(entity.DataManagementPlan.NewDataDetail, Is.Null, "New Data Detail inadvertently created");
            Assert.That(entity.DataManagementPlan.BackupPolicy, Is.Null, "Backup Policy inadvertently created");
            Assert.That(entity.DataManagementPlan.DataDocumentation, Is.Null, "Data Documentation inadvertently created");
            Assert.That(entity.DataManagementPlan.DataRetention, Is.Null, "Data Retention inadvertently created");
            Assert.That(entity.DataManagementPlan.DataSharing, Is.Null, "Data Sharing inadvertently created");
            Assert.That(entity.DataManagementPlan.Confidentiality, Is.Null, "Intellectual Property inadvertently created");
            Assert.That(entity.DataManagementPlan.Ethic, Is.Null, "Ethic inadvertently created");
            Assert.That(entity.DataManagementPlan.ExistingDataDetail, Is.Null, "Existing Data Detail inadvertently created");
            Assert.That(entity.DataManagementPlan.DataRelationshipDetail, Is.Null, "Data Relationship Detail created");
           Assert.That(entity.DataManagementPlan.CreationDate.ToShortDateString() == DateTime.Now.ToShortDateString());


            var key = entity.Id;
            entity.Description = "testing 2";
            _repository.Save(entity);
            Assert.That(entity.Id, Is.EqualTo(key), "Data Management Plan Id not kept constant");

            var createdEntity = _repository.Get(entity.Id);
            Assert.That(createdEntity, Is.Not.Null, "Data Management Plan not retrieved");
            Assert.That(createdEntity, Is.Not.Null, "Project not retrieved");

            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.Basic), "The created and retrieved Dmp do not have the same basic properties");
        }

        [Test]
        public void Submit_a_complete_data_management_plan()
        {
            var entity = Builder<Project>.CreateNew()
                .With(p => p.Id = 0)
                .And(p => p.ProvisioningStatus = ProvisioningStatus.NotStarted)
                .Build();
            var dmp = Builder<DataManagementPlan>.CreateNew()
                .With(o => o.Id = 0)
                .And(o => o.BackupPolicy = Builder<BackupPolicy>.CreateNew().Build())
                .And(o => o.DataDocumentation = Builder<DataDocumentation>.CreateNew().Build())
                
                .And(o => o.DataOrganisation = Builder<DataOrganisation>.CreateNew().Build())
                .And(o => o.DataStorage = Builder<DataStorage>.CreateNew().Build())

                .And(o => o.DataRetention = Builder<DataRetention>.CreateNew().Build())
                .And(o => o.DataSharing = Builder<DataSharing>.CreateNew().Build())
                .And(o => o.Ethic = Builder<Ethic>.CreateNew().Build())
                .And(o => o.ExistingDataDetail = Builder<ExistingDataDetail>.CreateNew().Build())
                .And(o => o.Confidentiality = Builder<Confidentiality>.CreateNew().Build())
                .And(o => o.NewDataDetail = Builder<NewDataDetail>.CreateNew().Build())
                .And(o => o.DataRelationshipDetail = Builder<DataRelationshipDetail>.CreateNew()
                    .With(q => q.RelationshipBetweenExistingAndNewData = Pick<DataRelationship>.RandomItemFrom(new[] { DataRelationship.ExistingFormat, DataRelationship.NewFormat, DataRelationship.UnifiedFormat }))
                    .Build())
                .Build();
            entity.DataManagementPlan = dmp;

            _repository.Save(entity);

            // creation assertions
            Assert.That(entity.Id, Is.GreaterThan(0), "Data Management Plan not created");
           
            var createdEntity = _repository.Get(entity.Id);
            Assert.That(createdEntity, Is.Not.Null, "Data Management Plan not retrieved");

            // comparison assertions
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.BackupPolicy), "Backup policy is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.DataDocumentation), "Data documentation is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.DataRetention), "Data retention is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.DataSharing), "Data sharing is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.DataManagementPlan), "Dmp is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.Ethic), "Ethic is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.ExistingDataDetail), "Existing data detail is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.IntellectualProperty), "Intellectual property is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.NewDataDetail), "New data detail is not the same");
            Assert.That(entity.IsEqualTo(createdEntity, ProjectCompareTypes.DataRelationshipDetail), "Data Relationship Detail is not the same");
            Assert.That(entity.DataManagementPlan.CreationDate.ToShortDateString() == DateTime.Now.ToShortDateString());

        }

        [Test]
        public void Return_null_when_trying_to_retrieve_data_management_plan_with_a_invalid_id()
        {
            var entity = Builder<Project>.CreateNew().With(p => p.Id = 0).Build();
            entity.DataManagementPlan = new DataManagementPlan();
          
            _repository.Save(entity);
            var createdEntity = _repository.Get(InvalidRepoId);
            Assert.That(createdEntity == null, "Data Management Plan not null");
        }

        [Test]
        public void Return_a_valid_data_management_plan_when_trying_to_retrieve_data_management_plan_with_a_valid_projectid()
        {
            var entity = Builder<Project>.CreateNew().With(p => p.Id = 0).Build();
            entity.DataManagementPlan = new DataManagementPlan();
          
            _repository.Save(entity);
            var createdEntity = _repository.Get(entity.Id);
            Assert.That(createdEntity != null, "Data Management Plan not retrieved");
            Assert.That(createdEntity.Title == entity.Title, "Project name not 'Project1'");
            Assert.That(createdEntity.Description == entity.Description, "Project description not 'Project details'");
        }
    }    
}
