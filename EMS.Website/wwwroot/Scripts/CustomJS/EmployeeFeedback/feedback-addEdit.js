(function ($) {
    function index() {
        var $this = this, formAddEdit;
        function initializeForm() {
            formAddEdit = new Global.FormHelper($("#frm-create-edit-employeefeedback form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: '' }
            });
        }

        function initializeEvent() {
            $(".disabled-radio").on('click', function () {
                return false;
            });

            $(".reasoncheckbox").on('change', function () {
                if ($(this).prop('checked') == true) {
                    if ($(this).attr("data-attr") == "Other") {
                        $("#comment").attr("required", true);
                        $("#commentbox").show();
                    }
                }
                else {
                    if ($(this).attr("data-attr") == "Other") {
                        $("#comment").attr("required", false);
                        $("#commentbox").hide();
                    }
                }
            });
            //$("#leavingDate").datepicker({
            //    defaultDate: "+1w",
            //    dateFormat: "dd/mm/yy",
            //    changeMonth: true,
            //    changeYear: true,
            //    numberOfMonths: 1,
            //    minDate: 0
            //});

            $('.reasoncheckbox').click(function () {
                var checked_boxes = $('input.reasoncheckbox:checked').length;

                if (checked_boxes > 3) {
                    $(event.target).prop("checked", false);
                }
            });
        }

        $this.init = function () {
            initializeForm();
            initializeEvent();
            $(".reasoncheckbox").trigger('change');
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));