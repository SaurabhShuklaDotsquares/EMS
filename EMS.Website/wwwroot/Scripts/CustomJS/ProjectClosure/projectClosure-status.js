(function ($) {
    function ClosureStatus() {
        var $this = this;

        function InitializeControl() {

            var modal = $("#modal-status-projectClosure");
            var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: '' }
                }, null, function (result) {                   

                    if (result.isSuccess) {

                        if ($.fn.ProjectClosureIndex) {
                            $.fn.ProjectClosureIndex.refreshClosures();
                        }
                        else if ($.fn.ProjectClosureReview) {
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

            form.find("#DeadResponseDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0,
                maxDate: 100
            });

            form.find("#IsPermanentDead").change(function (e) {
                $("#DeadResponseDate").toggle();
            });

            form.find("#ChangeStatusId").change(function (e) {
                if ($("#ChangeStatusId").val() == "3") {
                    $("#DeadResponseDateRow").show();
                } else {
                    $("#DeadResponseDateRow").hide();
                }
            });
        }
        
        $this.init = function () {
            InitializeControl();
        };
    }

    $(function () {
        var self = new ClosureStatus();
        self.init();
    });

}(jQuery));