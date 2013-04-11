using System.Text;
using System.Web.Mvc;
using Elmah.Contrib.Mvc;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class LibGuideController : Controller
    {
        private readonly ILibGuideService _libGuideService;

        public LibGuideController(ILibGuideService libGuideService)
        {
            _libGuideService = libGuideService;
        }

        [OutputCache(Duration = 3600, VaryByParam = "*" )]
        [ChildActionOnly]
        public ContentResult Index(int account, int item)
        {
            var doc = this._libGuideService.GetInstructions(account, item);
            return Content(doc,"text/html",Encoding.UTF8);
        }

    }
}
