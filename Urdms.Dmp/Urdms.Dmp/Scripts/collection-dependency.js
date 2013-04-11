// Ensure the Urdms.FlowWeb namespace exists
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};

Urdms.FlowWeb.formStart = function () {
    // depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly
    depends("AvailabilityDate", "Availability", "AfterASpecifiedEmbargoPeriod", false, false, true);
    depends("ShareAccessDescription", "ShareAccess", "Other", false, false, true);
    depends("AwareOfEthicsNo", "Availability", "Never", true, false, true);
    depends("EthicsApprovalNumber", "AwareOfEthics", "true", false, false, true);
};