using NHibernate;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Database.Repositories
{
    public interface ITimerRepository
    {
        FormTimer Get(int dmpId, int step);
        void Save(FormTimer formTimer);
    }

    public class TimerRepository : ITimerRepository
    {
        private readonly ISession _session;

        public TimerRepository(ISession session)
        {
            _session = session;
        }

        public FormTimer Get(int dmpId, int step)
        {
            return _session.Get<FormTimer>(new FormTimer{Id = dmpId, Step = step});
        }

        public void Save(FormTimer formTimer)
        {
            var existing = _session.Get<FormTimer>(formTimer);
            if (existing == null)
            {
                _session.Save(formTimer);
                _session.Flush();
            }
        }
    }

}