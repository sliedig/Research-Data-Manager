using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.SharePoint.Client;
using Urdms.ProvisioningService.Messages;
using log4net;

namespace Urdms.ProvisioningService.SharePoint.Helpers
{
    public class PollingResult
    {
        public ProvisioningRequestStatus ProvisioningRequestStatus { get; set; }
        public string SiteUrl { get; set; }
    }


    public interface ISiteCreationRequestListPoller
    {
        PollingResult Poll();
    }

    public class SiteCreationRequestListPoller : ISiteCreationRequestListPoller
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SharePointHelper _sharePoint;


        private List _siteRequestList;
        private const double PollingPeriod = 300;
        private DateTime _pollingEndTime;
        private readonly int _requestId;
        private PollingResult _pollResult;

        public SiteCreationRequestListPoller(SharePointHelper sharePoint, int requestId, string listName)
        {
            _sharePoint = sharePoint;

            Debug.Assert(requestId > 0);
            _requestId = requestId;

            _siteRequestList = _sharePoint.RootWeb.Lists.GetByTitle(listName);
            _sharePoint.ClientContext.Load(_siteRequestList);
            _sharePoint.ClientContext.ExecuteQuery();

            _pollResult = new PollingResult { ProvisioningRequestStatus = ProvisioningRequestStatus.Pending };
        }


        public PollingResult Poll()
        {
            _pollingEndTime = DateTime.Now.AddSeconds(PollingPeriod);

            do
            {
                PollListItem();
            }
            while ((DateTime.Now <= _pollingEndTime) && _pollResult.ProvisioningRequestStatus == ProvisioningRequestStatus.Pending);


            if (_pollResult.ProvisioningRequestStatus == ProvisioningRequestStatus.Pending)
            {
                _pollResult.ProvisioningRequestStatus = ProvisioningRequestStatus.TimeOut;
            }

            return _pollResult;
        }


        private void PollListItem()
        {
            var refListItem = _siteRequestList.GetItemById(_requestId);
            _sharePoint.ClientContext.Load(refListItem);
            _sharePoint.ClientContext.ExecuteQuery();

            var status = refListItem["UrdmsSiteProvisioningRequestStatus"].ToString();

            switch (status)
            {
                case "Provisioned":
                    Log.InfoFormat("[URDMS] Site for Request ID {0} has been provisioned.", _requestId);
                    Debug.Assert(!string.IsNullOrWhiteSpace(refListItem["UrdmsSiteUrl"].ToString()));
                    _pollResult.SiteUrl = refListItem["UrdmsSiteUrl"].ToString();
                    _pollResult.ProvisioningRequestStatus = ProvisioningRequestStatus.Provisioned;

                    if (string.IsNullOrWhiteSpace(_pollResult.SiteUrl))
                    {
                        Log.WarnFormat("[URDMS] Site for Request ID {0} has been provisioned but Site URL has not been set. Setting Provisioning Request Status to Error.", _requestId);
                        _pollResult.ProvisioningRequestStatus = ProvisioningRequestStatus.Error;
                    }
                    
                    break;

                case "Error":
                    _pollResult.SiteUrl = null;
                    Debug.Assert(!string.IsNullOrWhiteSpace(refListItem["UrdmsErrorMessage"].ToString()));
                    _pollResult.ProvisioningRequestStatus = ProvisioningRequestStatus.Error;

                    break;

                default:
                    Log.InfoFormat("[URDMS] Provisioning for Request ID {0} is still pending", _requestId);
                    Debug.Assert(_pollResult.ProvisioningRequestStatus == ProvisioningRequestStatus.Pending);
                    break;
            }

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}
