using System;
using System.Collections.Generic;
using System.Linq;
using Curtin.Framework.Common.Extensions;
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
    class DataCollectionRepositoryShould : DbTestBase
    {
        private DataCollectionRepository _repository;
        private const int ProjectId = 1;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _repository = new DataCollectionRepository(CreateSession());
        }

        [Test]
        public void Return_list_of_collection_descriptions_for_project()
        {
            var collectionDescriptions = Builder<DataCollection>.CreateListOfSize(5).All().Do(c => c.ProjectId = ProjectId).Build();
            collectionDescriptions.Do(c => Session.Save(c));
            Session.Flush();

            var collectionsByProject = _repository.GetByProject(1);

            Assert.That(collectionsByProject, Has.Count.EqualTo(collectionDescriptions.Count));
            collectionDescriptions.Do(c => Assert.That(collectionsByProject.Any(cc => cc.Id == c.Id)));
        }

        [Test]
        public void Return_collection_description_for_get()
        {
            var collectionDescription = Builder<DataCollection>.CreateNew().Build();
            Session.Save(collectionDescription);
            Session.Flush();

            var savedCollectionDescription = _repository.Get(collectionDescription.Id);

            Assert.That(savedCollectionDescription, Is.Not.Null);
            Assert.That(savedCollectionDescription.Id, Is.EqualTo(collectionDescription.Id));
        }

        [Test]
        public void Return_collection_description_for_give_state()
        {
            var dataCollections = Builder<DataCollection>.CreateListOfSize(5)
                .TheFirst(2)
                .With(cd => cd.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now))
                .TheNext(1)
                .With(cd => cd.CurrentState = new DataCollectionState(DataCollectionStatus.SecondaryApproved, DateTime.Now))
                .TheLast(2)
                .With(cd => cd.CurrentState = new DataCollectionState(DataCollectionStatus.Draft, DateTime.Now))
                .Build();

            foreach (var dataCollection in dataCollections)
            {
                Session.Save(dataCollection);
                Session.Flush();
            }

            var savedDataCollections =
                _repository.GetByStatus(new List<DataCollectionStatus>
                                            {DataCollectionStatus.Submitted, DataCollectionStatus.SecondaryApproved});

            Assert.That(savedDataCollections, Is.Not.Null,  "No DataCollections found");
            Assert.That(savedDataCollections.Count, Is.EqualTo(3), "Item count not as expected");
            Assert.That(savedDataCollections.Where(dc => dc.CurrentState.State == DataCollectionStatus.Submitted).Count(), Is.EqualTo(2), "Item count not as expected");
            Assert.That(savedDataCollections.Where(dc => dc.CurrentState.State == DataCollectionStatus.SecondaryApproved).Count(), Is.EqualTo(1), "Item count not as expected");
        }

        [Test]
        public void Return_true_if_data_collection_title_exists_already_on_another_data_collection_within_current_project()
        {
            const int projectId = 1;
            const int dataCollectionId = 10;
            var dataCollections = Builder<DataCollection>.CreateListOfSize(2)
                .TheFirst(1)
                    .With(dc => dc.ProjectId = projectId)
                    .And(dc => dc.Id = 11)
                    .And(dc => dc.Title = "I exist already")
                .TheLast(1)
                    .With(dc => dc.ProjectId = projectId)
                    .And(dc => dc.Id = 12)
                    .And(dc => dc.Title = "Me too")
                .Build();

            foreach(var dataCollection in dataCollections)
            {
                Session.Save(dataCollection);
            }
            Session.Flush();

            var result = _repository.TitleExistsAlreadyForProject(dataCollectionId, projectId, "I exist already");

            Assert.That(result, Is.True, "You should not be able to save a data collection with the same title as an existing one within the current project");
        }

        [Test]
        public void Allow_existing_data_collection_to_save_itself_again_rather_than_return_true_on_call_to_TitleExistsAlreadyForProject()
        {
            const int projectId = 1;
            var dataCollections = Builder<DataCollection>.CreateListOfSize(2)
                .TheFirst(1)
                    .With(dc => dc.ProjectId = projectId)
                    .And(dc => dc.Title = "I exist already but that's ok")
                .TheLast(1)
                    .With(dc => dc.ProjectId = projectId)
                    .And(dc => dc.Title = "Me too")
                .Build();

            foreach (var dataCollection in dataCollections)
            {
                Session.Save(dataCollection);
            }
            Session.Flush();

            var result = _repository.TitleExistsAlreadyForProject(dataCollections[0].Id, projectId, "I exist already but that's ok");

            Assert.That(result, Is.False, "You should be able to save the same data collection with it's own title again");
        }
    }
}
