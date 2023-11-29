(function ($) {
    'use strict';
    function Complaint() {
        var $this = this;
        function InitializeForm() {

            var form = new Global.FormHelper($("form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: '' }
            });

            form.find(".datepicker").datepicker({
                dateFormat: "dd/mm/yy"
            });

            form.find("input[type='radio'][name='ComplaintTypeId']").change(function () {
                if (this.value == "2") {
                    form.find("#ClientComplaintdiv").removeClass("hidden").find(":input").prop("disabled", false);
                }
                else {
                    form.find("#ClientComplaintdiv").addClass("hidden").find(":input").prop("disabled", true);
                }
            });

            form.find("input[type='radio'][name='ComplaintTypeId']:checked").change();
        }

        $(".multiple").select2({
            placeholder: "-- Select Employees --",
            allowClear: true,
            multiple: true,
            width: '100%'
        });

        $this.init = function () {
            InitializeForm();
        }
    }
    $(function () {
        var self = new Complaint();
        self.init();
    });
}(jQuery));