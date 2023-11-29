/*global jQuery, Global,secureDomain */

(function () {
    function ManageForecasting() {
        $this = this;
        var selectedcolor = '';
        var options = [];
        function initializeForm() {

            $(document).ready(function () {
                //$.ajax({
                //    url: "Forecasting/GetChasingLeadsAutoComplete",
                //    data: { "prefix": "cc" },
                //    type: 'POST',
                //    success: function (result) {
                //        //console.log(result);
                //        var data = JSON.parse(result);
                //        console.log(data);
                //        $.each(data, function (index, item) {
                //            $('#Lead').append($('<option>').text(item.Text).attr('value', item.Value));
                //            options.push({ id: item.Value, text: item.Text });
                //        });
                //        console.log(options);
                //        $("#Lead").select2();
                //    }
                //});
                //setTimeout(function(){
                //    $("#Lead").select2();
                //},1000)
            })

            $(document).on("click", "#IsHold", function () {
                if ($(this).is(":checked")) {
                    $("#HoldReasonShow").show();
                    $("#HoldReason").prop("required", true);
                } else {
                    $("#HoldReasonShow").hide();
                    $("#HoldReason").prop("required", false);
                }
            });


            $(".chosen").chosen();
            $("#divclient").css("display", "none");
            $('input[type=radio][name=ForecastingType]').change(function () {
                if (this.value == '1') { //for lead
                    $("#divclient").css("display", "none");
                    $("#divlead").css("display", "block");
                }
                else if (this.value == '2') { //for project
                    $("#divlead").css("display", "none");
                    $("#divclient").css("display", "block");
                    $("#divclient").removeAttr('style');
                }
            });

            $('.dept-group').change(function () {
                var checkcount = $('input[type = checkbox]:checked').length;
                if (checkcount > 0) {
                    $("#SelectedDepartment").val('test')
                } else {
                    $("#SelectedDepartment").val('')
                }
            });

            $("#TentiveDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
            });

            new Global.FormValidationReset('#Form1', { ignore: '.validated' });

            $('#Form1').on('submit', function (e) {
                //debugger;
                var forecatingtype = $("input:radio[name='ForecastingType']:checked").val();
                $('#forecastingTypeerror').css('display', 'none');
                if (forecatingtype == undefined) {
                    $('#forecastingTypeerror').css('display', 'block');
                    return false;
                }
                else if (forecatingtype == 1) {
                    var e = document.getElementById("Lead");
                    //alert(strLead)
                    var strLead = e.options[e.selectedIndex].value;
                    if (strLead == '' || strLead == null || strLead == undefined) {
                        $('#leaderror').css('display','block');
                        //alert('Lead required')
                        return false;
                    }
                    else {
                        $('#leaderror').css('display', 'none');
                    }
                }
                else if (forecatingtype == 2) {
                    var p = document.getElementById("ProjectId");
                    var strProject = p.options[p.selectedIndex].value;
                    if (strProject == '' || strProject == null || strProject == undefined) {
                        $('#projectiderror').css('display', 'block');
                        return false;
                        //alert('Project/client required')
                       
                    }
                    else {
                        $('#projectiderror').css('display', 'none');
                    }
                }
                if (($('#LeaveId').val() == '' || $('#LeaveId').val() == '0')) {
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

        $this.init = function () {
            initializeForm();
        }
    }

    $(function () {
        var self = new ManageForecasting;
        self.init();
    });
}(jQuery));



