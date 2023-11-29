(function ($) {
    function Login() {
        var $this = this, formLogin;

        function initializeForm() {
            formLogin = new Global.FormValidationReset('#form1');
        }

        $this.init = function () {
            initializeForm();
        };
    }

    $(function () {
        var self = new Login();
        self.init();
    });
}(jQuery));