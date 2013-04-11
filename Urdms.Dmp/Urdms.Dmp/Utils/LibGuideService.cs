using System.Text.RegularExpressions;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Utils
{
    public interface ILibGuideService
    {
        string GetInstructions(int account, int item);
    }

    public class LibGuideService : ILibGuideService
    {
        private readonly ISimpleWebRequestService _simpleWebRequestService;
        private readonly IAppSettingsService _appSettingsService;

        public LibGuideService(ISimpleWebRequestService simpleWebRequestService, IAppSettingsService appSettingsService)
        {
            _simpleWebRequestService = simpleWebRequestService;
            _appSettingsService = appSettingsService;
        }

        private const string UrlFormat = @"http://api.libguides.com/api_box.php?iid={0}&bid={1}&context=object&format=xml";
        private const string ClientSideFormat = @"<div id='api_box_iid{0}_bid{1}'></div><script type='text/javascript' src='http://api.libguides.com/api_box.php?iid={0}&bid={1}&context=object&format=js'></script>";
        private const string LinkPattern = @"<link\b[^>]*>";

        public string GetInstructions(int account, int item)
        {
            if (_appSettingsService.LibGuideSource == LibGuideSource.Client)
            {
               var result = string.Format(ClientSideFormat, account, item);
                return result;
            }
            var url = string.Format(UrlFormat, account, item);
            var responseText = _simpleWebRequestService.GetResponseText(url);
            if (!string.IsNullOrWhiteSpace(responseText))
            {
                responseText = Regex.Replace(responseText, LinkPattern, "", RegexOptions.IgnoreCase);
            }
            var instructions = string.Format(@"<div id='api_box_iid{0}_bid{1}'>{2}</div>", account, item, responseText);
            return instructions;
        }
    }
}