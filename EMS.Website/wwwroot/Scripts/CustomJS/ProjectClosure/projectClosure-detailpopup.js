(function ($) {
    function Detailpopup() {
        var $this = this;
        function initialize() {

            $("#modal-detail-projectClosure").off('hidden.bs.modal')
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
        var self = new Detailpopup();
        self.init();
    });

}(jQuery));