(function ($) {
    function ChangePassword() {
        var $this = this, formChangePassword;

        function initializeForm() {
            formChangePassword = new Global.FormValidationReset('#form1');
        }

        $this.init = function () {
            initializeForm();
        };
    }

    $(function () {
        var self = new ChangePassword();
        self.init();
    });
}(jQuery));