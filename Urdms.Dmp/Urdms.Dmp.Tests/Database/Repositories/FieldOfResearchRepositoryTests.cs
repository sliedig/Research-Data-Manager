using NUnit.Framework;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    [TestFixture]
    class FieldOfResearchRepositoryShould : DbTestBase
    {
        private IFieldOfResearchRepository _repository;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _repository = new FieldOfResearchRepository(CreateSession());
        }

        [Test]
        public void Get_a_field_of_research()
        {
            const string id = "010101";
            var entity = _repository.GetFieldOfResearch(id);
            Assert.That(entity,Is.Not.Null,"Field of research not found");
            Assert.That(entity.Id, Is.EqualTo(id), "Incorrect field of research retrieved");
        }

        [Test]
        public void Return_null_if_id_is_null_or_whitespace()
        {
            var entity = _repository.GetFieldOfResearch(null);
            Assert.That(entity, Is.Null, "Should return null if Id is null or whitespace");
        }

        [Test]
        public void Return_a_list_of_field_of_research_codes_with_call_to_getall()
        {
            var forList = _repository.GetMatching("01");
            Assert.That(forList, Is.Not.Null);
            Assert.That(forList.Count, Is.EqualTo(10));
        }

        [Test]
        public void Not_return_null_if_term_is_invalid()
        {
            var forList = _repository.GetMatching("xx");
            Assert.That(forList, Is.Not.Null);
        }

    }
}
