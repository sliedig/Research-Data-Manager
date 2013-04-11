// Ensure the Urdms.FlowWeb namespace exists
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};

Urdms.FlowWeb.formStart = function () {
    // depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly
    depends("AvailabilityDate", "Availability", "AfterASpecifiedEmbargoPeriod", false, false, true);
    // UNCOMMENT IF YOU WANT TO LINK SHARE ACCESS TO AVAILABILITY
    // depends("ShareAccessOpen", "Availability", "Never", true, false, true);
    depends("LocationsDescription", "LocationsOther", "Other", false, true, true);
    depends("ShareAccessDescription", "ShareAccess", "Other", false, false, true);
};
$(function () {
    $("input[id='AvailabilityNever']").click(function () {
        $("input[name=ShareAccess]:checked").attr("checked", false);
        $("#ShareAccessDescription").val(null).parent().hide();
        $("label[for='ShareAccessDescription']").parent().hide();
    });
    $("input[id='AvailabilityAfterASpecifiedEmbargoPeriod']").click(function() {
        $("input[name=ShareAccess]:checked").attr("checked", false);
    });
});