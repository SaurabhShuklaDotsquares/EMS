(function () {
    function AdditionalSupport() {
        var $this = this, grid;

        function initialize() {

            $("#Status").prepend("<option value='' selected='selected'>-Select Status-</option>");

            var form = new Global.FormHelper($('#additionalSupportForm'), null, null,
                        function (result) {
                            if (result.isSuccess) {
                                window.location.href = result.redirectUrl;
                            } else {
                                form.find("#NotificationMessage").html(result);
                            }
                        });

            form.find("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                onSelect: function (selectedDate) {
                    form.find("#EndDate").datepicker("option", "minDate", selectedDate);
                }
            });

            form.find("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                onSelect: function (selectedDate) {
                    form.find("#StartDate").datepicker("option", "maxDate", selectedDate);
                }
            });

            form.find("#AssignedUserIds").select2({
                placeholder: "-- Select Assigned Developers --",
                allowClear: true,
                width: '100%'
            });

        }

        $this.init = function () {
            initialize();
        }
    }
    $(function () {
        var self = new AdditionalSupport();
        self.init();
    });
}(jQuery))