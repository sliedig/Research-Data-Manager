using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Urdms.Dmp.Web.Controllers.ActionResults;

namespace Urdms.Dmp.Web.Controllers.Filters
{
    /// <summary>
    /// Checks the User's authentication using FormsAuthentication
    /// and redirects to the Login Url for the application on fail
    /// </summary>
    public class RequiresAuthenticationAttribute : ActionFilterAttribute
    {
        private readonly bool _useRewrite;
        private readonly bool _useReturnUrl;

        public RequiresAuthenticationAttribute(bool useRewrite = true, bool useReturnUrl = true)
        {
            _useRewrite = useRewrite;
            _useReturnUrl = useReturnUrl;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Redirect if not authenticated
            var controller = filterContext.RouteData.Values["controller"].ToString();

            if (!filterContext.IsChildAction && !filterContext.HttpContext.User.Identity.IsAuthenticated && controller != "Auth" && controller != "Error")
            {
                // Use the current url for the redirect
                string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;

                // Send them off to the login page
                var loginUrl = _useReturnUrl
                    ? string.Format("~/Auth/LogOn?ReturnUrl={0}", redirectOnSuccess)
                    : "~/Auth/LogOn";

                if (_useRewrite)
                {
                    filterContext.Result = new RewriteResult(loginUrl);
                }
                else
                {
                    filterContext.Result = new RedirectResult(loginUrl);
                }
            }
        }
    }

    public abstract class RequiresRoleBaseAttribute : ActionFilterAttribute
    {
        private readonly bool _useRewrite;
        private readonly bool _useReturnUrl;

        protected RequiresRoleBaseAttribute(bool useRewrite, bool useReturnUrl)
        {
            _useRewrite = useRewrite;
            _useReturnUrl = useReturnUrl;
        }

        public override void  OnActionExecuting(ActionExecutingContext filterContext)
        {
            var requiresAuth = new RequiresAuthenticationAttribute(_useRewrite, _useReturnUrl);
            requiresAuth.OnActionExecuting(filterContext);
            if (filterContext.Result !=null)
                return;

            var user = filterContext.HttpContext.User;
            var controller = filterContext.RouteData.Values["controller"].ToString();

            if (!filterContext.IsChildAction && !Validate(user) && controller != "Auth" && controller != "Error")
            {
                filterContext.Result = new RewriteResult("~/Auth/Denied");
            }
        }

        protected abstract bool Validate(IPrincipal user);
    }

    public class RequiresAllRoles : RequiresRoleBaseAttribute
    {
        private readonly string[] _roles;

        public RequiresAllRoles(bool useRewrite, bool useReturnUrl, params string[] roles) : base(useRewrite, useReturnUrl)
        {
            _roles = roles;
        }

        public RequiresAllRoles(params string[] roles) : base(true, true)
        {
            _roles = roles;
        }

        protected override bool Validate(IPrincipal user)
        {
            return _roles.All(user.IsInRole);
        }
    }

    public class RequiresAnyRole : RequiresRoleBaseAttribute
    {
        private readonly string[] _roles;

        public RequiresAnyRole(bool useRewrite, bool useReturnUrl, params string[] roles) : base(useRewrite, useReturnUrl)
        {
            _roles = roles;
        }

        public RequiresAnyRole(params string[] roles) : base(true, true)
        {
            _roles = roles;
        }

        protected override bool Validate(IPrincipal user)
        {
            return _roles.Any(user.IsInRole);
        }
    }
}