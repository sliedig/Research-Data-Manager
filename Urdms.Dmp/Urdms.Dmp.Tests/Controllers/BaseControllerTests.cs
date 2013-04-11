using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.UserService;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Integration.UserService;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Controllers
{

    [TestFixture]
    class BaseControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private BaseController _controller;
        private ICurtinUserService _lookup;
        
        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _controller = _autoSubstitute.GetController<BaseController>();
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
        }

        [Test]
        public void Return_a_list_of_roles_for_a_valid_Urdms_User_Id()
        {
            const string validUserId = "GA37493";
            _lookup.GetUser(validUserId)
                .Returns(new UrdmsUser{FullName = "Joe Research", EmailAddress = "j.research@domain.edu.au", CurtinId = validUserId});

            var user = _controller.GetUser(validUserId);

            Assert.That(user, Is.Not.Null, "User is null");
        }
    }
}
