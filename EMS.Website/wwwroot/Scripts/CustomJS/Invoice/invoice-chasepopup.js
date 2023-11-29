(function ($) {
    function Statuspopup() {
        var $this = this, form, modal;
        function InitForm() {
            var modal = $("#modal-chaseInvoice");

            modal.find("form .datepicker").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0
            });           

            form = new Global.FormHelper(modal.find("form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: '' }
            }, function () {
                $('.divoverlay').removeClass('hide');
            }, function (result) {
                $('#grid-invoice').DataTable().ajax.reload();
                $('.divoverlay').addClass('hide');
                modal.modal('hide');
                var $MessageDiv = $('#MessageDiv');
                if (Global.IsNotNullOrEmptyString(result.message)) {
                    $MessageDiv.addClass('alert-success').removeClass('alert-danger');
                    $MessageDiv.empty().html(result.message);
                }
                else if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                    $MessageDiv.addClass('alert-danger').removeClass('alert-success');
                    $MessageDiv.empty().html(result.errorMessage);
                }
                $MessageDiv.show();
                window.setTimeout(function () {
                    $MessageDiv.html('');
                    $MessageDiv.hide();
                }, 3000);
                if (result.isSuccess) {
                    return false;
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
        var self = new Statuspopup();
        self.init();
    });

}(jQuery));



