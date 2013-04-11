using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities.Components
{
    public class DataStorage
    {
        public virtual InstitutionalStorageTypes InstitutionalStorageTypes { get; set; }
        public virtual string InstitutionalOtherTypeDescription { get; set; }
        public virtual ExternalStorageTypes ExternalStorageTypes { get; set; }
        public virtual string ExternalOtherTypeDescription { get; set; }
        public virtual PersonalStorageTypes PersonalStorageTypes { get; set; }
        public virtual string PersonalOtherTypeDescription { get; set; }
        public virtual MaxDataSize MaxDataSize { get; set; }
        public virtual string FileFormats { get; set; }
        public virtual VersionControl VersionControl { get; set; }
        public virtual string VersionControlDescription { get; set; }
    }
}