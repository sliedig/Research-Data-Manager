using NUnit.Framework;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    [TestFixture]
    class SocioEconomicObjectiveRepositoryShould : DbTestBase
    {
        private ISocioEconomicObjectiveRepository _repository;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _repository = new SocioEconomicObjectiveRepository(CreateSession());
        }

        [Test]
        public void Get_a_socio_economic_objective()
        {
            const string id = "950307";
            var entity = _repository.GetSocioEconomicObjective(id);
            Assert.That(entity,Is.Not.Null,"Socio economic objective not found");
            Assert.That(entity.Id, Is.EqualTo(id), "Incorrect socio economic objective retrieved");
        }

        [Test]
        public void Return_null_if_id_is_null_or_whitespace()
        {
            var entity = _repository.GetSocioEconomicObjective(null);
            Assert.That(entity, Is.Null, "Should return null if Id is null or whitespace");
        }

        [Test]
        public void Return_a_list_of_socio_economic_objective_codes_with_call_to_getall()
        {
            var forList = _repository.GetMatching("82");
            Assert.That(forList, Is.Not.Null);
            Assert.That(forList.Count, Is.EqualTo(10));
        }
    }
}
