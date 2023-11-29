/*global jQuery, Global,secureDomain */

(function () {
    function ManageRoom() {
        $this = this;
        var selectedcolor = '';
        function initializeForm() {
            ValidateUID();
            new Global.FormValidationReset('#Form1', { ignore: '.validated' });
            $('#Form1').on('submit', function (e) {

                if (isRoleUKPM == 1 && ($('#LeaveId').val() == '' || $('#LeaveId').val() == '0')) {
                    if (!$('#Form1').hasClass('verified')) {
                        e.preventDefault();
                        var uid = currentuserid;
                        if ($('#Uid').length > 0) {
                            if ($('#IsSelfLeave').length == 0 || !$('#IsSelfLeave').prop('checked')) {
                                uid = $('#Uid').val();
                            }
                        }
                        $('#Form1').validate({ ignore: '.validated' });
                        if ($('#Form1').valid()) {
                            $.post(domain + "leave/checkforotherleave", { uid: uid, StartDate: $('#StartDate').val(), EndDate: $('#EndDate').val() }, function (data) {
                                if (data.needToShowMessage && !confirm(data.message)) {

                                }
                                else {
                                    $('#Form1').addClass('verified');
                                    $('#Form1').submit();
                                }
                            });
                        }
                    }
                }

            });
        }

        function ValidateUID() {
            if ($('#IsSelfLeave').length > 0) {
                if ($('#IsSelfLeave').prop('checked')) {
                    $('.dvuser').addClass('hide');
                    $('#Uid').addClass('validated');
                    $('.dvleavestatus').addClass('hide');
                    $('.dvleavestatus input,.dvleavestatus textarea').addClass('validated');
                }
                else {
                    $('#Uid').removeClass('validated');
                    $('.dvuser').removeClass('hide');
                    $('.dvleavestatus').removeClass('hide');
                    $('.dvleavestatus input,.dvleavestatus textarea').removeClass('validated');
                }
            }
        }

        $this.init = function () {
            initializeForm();
        }
    }


    $(function () {
        var self = new ManageRoom;
        self.init();
    });


    //$("#CompanyOfficeId").change(function () {
    //    $.ajax({
    //        url: domain + 'MeetingRoom/GetOfficeFloorsById',
    //        data: { OfficeId: $('option:selected', this).val() },
    //        dataType: "json",
    //        type: "GET",
    //        error: function () {
    //            //alert(" An error occurred.");
    //            $('#CompanyOfficeFloorId').empty();
    //            $('<option>',
    //                  {
    //                      value: "",
    //                      text: '--Select--'
    //                  }).html('--Select--').appendTo("#CompanyOfficeFloorId");
    //        },
    //        success: function (data) {
    //            $('#CompanyOfficeFloorId').empty()
    //            $.each(data, function (i, data) {      // bind the dropdown list using json result
    //                $('<option>',
    //                   {
    //                       value: data.Value,
    //                       text: data.Text
    //                   }).html(data.Text).appendTo("#CompanyOfficeFloorId");
    //            });
    //        }
    //    });
    //});
    //colordropdown();
    function colordropdown() {
        $.ajax({
            url: domain + 'MeetingRoom/GetColor',
            dataType: "json",
            type: "GET",
            error: function () {
                alert(" An error occurred.");
            },
            success: function (data) {
                $('#ThemeColor').empty()
                $.each(data, function (i, data) {      // bind the dropdown list using json result
                    $('<option class="' + data.Value + '" value="' + data.Value + '">',
                       {
                           value: data.Value,
                           text: data.Text
                       }).html(data.Text).appendTo("#ThemeColor");
             
                    
                });
                $('#ThemeColor').val(selectedcolor);
            }
        });
    }

}(jQuery));



