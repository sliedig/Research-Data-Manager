using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Urdms.Dmp.Utils
{
    public class AcceptMethodByParameterAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// Name of the HTTP Parameter to accept on
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional value to compare the HTTP Parameter value with to accept on
        /// </summary>
        public string Value { get; set; }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var req = controllerContext.RequestContext.HttpContext.Request;
            return Value == null ? Name.Split(',').Any(req.Form.Keys.OfType<string>().Contains) : Name.Split(',').Any(name => req.Form[name] == Value);
        }
    }
}