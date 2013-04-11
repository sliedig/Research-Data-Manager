using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class BaseController : Controller
    {
        private readonly ICurtinUserService _lookupService;

        public BaseController(ICurtinUserService lookupService)
        {
            _lookupService = lookupService;
        }

        public ICurtinUser CurrentUser
        {
            get
            {
                if (!ControllerContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    return null;
                }
                var user = _lookupService.GetUser(ControllerContext.HttpContext.User.Identity.Name);
                return user;
            }
        }

        public ICurtinUser GetUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }
            var urdmsUser = _lookupService.GetUser(userId);
            if(urdmsUser == null || urdmsUser.CurtinId == null)
            {
                return null;
            }

            return urdmsUser;
        }

        public void UpdateUrdmsPartyDetails(IEnumerable<Party> parties)
        {
            var urdmsParties = parties.Where(o => !string.IsNullOrWhiteSpace(o.UserId)).ToList();
            foreach (var urdmsParty in urdmsParties)
            {
                var user = _lookupService.GetUser(urdmsParty.UserId);
                if (user != null)
                {
                    // get information from active directory
                    urdmsParty.FirstName = user.FirstName;
                    urdmsParty.LastName = user.LastName;
                    urdmsParty.FullName = user.FullName;
                    urdmsParty.Email = user.EmailAddress;
                    urdmsParty.Organisation = "";	// TODO: Insert your organisation here
                }
                else
                {
                    // change the party to non URDMS party
                    // since the user no longer exists in AD
                    urdmsParty.UserId = "";
                    urdmsParty.Organisation = "";
                }
            }
        }

    }
}
