(function ($) {
    function AddEditReview() {
        var $this = this;

        function InitializeControl() {

            var modal = $("#modal-review-projectClosure");
            var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: '' }
                }, null, function (result) {                   

                    if (result.isSuccess) {

                        if ($.fn.ProjectClosureReview) {
                            $.fn.ProjectClosureReview.refreshReviews();
                        }
                        else if ($.fn.ProjectClosureReport) {
                            $.fn.ProjectClosureReport.refreshClosures();
                        }
                        
                        modal.modal('hide');
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }

                });

            modal.off('hidden.bs.modal')
                .on('hidden.bs.modal', function () {
                    modal.removeData('bs.modal');
                    modal.find('.modal-content').empty();
                });

            form.find("#NextStartDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0,
                maxDate: "+29",
                onSelect: function (dateText) {
                    SetPromising();
                }
            });

            form.find("#clrNextStartDate").on("click", function () {
                form.find("#NextStartDate").val("");
                SetPromising();
            });

            function SetPromising() {
                if (form.find("#NextStartDate").val() !="") {
                    form.find("#divPromisingPercentage").show().find(":input").prop("disabled", false);
                    form.find("#divDeveloperCount").show().find(":input").prop("disabled", false);
                }
                else {
                    form.find("#divPromisingPercentage").hide().find(":input").prop("disabled", true).removeClass("error");
                    form.find("#divDeveloperCount").hide().find(":input").prop("disabled", true).removeClass("error");
                }
            }

            SetPromising();
        }
        
        $this.init = function () {
            InitializeControl();
        };
    }

    $(function () {
        var self = new AddEditReview();
        self.init();
    });

}(jQuery));