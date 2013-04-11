using System;
using Urdms.Dmp.Database.Entities.Components;

namespace Urdms.Dmp.Database.Entities
{
    public class DataManagementPlan
    {
        private readonly DateTime _creationDate;

        public virtual int Id { get; set; }
        public virtual DataStorage DataStorage { get; set; }
        public virtual DataOrganisation DataOrganisation { get; set; }
        public virtual NewDataDetail NewDataDetail { get; set; }
        public virtual ExistingDataDetail ExistingDataDetail { get; set; }
        public virtual DataDocumentation DataDocumentation { get; set; }
        public virtual Ethic Ethic { get; set; }
        public virtual Confidentiality Confidentiality { get; set; }
        public virtual BackupPolicy BackupPolicy { get; set; }
        public virtual DataRetention DataRetention { get; set; }
        public virtual DataSharing DataSharing { get; set; }
        public virtual DataRelationshipDetail DataRelationshipDetail { get; set; }
        public virtual DateTime CreationDate { get { return _creationDate; } }

        /// <summary>
        /// Initializes a new instance of the DataManagementPlan class.
        /// </summary>
        public DataManagementPlan()
        {
            _creationDate = DateTime.Now;
        }

    }
}