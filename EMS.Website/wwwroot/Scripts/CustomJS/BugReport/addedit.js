(function ($) {
    'use strict';
    function BugReport() {
        var $this = this;
        function InitializeForm() {
            var form = new Global.FormHelperWithFiles($("form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: '' }
            }, function (result) {
                if (result.isSuccess) {
                    Global.ShowMessage(result.message, true, 'validation-summary');
                }
                else {
                    Global.ShowMessage(result.message, false, 'validation-summary');
                }               
            });
        }

        $this.init = function () {
            InitializeForm();
        }
    }
    $(function () {
        var self = new BugReport();
        self.init();
    });
}(jQuery));