using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.UserService;
using FizzWare.NBuilder;
using NSubstitute;
using NUnit.Framework;
using Subtext.TestLibrary;
using Urdms.Dmp.Controllers.ModelBinders;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Integration.UserService;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Controllers.ModelBinders
{
    [TestFixture]
    class CreateProjectViewModelBinderShould
    {
        private AutoSubstitute _autoSubstitute;
        private ControllerContext _context;
        private ICurtinUserService _lookup;
        private HttpSimulator _httpSimulator;

        [SetUp]
        public void Setup()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _context = _autoSubstitute.Resolve<ControllerContext>();
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_autoSubstitute.Container));
            _httpSimulator = new HttpSimulator().SimulateRequest();
        }

        [TearDown]
        public void TearDown()
        {
            _httpSimulator.Dispose();
        }

        private void CreateUser(string userId, string role = null)
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = userId).Build();
            if (!string.IsNullOrEmpty(role))
            {
                UserIs.AuthenticatedAs(_autoSubstitute, userId, new[] { role });
            }
            _lookup.GetUser(Arg.Is(userId)).Returns(user);
        }
        
        [Test]
        public void Return_null_if_user_is_not_authenticated()
        {
            CreateUser("123456A");

            var bindingContext = new ModelBindingContext();
            var modelBinder = new CreateProjectViewModelBinder();

            var result = modelBinder.BindModel(_context, bindingContext);
            Assert.That(result, Is.Null);
        }
        
    }
}
