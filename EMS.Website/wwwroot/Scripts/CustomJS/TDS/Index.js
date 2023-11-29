


(function ($) {

    var tableCount = 0;
    function index() {
        var $this = this, grid;
        

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-tds-table', {
                serverSide: true,
                destroy: true,
                ordering: true,
                searchDelay: 800,
                //"pageLength": 100,
                "bFilter": true,
                "bAutoWidth": false,
                
                "searching": false,
                "bSortable": true,
                "bPaginate": true,
                "sPaginationType": 'simple_numbers',
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search By Name"
                },
                dom: 'Bfrtip',
                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ]
                ,
                ajax:
                {
                    url: domain + "empInvestment/Index",
                    type: "POST",
                    data: getGridFilters()

                },

                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "width": "18%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "2%", "targets": 8 },
                    { "width": "2%", "targets": 9 },


                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },

                        {
                            name: "Name", data: "name", title: "EMPLOYEE NAME", sortable: true, searchable: true, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return ' <a class="ablue" href="' + domain + 'empInvestment/InvestmentDetails?UID=' + dataRow.uidEn + '&AssesmentYearId=' + dataRow.assesmentYearIdEn + '"> '
                                   + dataRow.name +
                                   ' </a>';
                                

                            }
                        },
                        {
                            name: "assesmentYear", data: "assesmentYear", title: "Assesment Year", sortable: true, searchable: false, visible: true
                        },
                        {
                            name: "AttendenceId", data: "attendenceCode", title: "Attendence Id", sortable: true, searchable: false, visible: true
                        },
                        {
                            name: "Email", data: "mailId", title: "Email", sortable: true, searchable: true, visible: true
                        },
                        {
                            name: "UpdatedByEmp", data: "lastUpdatedByEmp", title: "updated by Employee", sortable: true, searchable: true, visible: true
                        },

                        {
                            name: "UpdatedByEmployer", data: "updatedByEmp", title: "updated by Employer", sortable: true, searchable: true, visible: true
                        },
                        {
                            name: "UpdatedBy", data: "lastUpdatedByEmployer", title: "updated by Employer", sortable: true, searchable: true, visible: true
                        },
                        {
                            name: "DeclarationSheetDate", data: "declarationSheetDate", title: "DeclarationSheet Date", sortable: true, searchable: true, visible: true
                        },
                        {
                            name: "IsLockUnlock", data: "isLockUnlock", title: "Lock/Unlock", sortable: true, searchable: true, visible: true,
                            render: function (data, type, dataRow, meta) {
                                
                                return '<input type="checkbox" class="switchBox custom btnLockUnlock" data-id="' + dataRow.uid + ',' + dataRow.assesmentYearId + '"  data-on-color="primary" data-onlabel="Enabled" data-on-text="Lock" data-off-text="Unlock" data-off-color="info"  ' + (dataRow.isLockUnlock == true ? "checked" : "") + '>'

                            }
                        }

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {


                    var href = "empInvestment/DownloadReport";
                    var AssesmentYearId = $("#AssesmentYearId").val()
                    var Attendencecode = $("#Attendencecode").val()
                    var LockUnlockType = $("#LockUnlockType").val()
                    var ModifiedEmployeeId = $("#ModifiedEmployeeId").val()
                    var UserInfo = $("#UserInfo").val()
                    var StartDate = $("#StartDate").val()
                    var EndDate = $("#EndDate").val()
                    var TdsTypeId = $("#TdsTypeId").val()

                    tableCount = $("#grid-tds-table").DataTable().data().length
                    //debugger;
                    var tbl = $('#grid-tds-table').DataTable();
                    if (tableCount > 0) {
                        $("#btnexporttoexcel").attr('href', href + "?TDSTypeId=" + TdsTypeId + "&AssesmentYearId=" + AssesmentYearId + "&Attendencecode=" + Attendencecode + "&LockUnlockType=" + LockUnlockType + "&ModifiedEmployeeId=" + ModifiedEmployeeId + "&UserInfo=" + UserInfo + "&StartDate=" + StartDate + "&EndDate=" + EndDate);
                        $("#btnexporttoexcel").removeAttr('disabled', 'disabled');
                    }
                    else {
                        $("#btnexporttoexcel").attr('disabled', 'disabled');
                        $("#btnexporttoexcel").attr('href', "javascript:void(0)");
                    }
                    
                    if (!aData.isACUser) {
                        var tbl = $('#grid-tds-table');
                        //tbl.DataTable().column(7).visible(false);
                        tbl.DataTable().column(9).visible(false);
                        
                    }
                   
                },
                "fnDrawCallback": function (oSettings) {

                    var href = "empInvestment/DownloadReport";

                    var AssesmentYearId = $("#AssesmentYearId").val()
                    var Attendencecode = $("#Attendencecode").val()
                    var LockUnlockType = $("#LockUnlockType").val()
                    var ModifiedEmployeeId = $("#ModifiedEmployeeId").val()
                    var UserInfo = $("#UserInfo").val()
                    var StartDate = $("#StartDate").val()
                    var EndDate = $("#EndDate").val()
                    var TdsTypeId = $("#TdsTypeId").val()

                    tableCount = $("#grid-tds-table").DataTable().data().length
                    if (tableCount > 0) {
                        $("#btnexporttoexcel").attr('href', href + "?TDSTypeId=" + TdsTypeId + "&AssesmentYearId=" + AssesmentYearId + "&Attendencecode=" + Attendencecode + "&LockUnlockType=" + LockUnlockType + "&ModifiedEmployeeId=" + ModifiedEmployeeId + "&UserInfo=" + UserInfo + "&StartDate=" + StartDate + "&EndDate=" + EndDate);
                        $("#btnexporttoexcel").removeAttr('disabled', 'disabled');
                    }
                    else {
                        $("#btnexporttoexcel").attr('disabled', 'disabled');
                        $("#btnexporttoexcel").attr('href', "javascript:void(0)");
                    }
                   

                    $('.divoverlay').addClass('hide');
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');

                    $('.switchBox').each(function (index, element) {
                        if ($(element).data('bootstrapSwitch')) {
                            $(element).off('switch-change');
                            $(element).bootstrapSwitch('destroy');
                        }

                        $(element).bootstrapSwitch()
                            .on('switch-change', function () {
                                e.preventDefault();
                                var switchElement = this;
                                if ($(".btnLockUnlock").prop('checked') == true) {
                                    $('.btnLockUnlock').val(true)
                                }
                                else {
                                    $('.btnLockUnlock').val(false)
                                }

                            });
                    });

                    $('.switch-left').addClass('fa fa-lock')
                    $('.switch-left').text('')
                    $('.switch-left').css('font-size', '11px');
                    $('.switch-left').css('padding', '5px');
                    $('.switch-left').css('width', '34%;');
                    $('.switch-right').addClass('fa fa-unlock');
                    $('.switch-right').text('');
                    $('.switch-right').css('font-size', '11px');
                    $('.switch-right').css('padding', '5px');
                    $('.switch-right').css('width', '34%;');
                    $('.has-switch').css('min-width', '50%');
                    $('.has-switch').css('height', '25px');
                    //$('.has-switch').css('position', 'absolute');
                }
            });
        }

        function intializeModalWithForm() {

            $("#modal-add-edit-appraise").on('loaded.bs.modal', function () {

                var modal = $(this);

                modal.find("#appraiseDate").datepicker({
                    dateFormat: "dd/mm/yy"
                });

                modal.find("input[type='radio'][name='AppraiseId']").change(function () {
                    var value = this.value;
                    if (value == "2") {
                        modal.find("#clientAppraise").removeClass("hidden").find(":input").prop("disabled", false);
                        modal.find("#TlComment").data("rule-required", false).removeClass("error").blur();
                        modal.find("#ClientComment").data("rule-required", true);
                    }
                    else {
                        modal.find("#clientAppraise").addClass("hidden").find(":input").prop("disabled", true);;
                        modal.find("#ClientComment").data("rule-required", false);
                    }
                });

                modal.find("input[type='radio'][name='AppraiseId']:checked").change();

                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload(null, false);
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-delete-appraise").on('loaded.bs.modal', function () {

                var modal = $(this);

                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload(null, false);
                            Global.ShowMessage(result.message, true, 'MessageDiv');
                        }
                        else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                        }
                    });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        $this.init = function () {
            intializeGrid();
            $("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);

                }
            });
            $("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    //$("#StartDate").datepicker("option", "minDate", selectedDate);

                }
            });
            //debugger;
            //$('.switchBox').each(function (index, element) {
            //    if ($(element).data('bootstrapSwitch')) {
            //        $(element).off('switch-change');
            //        $(element).bootstrapSwitch('destroy');
            //    }

            //    $(element).bootstrapSwitch()
            //        .on('switch-change', function () {
            //            e.preventDefault();
            //            var switchElement = this;
            //            if ($(".btnLockUnlock").prop('checked') == true) {
            //                $('.btnLockUnlock').val(true)
            //            }
            //            else {
            //                $('.btnLockUnlock').val(false)
            //            }

            //        });
            //});

            //$('.switch-left').addClass('fa fa-lock')
            //$('.switch-left').text('')
            //$('.switch-left').css('font-size', '20px');
            //$('.switch-left').css('padding', '6px');
            //$('.switch-right').addClass('fa fa-unlock');
            //$('.switch-right').text('');
            //$('.switch-right').css('font-size', '20px');
            //$('.switch-right').css('padding', '6px');
            //$('.has-switch').css('float', 'right');
            //$('.has-switch').css('margin-right', '20px');
            //$('.has-switch').css('position', 'absolute');

            intializeModalWithForm();
            
        };

        


        $(document).off('change', '.btnLockUnlock').on('change', '.btnLockUnlock', function (e) {

            var IsLockUnlock = $(this).is(":checked")
            var d = IsLockUnlock == true ? 'Unlock.' : 'Lock.';
            var CnfrmMsg = confirm('Are you sure, You would like to' + " " + (IsLockUnlock == true ? 'Unlock' : 'Lock')+" this record.")
            var data_Id = $(this).attr('data-id')
            var Id = data_Id.split(',')[0];
            var assesmentYearId = data_Id.split(',')[1];
            
            if (IsLockUnlock) {
                IsLockUnlock = false;
            }
            else {
                IsLockUnlock = true;
            }
            e.preventDefault();
            if (CnfrmMsg) {
                $.ajax({
                    url: "empInvestment/UpdateLockUnlock",
                    type: "Post",
                    data: { Uid: JSON.parse(Id), LockUnlock: IsLockUnlock, AssesmentYearId: JSON.parse(assesmentYearId) },
                    success: function (data) {

                        if (data.value) {
                            var Message = (!IsLockUnlock) ? "Investment Detail UnLock Successfully." : "Investment Detail Lock Successfully.";
                            alertify.success(Message);
                            if (!IsLockUnlock) {
                                $("#btnLockUnlocktxt").text('Click here to lock')
                            }
                            else {
                                $("#btnLockUnlocktxt").text('Click here to unlock')
                            }
                        }
                        else {
                            alertify.error('Some error.');
                        }
                    }
                })
            }
            else {
                $('.switchBox').each(function (index, element) {
                    if ($(element).data('bootstrapSwitch')) {
                        $(element).off('switch-change');
                        $(element).bootstrapSwitch('destroy');
                    }

                    $(element).bootstrapSwitch()
                        .on('switch-change', function () {
                            //e.preventDefault();
                            var switchElement = this;
                            if ($(".btnLockUnlock").prop('checked') == true) {
                                $('.btnLockUnlock').val(true)
                            }
                            else {
                                $('.btnLockUnlock').val(false)
                            }

                        });
                });


                $('.switch-left').addClass('fa fa-lock')
                $('.switch-left').text('')
                $('.switch-left').css('font-size', '11px');
                $('.switch-left').css('padding', '5px');
                $('.switch-left').css('width', '34%;');
                $('.switch-right').addClass('fa fa-unlock');
                $('.switch-right').text('');
                $('.switch-right').css('font-size', '11px');
                $('.switch-right').css('padding', '5px');
                $('.switch-right').css('width', '34%;');
                $('.has-switch').css('min-width', '50%');
                $('.has-switch').css('height', '25px');
            }



        });


        $("#btnSearch").on("click", function () {
            intializeGrid();
        });
        function getGridFilters() {
            return {
                AssesmentYearId: $("#AssesmentYearId").val(),
                Attendencecode: $("#Attendencecode").val(),
                LockUnlockType: $("#LockUnlockType").val(),
                ModifiedEmployeeId: $("#ModifiedEmployeeId").val(),
                UserInfo: $("#UserInfo").val(),
                StartDate: $("#StartDate").val(),
                EndDate: $("#EndDate").val(),
                TdsTypeId: $("#TdsTypeId").val()
            }
        }

        
        //// Attendence code 
        if (JSON.parse($('#hdnAcUser').val().toLowerCase())) {
            var Attendencecodeinput = document.getElementById("Attendencecode");
            Attendencecodeinput.addEventListener("keypress", function (event) {
                if (event.key === "Enter") {
                    //event.preventDefault();
                    intializeGrid();
                }
            });
            $('#Attendencecode').keypress(function (e) {

                var charCode = (e.which) ? e.which : event.keyCode

                if (String.fromCharCode(charCode).match(/[^0-9]/g))

                    return false;

            });

            // UserInfoinput code
            var UserInfoinput = document.getElementById("UserInfo");
            UserInfoinput.addEventListener("keypress", function (event) {
                if (event.key === "Enter") {
                    //event.preventDefault();
                    intializeGrid();
                }
            });
        }
       

    }

   

    $(function () {
        var self = new index();
        self.init();
    });
    $("#btnSearch").on("click", function () {

        index();

        var href = "empInvestment/DownloadReport";
        var AssesmentYearId = $("#AssesmentYearId").val()
        var Attendencecode = $("#Attendencecode").val()
        var LockUnlockType = $("#LockUnlockType").val()
        var ModifiedEmployeeId = $("#ModifiedEmployeeId").val()
        var UserInfo = $("#UserInfo").val()
        var StartDate = $("#StartDate").val()
        var EndDate = $("#EndDate").val()
        var TdsTypeId = $("#TdsTypeId").val()

        tableCount = $("#grid-tds-table").DataTable().data().length
        if (tableCount > 0) {
            $("#btnexporttoexcel").attr('href', href + "?TDSTypeId=" + TdsTypeId + "&AssesmentYearId=" + AssesmentYearId + "&Attendencecode=" + Attendencecode + "&LockUnlockType=" + LockUnlockType + "&ModifiedEmployeeId=" + ModifiedEmployeeId + "&UserInfo=" + UserInfo + "&StartDate=" + StartDate + "&EndDate=" + EndDate);
            $("#btnexporttoexcel").removeAttr('disabled', 'disabled');
        }
        else {
            $("#btnexporttoexcel").attr('disabled', 'disabled');
            $("#btnexporttoexcel").attr('href', "javascript:void(0)");
        }
        
    });
    $('#AssesmentYearId').val(currentYearRangeId)
    
}(jQuery));