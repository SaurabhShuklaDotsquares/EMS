/*global jQuery, Global,secureDomain */
(function ($) {
    function CurrentOpenings() {
        var $this = this, formOpening;

        function initializeForm() {
            formOpening = new Global.FormValidationReset('#form1');

            formJobReference = new Global.FormValidationReset('#frm-referFriend');
            $("#modal-referFriend-add-edit").on('loaded.bs.modal', function (e) {
                $(this).on('keyup keypress', function (e) {
                    var code = e.keyCode || e.which;
                    if (code == 13) {
                        e.preventDefault();
                        return false;
                    }
                });

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }

        $this.init = function () {
            initializeForm();
        };
    }
    $(function () {
        var self = new CurrentOpenings();
        self.init();
    });
}(jQuery));


function Delete(id) {
    if (confirm('Are you sure?')) {
        $("#ProgressBar_Status").show();
        $.post(domain + "currentopening/delete?id=" + id, function (result) {
            $("#ProgressBar_Status").hide();
            location.reload();
        });
    }
}