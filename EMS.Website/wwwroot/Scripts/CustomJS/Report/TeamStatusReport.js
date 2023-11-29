(function ($) {
    function index() {
        var $this = this, grid;

        google.charts.load('current', { 'packages': ['corechart'] });
        //google.charts.setOnLoadCallback(LoadData);            

        function InitializeEvents() {
            $('#DateFrom').datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: $("#DateTo").val(),
                changeMonth: true,
                changeYear: true,
                onSelect: function (selectedDate) {
                    $("#DateTo").datepicker("option", "minDate", selectedDate);
                    $('#IsCurrentMonth').prop('checked', false);
                    $('#IsCurrentWeek').prop('checked', false);
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
                    $('#IsCurrentMonth').prop('checked', false);
                    $('#IsCurrentWeek').prop('checked', false);
                }
            });

            $('#btnGo').off('click').on('click', function () {
                $('.div-chart').hide();
                $('#grid-details tbody').html('');
                $('.div-details').hide();

                if ($('#Uid').val() == "") {
                    alert('Please select a Team Lead');
                    return false;
                }
                LoadData();
            });
            $('#btnGoAll').off('click').on('click', function () {
                $('.div-chart').hide();
                $('#grid-details tbody').html('');
                $('.div-details').hide();
                $('#Uid').val("");

                LoadData();
            });

            $('#IsCurrentMonth').on("click", function () {
                if ($("#IsCurrentMonth").is(":checked")) {
                    $('#IsCurrentWeek').prop('checked', false);
                    GetStartEndDate();
                    LoadData();
                }
            });

            $('#IsCurrentWeek').on("click", function () {
                if ($("#IsCurrentWeek").is(":checked")) {
                    $('#IsCurrentMonth').prop('checked', false);
                    GetStartEndDate();
                    LoadData();
                }
            });
        }

        function InitialVisibility() {
            if ($('#Uid').val() == "") {
                $('.div-chart').hide();
                $('.div-details').hide();
            }
        }
        function GetStartEndDate() {
            if ($("#IsCurrentMonth").is(":checked")) {
                var date = new Date();
                var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
                var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
                $("#DateFrom").datepicker("setDate", firstDay);
                $("#DateTo").datepicker("setDate", lastDay);

                //$("#DateFrom").datepicker("option", "minDate", firstDay);
                //$("#DateTo").datepicker("option", "maxDate", lastDay);
            }
            else if ($("#IsCurrentWeek").is(":checked")) {
                var curr = new Date; // get current date
                var first = curr.getDate() - curr.getDay() + 1; // First day is the day of the month - the day of the week
                var last = first + curr.getDay() - 1; // last day is the first day + 6

                var firstday = new Date(curr.setDate(first));
                var lastday = new Date(curr.setDate(last));
                $("#DateFrom").datepicker("setDate", firstday);
                $("#DateTo").datepicker("setDate", lastday);
                //$("#DateFrom").datepicker("option", "minDate", firstday);
                //$("#DateTo").datepicker("option", "maxDate", lastday);
            }
        }

        function LoadData() {

            var data = {
                DateFrom: $('#DateFrom').val(),
                DateTo: $('#DateTo').val(),
                Uid: $('#Uid').val(),
                IsCurrentWeek: $("#IsCurrentWeek").is(":checked") ? true : false,
                IsCurrentMonth: $('#IsCurrentMonth').is(":checked") ? true : false
            }
            $.ajax({
                url: domain + "report/TeamStatusReport",
                type: "POST",
                data: data,
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    console.log(err.message);
                },
                success: function (data) {
                    if (data.length == 0) {
                        CustomAlerts.error("Error !!!", "No running developer(s) found in the selected date range.");
                        DrawChart(data);
                    }
                    else {
                        $('.div-chart').show();
                        DrawChart(data);
                    }
                    return false;
                }
            });
            return false;
        }

        function DrawChart(data) {
            var dataTable = new google.visualization.DataTable();
            dataTable.addColumn('string', 'Date');
            dataTable.addColumn('number', 'Total Resources');
            dataTable.addColumn('number', 'Paid Resources');
            // A column for custom tooltip content
            dataTable.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
            var vAxisMaxValue = 0;
            $.each(data, function (i, item) {
                dataTable.addRow([item.date, item.memberOfTeam, item.noOfEmployee, createCustomHTMLContent(item.date, item.tlName, item.noOfEmployee, item.memberOfTeam)]);
                vAxisMaxValue = item.memberOfTeam;
            });
            //-------option for chart------------
            var options = {
                title: '',
                pointSize: 3,
                tooltip: { isHtml: true },
                legend: 'none',
                chartArea: {
                    width: '80%'
                },
                colors: ['#3a8a53', '#b0120a'],
                hAxis: {
                    title: 'Date'
                },
                vAxis: {
                    title: 'Resources',
                    minValue: 0,
                    maxValue: vAxisMaxValue
                }
            };
            var chart = new google.visualization.LineChart(document.getElementById('g_chart'));
            // chart.draw(data, options);
            chart.draw(dataTable, options);
            // Listen for the 'select' event, and call function selectHandler() when
            // the user selects something on the chart.
            google.visualization.events.addListener(chart, 'select', selectHandler);
            function selectHandler() {
                var selectedItem = chart.getSelection()[0];
                if (selectedItem) {
                    var value = dataTable.getValue(selectedItem.row, selectedItem.column);
                    // console.log('The user selected ' + value + "-selected row " + dataTable.getValue(selectedItem.row, 0));
                    var param = {
                        DateFrom: dataTable.getValue(selectedItem.row, 0),
                        Uid: $('#Uid').val(),
                        IsCurrentWeek: $("#IsCurrentWeek").is(":checked") ? true : false,
                        IsCurrentMonth: $('#IsCurrentMonth').is(":checked") ? true : false
                    }

                    $.ajax({
                        url: domain + "report/TeamStatusReportDetails",
                        type: "POST",
                        data: param,
                        error: function (xhr, status, error) {
                            var err = eval("(" + xhr.responseText + ")");
                            console.log(err.message);
                        },
                        success: function (data) {
                            $('#grid-details tbody').html('');
                            $('.div-details').show();
                            $.each(data, function (i, item) {

                                if (item.status.toLowerCase() == 'running' || item.status.toLowerCase() == 'additional support') {
                                    $('#grid-details tbody').append('<tr><td>' + item.date + '</td><td><span style=color:#0f7801; font-weight:bold;>' + item.employee + '</span></td><td><span style=color:#0f7801; font-weight:bold;>' + item.projectName + '<span></td><td><span style=color:#0f7801; font-weight:bold;>' + item.status + '<span></td></tr>');
                                }
                                else if (item.status.toLowerCase() == 'free')  {
                                    $('#grid-details tbody').append('<tr><td>' + item.date + '</td><td><span style=color:red; font-weight:bold;>' + item.employee + '</span></td><td>' + item.projectName + '</td><td><span style=color:red; font-weight:bold;>' + item.status + '</span></td></tr>');
                                }
                                else {
                                    $('#grid-details tbody').append('<tr><td>' + item.date + '</td><td>' + item.employee + '</td><td>' + item.projectName + '</td><td>' + item.status + '</td></tr>');
                                }
                            });
                            return true;
                        }
                    });
                }
            }

            function createCustomHTMLContent(date, tlName, noOfEmployee, memberOfTeam) {
                return '<div style="padding:5px 5px 5px 5px;"><span class="badge badge-light">' + date + ' ' + tlName + '</span>' +
                    '<br/>Paid Resources : <span class="badge badge-success"> ' + noOfEmployee + '/' + memberOfTeam + '</span>' +
                    '</div>';
            };

            return false;
        }

        $this.init = function () {
            InitializeEvents();
            InitialVisibility();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));