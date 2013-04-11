using System.Reflection;
using System.Web.Mvc;

namespace Urdms.Dmp.Utils
{
    public class DenyMethodByParameterAttribute : AcceptMethodByParameterAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return !(base.IsValidForRequest(controllerContext, methodInfo));
        }
    }
}