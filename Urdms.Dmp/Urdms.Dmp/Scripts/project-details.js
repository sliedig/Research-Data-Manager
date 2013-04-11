// Ensure the Urdms.FlowWeb namespace exists
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};

Urdms.FlowWeb.formStart = function () {
    // depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly
    depends("ArcFunder_GrantNumber", "ArcFunder.IsFunded", "true", false, false, true);
    depends("NmhrcFunder_GrantNumber", "NmhrcFunder.IsFunded", "true", false, false, true);
    
    // Submit buttons that should perform client-side validation
    $("#SaveAndNext").click(function () {
        if ($(this).parents("form").validate().form()) {
            return true;
        } else {
           	Urdms.FlowWeb.focusFirstErrorElement();
            return false;
        }
    });
};

Urdms.FlowWeb.Custom.validationFunction = function () {
    return { onsubmit: false };
};

// DOM ready
$(function () {
    // FoR autocomplete
    $("#FieldOfResearchCode").autocomplete({
        source: "/Ajax/GetForList"
    });

    // SEO autocomplete
    $("#SocioEconomicObjectiveCode").autocomplete({
        source: "/Ajax/GetSeoList"
    });
});