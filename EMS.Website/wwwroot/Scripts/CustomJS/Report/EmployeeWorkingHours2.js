(function ($) {
    function index() {
        var $this = this, grid;

        function InitializeEvents() {
            $('#DateFrom').datepicker({
                dateFormat: "dd/mm/yy",
                //maxDate: $("#DateTo").val(),
                changeMonth: true,
                changeYear: true,
                //onSelect: function (selectedDate) {
                //    $("#DateTo").datepicker("option", "minDate", selectedDate);
                //}
            });
            //$('#DateTo').datepicker({
            //    dateFormat: "dd/mm/yy",
            //    maxDate: new Date(),
            //    minDate: $("#DateFrom").val(),
            //    changeMonth: true,
            //    changeYear: true,
            //    onSelect: function (selectedDate) {
            //        $("#DateFrom").datepicker("option", "maxDate", selectedDate);
            //    }
            //});
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
                else {
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
                DateFrom: $('#DateFrom').val(),
                DateTo: $('#DateFrom').val(),
                Uid: $('#Uid').val(),
                DepartmentId: $('#DepartmentId').val(),
                WorkingHourTypeId: $('#WorkingHourTypeId').val()
            }
            grid = new Global.GridHelper('#grid-EmployeeWorkingHour-table', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                ordering: false,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "bFilter": false,
                "dom": '<"pull-left"f><"pull-right"l>tip',
                "bPaginate": false,
                ajax:
                {
                    url: domain + "report/GetEmployeeWorkingHoursData2",
                    type: "POST",
                    data: data
                },


                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "18%", "targets": 1 },
                    { "width": "80%", "targets": 2 },
                    //{ "width": "30%", "targets": 3 },
                    //{ "width": "10%", "targets": 4, className: "text-right padding-right" },
                    //{ "width": "10%", "targets": 5, className: "text-right padding-right" }
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", sortable: false, searchable: false, visible: true },
                        { name: "MemberName", data: "memberName", sortable: false, searchable: false, visible: true },
                        { name: "HTML", data: "html", sortable: false, searchable: false, visible: true },
                        //{ name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true },
                        //{ name: "TaskName", data: "taskName", title: "Task Name", sortable: false, searchable: false, visible: true },
                        //{ name: "PlanHour", data: "planHour", title: "Planned Hours", sortable: false, searchable: false, visible: true },
                        //{ name: "ActualHours", data: "actualHours", title: "Actual Hours", sortable: false, searchable: false, visible: true },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    $(nRow).children(':nth-child(3)').css('text-align', 'center');
                    $(nRow).children().css('vertical-align', 'top');
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
                        var _href = $(".export-btn").data("href");
                        $(".export-btn").attr("href", _href + "?DateFrom=" + $('#DateFrom').val() + "&DateTo=" + $('#DateFrom').val() + "&Uid=" + $('#Uid').val() + "&DepartmentId=" + $('#DepartmentId').val() + "&WorkingHourTypeId=" + $('#WorkingHourTypeId').val());
                    }
                    else {
                        $('.export-btn').hide();
                    }
                    var html = '<div class="col-md-6 pull-right text-right">';
                    if (json.TotalPlannedHours != undefined && json.TotalPlannedHoursFormatted != undefined && json.TotalPlannedHours > 0) {

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
                    $(".divoverlay").addClass('hide');
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