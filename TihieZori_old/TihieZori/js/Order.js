//Date.prototype.getTimezoneOffset = function () { return 0; };

$(function () {
    $("#btnCancelOrder").click(function () {
        $.getJSON('cust/ordercancel.dbo'
            , function (data) {
                $("#orderrow").hide();
                $("#cancelrow").show();
            });
    })
})



