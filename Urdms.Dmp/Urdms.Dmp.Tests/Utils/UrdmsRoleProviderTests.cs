using System.Linq;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Utils
{
    [TestFixture]
    internal class UrdmsRoleProviderShould
    {
        private AutoSubstitute _autoSubstitute;

        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            
            var appSettings = _autoSubstitute.Resolve<IAppSettingsService>();
            appSettings.LdapUri.Returns(@"LDAP://...");
            appSettings.LdapUser.Returns(@"query");
            appSettings.LdapPassword.Returns("abcdef");

            var directoryEntry = _autoSubstitute.Resolve<IDirectoryEntryService>();
            directoryEntry.GetGroupMembership(Arg.Any<string>(),Arg.Any<string>(),Arg.Any<string>(),Arg.Any<string>())
				.Returns(new[] { "Secondary-Approver" });

            var dependencyResolver = _autoSubstitute.Resolve<IDependencyResolver>();
            DependencyResolver.SetResolver(dependencyResolver);
            dependencyResolver.GetService<IAppSettingsService>().Returns(appSettings);
            dependencyResolver.GetService<IDirectoryEntryService>().Returns(directoryEntry);

        }

        [Test]
        public void Get_role_assigned_to_user_with_matching_group()
        {
			//We know that user XH38485 has group membership 'Secondary-Approver'
			const string userId = "XH38485";

            var roleProvider = new UrdmsRoleProvider();

            var roles = roleProvider.RolesFor(userId).ToList();
            Assert.That(roles.Count, Is.EqualTo(1), "Roles not assigned");
			Assert.That(roles.First(), Is.EqualTo("Secondary-Approver"), "Role Secondary-Approver was not assigned to user");
        }
    }
}
