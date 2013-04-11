using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities.Components
{
    public class ExistingDataDetail
    {
        private bool _useExistingData;

        public virtual bool UseExistingData { get { return _useExistingData; } set { _useExistingData = value; } }
        public virtual string ExistingDataOwner { get; set; }
        public virtual ExistingDataAccessTypes ExistingDataAccessTypes { get; set; }
        public virtual string AccessTypesDescription { get; set; }

        public ExistingDataDetail()
        {
            _useExistingData = true;
        }
    }
}