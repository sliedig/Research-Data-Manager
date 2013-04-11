// Ensure the Urdms.FlowWeb namespace exists
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};

Urdms.FlowWeb.formStart = function () {
    // depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly
    depends("Ethic_EthicComments", "Ethic.EthicRequiresClearance", "false", true, false, true);
    depends("Confidentiality_ConfidentialityComments", "Confidentiality.IsSensitive", "false", true, false, true);
};