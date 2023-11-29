(function ($) {
    function index() {
        var $this = this, grid, formAddEdit;
        function initializeForm() {
            //formAddEdit = new Global.FormHelper($("#frm-create-edit-employeefeedback form"), {
            //    updateTargetId: "validation-summary",
            //    validateSettings: { ignore: '' }
            //});
            var start = moment().subtract(29, 'days');
            var end = moment();
            //var start = moment().subtract(29, 'days');
            //var end = moment();
            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            };

            function rangeChangeCB(start, end) {
                $('#JoiningDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
            }

            $('#JoiningDateRange').daterangepicker({
                "locale": localeOpts,
                startDate: start,
                endDate: end,
                autoUpdateInput: false,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, rangeChangeCB);

            // rangeChangeCB(start, end);
            $("#JoiningDateRange").val('');
        }

        //function leavingdate() {
        //    $("#leavingDate").datepicker({
        //        defaultDate: "+1w",
        //        dateFormat: "dd/mm/yy",
        //        changeMonth: true,
        //        changeYear: true,
        //        numberOfMonths: 1,
        //        minDate: 0
        //    });
        //}
        function initializeEvent() {
            $('#btnReset').on('click', function () {
                $('#PMList').val('');
                $('#DepartmentList').val('');
                $('#DepartmentList').val('');
                $('#txt_search').val('');
                $('#JoiningDateRange').val('');
                $('#isEligibleForRehire').val('');
                $('#isVoluntryExit').val('');
                //$('#isVoluntaryExit').val('');

                // reset the ReasonList multiselect
                $('#ReasonList').multiselect('deselectAll', false);
                $('#ReasonList').multiselect('updateButtonText');

                loadfeedbackgrid();
                loadfeedbackreason();
                loadLeaveReasonByPM();
            });

            $('#clrFilterDate').on('click', function () {
                $('#JoiningDateRange').val('');
            });

            $('#btnSearch').on('click', function () {

                loadfeedbackgrid();
                loadfeedbackreason();
                loadLeaveReasonByPM();
            })
        }

        function getFilter() {
            var dateRange = $('#JoiningDateRange').val();
            var dateFrom = '', dateTo = '';
            if (dateRange) {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }
            var data =
            {
                pmId: $('#PMList').val(),
                deptId: $('#DepartmentList').val(),
                dateFrom,
                dateTo,
                txtEmployee: $('#txt_search').val(),
                isEligibleForRehire: $('#isEligibleForRehire').val(),
                isVoluntryExit: $('#isVoluntryExit').val(),
                reasons: $('#ReasonList option:selected')
                    .toArray().map(item => item.value),
                //isVoluntryExit: $('#isVoluntaryExit :selected').val()
            }
            return data;
        }
        function loadfeedbackgrid() {
            $('.divoverlay').removeClass('hide');
            var FeedbackGrid = new Global.GridHelper('#grid-managefeedback', {
                serverSide: true,
                destroy: true,
                "pageLength": 25,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": true,
                ajax:
                {
                    url: domain + "feedback/index",
                    type: "Post",
                    data: getFilter()
                },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "9%", "targets": 3 },
                    { "width": "9%", "targets": 4 },
                    { "width": "10%", "targets": 4 },
                    { "width": "9%", "targets": 5 },
                    { "width": "25%", "targets": 6 },
                    { "width": "8%", "targets": 7 },
                    { "width": "8%", "targets": 8 },
                    { "width": "1%", "targets": 9 }
                ],
                columns:
                    [
                        { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        //{ name: "EmpCode", data: "empCode", title: "Employee Code", sortable: false, searchable: false, visible: true },
                        { name: "userName", data: "userName", title: "Name", sortable: true, searchable: true, visible: true },
                        { name: "pmName", data: "pmName", title: "PM", sortable: false, searchable: true, visible: true },
                        { name: "department", data: "department", title: "Department", sortable: true, searchable: true, visible: true },
                        { name: "designation", data: "designation", title: "Designation", sortable: false, searchable: true, visible: true },
                        {
                            name: "feedBackReasons", data: "feedBackReasons", title: "Reasons", sortable: false, searchable: true, visible: true
                        },
                        { name: "JoiningDate", data: "joiningDate", title: "Joining Date", sortable: false, searchable: false, visible: true },
                        { name: "LeavingDate", data: "leavingDate", title: "Leaving Date", sortable: false, searchable: false, visible: true },
                        {
                            name: "CreatedDate", data: "createdDate", title: "Created Date", sortable: false, searchable: false, visible: true
                            , "mRender":
                                function (data, type, row) {
                                    return moment(data).format("MM/DD/YYYY hh:mm A")
                                }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                return '<a title="view" class="fa fa-eye" style="font-size: 15px" href="' + domain + "feedback/add/" + data.uid + '"></a>';
                            }
                        }
                    ],
                "fnDrawCallback": function (oSettings) {

                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                    $('.divoverlay').addClass('hide');
                }
            })
            return FeedbackGrid;
        }

        function loadfeedbackreason() {
            $.ajax({
                type: "POST",
                url: domain + "feedback/GetFeedbackReasonResult",
                data: getFilter(),
                success: function (data) {

                    var itemArray = [];

                    $.each(data, function (i, item) {

                        if (item.totalFeedBack > 0) {
                            itemArray.push({
                                name: item.feedbackname,
                                y: item.totalFeedBack,
                                color: item.color
                            });
                        }

                    });
                    //var chart = new Highcharts.Chart(options);
                    var chart = Highcharts.chart('container-chart', {
                        chart: {
                            plotBackgroundColor: null,
                            plotBorderWidth: null,
                            plotShadow: false,
                            type: 'pie'
                        },
                        title: {
                            text: 'Employee Turnover Reasons',
                            style: { "font-weight": "bold" }
                        },
                        //tooltip: {
                        //    //pointFormat: '{series.name}: <b>{point.percentage:.1f}</b>'
                        //    pointFormat: '{series.name}: <b>{point.y:.1f}</b>'
                        //},
                        tooltip: {
                            formatter: function () {
                                return '<b>' + this.point.name + '</b>: ' + this.y;
                            }
                        },
                        plotOptions: {
                            pie: {
                                size: '100%',
                                allowPointSelect: true,
                                cursor: 'pointer',
                                dataLabels: {
                                    //enabled: function () {
                                    //    return (this.y > 0) ? true : false;
                                    //},
                                    //format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                                    //formatter: function () {
                                    //    return '<b>' + this.point.name + '</b>: ' + this.y;
                                    //   // return (this.y > 0) ? '<b>' + this.point.name + '</b>: ' + this.y : 0; // count value for hidden
                                    //}
                                    formatter: function () {
                                        if (this.y != 0) {
                                            return '<b>' + this.point.name + '</b>: ' + this.y;
                                        } else {
                                            return null;
                                        }
                                    }
                                }
                            }
                        },
                        series: [{
                            data: itemArray
                        }]
                    },
                        function (chart) { // on complete
                            if (itemArray.length == 0) {
                                chart.renderer.text('No Data Available, try Another Search.', 140, 395)
                                    .css({
                                        color: 'Red',
                                        fontSize: '15px'
                                    })
                                    .add();
                            }
                        });
                    $('.highcharts-credits').css("display", "none");
                }
            });
        }

        function loadLeaveReasonByPM() {
            $.ajax({
                type: "POST",
                url: domain + "feedback/GetLeaveReasonByPM",
                data: getFilter(),
                success: function (data) {

                    var itemArray = [];

                    $.each(data, function (i, item) {

                        if (item.totalFeedBack > 0) {
                            itemArray.push({
                                name: item.pmName,
                                //y: 5,
                                y: item.totalFeedBack,
                                color: item.color
                            });
                        }

                    });
                    //var chart = new Highcharts.Chart(options);
                    var chart = Highcharts.chart('leavereasonpm-chart', {
                        chart: {
                            plotBackgroundColor: null,
                            plotBorderWidth: null,
                            plotShadow: false,
                            type: 'pie'
                        },
                        title: {
                            text: 'No. of Employees by Team',
                            style: { "font-weight": "bold" }
                        },
                        //tooltip: {
                        //    //pointFormat: '{series.name}: <b>{point.percentage:.1f}</b>'
                        //    pointFormat: '{series.name}: <b>{point.y:.1f}</b>'
                        //},
                        tooltip: {
                            formatter: function () {
                                return '<b>' + this.point.name + '</b>: ' + this.y;
                            }
                        },
                        plotOptions: {
                            pie: {
                                size: '100%',
                                allowPointSelect: true,
                                cursor: 'pointer',
                                dataLabels: {
                                    //enabled: function () {
                                    //    return (this.y > 0) ? true : false;
                                    //},
                                    //format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                                    //formatter: function () {
                                    //    return '<b>' + this.point.name + '</b>: ' + this.y;
                                    //   // return (this.y > 0) ? '<b>' + this.point.name + '</b>: ' + this.y : 0; // count value for hidden
                                    //}
                                    formatter: function () {
                                        if (this.y != 0) {
                                            return '<b>' + this.point.name + '</b>: ' + this.y;
                                        } else {
                                            return null;
                                        }
                                    }
                                }
                            }
                        },
                        series: [{
                            data: itemArray
                        }]
                    },
                        function (chart) { // on complete
                            if (itemArray.length == 0) {
                                chart.renderer.text('No Data Available, try Another Search.', 140, 395)
                                    .css({
                                        color: 'Red',
                                        fontSize: '15px'
                                    })
                                    .add();
                            }
                        });
                    $('.highcharts-credits').css("display", "none");
                }
            });
        }


        $(function () {
            $('#ReasonList').multiselect({
                includeSelectAllOption: true,
                nonSelectedText: 'No Reason Selected',
                buttonWidth: '100%',
            });

        });

        $this.init = function () {
            loadfeedbackgrid();
            initializeForm();
            //leavingdate();
            initializeEvent();
            loadfeedbackreason(); // method used to get feedback reason data
            loadLeaveReasonByPM(); // method used to get feedback reason data According to PM
            $(".reasoncheckbox").trigger('change');
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));