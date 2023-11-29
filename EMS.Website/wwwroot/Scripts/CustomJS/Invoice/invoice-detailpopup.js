(function ($) {
    function InvoceDetail() {
        var $this = this, modal;

        function initialize() {
            var modal = $("#modal-ViewInvoice");

            modal.off('hidden.bs.modal')
                .on('hidden.bs.modal', function () {
                    $(this).removeData('bs.modal');
                    $(this).find('.modal-content').empty();
                });
        }

        $this.init = function () {
            initialize();
        };
    }

    $(function () {
        var self = new InvoceDetail();
        self.init();
    });

}(jQuery));
