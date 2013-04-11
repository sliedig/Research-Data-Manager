using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Database.Entities
{
    public class DataCollectionHashCode
    {
        public virtual int Id { get; set; }
        public virtual int DataCollectionId { get; set; }
        public virtual string HashCode { get; set; }

        public virtual bool UpdateHashCode(DataCollection collection)
        {
            var hashCode = collection.GetDataCollectionHash();
            if (this.HashCode != hashCode)
            {
                this.HashCode = hashCode;
                return true;
            }
            return false;
        }

        public virtual bool IsDifferentHashCode(DataCollection collection)
        {
            var hashCode = collection.GetDataCollectionHash();
            return hashCode == null || this.HashCode != hashCode;
        }
    }
}
