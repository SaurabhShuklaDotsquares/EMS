(function () {
    function EmployeeActivityReport() {
        $this = this;

        function initialize() {
            $("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onSelect: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);
                    GenerateDays();
                }
            });
            $("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: new Date(),
                changeMonth: true,
                changeYear: true,
                onSelect: function (selectedDate) {
                    $("#StartDate").datepicker("option", "maxDate", selectedDate);
                    GenerateDays();
                }
            });

            $('#searchDate').on('click', function () {

                $("#sDate").val($("#StartDate").val());
                $("#eDate").val($("#EndDate").val());

                LoadLeavesGrid();
            });

            $("#modal-activity-detail").on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-performance-detail").on('loaded.bs.modal', function () {
                var modal = $(this);

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
                });

            $("#modal-perf-extra-detail").on('loaded.bs.modal', function () {
                var modal = $(this);

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        function LoadLeavesGrid() {
            $('.divoverlay').removeClass('hide');

            //var data = { deptId: $('#DeptId').val(), pmId: $('#PMId').val(), startDate: $('#sDate').val(), endDate: $('#eDate').val(), noOfFreeDays: $("#NoOfFreeDays").val() };
            var dataOuter = {
                StartDate: $('#sDate').val(),
                EndDate: $('#eDate').val(),
                PmId: $('#PMId').val(),
                DeptId: $('#DeptId').val(),
                NoOfFreeDays: $("#NoOfFreeDays").val()
            };
            var grid = new Global.GridHelper('#grid-report', {
                serverSide: true,
                destroy: true,
                "paging": false,
                "bFilter": false,
                "bSort": false,
                ajax: {
                    url: domain + 'report/employeeactivityreport',
                    type: 'POST',
                    data: dataOuter
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "25%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 6 }
                ],
                columns: [
                    { name: 'rowId', data: 'rowId', title: '#', sortable: false, searchable: false },
                    { name: 'Name', data: 'name', title: 'Name', sortable: false, searchable: false, visible: true },
                    { name: 'Department', data: 'department', title: 'Department', sortable: false, searchable: false, visible: true },
                    { name: 'Designation', data: 'designation', title: 'Designation', sortable: false, searchable: false, visible: true },
                    { name: 'TeamManager', data: 'teamManager', title: 'Team Manager', sortable: false, searchable: false, visible: true },
                    { name: 'TotalFreeDays', data: 'totalFreeDays', title: 'Free Days', sortable: false, searchable: false },
                    {
                        name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                        render: function (data, type, row, meta) {
                            var actionButtons = $("<a/>", {
                                class: "btn btn-default btn-sm activity-details",
                                'data-uid': row.id,
                                html: $("<i/>", {
                                    class: "fa fa-info-circle",
                                    style: "color:black"
                                }).get(0).outerHTML + "&nbsp; Details",
                            }).get(0).outerHTML + "&nbsp; "
                                + $("<a/>", {
                                href: domain + "performance/GetPerformance?uid=" + row.id +"&startDate=" + dataOuter.StartDate + "&endDate=" + dataOuter.EndDate,
                                id: "performance",
                                title: "performance",
                                'data-toggle': "modal",
                                'data-target': "#modal-performance-detail",
                                'data-backdrop': "static",
                                class:"btn btn-default btn-sm",
                                html: $("<i/>", {
                                    class: "glyphicon glyphicon-stats",
                                    style: "color:black"
                                }),
                            }).get(0).outerHTML;

                            return actionButtons;
                        }
                    },
                ],
                "fnInitComplete": function (oSettings, json) {

                    var html = '';

                    if (json.filteredDates != undefined) {
                        html += '<label style="color:#e69400">Dates Filtered :</label> ' + json.filteredDates
                    }

                    if (json.totalWorkingDays != undefined) {
                        html += '<br><label style="color:#e69400">Working Days :</label> ' + json.totalWorkingDays
                    }

                    $('#statistic').html(html);

                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');

                    if (oSettings.fnRecordsDisplay() > 0) {
                        $('#btnDownload').show();
                    }
                    else {
                        $('#btnDownload').hide();
                    }

                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                }
            });

            grid.on('page.dt', function () {
                $('.divoverlay').removeClass('hide');
            });

            grid.on('click', 'a.activity-details', function () {
                var uid = $(this).data('uid');

                if (uid) {
                    var startDate = $('#sDate').val(), endDate = $('#eDate').val();

                    $('#modal-activity-detail').modal({
                        remote: encodeURI(domain + "report/employeeactivities/" + uid + "?startDate=" + startDate + "&endDate=" + endDate)
                    });
                }
            });
        }

        function GenerateDays() {

            var noOfFreeDays = $("#NoOfFreeDays");
            noOfFreeDays.empty();

            var oneDay = 24 * 60 * 60 * 1000;
            var firstDate = $("#StartDate").datepicker('getDate');
            var secondDate = $("#EndDate").datepicker('getDate');

            var diffDays = (Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay))));

            var daysCount = diffDays + 1;
            diffDays = 0;

            while (daysCount > 0) {

                if (firstDate.getDay() != 0 && firstDate.getDay() != 6) {
                    diffDays++;
                }

                firstDate = new Date(firstDate.getTime() + oneDay);
                daysCount--;
            }

            while (diffDays > 0) {
                noOfFreeDays.prepend("<option value='" + diffDays + "'>" + diffDays + (diffDays > 1 ? " Days" : " Day") + "</option>");
                diffDays--;
            }

            noOfFreeDays.prepend("<option value='" + "" + "'>" + "All" + "</option>");

            noOfFreeDays.val("");
        }

        $this.init = function () {
            LoadLeavesGrid();
            initialize();
            GenerateDays();
        }
    }

    $(function () {
        var self = new EmployeeActivityReport();
        self.init();
    });
}(jQuery));