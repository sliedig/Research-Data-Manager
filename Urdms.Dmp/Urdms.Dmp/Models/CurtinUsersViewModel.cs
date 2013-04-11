using System;
using System.Collections.Generic;
using System.Linq;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Models
{
    public abstract class UrdmsUsersBaseViewModel
    {
        protected UrdmsUsersBaseViewModel()
        {
            this.Users = new List<UrdmsUserViewModel>();
        }

        protected UrdmsUsersBaseViewModel(IEnumerable<UrdmsUserViewModel> users) : this()
        {
            this.Users.AddRange(users);
        }

        public IList<UrdmsUserViewModel> Users { get; private set; }
        public abstract IDictionary<int, string> Relationships { get; }
        public virtual string RelationshipDescription
        {
            get { return "Relationship"; }
        }
    }

    public class ProjectUrdmsUsersViewModel : UrdmsUsersBaseViewModel
    {
        public ProjectUrdmsUsersViewModel() { }
        public ProjectUrdmsUsersViewModel(IEnumerable<UrdmsUserViewModel> users) : base(users) { }

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

    public class DataCollectionUrdmsUsersViewModel : UrdmsUsersBaseViewModel
    {
        public DataCollectionUrdmsUsersViewModel() { }
        public DataCollectionUrdmsUsersViewModel(IEnumerable<UrdmsUserViewModel> users) : base(users) { }
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