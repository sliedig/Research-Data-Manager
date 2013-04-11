using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using Urdms.ProvisioningService.Events;
using Urdms.ProvisioningService.ViewModelUpdater;
using Urdms.ProvisioningService.ViewModelUpdater.Database.Repositories;

namespace ProvisioningServiceTests
{
    [TestFixture]
    class ViewModelUpdaterTests
    {
        private IDataCollectionRepository _repository;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            Test.Initialize();
        }

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IDataCollectionRepository>();
            
        }

        [TearDown]
        public void TearDown()
        {
            _repository = null;
        }

        [Test]
        public void Update_status_of_the_data_management_plan_when_a_site_when_the_status_of_provisioning_changes()
        {
            var handler = new ViewModelUpdaterHandler(_repository);
            _repository.UpdateStatusByProjectId(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int>());
           
            Test.Handler(handler)
                .OnMessage<ProvisioningStatusChanged>(m =>
                {
                    m.ProjectId = 1;
                    m.SiteUrl = "http://mydomain.org";
                    m.RequestId = 1;
                    m.ProvisioningRequestStatusId = 1;
                });
            _repository.Received().UpdateStatusByProjectId(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int>());

        }
        
    }

}
