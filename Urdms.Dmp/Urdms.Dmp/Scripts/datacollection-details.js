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