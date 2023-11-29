/*global jQuery, Global,secureDomain */
(function () {
    function ManageUser() {       
        var $this = this, form;
        var isPMorPMO = $('#hdnIsPMPMO').val() == "True" ? true : false;
        //date for DOB
        $("#DOB").datepicker({
            dateFormat: "dd/mm/yy",
            changeMonth: true,
            changeYear: true,
            yearRange: "-100:+0"
        });

        //date for Date of Joining
        $("#JoinedDate").datepicker({
            dateFormat: "dd/mm/yy",
            changeMonth: true,
            changeYear: true,
            yearRange: "-100:+0"
        });

        //date for Anniversary Date
        $("#MarraigeDate").datepicker({
            dateFormat: "dd/mm/yy",
            changeMonth: true,
            changeYear: true,
            yearRange: "-100:+0"
        });

        $(document).on('change', '#PMUid', function () {
            $("#TLId").html("");
            var DDLItems = "";
            if ($('#PMUid').val() != "") {
                $.get(domain + 'user/BindTeamLead', { PMId: $('#PMUid').val() }, function (data) {
                    if (data != null) {
                        $.each(data.teamLeadList, function (i, developer) {
                            DDLItems += "<option value='" + developer.value + "'>" + developer.text + "</option>";
                        });
                        $("#TLId").html(DDLItems);
                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#TLId").html(DDLItems);
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#TLId").html(DDLItems);
            }
        });


        //for rolecategory on change event to get Role

        $(document).on('change', '#RoleCateGoryId', function () {
            //debugger;
            $("#RoleId").html("");
            $("#DesignationId").html("");           
            var DDLItems = "";
            if ($('#RoleCateGoryId').val() != "") {
                $.get(domain + 'user/BindRoleList', { RoleCateGoryId: $('#RoleCateGoryId').val() }, function (data) {
                    if (data != null) {
                        $.each(data.roleList, function (i, Rolecat) {
                            DDLItems += "<option value='" + Rolecat.value + "'>" + Rolecat.text + "</option>";
                        });
                        $("#RoleId").html(DDLItems);
                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#RoleId").html(DDLItems);
                        $("#DesignationId").html(DDLItems);
                    }
                });              
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#RoleId").html(DDLItems);
                $("#DesignationId").html(DDLItems);
            }
        });




        //for role on change event to get desi

        $(document).on('change', '#RoleId', function () {            
            $("#DesignationId").html("");
            var DDLItems = "";
            if ($('#RoleId').val() != "") {
                $.get(domain + 'user/BindDesignationList', { RoleId: $('#RoleId').val() }, function (data) {
                    if (data != null) {
                        $.each(data.designationList, function (i, Designation) {
                            DDLItems += "<option value='" + Designation.value + "'>" + Designation.text + "</option>";
                        });
                        $("#DesignationId").html(DDLItems);
                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#DesignationId").html(DDLItems);
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#DesignationId").html(DDLItems);
            }
        });

        function initializeForm() {
           
            form = new Global.FormValidationReset('#edituserform');
            form.on("keypress keyup blur", ".decimal-number", function (event) {
                $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                    event.preventDefault();
                }
            });

            if (form.length) {

                form.on("change", "#chkOtherTech", function (event) {
                    if (this.checked) {
                        $(this).next('label').css('color', 'green');
                        $("#OtherTechnology").show();
                    }
                    else {
                        $(this).next('label').css('color', 'black');
                        $("#OtherTechnology").val('');
                        $("#OtherTechnology").hide();
                    }
                });

                form.find('.checkList input[name="Technology"]').each(
                    function () {
                        var specializations = $(this).closest('.checkList').find('input.checkspl');

                        if (this.checked) {
                            if (specializations.filter(':checked').length === 0) {
                                specializations.next('label').css('color', 'red');
                            }
                            else {
                                specializations.next('label').css('color', 'green');
                                $(this).next('label').css('color', 'green');
                            }
                        }
                        else {
                            specializations.prop('disabled', true).prop('checked', false);
                        }
                    }).on('change', function () {

                        var specializations = $(this).closest('.checkList').find('input.checkspl');

                        if (this.checked) {
                            specializations.prop('disabled', false);
                            if (specializations.filter(':checked').length === 0) {
                                specializations.next('label').css('color', 'red');
                            }
                            else {
                                specializations.next('label').css('color', 'green');
                                $(this).next('label').css('color', 'green');
                            }
                        }
                        else {
                            specializations.prop('disabled', true).prop('checked', false);
                            specializations.next('label').css('color', '');
                            $(this).next('label').css('color', '');
                        }
                    });

                form.find('.checkList input[name="Domain"]').each(
                    function () {
                        if (this.checked) {
                            $(this).next('label').css('color', 'green');
                        }
                    }).on('change', function () {
                        if (this.checked) {
                            $(this).next('label').css('color', 'green');
                        }

                    });



                form.on('change', '.chkSecialization input[type="radio"]', function () {
                    $(this).closest('.chkSecialization').find('label').css('color', 'green');
                    $(this).closest('.checkList').find('input[name="Technology"]+label').css('color', 'green');
                });

                $('#AttendanceId').on("keydown keyup", function (event) {
                    if (event.which != 8 && event.which != 0 && event.which < 48 || event.which > 57) {
                        this.value = this.value.replace(/\D/g, "");
                    }
                });

                $("#EmployeeMedicalData_PremiumTotal").on("change paste keyup", function () {
                    $("#EmployeeMedicalData_PremiumPerMonth").val(($(this).val()) / 12);
                });

                form.off('click', "#btnsave").on('click', "#btnsave", function () {
                    if (form.valid()) {
                        if (parseInt(form.find("#Uid").val()) === 0) {
                            if (form.find("#UserName").val() == "") {
                                CustomAlerts.error("Error !!!", "User name is required");
                                return false;
                            }
                            else if (form.find("#Password").val() == "") {
                                CustomAlerts.error("Error !!!", "Password is required");
                                return false;
                            }
                        }

                        var userTechs = [];
                        var selectedTechs = $('.checkList input[name="Technology"]:checked');
                        if (selectedTechs.length) {
                            var errorMsg = false;
                            selectedTechs.each(function () {
                                var specType = $(this).closest('.checkList').find('input.checkspl:checked').val();

                                if (!specType) {
                                    errorMsg = ('Please choose specialization level for technology : "' + $(this).next('label').text().trim() + '"');
                                    return false;
                                }
                                else {
                                    userTechs.push({
                                        TechId: this.value,
                                        SpecTypeId: specType
                                    });
                                }
                            });

                            if (errorMsg) {
                                CustomAlerts.error("Error !!!", errorMsg);
                                return false;
                            }
                        }


                        var userDomain = [];
                        var selectedDomains = $('.checkList input[name="Domain"]:checked');
                        if (selectedDomains.length) {
                            selectedDomains.each(function () {
                                userDomain.push({
                                    DomainId: this.value
                                });
                            });
                        }

                        var formdata = form.serializeArray();
                        if (userTechs.length) {

                            $.each(userTechs, function (i, item) {
                                formdata.push({ name: 'TechnologyList[' + i + '].TechId', value: item.TechId });
                                formdata.push({ name: 'TechnologyList[' + i + '].SpecTypeId', value: item.SpecTypeId });
                            });
                        }

                        console.log(userDomain);

                        if (userDomain.length) {
                            $.each(userDomain, function (i, item) {
                                formdata.push({ name: 'DomainExpert[' + i + '].DomainId', value: item.DomainId });
                            });
                        }

                        //$.post(form[0].action, formdata, function (data) {
                        //    if (data.success) {
                        //        //window.location.href = data.data.redirectUrl;
                        //        Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                        //    } else {
                        //        CustomAlerts.error("Error !!!", data.Message);
                        //    }
                        //});

                        var submitBtn = $('#btnsave');
                        var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();
                        submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
                        submitBtn.prop('disabled', true);

                        $.post(form[0].action, formdata, function (data) {
                            submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                            submitBtn.prop('disabled', false);
                            if (data.success) {
                                Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                                $('html, body').animate({
                                    'scrollTop': $("#NotificationDiv").position().top
                                });
                            } else {
                                CustomAlerts.error("Error !!!", data.message);
                            }
                        });
                    }
                });
            }
        }

        $(document).delegate('#btn_search', 'click', function () {
            LoadUsersGrid();
        });
        $(document).delegate('#ddl_pm', 'change', function () {
            LoadUsersGrid();
        });
        $(document).delegate('#ddl_status', 'change', function () {
            LoadUsersGrid();
        });
        function initGridControlWithEvents() {
            $('.switchBox').on('change', function () {
                var switchElement = this;
                alert(this.value);
                $.get(domain + 'User/UpdateStatus', {
                    id: this.value
                });
            });
        }
        function LoadUsersGrid() {
            
            $.post(domain + 'user/checkloginuser', function (data) {
                if (data.isSuccess == undefined) {
                    window.location.href = domain + 'login/index';
                    return false;
                }
            });
            var manageUserGrid = new Global.GridHelper('#grid-manageuser', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": true,
                ajax: {
                    url: domain + 'User/ManageUsersList',
                    type: 'POST',
                    data: { status: $('#ddl_status').val(), username: $('#txtusername').val(), pmId: $('#ddl_pm').val() }
                },
                order: [[0, 'desc']],
                "columnDefs": [
                ],
                columns: [
                    { name: 'userId', data: 'userId', title: "ID", sortable: false, searchable: false, visible: false },
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                    {
                        name: 'name', data: null, title: "Name", sortable: true, searchable: false, render: function (data, type, full, meta) {
                            var content = "";
                            content = content + full.title + " " + full.name;
                            if (full.empCode != "") {
                                content = content + " <b>[" + full.empCode + "]</b>";
                            }
                            content = content + "<br/>" + full.emailOffice;
                            return content;
                        }
                    },
                    {
                        name: 'roledesignation', data: null, title: "Role/Designation", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            content = "";
                            content = content + full.role + " / " + full.designation;
                            return content;
                        }
                    },
                    { name: 'teamLead', data: 'teamLead', title: "Team/Project Lead ", sortable: false, searchable: false },
                    { name: 'department', data: 'department', title: "Department", sortable: false, searchable: false },
                    { name: 'mobileNumber', data: 'mobileNumber', title: "Mobile Number", sortable: false, searchable: false },
                    {
                        name: 'other', data: null, title: "Other", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            content = "";
                            if (full.aadharnumber != "") {
                                content = content + "Adhaar No.: " + full.aadharnumber + "<br/>";
                            }
                            if (full.passportnumber != "") {
                                content = content + "Passport No.: " + full.passportnumber;
                            }
                            return content;
                        }
                    },
                    { name: 'bloodGroup', data: 'bloodGroup', title: "Blood Group", sortable: false, searchable: false },
                    { name: 'bloodGroup', data: 'bloodGroup', title: "Blood Group", sortable: false, searchable: false },
                    {
                        name: 'action', data: null, title: "Action", className: "text-center", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            if (isPMorPMO) {
                                return '<a class="trans-btn"  href="' + domain + 'user/edituser/' + full.userId + '" "><i class="fa fa-edit"></i></a> | <a title="Change User Password" data-toggle="modal" data-target="#modal-updatePassword" href="' + domain + 'user/changeuserpassword?userId=' + full.userId + '" > <i class="fa fa-key"></i></a>';
                                    /*| <a title="View User Password" data-toggle="modal" data-target="#modal-ShowPassword" href="' + domain + 'user/showuserpassword?userId=' + full.userId + '" ><i class="glyphicon glyphicon-eye-open"></i></a>';*/
                            }
                            else {
                                return '<a class="trans-btn"  href="' + domain + 'user/edituser/' + full.userId + '" "><i class="fa fa-edit"></i>  </a>';
                            }
                        }

                    }
                ],

                "fnDrawCallback": function (oSettings) {
                    initGridControlWithEvents();
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                },
            });
        }
        function InitializeModel() {
          
            $(document).delegate("#btn-submit", "click", function () {
                var formChangePass = new Global.FormValidationReset('#frm-changepassword');
                //var form = $('#frm-changepassword');
                formChangePass.validate();
                if (formChangePass.valid()) {

                    if ($("#ConfirmPassword").val() != $("#NewPassword").val()) {
                        //CustomAlerts.error("Error !!!", "Password Doesn't Match");
                        $('.ConfirmPasswordError').show();
                        $("#ConfirmPassword").val('');
                        return false;
                    } else {
                        $('.ConfirmPasswordError').hide();
                    }
                    return true;
                }
                else { return false; }
            });
            $("#modal-updatePassword").on("hidden.bs.modal", function (e) {
                $(this).removeData("bs.modal");
            });

            $("#modal-showPassword").on("hidden.bs.modal", function () {
                $(this).removeData("bs.modal");
               // $(this).find('.modal-content').empty();
            });           

        }      

        $this.init = function () {
            initializeForm();
            LoadUsersGrid();
            InitializeModel();
        }
    }

    $(function () {
        var self = new ManageUser;
        self.init();
    });

}(jQuery));
