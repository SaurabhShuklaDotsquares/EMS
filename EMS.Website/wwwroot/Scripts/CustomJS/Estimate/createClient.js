(function ($) {
    function CreateClient() {
        var $this = this, CreateClientform,SelectClientform, modal;

        function InitForm() {

            var modal = $("#modal-createSelectClient");

            CreateClientform = new Global.FormHelper(modal.find('#createClient'), {
                updateTargetId: "error-ModalMessage",
                validateSettings: { ignore: '' }
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
                    Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageNotification');
                }
            });

            SelectClientform = new Global.FormHelper(modal.find('#selectClient'), {
                updateTargetId: "error-ModalMessage",
                validateSettings: { ignore: '' }
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
                else
                {
                    Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageNotification');
                }
            });

            modal.off('hidden.bs.modal').on('hidden.bs.modal', function () {
                modal.removeData('bs.modal');
                modal.find('.modal-content').empty();
            });
        }

        $this.init = function () {
            InitForm();
        };
    }

    $(function () {
        var self = new CreateClient();
        self.init();
    });
}(jQuery));
