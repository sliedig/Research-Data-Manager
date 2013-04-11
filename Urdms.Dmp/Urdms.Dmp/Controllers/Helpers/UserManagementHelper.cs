using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Interfaces;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers.Helpers
{
    public static class UserManagementHelper
    {
        public static void AddUrdmsUser<T>(this IList<UrdmsUserViewModel> urdmsUsers, ICurtinUser user, T role)
        {
            if (user != null && !string.IsNullOrWhiteSpace(user.FullName) && !urdmsUsers.Any(o => o.UserId == user.CurtinId))
            {
                urdmsUsers.Add(new UrdmsUserViewModel
                {
                    FullName = user.FullName,
                    UserId = user.CurtinId,
                    Relationship = (int)Convert.ChangeType(role, typeof(int))
                });

            }
        }

        public static void AddNonUrdmsUser<T>(this IList<NonUrdmsUserViewModel> nonUrdmsUsers, string fullName, T role)
        {
            if (!string.IsNullOrWhiteSpace(fullName) && !nonUrdmsUsers.Any(o => o.FullName == fullName))
            {
                nonUrdmsUsers.Add(new NonUrdmsUserViewModel
                {
                    FullName = fullName,
                    Relationship = (int)Convert.ChangeType(role, typeof(int))
                });

            }
        }

        public static void DeserializeUrdmsUsers<T>(this IUserManagementViewModel model, NameValueCollection formParams)
        {
            model.UrdmsUsers = new List<UrdmsUserViewModel>();
			var rows = formParams.GetValues("urdms.users.row");
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var userParts = row.Split(',');
                    var id = int.Parse(userParts[0]);
                    var isDelete = id == 0 ? formParams["RemoveUrdmsUser" + userParts[1]] : formParams["RemoveUrdmsUser" + id];
                    var relationship = (int)(object)(id == 0
                                           ? formParams["UrdmsUserRelationship" + userParts[1]]
										   : formParams["UrdmsUserRelationship" + id]).GetEnumValue<T>();

					var isRemoveOperation = !isDelete.Contains("true") && formParams["DeleteUrdmsUser"] != null;
                    var isAddOperation = formParams["AddUrdmsUser"] != null;
					var isSubmitOperation = formParams["DeleteUrdmsUser"] == null && formParams["AddUrdmsUser"] == null;

                    if (isRemoveOperation || isAddOperation || isSubmitOperation)
                    {
                        var user = new UrdmsUserViewModel
                                       {
                                           Id = id,
                                           FullName = userParts[2],
                                           UserId = userParts[3],
                                           Relationship = relationship
                                       };
                        model.UrdmsUsers.Add(user);
                    }
               }
            }
        }

        public static void DeserializeNonUrdmsUsers<T>(this IUserManagementViewModel model, NameValueCollection formParams)
        {
            model.NonUrdmsUsers = new List<NonUrdmsUserViewModel>();
			var rows = formParams.GetValues("nonurdms.users.row");	
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var userParts = row.Split(',');
                    var id = int.Parse(userParts[0]);
					var isDelete = id == 0 ? formParams["RemoveNonUrdmsUser" + userParts[1]] : formParams["RemoveNonUrdmsUser" + id];
                    var relationship = (int)(object)(id == 0
										   ? formParams["NonUrdmsUserRelationship" + userParts[1]]
										   : formParams["NonUrdmsUserRelationship" + id]).GetEnumValue<T>();

                    var isRemoveOperation = !isDelete.Contains("true") && formParams["DeleteNonUrdmsUser"] != null;
                    var isAddOperation = formParams["AddNonUrdmsUser"] != null;
                    var isSubmitOperation = formParams["DeleteNonUrdmsUser"] == null && formParams["AddNonUrdmsUser"] == null;

                    if (isRemoveOperation || isAddOperation || isSubmitOperation)
                    {
                        var user = new NonUrdmsUserViewModel
                                       {
                                           Id = id,
                                           FullName = userParts[2],
                                           Relationship = relationship
                                       };
                        model.NonUrdmsUsers.Add(user);
                    }
                }
            }
        }

        
    }
}
