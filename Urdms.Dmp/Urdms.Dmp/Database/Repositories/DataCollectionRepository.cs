using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Repositories
{
    public interface IDataCollectionRepository
    {
        IList<DataCollection> GetAll();
        DataCollection Get(int id);
        IList<DataCollection> GetByProject(int id);
        IList<DataCollection> GetByStatus(IList<DataCollectionStatus> statuses);
        void Save(DataCollection dataCollection);
        bool TitleExistsAlreadyForProject(int dataCollectionId, int projectId, string title);
    }

    public class DataCollectionRepository :IDataCollectionRepository
    {
        private readonly ISession _session;

        public DataCollectionRepository(ISession session)
        {
            _session = session;
        }

        public IList<DataCollection> GetAll()
        {
            return _session.Query<DataCollection>().ToList();
        }

        public DataCollection Get(int id)
        {
            return _session.Get<DataCollection>(id);
        }

        public IList<DataCollection> GetByProject(int id)
        {
            return _session.QueryOver<DataCollection>()
                .Where(o => o.ProjectId == id).List();
        }

        public IList<DataCollection> GetByStatus(IList<DataCollectionStatus> statuses)
        {
            var dataCollections = new List<DataCollection>();

            foreach (var status in statuses)
            {
                var st = status;
                dataCollections.AddRange((from dc in _session.QueryOver<DataCollection>()
                                          where dc.CurrentState.State == st
                                          select dc).List());
            }
            return dataCollections;
        }

        private void EnsureNonDuplicationOfParty(IEnumerable<DataCollectionParty> dataCollectionParties)
        {
            var newUrdmsParties = (from o in dataCollectionParties
                                    where o.Party.Id < 1 && !string.IsNullOrWhiteSpace(o.Party.UserId)
                                    select o.Party).ToList();
            if (newUrdmsParties.Count != 0)
            {
                var parties = _session.QueryOver<Party>().List();

                foreach (var newUrdmsParty in newUrdmsParties)
                {
                    var key = newUrdmsParty.UserId;
                    var existingParty = parties.Where(o => o.UserId == key).Take(1).FirstOrDefault();
                    if (existingParty != null)
                    {
                        existingParty.FirstName = newUrdmsParty.FirstName;
                        existingParty.LastName = newUrdmsParty.LastName;
                        existingParty.FullName = newUrdmsParty.FullName;
                        existingParty.Email = newUrdmsParty.Email;
                        var dataCollectionParty = dataCollectionParties.Single(o => o.Party.UserId == key);
                        dataCollectionParty.Party = existingParty;
                    }
                }
            }
        }

        public void Save(DataCollection dataCollection)
        {
           EnsureNonDuplicationOfParty(dataCollection.Parties);
           _session.SaveOrUpdate(dataCollection);
           _session.Flush();
        }

        public bool TitleExistsAlreadyForProject(int dataCollectionId, int projectId, string title)
        {
            var dataCollections = _session.QueryOver<DataCollection>()
                .Where(dc => dc.ProjectId == projectId)
                .List();

            return dataCollections.Any(dc => dc.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase) && dc.Id != dataCollectionId);
        }
    }
}