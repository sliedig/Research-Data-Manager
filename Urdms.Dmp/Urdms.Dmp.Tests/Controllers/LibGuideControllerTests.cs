using AutofacContrib.NSubstitute;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    class LibGuideControllerAndLibGuideServiceShould
    {
        private AutoSubstitute _autoSubstitute;
        private LibGuideController _controller;
        private IAppSettingsService _appSettingsService;
        private ISimpleWebRequestService _simpleWebRequestService;
        private int _account;
        private int _item;

        [SetUp]
        public void SetUp()
        {
            const string payload = @"<link href='global.css' type='text/css' /><div>Hello World</div>";
            _account = 1470;
            _item = 7418569;
            _autoSubstitute = AutoSubstituteContainer.Create();
            _appSettingsService = _autoSubstitute.Resolve<IAppSettingsService>();
            _simpleWebRequestService = _autoSubstitute.Resolve<ISimpleWebRequestService>();
            _simpleWebRequestService.GetResponseText(Arg.Any<string>()).Returns(payload);
            var libGuideService = new LibGuideService(_simpleWebRequestService, _appSettingsService);
            _autoSubstitute.Provide<ILibGuideService>(libGuideService);
            _controller = _autoSubstitute.Resolve<LibGuideController>();
        }

        [Test]
        // Note: This is actually an integration test between the LibGuideController and the LibGuideService
        public void Render_server_libguide_item()
        {
            var expectedText = string.Format("<div id='api_box_iid{0}_bid{1}'><div>Hello World</div></div>", _account, _item);
            _appSettingsService.LibGuideSource.Returns(LibGuideSource.Server);

            _controller.WithCallTo(c => c.Index(_account, _item)).ShouldRenderContentResult(expectedText);
            _simpleWebRequestService.Received().GetResponseText(Arg.Any<string>());
        }

        [Test]
        // Note: This is actually an integration test between the LibGuideController and the LibGuideService
        public void Render_client_libguide_item()
        {
            _appSettingsService.LibGuideSource.Returns(LibGuideSource.Client);
            var expectedText = string.Format(@"<div id='api_box_iid{0}_bid{1}'></div><script type='text/javascript' src='http://api.libguides.com/api_box.php?iid={0}&bid={1}&context=object&format=js'></script>", _account, _item);

            _controller.WithCallTo(c => c.Index(_account, _item)).ShouldRenderContentResult(expectedText);
            _simpleWebRequestService.DidNotReceive().GetResponseText(Arg.Any<string>());
        }
    }
}
