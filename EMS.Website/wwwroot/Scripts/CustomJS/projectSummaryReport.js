/*global jQuery, Global,secureDomain */
(function ($) {
    $('#btn_search').on('click', function () {

        if ($('form').valid()) {
            $.get(domain + 'TimesheetReport/GetProjectSummaryReport', { userID: $('#ddl_User').val() }, function (data) {
                $(".export-btn").hide();
                $("#div_Table").html(data);
                if (!$("#div_Table tbody .tr-norecord").length > 0) {
                    $(".export-btn").show();
                    var _href = $(".export-btn").data("href");
                    $(".export-btn").attr("href", _href + "?userID=" + $('#ddl_User').val());
                }
            });
        }
        return false;
    });
    $('form').validate({
        highlight: function (element, errorClass, validClass) {
            $(element).removeClass(errorClass);
        }
    });

}(jQuery));
