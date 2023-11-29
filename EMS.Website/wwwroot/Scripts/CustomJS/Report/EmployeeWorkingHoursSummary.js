(function ($) {
    function index() {
        var $this = this, grid;

        function InitializeEvents() {
            $('#DateFrom').datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: $("#DateTo").val(),
                changeMonth: true,
                changeYear: true,
                onSelect: function (selectedDate) {
                    $("#DateTo").datepicker("option", "minDate", selectedDate);
                }
            });
            $('#DateTo').datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: new Date(),
                minDate: $("#DateFrom").val(),
                changeMonth: true,
                changeYear: true,
                onSelect: function (selectedDate) {
                    $("#DateFrom").datepicker("option", "maxDate", selectedDate);
                }
            });
            $('#WorkingHourTypeId').off('change').on('change', function () {
                if ($(this).val() == 1) {
                    $('.div-Employee').hide();
                    $("#Uid option:selected").removeAttr("selected");
                    $('.div-Department').show();
                }
                else if ($(this).val() == 2) {
                    $('.div-Department').hide();
                    $("#DepartmentId option:selected").removeAttr("selected");
                    $('.div-Employee').show();
                }
                else
                {
                    $('.div-Employee').hide();
                    $("#Uid option:selected").removeAttr("selected");
                    $('.div-Department').hide();
                    $("#DepartmentId option:selected").removeAttr("selected");
                }
            })
            $('#btnGo').off('click').on('click', function () {
                if ($('#WorkingHourTypeId').val() == 1 && $('#DepartmentId').val() == "") {
                    alert('Please select a department');
                    return false;
                }
                else if ($('#WorkingHourTypeId').val() == 2 && $('#Uid').val() == "") {
                    alert('Please select an employee');
                    return false;
                }
                loadtaskgrid();
            });
        }
        function InitialVisibility() {
            if ($('#WorkingHourTypeId').val() == "") {
                $('.div-Department').hide();
                $('.div-Employee').hide();
            }
        }

        function loadtaskgrid() {
            $('.divoverlay').removeClass('hide');
            var data = {
                DateFrom:$('#DateFrom').val(),
                DateTo: $('#DateTo').val(),
                Uid:$('#Uid').val(),
                DepartmentId:$('#DepartmentId').val(),
                WorkingHourTypeId:$('#WorkingHourTypeId').val()
            }
            grid = new Global.GridHelper('#grid-EmployeeWorkingHour-table', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                ordering: false,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "bFilter":false,
                "dom": '<"pull-left"f><"pull-right"l>tip',
                "bPaginate":false,
                ajax:
                {
                    url: domain + "report/GetEmployeeWorkingHoursSummary",
                    type: "POST",
                    data:data
                },


                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "20%", "targets": 1 },
                    { "width": "48%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", sortable: false, searchable: false, visible: true },
                        { name: "MemberName", data: "memberName", sortable: false, searchable: false, visible: true },
                        { name: "ProjectHtml", data: "projectHtml", sortable: false, searchable: false, visible: true },
                        //{
                        //    name: "ProjectName", data: "projectName", sortable: false, searchable: false, visible: true,
                        //    "render": function (data, type, row, meta) {
                        //        //return '<a href="' + data + '">Download</a>';
                        //        return data;
                        //    }
                        //},
                        { name: "TotalPlanHours", data: "totalPlanHours", sortable: false, searchable: false, visible: true },
                        { name: "TotalActualHours", data: "totalActualHours", sortable: false, searchable: false, visible: true }
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    $(nRow).css('background-color', aData.colorCode); 
                    
                    $(nRow).children(':nth-child(3)').css('text-align', 'center');
                    if ($('#WorkingHourTypeId').val() == 3) {
                        $(nRow).children(':nth-child(4)').css('text-align', 'center');
                        $(nRow).children(':nth-child(5)').css('text-align', 'center');
                    } else {
                        $(nRow).children().css('vertical-align', 'top');
                        //$(nRow).children(':nth-child(3)').css('vertical-align', 'top');
                        $(nRow).children(':nth-child(4)').css('text-align', 'right');
                        $(nRow).children(':nth-child(5)').css('text-align', 'right');
                    }
                    
                    //use below one for project specific color
                    //$('tr', nRow).css('background-color', 'red');
                },

                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                },
                "fnInitComplete": function (oSettings, json) {
                    if (oSettings.fnRecordsDisplay() > 0) {
                        $('.export-btn').show();
                    }
                    else {
                        $('.export-btn').hide();
                    }
                    $(".divoverlay").addClass('hide');
                    var html = '<div class="col-md-6 pull-right text-right">';
                    if (json.TotalPlannedHours != undefined && json.TotalPlannedHoursFormatted !=undefined && json.TotalPlannedHours > 0) {
                        $('.planned-hours').removeClass('hide')
                        $('.total-planned-hours').html(json.TotalPlannedHoursFormatted);
                    }
                    else {
                        $('.planned-hours').addClass('hide');
                    }
                    if (json.TotalActualHours != undefined && json.TotalActualHoursFormatted != undefined && json.TotalActualHours > 0) {
                        $('.actual-hours').removeClass('hide')
                        $('.total-actual-hours').html(json.TotalActualHoursFormatted);
                    }
                    else {
                        $('.actual-hours').addClass('hide');
                    }
                }
            });
        }

        

        $this.init = function () {
            InitializeEvents();
            loadtaskgrid();
            InitialVisibility();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));