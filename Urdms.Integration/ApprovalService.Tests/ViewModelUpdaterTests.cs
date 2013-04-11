using System;
using NServiceBus.Testing;
using NUnit.Framework;
using Urdms.Approvals.ApprovalService.Events;
using Urdms.Approvals.ViewModelUpdater;

namespace ApprovalService.Tests
{
    [TestFixture]
    public class ViewModelUpdaterShould
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Test.Initialize();
        }

        [Test]
        public void Update_status_of_the_data_collection_given_a_approval_state_changed_message()
        {
            Test.Handler<ApprovalStateChangedHandler>()
                .OnMessage<ApprovalStateChanged>(m =>
                                                     {
                                                         m.DataCollectionId = 1;
                                                         m.ApprovalState = DataCollectionApprovalState.Submitted;
                                                         m.StateChangedOn = DateTime.Now;
                                                     });
        }
      
    }
}
