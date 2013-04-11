using AutofacContrib.NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    class PageControllerShould
    {
        private PageController _controller;
        private AutoSubstitute _autoSubstitute;

        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _controller = _autoSubstitute.GetController<PageController>();
        }

        [Test]
        public void Render_default_view_for_get_to_index()
        {
            _controller.WithCallTo(c => c.Index()).ShouldRenderDefaultView();
        }
    }
}
