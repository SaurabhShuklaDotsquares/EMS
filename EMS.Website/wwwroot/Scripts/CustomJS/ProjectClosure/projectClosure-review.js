(function ($) {
    function Review() {
        var $this = this, grid, formAddEdit, teamSummary, localeOpts;

        function getfilter() {
            var dateRange = $('#ReportDateRange').val();
            var dateFrom = '', dateTo = '';

            if (dateRange && dateRange != '') {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }

            return data = {
                TextSearch: $("#txtSearch").val(),
                DateFrom: dateFrom,
                DateTo: dateTo,
                BA: $("#Uid_BA").val(),
                TL: $("#Uid_TL").val(),
                PMUid: $("#PMUid").val(),
                ReviewPercentageId: $("#ReviewPercentageId").val(),
                ProjectionData: $("#ProjectionData").val(),
            }
        }

        function loadGrid() {
            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-projectcloser', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "ordering": false,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "projectclosure/projectionreport",
                    type: "POST",
                    data: getfilter()
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "8%", "targets": 2 },
                    { "width": "8%", "targets": 3, },
                    { "width": "8%", "targets": 4, },
                    { "width": "8%", "targets": 5 },
                    { "width": "8%", "targets": 6, },
                    { "width": "8%", "targets": 7, },
                    { "width": "20%", "targets": 8, },
                    { "width": "5%", "targets": 9, "className": "rowCenterText" },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "S.No", sortable: false, searchable: false, visible: true },
                        {
                            name: "ClientName", data: "clientName", title: "CLIENT", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var projectDetails = "";
                                projectDetails += row.projectName;
                                projectDetails += "<br/>CRM ID: [" + row.crmProjectId + "]<br/>" + data + (row.pCountry ? "[" + row.pCountry + "]" : "");

                                return projectDetails;
                            }
                        },
                        { name: "PromisingPercentage", data: "promisingPercentage", title: "Promising Percentage", sortable: false, searchable: false },
                        { name: "Developers", data: "developers", title: "Number of Developers", sortable: false, searchable: false, visible: true },
                        { name: "NextStartDate", data: "nextStartDate", title: "May start again on", sortable: false, searchable: false, visible: true },
                        {
                            name: "AddedBy", data: "addedBy", title: "Added By", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return data + "<div style='font-size:11px;'>Review Date : " + row.createdDate + "</div>";
                            }
                        },
                        {
                            name: "BA", data: "ba", title: "BA/ TL NAME", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var baTLNames = "";
                                if (row.ba) {
                                    baTLNames += row.ba + (row.baId == row.addedBy ? "&nbsp; <img border='0' src='./Styles/images/Plus.png' alt='' title='Request Added By BA' height='15px'>" : "");
                                }

                                if (row.tl) {
                                    baTLNames += (baTLNames != "" ? " / " : "") + "<div>" + row.tl + (row.tlId == row.addedBy ? "&nbsp; <img border='0' src='./Styles/images/Plus.png' alt='' title='Request Added By TL' height='15px'>" : "") + "</div>";
                                }
                                return baTLNames;
                            }
                        },
                        {
                            name: "PM", data: "pm", title: "Team Manager", sortable: false, searchable: false, visible: true
                        },
                        { name: "Comments", data: "comments", title: "Comments", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {

                                var actionButtons = "";

                                if (data.allowUpdateStatus) {
                                    actionButtons += $("<a/>", {
                                        id: "statusProjectClosure",
                                        title: "Status",
                                        href: domain + "projectclosure/updateprojectstatus/" + data.id + "?fromreview=true",
                                        text: " Status",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-status-projectClosure",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            'class': "fa fa-check-circle-o",
                                            style: "font-size:14px"
                                        }),
                                    }).get(0).outerHTML;
                                }

                                return actionButtons;
                            }
                        },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if(aData.bgStyle == "dark"){
                        $(nRow).addClass('darkred')
                    }
                    else if (aData.bgStyle == "light") {
                        $(nRow).addClass('lightred')
                    }
                },
                "fnDrawCallback": function (oSettings) {

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

            grid.on('preXhr.dt', function () {
                $('.divoverlay').removeClass('hide');
            }).on('xhr.dt', function () {
                $('.divoverlay').addClass('hide');
            });
        }

        function InitializeControl() {

            teamSummary = $("#div_baconversion");

            var start = moment();
            var end = moment().add(29, 'days');
            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            };

            function rangeChangeCB(start, end) {
                $('#ReportDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
            }

            $('#ReportDateRange').daterangepicker({
                "locale": localeOpts,
                startDate: start,
                endDate: end,
                autoUpdateInput: false,
                ranges: {
                    'Today': [moment(), moment()],
                    'Tomorrow': [moment().add(1, 'days'), moment().add(1, 'days')],
                    'Next 7 Days': [moment(), moment().add(6, 'days')],
                    'Next 15 Days': [moment(), moment().add(14, 'days')],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Next Month': [moment().add(1, 'month').startOf('month'), moment().add(1, 'month').endOf('month')]
                }
            }, rangeChangeCB);

            rangeChangeCB(start, end);

            $("#btnSearch").on("click", function () {
                loadGrid();
                LoadSummary();
                LoadRunningDeveloper();
                LoadProjectionReport();
                LoadProjectionReportPast();

            });

            $("#btnReset").on("click", function () {
                $("#txtSearch").val('');
                $("#chaseStatus").val('');
                $("#ReportDateRange").val('');
                $("#Uid_BA").val('');
                $("#CRMStatusId").val('');
                $("#ProjectStatus").val('');
                $("#ProjectionData").val('1');
                LoadRunningDeveloper();
                LoadProjectionReport();
                LoadProjectionReportPast();
                loadGrid();
                LoadSummary();
            });

            $("#txtSearch").on("keypress", function (e) {
                if (e.keyCode == 13) {
                    $("#btnSearch").click();
                    return false;
                }
            });

            $("#clrFilterDate").click(function () {
                $('#ReportDateRange').val('');
            });
        }

        function LoadSummary() {
            teamSummary.empty();
            var data = getfilter();
            $.ajax({
                url: domain + "projectclosure/projectionsummary",
                type: 'POST',
                data: data,
                success: function (result) {
                    teamSummary.html(result);
                },
                error: function (ex) {

                }
            });
        }

        //function LoadProjectionReport() {
        //    $.ajax({
        //        url: domain + "projectclosure/ProjectionReportWeekWise",
        //        type: 'POST',
        //        data: getfilter(),
        //        success: function (result) {

        //            var tableString = "<table class='table-bordered text-align-center'  style='width:100%;'>";
        //            var headerMonthNo = "<tr><th rowspan='4'style='vertical-align: bottom;padding-left:2px;padding-right:2px;width:80px;'>Forcast Occupancy</th>";
        //            var headerWeekNo = "<tr>";
        //            var rowDates = "<tr class='font-size-custom'>";
        //            var rowForecastOccupancy = "<tr class='bold-text'>";
        //            result.months.forEach(function (month) {
        //                headerMonthNo += "<th style='text-align:center; background-color:" + month.color + "' colspan='" + month.colspan + "'>" + month.monthName + "<small> (" + month.totalMonthOccupancy+")</small>" + "</th>";
        //            });
        //            headerMonthNo += "</tr>";
        //            result.months.forEach(function (month) {
        //                month.weeks.forEach(function (week) {
        //                    headerWeekNo += "<th style='background-color:" + month.color+"'>" + "Week " + week.weekNo + "</th>";
        //                    rowDates += "<td style='background-color:" + month.color + "'>" + week.startDateEndDate + "</td>";
        //                    rowForecastOccupancy += "<td style='background-color:" + month.color + "'>" + week.developerCount + "</td>";
                            
        //                });
                            
        //            });
        //            headerWeekNo += "</tr>";
        //            rowDates += "</tr>";
        //            rowForecastOccupancy += '</tr>'
        //            tableString += headerMonthNo+headerWeekNo + rowDates + rowForecastOccupancy;
        //            tableString += "</table>";
        //            $('#divProjectionReport').html(tableString);

        //        },
        //        error: function (ex) {
        //            console.log(ex)
        //        }
        //    });
        //}

        function LoadRunningDeveloper() {
            $.ajax({
                url: domain + "projectclosure/RunningDevelopers",
                type: 'POST',
                data:getfilter(),
                success: function (result) {
                    var tableString = "<table class='table-bordered text-align-center'  style='width:100%;'>";
                    var header = "<tr><th style='vertical-align: bottom;'>Running Developers</th></tr>";
                    var data = "<tr><td>" + result.runningDevelopers + "</td></tr>";
                    tableString += header + data;
                    tableString += "</table>";
                    $('#divRunningDeveoper').html(tableString);

                },
                error: function (ex) {
                    console.log(ex)
                }
            });
        }
        
        function LoadProjectionReportPast() {
            $.ajax({
                url: domain + "projectclosure/ProjectionReportPast",
                type: 'POST',
                data: getfilter(),
                success: function (result) {
                    var tableString = "<table class='table-bordered text-align-center tableAnchor converted' style='width:100%'>";
                    var headerWeekNo = "<tr><th rowspan='4' style='vertical-align: bottom;width:80px;'>Past Occupancy</th>";
                    var rowDates = "<tr>";
                    var rowForecastOccupancy = "<tr class='bold-text'>";
                    var rowConverted = "<tr class='bold-text'><th>Converted</th>";
                    result.forEach(function (item) {
                        headerWeekNo += "<th>" + "Week " + item.weekNo + "</th>";
                        rowDates += "<td>" + item.startDateEndDate + "</td>";

                        if (item.developerCount > 0) {
                            rowForecastOccupancy += "<td><a href='javascript:void(0)' data-startdate='" + item.startDate + "' data-enddate='" + item.endDate+"'>" + item.developerCount + "</a></td>";
                        }
                        else {
                            rowForecastOccupancy += "<td>" + item.developerCount + "</td>";
                        }
                        if (item.convertedCount>0) {

                            rowConverted += "<td><a href='javascript:void(0)' data-converted='converted' data-startdate='" + item.startDate + "' data-enddate='" + item.endDate + "'>" + item.convertedCount + "</a></td>";
                        }
                        else {
                            rowConverted += "<td>" + item.convertedCount + "</td>";
                        }
                    });
                    headerWeekNo += "<tr>";
                    rowDates += "</tr>";
                    rowForecastOccupancy += '</tr>'
                    rowConverted += "</tr>";
                    tableString += headerWeekNo + rowDates + rowForecastOccupancy+rowConverted;
                    tableString += "</table>";
                    $('#divPastProjectionReport').html(tableString);
                    $('.tableAnchor a').on("click", function () {
                        $('#ReportDateRange').val(moment($(this).attr("data-startdate")).format(localeOpts.format) + localeOpts.separator + moment($(this).attr("data-enddate")).format(localeOpts.format));
                        if ($(this).attr("data-converted") == "converted") {
                            ProjectionData: $("#ProjectionData").val("3")
                        }
                        else {
                            ProjectionData: $("#ProjectionData").val("2")
                        }
                        LoadSummary();
                        loadGrid();
                        $("#ProjectionData").val("1")

                    });
                },
                error: function (ex) {

                }
            });
        }

        function LoadProjectionReport() {
            $.ajax({
                url: domain + "projectclosure/ProjectionReportWeekWise",
                type: 'POST',
                data: getfilter(),
                success: function (result) {

                    var tableString = "<table class='table-bordered text-align-center' style='width:100%'>";
                    var headerWeekNo = "<tr><th rowspan='4' style='vertical-align: bottom;width:80px;'>Forcast Occupancy</th>";
                    var rowDates = "<tr>";
                    var rowForecastOccupancy = "<tr class='bold-text'>";
                    var strColors = ["#ff6347", "#90ee90", "#add8e6", "#fafad2", "#fbca95", "#b84dff"];
                    var loopCount = 1, colorCount = 0;
                    result.forEach(function (item) {
                        
                        headerWeekNo += "<th style='background-color:" + strColors[colorCount]+";'>" + "Week " + item.weekNo + "</th>";
                        rowDates += "<td style='background-color:" + strColors[colorCount] + ";'>" + item.startDateEndDate + "</td>";
                        rowForecastOccupancy += "<td style='background-color:" + strColors[colorCount] + ";'>" + item.developerCount + "</td>";
                        if (loopCount===4) {
                            loopCount = 0;
                            ++colorCount;
                        }
                        ++loopCount;

                    });
                    headerWeekNo += "<tr>";
                    rowDates += "</tr>";
                    rowForecastOccupancy += '</tr>'
                    tableString += headerWeekNo + rowDates + rowForecastOccupancy;
                    tableString += "</table>";
                    $('#divProjectionReport').html(tableString);

                },
                error: function (ex) {

                }
            });
        }

        $this.refreshReviews = function () {
            grid.ajax.reload(null, false);
            LoadSummary();
        };

        $this.init = function () {
            InitializeControl();
            LoadSummary();
            LoadRunningDeveloper();
            LoadProjectionReport();
            LoadProjectionReportPast();
            loadGrid();
        };
    }
    $(function () {
        var self = new Review();
        self.init();

        $.fn.ProjectClosureReview = self;
    });
}(jQuery));