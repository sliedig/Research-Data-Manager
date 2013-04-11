using System.Linq;
using Curtin.Framework.Common.Extensions;
using FizzWare.NBuilder;
using NUnit.Framework;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    [TestFixture]
    class DataCollectionHashCodeRepositoryShould : DbTestBase
    {
        private IDataCollectionHashCodeRepository _hashCodeRepository;
        private IDataCollectionRepository _dataCollectionRepository;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _hashCodeRepository = new DataCollectionHashCodeRepository(CreateSession());
            _dataCollectionRepository = new DataCollectionRepository(CreateSession());
        }

        [Test]
        public void Get_a_different_hashcode()
        {
            var collection = CreateDataCollection();
            Session.Save(collection);
            Session.Flush();

            var savedCollection = _dataCollectionRepository.Get(collection.Id);
            Assert.That(savedCollection,Is.Not.Null);

            var hashCode = savedCollection.NewDataCollectionHashCode();
            Assert.That(string.IsNullOrWhiteSpace(hashCode.HashCode), Is.False);

            Session.Save(hashCode);
            Session.Flush();

            var savedHashCode = _hashCodeRepository.Get(hashCode.Id);
            Assert.That(savedHashCode,Is.Not.Null);
            Assert.That(hashCode.HashCode,Is.EqualTo(savedHashCode.HashCode));

            collection.Keywords = "New keywords";
            Assert.That(savedHashCode.UpdateHashCode(collection),Is.True);

            Session.Save(savedHashCode);
            Session.Flush();

            var updatedHashCode = _hashCodeRepository.Get(hashCode.Id);

            Assert.That(updatedHashCode,Is.Not.Null);
            Assert.That(savedHashCode.HashCode,Is.EqualTo(updatedHashCode.HashCode));
            Assert.That(hashCode.HashCode,Is.Not.EqualTo(updatedHashCode.HashCode));
        }

        [Test]
        public void Return_a_hashcode_via_the_try_save_method()
        {
            var collection = CreateDataCollection();
            Session.Save(collection);
            Session.Flush();

            var savedCollection = _dataCollectionRepository.Get(collection.Id);
            Assert.That(savedCollection, Is.Not.Null);

            DataCollectionHashCode hashCode;
            Assert.That(_hashCodeRepository.TrySave(collection, out hashCode),Is.True);
            Assert.That(_hashCodeRepository.TrySave(collection, out hashCode), Is.False);

        }
        
        private static DataCollection CreateDataCollection()
        {
            var parties = Builder<Party>.CreateListOfSize(2)
                .All()
                .With(o => o.Id = 0)
                .Build();

            var dataCollectionParties = Builder<DataCollectionParty>.CreateListOfSize(2)
                .All()
                .With(o => o.Id = 0)
                .TheFirst(1)
                .With(o => o.Relationship = DataCollectionRelationshipType.Manager)
                .And(o => o.Party = parties.First())
                .TheLast(1)
                .With(o => o.Relationship = DataCollectionRelationshipType.AssociatedResearcher)
                .And(o => o.Party = parties.Last())
                .Build();

            var seoCodes = Builder<SocioEconomicObjective>.CreateListOfSize(4).Build();
            var dataCollectionSeoCodes = Builder<DataCollectionSocioEconomicObjective>.CreateListOfSize(seoCodes.Count)
                .All()
                .With(o => o.Id = 0)
                .Build();
            dataCollectionSeoCodes.Do(o => o.SocioEconomicObjective = seoCodes[dataCollectionSeoCodes.IndexOf(o)]);

            var forCodes = Builder<FieldOfResearch>.CreateListOfSize(5).Build();
            var dataCollectionForCodes = Builder<DataCollectionFieldOfResearch>.CreateListOfSize(forCodes.Count)
                .All()
                .With(o => o.Id = 0)
                .Build();
            dataCollectionForCodes.Do(o => o.FieldOfResearch = forCodes[dataCollectionForCodes.IndexOf(o)]);



            var collection = Builder<DataCollection>.CreateNew()
                .Do(o =>
                {
                    o.Parties.AddRange(dataCollectionParties);
                    o.SocioEconomicObjectives.AddRange(dataCollectionSeoCodes);
                    o.FieldsOfResearch.AddRange(dataCollectionForCodes);
                })
                .Build();

            collection.Parties.ForEach(o => o.DataCollection = collection);
            return collection;
        }
    }
}
