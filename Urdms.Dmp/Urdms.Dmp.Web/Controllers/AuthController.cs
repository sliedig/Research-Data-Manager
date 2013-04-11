using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Curtin.Framework.Common.Extensions;
using Urdms.Dmp.Web.Auth;

namespace Urdms.Dmp.Web.Controllers
{
    public class AuthController : Controller
    {
        protected readonly HttpContextBase CurrentHttpContext;
        protected readonly IAuthenticationService AuthenticationService;

        public AuthController(HttpContextBase currentHttpContext, IAuthenticationService authenticationService)
        {
            CurrentHttpContext = currentHttpContext;
            AuthenticationService = authenticationService;
        }

        // **************************************
        // URL: /Auth/LogOn
        // **************************************

        public virtual ActionResult LogOn(string returnUrl)
        {
            EndAjaxRequest(returnUrl);

            return View();
        }

        [HttpPost]
        public virtual ActionResult LogOn(LogOnViewModel viewModel, string returnUrl)
        {
            EndAjaxRequest(returnUrl);

            if (!ModelState.IsValid)
                return View(viewModel);

            var logOnResponse = AuthenticationService.LogOn(viewModel.UserName, viewModel.Password);
            if(logOnResponse.IsValid)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToRoute("Homepage");
            }

            if(logOnResponse.Errors.IsNotEmpty())
                logOnResponse.Errors.Do(error=>ModelState.AddModelError("", error));    
            else
                ModelState.AddModelError("", "The user name or password provided is incorrect.");    
            return View(viewModel);
        }

        // **************************************
        // URL: /Auth/LogOff
        // **************************************
        public virtual ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                AuthenticationService.LogOff();
                return RedirectToAction("LogOff");
            }
            return View();
        }

        // **************************************
        // URL: /Auth/Denied
        // **************************************
        public virtual ActionResult Denied()
        {
            return View();
        }

        // **************************************
        // URL: /Auth/SessionExpired
        // **************************************
        public virtual ActionResult SessionExpired()
        {
            return View();
        }

        private void EndAjaxRequest(string returnUrl)
        {
            if (String.IsNullOrEmpty(returnUrl) || !CurrentHttpContext.Request.IsAjaxRequest())
                return;

            CurrentHttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            CurrentHttpContext.Response.End();
        }
    }

    public class LogOnViewModel
    {
        [Required]
        [Display(Name = "User ID")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
