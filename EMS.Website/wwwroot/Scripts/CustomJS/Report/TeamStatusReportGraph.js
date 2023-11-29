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
        }

        function InitialVisibility() {
            if ($('#Uid').val() == "") {
                $('.div-chart').hide();
                $('.div-details').hide();
            }
        }

        function LoadData() {
            var data = {
                DateFrom: $('#DateFrom').val(),
                DateTo: $('#DateTo').val(),
                Uid: $('#Uid').val()
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
            dataTable.addColumn('number', 'Paid Resources');
            // A column for custom tooltip content
            dataTable.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
            var vAxisMaxValue = 0;
            $.each(data, function (i, item) {
                dataTable.addRow([item.date, item.noOfEmployee, createCustomHTMLContent(item.date, item.tlName, item.noOfEmployee, item.memberOfTeam)]);
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
                colors: ['#b0120a'],
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
                        Uid: $('#Uid').val()
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
                                $('#grid-details tbody').append('<tr><td>' + item.date + '</td><td>' + item.employee + '</td><td>' + item.projectName + '</td><td>' + item.status + '</td></tr>');
                            });
                            return false;
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