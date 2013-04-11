using System.Collections.Generic;
using NHibernate;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Database.Repositories
{
    public interface ISocioEconomicObjectiveRepository
    {
        SocioEconomicObjective GetSocioEconomicObjective(string id);
        IList<SocioEconomicObjective> GetMatching(string code);
        IList<SocioEconomicObjective> GetAll();
    }

    public class SocioEconomicObjectiveRepository : ISocioEconomicObjectiveRepository
    {
        private readonly ISession _session;

        public SocioEconomicObjectiveRepository(ISession session)
        {
            _session = session;
        }

        public SocioEconomicObjective GetSocioEconomicObjective(string id)
        {
            return string.IsNullOrWhiteSpace(id) ? null : _session.Get<SocioEconomicObjective>(id);
        }

        public IList<SocioEconomicObjective> GetMatching(string code)
        {
            return _session.QueryOver<SocioEconomicObjective>()
                .WhereRestrictionOn(s => s.Id)
                .IsLike(code + "%")
                .Take(10)
                .List();
        }

        public IList<SocioEconomicObjective> GetAll()
        {
            return _session.QueryOver<SocioEconomicObjective>().List();
        }
    }
}