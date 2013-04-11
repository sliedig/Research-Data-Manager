using System.Net;
using System.Web.Mvc;

namespace Urdms.Dmp.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // /Error/Unknown
        public ActionResult Unknown()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View();
        }

        //
        // /Error/NotFound
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            // TODO: Rewrite URL to get rid of the nasty aspxerrorpath
            return View();
        }

    }
}
