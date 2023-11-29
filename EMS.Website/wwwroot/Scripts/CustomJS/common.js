(function () {

    function Common() {
        var $this = this;
        function initialize() {
            setInterval(function () {
                if ($('body').attr('logoutMessageDisplayed') == undefined || $('body').attr('logoutMessageDisplayed') == false) {
                    $.post(domain + "ajax/isLoggedIn", function (result) {
                        if ((result == undefined || result == 0)) {
                            $('body').attr('logoutMessageDisplayed', true);
                            $.confirm({
                                title: 'Alert!',
                                btnClass: 'btn-blue',
                                content: "You've been logged out, please login again.",
                                buttons: {
                                    Ok: function () {
                                        window.location.href = domain + "?returnurl=" + window.location.href;
                                    }
                                }
                            });
                        }
                        else {
                            $('body').removeAttr('logoutMessageDisplayed');
                        }
                    });
                }
            }, 600000);
        }

        $this.init = function () {
            initialize();
        }
    }

    $(function () {
        var self = new Common;
        self.init();
    });
}(jQuery));