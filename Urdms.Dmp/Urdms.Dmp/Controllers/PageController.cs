using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class PageController : BaseController
    {
        public PageController(ICurtinUserService lookupService)
            : base(lookupService)
        {
            
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Instructions()
        {
            return View();
        }
    }
}
