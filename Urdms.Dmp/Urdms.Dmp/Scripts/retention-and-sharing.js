// Ensure the Urdms.FlowWeb namespace exists
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};

Urdms.FlowWeb.formStart = function () {
    // depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly
    depends("DataSharing_DataSharingDataSharingAvailabilityDate", "DataSharing.DataSharingAvailability", "AfterASpecifiedEmbargoPeriod", false, false, true);
    // UNCOMMENT IF YOU WANT TO LINK SHARE ACCESS TO AVAILABILITY
    // depends("DataSharing_ShareAccessOpen", "DataSharing.DataSharingAvailability", "Never", true, false, true);
    depends("DataSharing_ShareAccessDescription", "DataSharing.ShareAccess", "Other", false, false, true);

    depends("DataRetention_DataRetentionResponsibilitiesDescription", "DataRetention_DataRetentionResponsibilitiesOther", "Other", false, true, true);
    depends("DataRetention_DataRetentionLocationsInternal", "DataRetention.DepositToRepository", "false", true, false, true);
    depends("DataRetention_DataRetentionLocationsDescription", "DataRetention_DataRetentionLocationsOther", "Other", false, true, true);
};
$(function () {
    $("input[id='DataRetention.DepositToRepositoryNo']").click(function () {
        $("input[name='DataRetention.DataRetentionLocations']:checked").attr("checked", false);
        $("#DataRetention_DataRetentionLocationsDescription").val(null).parent().hide();
        $("label[for='DataRetention_DataRetentionLocationsDescription']").parent().hide();
    });
});