(function ($) {
    function index() {
        var $this = this;

        function initializeForm() {
            var start = moment().subtract(29, 'days');
            var end = moment();

            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            }

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
            //$("#JoiningDateRange").val('');
        }

        function initializeEvent() {
            $('#btnReset').on('click', function () {
                $('#PMList').val('');
                $('#DepartmentList').val('');
                $('#txt_search').val('');
                //$('#JoiningDateRange').val('');
                loadOnNoticeGrid();
            });

            $('#clrFilterDate').on('click', function () {
                //$('#JoiningDateRange').val('');
            });

            $('#btnSearch').on('click', function () {
                loadOnNoticeGrid();
            })

            $(document).on('click', '.anchor-send-mail', function () {
                $(this).text("sending...");
                $thislocal = this;
                var uid = $(this).data("uid");
                $.post(domain + "OnNotice/SendEmail", { id: uid }, function (result) {
                    if (result.isSuccess) {
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message, false, 'MessageDiv');
                    }
                    $($thislocal).text("");
                    loadOnNoticeGrid();
                });

            });



        }
        function getFilter() {
            //var dateRange = $('#JoiningDateRange').val();
            //var dateFrom = '', dateTo = '';
            //if (dateRange) {
            //    dateFrom = dateRange.split(localeOpts.separator)[0];
            //    dateTo = dateRange.split(localeOpts.separator)[1];
            //}
            var data =
            {
                pmId: $('#PMList').val(),
                deptId: $('#DepartmentList').val(),
                //dateFrom,
                //dateTo,
                txtEmployee: $('#txt_search').val(),
                //IsVoluntaryExit:!!+$('#IsVoluntaryExit').val(),
                //IsVoluntaryExit:$('#IsVoluntaryExit').val(),
            }
            if ($('#IsVoluntaryExit').val()!="") {
                data.IsVoluntaryExit=!!+$('#IsVoluntaryExit').val()
            }
            return data;
        }
        function loadOnNoticeGrid() {
            $('.divoverlay').removeClass('hide');
            var OnNoticeGrid = new Global.GridHelper('#grid-OnNotice', {
                serverSide: true,
                destroy: true,
                "pageLength": 25,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "OnNotice/index",
                    type: "Post",
                    data: getFilter()
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "5%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "11%", "targets": 4 },
                    { "width": "11%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "13%", "targets": 7 },
                    { "width": "15%", "targets": 8 },
                    //{ "width": "0%", "targets": 7 },
                ],
                columns:
                    [
                        { name: "uid", data: "uid", title: "uid", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "userName", data: "userName", title: "Employee Name", sortable: true, searchable: false, visible: true },
                        { name: "pmName", data: "pmName", title: "PM Name", sortable: false, searchable: false, visible: true },
                        { name: "designation", data: "designation", title: "Designation", sortable: true, searchable: false, visible: true },
                        { name: "Department", data: "department", title: "Department", sortable: true, searchable: false, visible: true },
                        {
                            name: "resignationDate", data: "resignationDate", title: "Resignation Date", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "relivedDate", data: "relivedDate", title: "Relieving Date", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                console.log("dataRow", dataRow)
                                if (dataRow.isformalitiescomplted === true) {
                                    return '<a href="' + domain + "onnotice/usernoc?user=" + data.uid + '" target="_blank" title="Relieved"><strong>Relieved: ' + dataRow.relivedDate +'</strong></a>';                                   
                                }
                                else {
                                    var revert = '  <a title="Revert" class="glyphicon glyphicon-refresh" style="font-size: 15px;margin-left:3%;"></a>';
                                    var relieve = '  <a href="' + domain + "onnotice/usernoc?user=" + data.uid + '" title="Relieve Status" class="glyphicon glyphicon-check" style="font-size: 15px;margin-left:3%;"></a>';
                                    if (dataRow.isFeedbackReceived === true) {
                                        return '<a href="' + domain + "feedback/add/" + data.feedBackId + '" target="_blank" title="Feedback Received" class="glyphicon glyphicon-envelope" style="font-size: 15px;color:Green;"></a>' + relieve;
                                    }
                                    else if (dataRow.isEmailSent === true && dataRow.isFeedbackReceived === false) {
                                        return '<a title="Resend Email" class="glyphicon glyphicon-envelope anchor-send-mail" style="font-size: 15px;color:red;" data-uid=' + data.uid + '></a>' + relieve;
                                    }
                                    else if (dataRow.isEmailSent === false && dataRow.isFeedbackReceived === false) {
                                        return '<a title="Send Email" class="glyphicon glyphicon-envelope anchor-send-mail" style="font-size: 15px" data-uid=' + data.uid + '></a>' + relieve;
                                    }
                                }
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

            });

            return OnNoticeGrid
        }

        $this.init = function () {
            initializeForm();
            initializeEvent();
            loadOnNoticeGrid()
        }
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));
