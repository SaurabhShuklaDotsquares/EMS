(function ($) {
    function LeadStatus() {
        var $this = this, form, modal;

        function InitForm() {

            var modal = $("#modal-leadStatus");

            var newdate = new Date();
            modal.find("#NextChaseDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "100:+1",
                minDate: 0,
                maxDate: 14
            });

            modal.find("#ConversionDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                minDate: 0,
                maxDate: 100
            });

            modal.off("change", "#Status").on("change", "#Status", function () {
                if (this.value === "21") {
                    $("#divNextChanseDate").hide().find(":input").prop("disabled", true);
                }
                else {
                    $("#divNextChanseDate").show().find(":input").prop("disabled", false);
                }
            });

            CKEDITOR.replace('Notes', { toolbar: "Basic" });
            attachEventCKEditor('Notes');            

            form = new Global.FormHelperWithFiles(modal.find('form'), {
                updateTargetId: "error-ModalMessage", validateSettings: { ignore: '' }
            }, function (result) {

                if (result.isSuccess) {
                    
                    if ($.fn.HomeIndex) {
                        $.fn.HomeIndex.RefreshPreSales();
                    }
                    else if ($.fn.EstimateIndex) {
                        $.fn.EstimateIndex.bindEstimateGrid(false, false);
                    }

                    modal.modal('hide');
                    Global.ShowMessage(result.message, true, 'MessageDiv');
                }
                else {
                    if (result.data !== undefined) {
                        if (confirm('You have already scheduled ' + result.data + ' activity on same date. Are you sure you want to continue?')) {
                            $('#IsConfirmSubmit').val(true);
                            form.submit();
                        }
                    } else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageNotification');
                    }
                }
            });

            modal.off('hidden.bs.modal').on('hidden.bs.modal', function () {
                modal.removeData('bs.modal');
                modal.find('.modal-content').empty();
            });
        }

        function attachEventCKEditor(instance) {
            CKEDITOR.on('instanceReady', function (e) {
                e.editor.document.on('keypress', function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }

        $this.init = function () {
            InitForm();            
        };
    }

    $(function () {
        var self = new LeadStatus();
        self.init();
    });

}(jQuery));