(function ($) {
    'use strict';
    function Document() {
        var $this = this;
        function InitializeForm() {

            var form = new Global.FormHelper($("form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: '' }
            });

            form.find(".datepicker").datepicker({
                dateFormat: "dd/mm/yy"
            });

        }

        $this.init = function () {
            InitializeForm();
        }
    }
    $(function () {
        var self = new Document();
        self.init();
    });
}(jQuery));