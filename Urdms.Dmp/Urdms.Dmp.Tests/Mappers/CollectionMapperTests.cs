using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using NUnit.Framework;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Mappers
{
    [TestFixture]
    public class CollectionMapperShould
    {
        private const string DataCollectionTitle = "Collection Description Title";
        private const string ConstKeywordtestproject = "key,word, test project";

        DataCollectionViewModelStep2 _dataCollectionViewModelStep2;

        [SetUp]
        public void Setup()
        {
            Builder<DataCollectionViewModelStep1>
                .CreateNew()
                .With(o => o.ProjectId = 1)
                .With(o => o.Title = DataCollectionTitle)
                .With(o => o.StartDate = new DateTime(2011, 8, 22))
                .With(o => o.EndDate = new DateTime(2012, 12, 12))
                .With(o => o.ResearchDataDescription = "This is a description")
                .Build();

            var manager = Builder<Party>.CreateNew().Build();

            var urdmsUserViewModel = Builder<UrdmsUserViewModel>
                .CreateListOfSize(2)
                .All()
                .With(o => o.Id = 0)
                .Build();

            var nonUrdmsUserViewModel = Builder<NonUrdmsUserViewModel>
                .CreateListOfSize(2)
                .TheFirst(1)
                .With(o => o.PartyId = 3)
                .TheNext(1)
                .With(o => o.PartyId = 4)
                .All()
                .With(o => o.Id = 0)
                .Build();

            var forList = new List<DataCollectionFieldOfResearch>
                              {
                                  new DataCollectionFieldOfResearch
                                      {Id = 1, FieldOfResearch = new FieldOfResearch {Id = "FOF", Name = "FOF"}},
                                  new DataCollectionFieldOfResearch
                                      {Id = 2, FieldOfResearch = new FieldOfResearch {Id = "POP", Name = "POP"}}
                              };


            var seoList = new List<DataCollectionSocioEconomicObjective>
                              {
                                  new DataCollectionSocioEconomicObjective
                                      {
                                          Id = 1,
                                          SocioEconomicObjective = new SocioEconomicObjective {Id = "SOP", Name = "SOP"}
                                      },
                                  new DataCollectionSocioEconomicObjective
                                      {
                                          Id = 2,
                                          SocioEconomicObjective = new SocioEconomicObjective {Id = "SOB", Name = "SOB"}
                                      }
                              };


            _dataCollectionViewModelStep2 = Builder<DataCollectionViewModelStep2>
                .CreateNew()
                .With(o => o.ProjectId = 1)
                .With(o => o.Keywords = ConstKeywordtestproject)
                .With(o => o.Manager = manager)
                .With(o => o.NonUrdmsUsers = nonUrdmsUserViewModel)
                .With(o => o.UrdmsUsers = urdmsUserViewModel)
                .With(o => o.FieldsOfResearch = forList.Select(c => (ClassificationBase)c).ToList())
                .With(o => o.SocioEconomicObjectives = seoList.Select(c => (ClassificationBase)c).ToList())
                .Build();
        }


        [Test]
        public void Map_DataCollectionViewModelStep2_with_keywords_to_DataCollection()
        {

            var dataCollection = new DataCollection();
            dataCollection.MapFrom(_dataCollectionViewModelStep2);

            Assert.That(dataCollection.ProjectId == 1);
            Assert.That(dataCollection.Keywords.Split(',').Length == 3);
        }

        [Test]
        public void Map_DataCollectionViewModelStep2_with_for_codes_to_DataCollection()
        {
            var dataCollection = new DataCollection();
            dataCollection.MapFrom(_dataCollectionViewModelStep2);

            Assert.That(dataCollection.ProjectId == 1);
            Assert.That(dataCollection.FieldsOfResearch.Count == 2);
        }

        [Test]
        public void Map_DataCollectionViewModelStep2_with_for_codes_to_DataCollection_with_existing_for_codes()
        {
            var dataCollection = new DataCollection();
            var existingFieldsOfResearch = Builder<DataCollectionFieldOfResearch>
                .CreateListOfSize(3)
                .TheFirst(1).With(f => f.FieldOfResearch = new FieldOfResearch{ Id = "FOF", Name = "FOF" }).And(f => f.Id = 1)
                .TheNext(1).With(f => f.FieldOfResearch = new FieldOfResearch{ Id = "CAL", Name = "Calculus" }).And(f => f.Id = 3)
                .TheLast(1).With(f => f.FieldOfResearch = new FieldOfResearch{ Id = "SPA", Name = "Spatial Sciences"}).And(f => f.Id = 4)
                .Build();
            dataCollection.FieldsOfResearch.AddRange(existingFieldsOfResearch);

            dataCollection.MapFrom(_dataCollectionViewModelStep2);

            Assert.That(dataCollection.ProjectId, Is.EqualTo(1), "Project Id incorrect");
            Assert.That(dataCollection.FieldsOfResearch.Count, Is.EqualTo(2), "Incorrect number of FoR codes");
            Assert.That(dataCollection.FieldsOfResearch[0].Id, Is.EqualTo(1), "First FoR Id does not match");
            Assert.That(dataCollection.FieldsOfResearch[0].FieldOfResearch.Name, Is.EqualTo("FOF"), "First FoR code Name does not match");
            Assert.That(dataCollection.FieldsOfResearch[1].Id, Is.EqualTo(0), "Second FoR Id does not match (needs to be zero as it is a new record)");
            Assert.That(dataCollection.FieldsOfResearch[1].FieldOfResearch.Name, Is.EqualTo("POP"), "Second FoR code Name does not match");
        }

        [Test]
        public void Map_DataCollectionViewModelStep2_with_seo_codes_to_DataCollection()
        {
            var dataCollection = new DataCollection();
            dataCollection.MapFrom(_dataCollectionViewModelStep2);

            Assert.That(dataCollection.ProjectId == 1);
            Assert.That(dataCollection.SocioEconomicObjectives.Count == 2);
        }

        [Test]
        public void Map_DataCollectionViewModelStep2_with_seo_codes_to_DataCollection_with_existing_seo_codes()
        {
            var dataCollection = new DataCollection();
            var existingSocioEconomicObjectiveCodes = Builder<DataCollectionSocioEconomicObjective>
                .CreateListOfSize(3)
                .TheFirst(1).With(s => s.Id = 1).And(s => s.SocioEconomicObjective = new SocioEconomicObjective { Id = "SOP", Name = "SOP" })
                .TheNext(1).With(s => s.Id = 3).And(s => s.SocioEconomicObjective = new SocioEconomicObjective {Id = "ARM", Name = "Army"})
                .TheLast(1).With(s => s.Id = 4).And(s => s.SocioEconomicObjective = new SocioEconomicObjective {Id = "DEF", Name = "Defence"})
                .Build();
            dataCollection.SocioEconomicObjectives.AddRange(existingSocioEconomicObjectiveCodes);

            dataCollection.MapFrom(_dataCollectionViewModelStep2);

            Assert.That(dataCollection.ProjectId, Is.EqualTo(1), "Project Id incorrect");
            Assert.That(dataCollection.SocioEconomicObjectives.Count, Is.EqualTo(2), "Incorrect number of SEO codes");
            Assert.That(dataCollection.SocioEconomicObjectives[0].Id, Is.EqualTo(1), "First SEO Id does not match");
            Assert.That(dataCollection.SocioEconomicObjectives[0].SocioEconomicObjective.Name, Is.EqualTo("SOP"), "First SEO code Name does not match");
            Assert.That(dataCollection.SocioEconomicObjectives[1].Id, Is.EqualTo(0), "Second SEO Id does not match (needs to be zero as it is a new record)");
            Assert.That(dataCollection.SocioEconomicObjectives[1].SocioEconomicObjective.Name, Is.EqualTo("SOB"), "Second SEO code Name does not match");
        }

        [Test]
        public void Map_DataCollectionViewModelStep2_with_parties_to_DataCollection()
        {
            var dataCollection = new DataCollection();
            dataCollection.MapFrom(_dataCollectionViewModelStep2);

            Assert.That(dataCollection.ProjectId == 1);
            Assert.That(dataCollection.Parties.Count == 4);
        }
    }
}
