/*global jQuery, Global,secureDomain */

(function () {
    function ManageForecasting() {
        $this = this;
        var selectedcolor = '';
        function initializeForm() {
            $('input[type=radio][name=ForecastingType]').change(function () {
                if (this.value == '1') { //for lead
                    $("#divclient").css("display", "none");
                    $("#divlead").css("display", "block");
                }
                else if (this.value == '2') { //for project
                    $("#divlead").css("display", "none");
                    $("#divclient").css("display", "block");
                }
            });
            $('.dept-group').change(function () {
                var checkcount = $('input[type = checkbox]:checked').length;
                if (checkcount > 0) {
                    $("#SelectedDepartment").val('test');
                } else {
                    $("#SelectedDepartment").val('');
                }
            });

            $("#TentiveDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
            });

            new Global.FormValidationReset('#Form1', { ignore: '.validated' });

            $('#Form1').on('submit', function (e) {
                var forecatingtype = $("input:radio[name='ForecastingType']:checked").val();
                if (forecatingtype == 1) {
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



