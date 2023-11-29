/*global jQuery, Global,secureDomain */
(function () {
    "use strict"

    function Permission() {
        var $this = this;

        function initializeForm() {

            $("#userPermission_StartDate").datepicker({
                defaultDate: "0",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: 0,
                //maxDate: "+" + mindateval + "D", 
                onClose: function (selectedDate) {
                    //debugger
                    if (selectedDate != "" && $("#StartDate").val() != selectedDate) {
                        $("#userPermission_EndDate").datepicker("option", "minDate", selectedDate);
                    }
                }
            });

            $("#userPermission_EndDate").datepicker({
                defaultDate: "0",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: 0,
                //maxDate: "+" + mindateval + "D"
            });

        }

        // init
        $this.init = function () {

            initializeForm();
        }

    }

    // start
    $(function () {
        var self = new Permission();
        self.init();
    });

}(jQuery));