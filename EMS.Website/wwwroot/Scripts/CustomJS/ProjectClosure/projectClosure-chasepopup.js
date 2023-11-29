(function ($) {
    function Chasepopup() {
        var $this = this;

        function InitializeControl() {

            var modal = $("#modal-chase-projectClosure");
            var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: '' }
                }, null, function (result) {                   
                    if (result.isSuccess) {
                        if (result.redirectUrl !== undefined && result.redirectUrl !== null) {
                            window.location.href = result.redirectUrl;
                        } else {
                            if ($.fn.ProjectClosureIndex) {
                                $.fn.ProjectClosureIndex.refreshClosures();
                            }
                            else if ($.fn.HomeIndex) {
                                $.fn.HomeIndex.RefreshPostSales();
                            }
                            modal.modal('hide');
                            Global.ShowMessage(result.message, true, 'MessageDiv');
                        }
                    }
                    else {
                        if (result.data !== undefined && result.data !== null) {
                            if (confirm('You have already scheduled ' + result.data + ' activity on same date. Are you sure you want to continue?')) {
                                $('#IsConfirmSubmit').val(true);
                                form.submit();
                            }
                        } else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                        }
                    }
                });

            modal.off('hidden.bs.modal')
                .on('hidden.bs.modal', function () {
                    modal.removeData('bs.modal');
                    modal.find('.modal-content').empty();
                });

            form.find("#EnagementDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0,
                maxDate: 29
            });

            form.find("#ConversionDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0,
                maxDate: 29
            });

            form.find("#ExpectedNewWork").change(function(e) {
                $("#ConversionDate").toggle();
            });
        }
        
        $this.init = function () {
            InitializeControl();
        };
    }

    $(function () {
        var self = new Chasepopup();
        self.init();
    });

}(jQuery));