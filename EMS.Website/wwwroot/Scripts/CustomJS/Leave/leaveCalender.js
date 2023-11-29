$(function ($) {

    function CommonCalendar() {
        var $this = this, form;

        function LoadCalendar() {

            var userid = $("#Uid").val();
            if ($('#selfleave').prop('checked')) {
                userid = $('#selfleave').val();
            }

            $("#calendardiv").load(domain + "leave/LeaveCalender", { steps: 'reload', leaveType: $("#leaveType").val(), leaveCategory: $("#leaveCategory").val(), pmid: $("#PmId").val(), uid: userid, status: $('#status').val() }, function () {
                $('.divoverlay').addClass('hide');
                $(".prc-btn2").parent('td').on("click", function (e) {
                });
            });
        }

        function initializeModalWithForm() {
            $('.divoverlay').addClass('hide');
            $("#PmId").on("change", function (e) {
                $('.divoverlay').removeClass('hide');
                GetEmployeeList();
                $("#Uid").val("0");
                LoadCalendar();
                AutoScroll();
            });

            $("#Uid").on("change", function (e) {
                $('.divoverlay').removeClass('hide');
                LoadCalendar();
                AutoScroll();
            });

            $("#leaveType").on("change", function (e) {
                $('.divoverlay').removeClass('hide');
                LoadCalendar();
                AutoScroll();
            });

            $("#leaveCategory").on("change", function (e) {
                $('.divoverlay').removeClass('hide');
                LoadCalendar();
                AutoScroll();
            });

            $("#status").on("change", function (e) {
                $('.divoverlay').removeClass('hide');
                LoadCalendar();
                AutoScroll();
            });

            $('#selfleave').on('click', function () {
                $('.divoverlay').removeClass('hide');
                if ($('#selfleave').prop('checked')) {
                    $("#Uid").val("0");
                    if ($("#PmId") && $("#PmId").children('option') && $("#PmId").children('option').length > 0) {
                        $("#PmId").val("0");
                        GetEmployeeList();
                    }
                    $('#Uid').attr('disabled', true);
                    $('#PmId').attr('disabled', true);
                } else {
                    $('#Uid').removeAttr('disabled');
                    $('#PmId').removeAttr('disabled');
                }
                LoadCalendar();
                AutoScroll();
            });


            //$('#selfleave').on('click', function () {
            //    $('.divoverlay').removeClass('hide');
            //    var uid = $("#Uid").val();
            //    $('#Uid').removeAttr('disabled');
            //    $('#PmId').removeAttr('disabled');
            //    if ($('#selfleave').prop('checked')) {
            //        //$("#Uid").val("0");
            //        //if ($("#PmId").length > 0 && $("#PmId").val() != '0') {
            //        //    $("#PmId").val("0");
            //        //}

            //        $("#Uid").val("0");
            //        if ($("#PmId") && $("#PmId").children('option') && $("#PmId").children('option').length > 0) {
            //            $("#PmId").val("0");
            //        }

            //        GetEmployeeList();
            //        $('#Uid').attr('disabled', true);
            //        $('#PmId').attr('disabled', true);
            //        uid = $('#selfleave').val();
            //    }
            //    //GetEmployeeList();
            //    LoadCalendar();
            //    AutoScroll();
            //});

            $("#modal-leave-detail").on('loaded.bs.modal', function (e) {

                form = new Global.FormHelper($(this).find("form"), { updateTargetId: "validation-summary" });

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });

            function GetEmployeeList() {
                //var pmid = {};
                //pmid.pmid = $("#PmId").val();
                var pmid = $("#PmId").val();
                if (pmid == '')
                    pmid = 0;
                var emp = $("#Uid");

                //$.post('Leave/GetEmployeesByPM', {
                //    pmid: pmid
                //}, function () {
                //});
                //return false;

                $.ajax
                       ({
                        url: domain + 'Leave/GetEmployeesByPM',
                        type: 'POST',
                        data: { pmid: pmid },
                        //datatype: 'application/json',
                        //contentType: 'application/json',
                           //data: JSON.stringify({
                           //    pmid: pmid
                           // }),
                           success: function (result) {
                               emp.empty().append('<option selected="selected" value="0">All Employee</option>');
                               $.each(result, function () {
                                   emp.append($("<option></option>").val(this['value']).html(this['text']));
                               });
                           },
                           error: function (ex) {
                               alert("Whooaaa! Something went wrong.." + ex);
                           },
                       });
            }
            
        }
        function LoadLeaveBalance() {
            $.ajax({
                url: domain + "LeaveBalance/Index",
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    $('#LeaveBalance').html(result);
                },
                error: (function (xhr, status) {
                    alert(status);
                })
            });
        }
        $this.init = function () {
            initializeModalWithForm();
            LoadLeaveBalance();
        }
    }

    $(function () {
        var self = new CommonCalendar();
        self.init();
    })

})
function GetEmpLeaves(e) {
    var userid = $("#Uid").val();
    $('.divoverlay').removeClass('hide');
    if ($('#selfleave').prop('checked')) {
        userid = $('#selfleave').val();
    }
    $("#calendardiv").load(domain + "leave/LeaveCalender", { steps: $(e).attr("steps-type"), leaveType: $("#leaveType").val(), leaveCategory: $("#leaveCategory").val(), pmid: $("#PmId").val(), uid: userid, status: $('#status').val() }, function () {
        $('.divoverlay').addClass('hide');
        $(".prc-btn2").parent('td').on("click", function () {
        });
    });
    AutoScroll();
}