using System;
using System.Collections.Generic;
using System.Linq;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Models
{
    public abstract class NonUrdmsUsersBaseViewModel
    {
        protected NonUrdmsUsersBaseViewModel()
        {
            this.Users = new List<NonUrdmsUserViewModel>();
        }

        protected NonUrdmsUsersBaseViewModel(IEnumerable<NonUrdmsUserViewModel> users)
            : this()
        {
            this.Users.AddRange(users);
        }

        public IList<NonUrdmsUserViewModel> Users { get; private set; }
        public abstract IDictionary<int, string> Relationships { get; }
        public virtual string RelationshipDescription
        {
            get { return "Relationship"; }
        }
    }

    public class ProjectNonUrdmsUsersViewModel : NonUrdmsUsersBaseViewModel
    {
        public ProjectNonUrdmsUsersViewModel() { }
        public ProjectNonUrdmsUsersViewModel(IEnumerable<NonUrdmsUserViewModel> users) : base(users) { }

        public override IDictionary<int, string> Relationships
        {
            get
            {
                var values = Enum.GetValues(typeof(AccessRole)).Cast<AccessRole>().Except(new[] { AccessRole.Owners, AccessRole.None });
                return values.ToDictionary(o => (int)o, o => o.GetDescription());
            }
        }

        public override string RelationshipDescription
        {
            get { return "Role"; }
        }
    }

    public class DataCollectionNonUrdmsUsersViewModel : NonUrdmsUsersBaseViewModel
    {
        public DataCollectionNonUrdmsUsersViewModel() { }
        public DataCollectionNonUrdmsUsersViewModel(IEnumerable<NonUrdmsUserViewModel> users) : base(users) { }
        public override IDictionary<int, string> Relationships
        {
            get
            {
                var values = Enum.GetValues(typeof(DataCollectionRelationshipType)).Cast<DataCollectionRelationshipType>().Except(new[] { DataCollectionRelationshipType.Manager, DataCollectionRelationshipType.None });
                return values.ToDictionary(o => (int)o, o => o.GetDescription());
            }
        }
    }


}