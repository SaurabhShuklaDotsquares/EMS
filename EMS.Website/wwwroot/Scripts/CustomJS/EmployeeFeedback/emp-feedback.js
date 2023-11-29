/*global jQuery, Global,secureDomain */
(function () {
    function ManageEmpFeedback() {
        $this = this;
        function initializeForm() {
            $('.searchfilterpm').on('change', function () {
                GetProjectManagerUsers();
            });

            $('.userinfo').on('change', function () {
                GetEmpinfoUsers();
            });
            $('.reasoncheckbox').click(function () {
                var checked_boxes = $('input.reasoncheckbox:checked').length;

                if (checked_boxes > 3) {
                    $(event.target).prop("checked", false);
                }
            });
            $(".reasoncheckbox").on('change', function () {
                if ($(this).prop('checked') == true) {
                    if ($(this).attr("data-attr") == "Other") {
                        $("#comment").attr("required", true);
                        $("#commentbox").show();
                    }
                    
                }
                else {
                    if ($(this).attr("data-attr") == "Other") {
                        $("#comment").attr("required", false);
                        $("#commentbox").hide();
                    }
                }
            });

            $('.btnSubmit').click('', function myfunction() {
                var checked_boxes = $('input.reasoncheckbox:checked').length;
                if (checked_boxes<1) {
                    alert('Please choose atleast 1 reason for leaving.');
                    return false;
                }
            });

        }

        function GetProjectManagerUsers() {
            var pmid = $("#PmUid").val();
            $("#Uid").val('');
            var emp = $("#Uid");
            if (pmid > 0) {
                $.ajax
                    ({
                        url: domain + 'feedback/GetEmployeesByPM',
                        type: 'POST',
                        //datatype: 'application/json',
                        //contentType: 'application/json',
                        data: {
                            pmid: pmid
                        },
                        success: function (result) {
                            //console.log('result', result.length)
                            if (result.length > 0) {
                                $.each(result, function () {
                                    emp.append($("<option></option>").val(this['value']).html(this['text']));
                                });
                            }
                            else {
                                emp.empty().append('<option value="" selected>--All Employee--</option>');
                            }
                        },
                        error: function (ex) {
                            alert("Whooaaa! Something went wrong.." + ex.responseText);
                        },
                    });
            }
            else {
                emp.empty().append('<option value="" selected>--All Employee--</option>');
            }
            $('select#Uid').prop('selectedIndex', 0)

        }

        function GetEmpinfoUsers() {
            $('#empname, #empemail, #empcode, #emppmname, #empdesignation, #empdepartment').text('')
            var empid = $(".userinfo").val();
            $.ajax
                ({
                    url: domain + 'feedback/GetEmployeesDataByid',
                    type: 'POST',
                    data: {
                        empid: empid
                    },
                    success: function (result) {

                        if (result.isfeedSubmitted == true) {
                            $('.feedbacksubmitted').addClass('hide');
                            $('.checkfeedbackerror').removeClass('hide');
                        }
                        else {
                            $('.feedbacksubmitted').removeClass('hide');
                            $('.checkfeedbackerror').addClass('hide');
                        }

                        $('#empname').text(result.name);
                        $('#empemail').text(result.emailOffice);
                        $('#empcode').text(result.empCode);
                        $('#emppmname').text(result.pmName);
                        $('#empdesignation').text(result.department);
                        $('#empdepartment').text(result.designation);
                    },
                    error: function (ex) {
                        alert("Whooaaa! Something went wrong.." + ex);
                    },
                });
        }


        $this.init = function () {
            initializeForm();
        }
    }

    $(function () {
        var self = new ManageEmpFeedback;
        self.init();
    });

}(jQuery));
