(function ($) {
    function Chasepopup() {
        var $this = this;

        function InitForm() {           
            var modal = $("#modal-chase-task");          
            var form = new Global.FormHelper(modal.find("form"),
            {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: '' }
            }, null, function (result) {
                if (result.isSuccess) {
                    modal.modal('hide');
                    Global.ShowMessage(result.message, true, 'MessageDiv');
                }
                else {
                    Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
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
        var self = new Chasepopup();
        self.init();
    });

}(jQuery));