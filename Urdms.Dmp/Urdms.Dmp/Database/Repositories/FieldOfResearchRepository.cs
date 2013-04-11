using System.Collections.Generic;
using NHibernate;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Database.Repositories
{
    public interface IFieldOfResearchRepository
    {
        FieldOfResearch GetFieldOfResearch(string id);
        IList<FieldOfResearch> GetMatching(string code);
        IList<FieldOfResearch> GetAll();
    }

    public class FieldOfResearchRepository : IFieldOfResearchRepository
    {
        private readonly ISession _session;

        public FieldOfResearchRepository(ISession session)
        {
            _session = session;
        }

        public FieldOfResearch GetFieldOfResearch(string id)
        {
            return string.IsNullOrWhiteSpace(id) ? null : _session.Get<FieldOfResearch>(id);
        }

        public IList<FieldOfResearch> GetMatching(string code)
        {
            return _session.QueryOver<FieldOfResearch>()
                .WhereRestrictionOn(f => f.Id)
                .IsLike(code + "%")
                .Take(10)
                .List();
        }

        public IList<FieldOfResearch> GetAll()
        {
            return _session.QueryOver<FieldOfResearch>().List();
        }
    }
}