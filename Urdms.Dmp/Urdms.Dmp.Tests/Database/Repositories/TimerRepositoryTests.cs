using System.Threading;
using FizzWare.NBuilder;
using NUnit.Framework;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    [TestFixture]
    internal class TimerRepositoryShould : DbTestBase
    {
        private ITimerRepository _repository;
        private const int InvalidRepoId = 21;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _repository = new TimerRepository(CreateSession());
        }

        [Test]
        public void Save_an_new_record_for_dmpId_and_step()
        {
            var timerEvent = Builder<FormTimer>.CreateNew().Build();

            _repository.Save(timerEvent);

            var timerEventSaved = _repository.Get(timerEvent.Id, timerEvent.Step);

            Assert.That(timerEventSaved, Is.Not.Null, "Timer event not saved");
            Assert.That(timerEventSaved.UserId, Is.EqualTo(timerEvent.UserId), "Saved user is not the same");
            Assert.That(timerEventSaved.StartTime, Is.EqualTo(timerEvent.StartTime), "Saved start time is not the same");
            Assert.That(timerEventSaved.EndTime, Is.EqualTo(timerEvent.EndTime), "Saved end time is not the same");
        }

        [Test]
        public void Not_save_an_new_record_for_a_existing_dmpId_and_step()
        {
            var timerEvent1 = Builder<FormTimer>.CreateNew().Build();

            _repository.Save(timerEvent1);

            var timerEventSaved1 = _repository.Get(timerEvent1.Id, timerEvent1.Step);

           Thread.Sleep(1000);

           var timerEvent2 = Builder<FormTimer>.CreateNew().Build();

            _repository.Save(timerEvent2);

            var timerEventSaved2 = _repository.Get(timerEvent2.Id, timerEvent2.Step);

            Assert.That(timerEventSaved1.StartTime, Is.EqualTo(timerEventSaved2.StartTime), "Saved start time is not the same");
            Assert.That(timerEventSaved1.EndTime, Is.EqualTo(timerEventSaved2.EndTime), "Saved end time is not the same");
        }

        
    }    
}
