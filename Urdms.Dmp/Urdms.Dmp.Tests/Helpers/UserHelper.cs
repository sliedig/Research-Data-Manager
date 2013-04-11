using System.Web;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.Extensions;
using NSubstitute;

namespace Urdms.Dmp.Tests.Helpers
{
    public class UserIs
    {
        public static void AuthenticatedAs(AutoSubstitute autoSubstitute, string username, params string[] roles)
        {
            var principal = autoSubstitute.Resolve<HttpContextBase>().User;
            principal.Identity.Name.Returns(username);
            principal.Identity.IsAuthenticated.Returns(true);
            roles.Do(r => principal.IsInRole(r).Returns(true));
        }

        public static void Unauthenticated(AutoSubstitute autoSubstitute)
        {
            var principal = autoSubstitute.Resolve<HttpContextBase>().User;
            principal.Identity.Name.Returns(string.Empty);
            principal.Identity.IsAuthenticated.Returns(false);
            principal.IsInRole(Arg.Any<string>()).ReturnsForAnyArgs(false);
        }
    }
}
