using System.Collections.Generic;
using System.Linq;
using Curtin.Framework.Common.Extensions;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Interfaces;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Models.Interfaces;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Mappers
{
    public static class UserManagementMapper
    {
        public static IList<T> MapFrom<T>(this IList<T> entities, IUserManagementViewModel model) 
            where T : class, IUserManagementParty
        {
            if (model == null || (model.UrdmsUsers.IsEmpty() && model.NonUrdmsUsers.IsEmpty()))
            {
                return entities;
            }

            if (typeof(T) == typeof(ProjectParty))
            {
                entities.Cast<ProjectParty>().UpdateParties(model);
                entities.AddRange(GetNewProjectParties(model).Cast<T>());
            }
            else if(typeof(T) == typeof(DataCollectionParty))
            {
                entities.Cast<DataCollectionParty>().UpdateParties(model);
                entities.AddRange(GetNewDataCollectionParties(model).Cast<T>());

            }

            return entities;
        }

        #region Project Party
        private static void UpdateParties(this IEnumerable<ProjectParty> entities, IUserManagementViewModel model)
        {
            foreach (var entity in entities.Where(o => o.Id > 0))
            {
                dynamic vm = !string.IsNullOrWhiteSpace(entity.Party.UserId) ? model.UrdmsUsers.SingleOrDefault(o => o.Id == entity.Id) : (object)model.NonUrdmsUsers.SingleOrDefault(o => o.Id == entity.Id);
                if (vm != null)
                {
                    var role = (AccessRole)vm.Relationship;
                    entity.Role = role;

                }
            }
        }

        private static IEnumerable<ProjectParty> GetNewProjectParties(IUserManagementViewModel model)
        {
            var newNonUrdmsUsers = from o in model.NonUrdmsUsers
                                    where o.Id < 1
                                    select new ProjectParty
                                    {
                                        Party = new Party { FullName = o.FullName, Id = o.PartyId, Organisation = o.Organisation },
                                        Role = (AccessRole)o.Relationship
                                    };

            var newUrdmsUsers = from o in model.UrdmsUsers
                                 where o.Id < 1
                                 select new ProjectParty
                                 {
									 Party = new Party { FullName = o.FullName, Id = o.PartyId, UserId = o.UserId, Organisation = "" },	// TODO: Insert your organisation here
                                     Role = (AccessRole)o.Relationship
                                 };

            return newNonUrdmsUsers.Union(newUrdmsUsers);
        }
        #endregion

        #region DataCollection Party
        private static void UpdateParties(this IEnumerable<DataCollectionParty> entities, IUserManagementViewModel model)
        {
            foreach (var entity in entities.Where(o => o.Id > 0))
            {
                dynamic vm = !string.IsNullOrWhiteSpace(entity.Party.UserId) ? model.UrdmsUsers.SingleOrDefault(o => o.Id == entity.Id) : (object)model.NonUrdmsUsers.SingleOrDefault(o => o.Id == entity.Id);
                if (vm != null)
                {
                    var relationship = (DataCollectionRelationshipType)vm.Relationship;
                    entity.Relationship = relationship;

                }
            }
        }

        private static IEnumerable<DataCollectionParty> GetNewDataCollectionParties(IUserManagementViewModel model)
        {
            var newNonUrdmsUsers = from o in model.NonUrdmsUsers
                                    where o.Id < 1
                                    select new DataCollectionParty
                                    {
                                        Party = new Party { FullName = o.FullName, Id = o.PartyId, Organisation = o.Organisation },
                                        Relationship = (DataCollectionRelationshipType)o.Relationship
                                    };

            var newUrdmsUsers = from o in model.UrdmsUsers
                                 where o.Id < 1
                                 select new DataCollectionParty
                                 {
									 Party = new Party { FullName = o.FullName, Id = o.PartyId, UserId = o.UserId, Organisation = "" },	// TODO: Insert your organisation here
                                     Relationship = (DataCollectionRelationshipType)o.Relationship
                                 };

            return newNonUrdmsUsers.Union(newUrdmsUsers);
        }
        #endregion


    }

}
