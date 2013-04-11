using NHibernate;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Database.Repositories
{
    public interface IDataCollectionHashCodeRepository
    {
        DataCollectionHashCode Get(int id);
        DataCollectionHashCode GetByDataCollectionId(int id);
        void Save(DataCollectionHashCode hashCode);
        void Delete(DataCollectionHashCode hashCode);
        bool TrySave(DataCollection collection, out DataCollectionHashCode hashCode);
        void SaveByDataCollection(DataCollection collection);
    }

    public class DataCollectionHashCodeRepository : IDataCollectionHashCodeRepository
    {
        private readonly ISession _session;

        public DataCollectionHashCodeRepository(ISession session)
        {
            _session = session;
        }

        public DataCollectionHashCode Get(int id)
        {
            return _session.Get<DataCollectionHashCode>(id);
        }

        public DataCollectionHashCode GetByDataCollectionId(int id)
        {
            return _session.QueryOver<DataCollectionHashCode>()
                .Where(o => o.DataCollectionId == id)
                .Take(1)
                .SingleOrDefault();
        }

        public void Save(DataCollectionHashCode hashCode)
        {
            _session.SaveOrUpdate(hashCode);
            _session.Flush();
        }

        public void Delete(DataCollectionHashCode hashCode)
        {
            _session.Delete(hashCode);
            _session.Flush();
        }

        public void SaveByDataCollection(DataCollection collection)
        {
            DataCollectionHashCode hashCode;
            TrySave(collection, out hashCode);
        }

        public bool TrySave(DataCollection collection, out DataCollectionHashCode hashCode)
        {
            hashCode = GetByDataCollectionId(collection.Id);
            if (hashCode == null || hashCode.UpdateHashCode(collection))
            {
                if (hashCode == null)
                {
                    hashCode = collection.NewDataCollectionHashCode();
                }
                Save(hashCode);
                return true;
            }
            return false;
        }

    }
}