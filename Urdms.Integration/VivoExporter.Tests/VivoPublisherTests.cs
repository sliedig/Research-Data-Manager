using System.Diagnostics;
using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using Urdms.Approvals.ApprovalService.Messages;
using Urdms.Approvals.VivoPublisher;
using Urdms.Approvals.VivoPublisher.Database.Entities;
using Urdms.Approvals.VivoPublisher.Database.Repositories;
using Urdms.Approvals.VivoPublisher.Messages;

namespace VivoExporter.Tests
{
    [TestFixture]
    public class VivoPublisherShould
    {
        private IVivoDataCollectionRepository _vivoDataCollectionRepository;
        private IDataCollectionRepository _dataCollectionRepository;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
          Test.Initialize();
        }

        [SetUp]
        public void Setup()
        {
            _dataCollectionRepository = Substitute.For<IDataCollectionRepository>();
            _vivoDataCollectionRepository = Substitute.For<IVivoDataCollectionRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _vivoDataCollectionRepository = null;
            _dataCollectionRepository = null;
        }

        [Test]
        public void Send_a_export_to_vivo_response_on_message_of_export_to_vivo_message()
        {
            var handler = new ExportToVivoHandler(_vivoDataCollectionRepository, _dataCollectionRepository);
            _dataCollectionRepository.Get(Arg.Is(1));

            Test.Handler(handler)
                .ExpectReply<ExportToVivoResponse>(m => m.DataCollectionId == 1)
                .OnMessage<ExportToVivo>(m =>
                                             {
                                                 m.DataCollectionId = 1;
                                             });

            _dataCollectionRepository.Received().Get(Arg.Is(1));
            _vivoDataCollectionRepository.Received().Save(Arg.Any<DataCollection>());
        }

        #region SPIKE IntegrationTesting
        [Ignore]
        [TestCase(1)]
        [TestCase(2)]
        public void Publish_a_data_collection(int id)
        {
            var collection = GetDataCollectionRepository().Get(id);
            Assert.That(collection, Is.Not.Null, "Data collection is null");

            GetVivoDataCollectionRepository().Save(collection);

        }

        [DebuggerStepThrough]
        private static IDataCollectionRepository GetDataCollectionRepository()
        {
            const string connectionString = @"Data Source=;Initial Catalog=UrdmsCI;Integrated Security=True;Pooling=False";
            var repository = new DataCollectionRepository(connectionString);
            return repository;
        }

        [DebuggerStepThrough]
        private static IVivoDataCollectionRepository GetVivoDataCollectionRepository()
        {
            const string connectionString = "Data Source=;User Id=;Password=";
            var repository = new VivoDataCollectionRepository();	// Not passing conn string at the moment
            return repository;
        }
        #endregion

    }
}
