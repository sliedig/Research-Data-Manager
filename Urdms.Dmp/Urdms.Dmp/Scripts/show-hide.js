$(function () {
    $(".moreText").hide();
    $(".showMore").click(function () {
        $(this).prev().fadeToggle();

        if ($(this).text() === "... [show more]") {
            $(this).text("... [show less]");
        }
        else {
            $(this).text("... [show more]");
        }

        $(this).blur();
        return false;
    });
});