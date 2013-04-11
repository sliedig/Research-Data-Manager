$(function () {
   
    $("#NextButton").click(function() {
        
        $.ajax({
                url: "/Ajax/SaveTime/",
                data: {
                    term: el.val()
                },
                type: "POST",
                error: function() {
                }
            });
    });
}