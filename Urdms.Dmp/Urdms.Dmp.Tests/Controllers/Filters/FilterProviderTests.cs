using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using Curtin.Framework.Common.Extensions;
using Urdms.Dmp.Web.Controllers.Filters;
using NSubstitute;
using NUnit.Framework;
using Subtext.TestLibrary;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Controllers.Filters
{
    [TestFixture]
    class FilterProviderShould
    {
        #region Setup
        private FilterProvider _filterProvider;
        private HttpSimulator _httpSimulator;
        private ControllerContext _context;

        [SetUp]
        public void Setup()
        {
            var autoSubstitute = AutoSubstituteContainer.Create();
            _context = autoSubstitute.Resolve<ControllerContext>();
            _filterProvider = new FilterProvider();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(autoSubstitute.Container));
            _httpSimulator = new HttpSimulator().SimulateRequest();
        }

        [TearDown]
        public void Teardown()
        {
            _httpSimulator.Dispose();
        }

        private IEnumerable<Filter> Act()
        {
            var filters = new List<Filter>();
            _filterProvider.GetFilters(_context, Substitute.For<ActionDescriptor>())
                .Do(filters.Add);
            return filters;
        }
        #endregion

        [Test]
        public void Produce_correct_ordering_for_filters()
        {
            var filters = Act().OrderBy(f => f.Scope).OrderBy(f => f.Order).ToList();
            var index = 0;
#if !DEBUG
            Assert.That(filters.Count, Is.EqualTo(2));
            Assert.That(filters[index++].Instance, Is.TypeOf<RedirectToHttpsAttribute>());
#else
            Assert.That(filters.Count, Is.EqualTo(1));
#endif
            Assert.That(filters[index++].Instance, Is.TypeOf<RequiresAllRoles>());
        }
    }
}
