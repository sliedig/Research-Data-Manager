using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Curtin.Framework.Common.Auth;
using Curtin.Framework.Common.Extensions;

namespace Urdms.Dmp.Web.Auth
{
    public interface IAuthenticationService
    {
        LogOnResponse LogOn(string userName, string password, bool rememberMe = false);
        void PassiveLogOn(string userName, List<string> roles = null, bool rememberMe = false);
        void LogOff();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IEnumerable<IMembershipService> _membershipServices;
        private readonly IEnumerable<IRoleProvider> _roleProviders;
        private readonly HttpContextBase _httpContextBase;

        public AuthenticationService(IEnumerable<IMembershipService> membershipServices, IEnumerable<IRoleProvider> roleProviders, HttpContextBase httpContextBase)
        {
            _membershipServices = membershipServices;
            _roleProviders = roleProviders;
            _httpContextBase = httpContextBase;
        }

        public LogOnResponse LogOn(string userName, string password, bool rememberMe = false)
        {
            var logOnResponse = new LogOnResponse();
            var roles = new List<string>();
            var result = new MembershipValidationResult();
            foreach (var membershipService in _membershipServices)
            {
                result = membershipService.ValidateUser(userName, password);
                if (result.Success)
                {
                    logOnResponse.IsValid = true;
                    roles.AddRange(result.Roles);
                    userName = result.UserName;
                    break;
                }
                if(!String.IsNullOrEmpty(result.Error))
                    logOnResponse.Errors.Add(result.Error);
            }

            if(logOnResponse.IsValid)
            {
                _roleProviders.ForEach(r => roles.AddRange(r.RolesFor(userName, result.Memberships)));
                SetCookie(userName, rememberMe, roles);
                
                logOnResponse.IsValid = true;
            }

            return logOnResponse;
        }

        public void PassiveLogOn(string userName, List<string> roles = null, bool rememberMe = false)
        {
            if (roles == null)
                roles = new List<string>();

            _roleProviders.ForEach(r => roles.AddRange(r.RolesFor(userName, null)));
            SetCookie(userName, rememberMe, roles);
        }

        private void SetCookie(string userName, bool rememberMe, IEnumerable<string> roles)
        {
            var rolesString = String.Join(",", roles.ToArray());
            if (rolesString.Length > 900)
            {
                throw new Exception(string.Format("A user with role string greater than 941 characters is known to cause issues. Your list of roles is too close for comfort: {0}", rolesString));
            }
            var ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), rememberMe, rolesString, FormsAuthentication.FormsCookiePath);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) {Secure = FormsAuthentication.RequireSSL, Path = FormsAuthentication.FormsCookiePath, Domain = FormsAuthentication.CookieDomain };
            _httpContextBase.Response.Cookies.Add(authCookie);
        }

        public void LogOff()
        {
            FormsAuthentication.SignOut();
        }
    }

    public class LogOnResponse
    {
        public LogOnResponse()
        {
            Errors = new List<string>();
        }
        public bool IsValid { get; set; }
        public IList<string> Errors { get; private set; }
    }
}
