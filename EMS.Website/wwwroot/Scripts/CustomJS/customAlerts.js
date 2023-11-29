var CustomAlerts = {
    alertdiv: $("#divMessage"),
    error: function (title, description) {
        $('html, body').animate({ scrollTop: 0 }, '1000');
        $(CustomAlerts.alertdiv).show();
        $(CustomAlerts.alertdiv).removeClass();
        //$(CustomAlerts.alertdiv).addClass("errormsg message");
        $(CustomAlerts.alertdiv).addClass("alert alert-danger");
        $("#lblMessageheading").text(title);
        $("#lblMessaggedescription").text(description);
    },
    success: function (title, description) {
        $('html, body').animate({ scrollTop: 0 }, '1000');
        $(CustomAlerts.alertdiv).show();
        $(CustomAlerts.alertdiv).removeClass();
        //$(CustomAlerts.alertdiv).addClass("success message");
        $(CustomAlerts.alertdiv).addClass("alert alert-success");
        $("#lblMessageheading").text(title);
        $("#lblMessaggedescription").text(description);
    },
    info: function (title, description) {
        $('html, body').animate({ scrollTop: 0 }, '1000');
        $(CustomAlerts.alertdiv).show();
        $(CustomAlerts.alertdiv).removeClass();
        //$(CustomAlerts.alertdiv).addClass("info message");
        $(CustomAlerts.alertdiv).addClass("alert alert-info");
        $("#lblMessageheading").text(title);
        $("#lblMessaggedescription").text(description);
    },
    warning: function (title, description) {
        $('html, body').animate({ scrollTop: 0 }, '1000');
        $(CustomAlerts.alertdiv).show();
        $(CustomAlerts.alertdiv).removeClass();
        $(CustomAlerts.alertdiv).addClass("alert alert-warning");
        //$(CustomAlerts.alertdiv).addClass("warning message");
        $("#lblMessageheading").text(title);
        $("#lblMessaggedescription").text(description);
    }

};
(function ($) {
    $(document).delegate("#divMessage .close", "click", function () {
        $("#divMessage").hide();
    });
    $(window).click(function () {
        if ($('#divMessage').css('display') == 'block') {
            $("#divMessage").fadeOut('800');
        }
    });
    $('#closealert').click(function () {
        if ($('#divMessage').css('display') == 'block') {
            $("#divMessage").fadeOut('800');
        }
    });
}(jQuery));