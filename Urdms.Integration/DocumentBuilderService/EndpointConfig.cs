using System.Configuration;
using NServiceBus;
//using WebSupergoo.ABCpdf8;

namespace Urdms.DocumentBuilderService
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Init()
        {
			// TODO: We used ABC pdf to generate our documents, you can reuse this or use another library

			//Configure.With(AllAssemblies.Except("ABCpdf8-64.dll"))
			//    .DefaultBuilder()
			//    .XmlSerializer();

			//XSettings.InstallRedistributionLicense(ConfigurationManager.AppSettings["ABCPDFLicenseKey"]);
        }
    }
}
