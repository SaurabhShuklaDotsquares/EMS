(function ($) {
    function index() {        
        var $this = this, grid, formAddEdit, baConversion, localeOpts, isDirectorRole = false;

        function getfilter() {
            var dateRange = $('#ReportDateRange').val();
            var dateFrom = '', dateTo = '';

            if (dateRange && dateRange != '') {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }

            var data = {
                TextSearch: $("#txtSearch").val(),
                ChaseStatus: $("#chaseStatus option:selected").val(),
                DateFrom: dateFrom,
                DateTo: dateTo,
                BA: $("#Uid_BA").val(),
                TL: $("#Uid_TL").val(),
                PMUid: $("#PMUid").val(),
                CRMStatusId: $("#CRMStatusId").val(),
                ProjectStatus: $("#ProjectStatus option:selected").val(),
            };

            return data;
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
                    url: domain + "projectclosure/index",
                    type: "Post",
                    data: getfilter()
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "12%", "targets": 1 },
                    { "width": "19%", "targets": 2 },
                    { "width": "8%", "targets": 3 },
                    { "width": "8%", "targets": 4 },
                    { "width": "8%", "targets": 5 },
                    { "width": "8%", "targets": 6 },
                    { "width": "8%", "targets": 7 },
                    { "width": "8%", "targets": 8 },
                    { "width": "8%", "targets": 9 },
                    { "width": "8%", "targets": 10, "className": "rowCenterText" },
                ],
                columns:
                    [
                        {
                            name: "rowIndex", data: "rowIndex", title: "S.No", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "ClientName", data: "clientName", title: "CLIENT", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return "<span style=color:#057db2; font-weight:600 !important;> CRM ID: </span> [" + row.crmProjectId + "] (" + row.crmStatus + ")<br/>" +
                                    data + (row.pCountry ? "[" + row.pCountry + "]" : "");
                            }
                        },
                        {
                            name: "ProjectName", data: "projectName", title: "PROJECT NAME", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return "<span style='font-weight:600 !important; margin: 0;'>" + (row.allowUpdateStatus ? "<a class='ablue' data-toggle='modal' data-target='#modal-status-projectClosure' href='projectclosure/UpdateProjectStatus/" + row.id + "' >" + data + "</a>" : data) + "<span>" +
                                    "<div style='font-size:11px;'>Generated Date : " + row.createdDate + "</div>" +
                                    "<div style='font-size:11px;'>Last Activity : " + row.modityDate + "</div>"
                            }
                        },
                        { name: "CloserType", data: "closerType", title: "Status", sortable: false, searchable: false },
                        { name: "StartEndDate", data: "startEndDate", title: "CRM Start/ End Date", sortable: false, searchable: false },
                        { name: "Estimate", data: "estimate", title: "Estimate", sortable: false, searchable: false, visible: true },
                        {
                            name: "Invoice", data: "invoice", title: "Invoice", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (row.bucketHours) {
                                    return (data != "-" ? data + " /<br>" : "") + "Bucket: " + row.bucketHours;
                                }
                                return data;
                            }
                        },
                        { name: "EngagementDate", data: "engagementDate", title: "ENGAGEMENT DATE", sortable: false, searchable: false, visible: true },
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
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {                               
                                var actionButtons = $("<a/>", {
                                    id: "viewDetail",
                                    title: "View Detail",
                                    href: domain + "projectclosure/detail/" + data.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-detail-projectClosure",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-eye-open",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";

                                if (data.allowUpdateStatus) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "chase",
                                        href: domain + "projectclosure/chase/" + data.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-chase-projectClosure",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";

                                    actionButtons += $("<a/>", {
                                        id: "editProjectClosure",
                                        title: "edit",
                                        href: domain + "projectclosure/add/" + data.id + "?referrer=engagement",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                if (data.allowDelete) {
                                    actionButtons += "&nbsp; " + $("<a/>", {
                                        href: domain + "projectclosure/delete/" + data.id,
                                        id: "deletetask",
                                        title: "delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-projectClosure",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-trash",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                actionButtons += $("<a/>", {
                                    id: "DownloadPDF",
                                    title: "Download PDF",
                                    href: domain + "projectclosure/DownloadPDF/" + data.id,
                                    target: "_blank",
                                    html: $("<i/>", {
                                        class: "fa fa-file-pdf-o",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML

                                return actionButtons;
                            }
                        },

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    aData["status"] == "4" ? $(nRow).addClass('darkgreen') : aData["status"] == "3" ? $(nRow).addClass('escalated') : aData["clientQuality"] == "1"
                        ? $(nRow).addClass('lightyellow') : aData["clientQuality"] == "2" ? $(nRow).addClass('green') : aData["clientQuality"] == "3" ?
                            $(nRow).addClass('lightred') : $(nRow).addClass('away');
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

                    $("html, body").animate({ scrollTop: 170 }, "fast");
                },
                "fnInitComplete": function (oSettings, json) {
                    if (oSettings.aiDisplay.length) {
                        $("div.btnexportexcel hidden").removeClass('hidden');
                    }
                    else {
                        $("div.btnexportexcel").addClass('hidden');
                    }
                }
            });

            grid.on('preXhr.dt', function () {
                $('.divoverlay').removeClass('hide');
            }).on('xhr.dt', function () {
                $('.divoverlay').addClass('hide');
            });
        }

        function InitializeControl() {

            isDirectorRole = isDirector == "true";

            baConversion = $("#div_baconversion");

            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            };

            function rangeChangeCB(start, end) {
                $('#ReportDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
            }

            $('#ReportDateRange').daterangepicker({
                "locale": localeOpts,
                startDate: moment().subtract(29, 'days'),
                endDate: moment(),
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

            $("#modal-delete-projectClosure").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        grid.ajax.reload(null, false);
                        modal.modal('hide');

                        Global.ShowMessage(result.message || result.errorMessage, result.isSuccess, 'MessageDiv');
                    });
            }).on('hidden.bs.modal', function () {
                $('.divoverlay').addClass('hide');
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#btnSearch").on("click", function () {
                loadGrid();
                LoadBAConversion();
            });

            $("#btnReset").on("click", function () {
                $("#txtSearch").val('');
                $("#chaseStatus").val('');
                $("#ReportDateRange").val('');
                $("#Uid_BA").val('');
                $("#Uid_TL").val('');
                $("#CRMStatusId").val('');
                $("#ProjectStatus").val('');
                loadGrid();
                LoadBAConversion();
            });

            $("#txtSearch").on("keypress", function (e) {
                if (e.keyCode == 13) {
                    $("#btnSearch").click();
                    return false;
                }
            });

            $("#clrFilterDate").click(function () {
                $("#ReportDateRange").val('');
            });
        }

        function LoadBAConversion() {
            if (!isDirectorRole) {
                baConversion.empty();
                var data = getfilter();
                $.ajax({
                    url: domain + "projectclosure/baconversion",
                    type: 'POST',
                    data: data,
                    success: function (result) {
                        baConversion.html(result);
                    },
                    error: function (ex) {

                    }
                });
            }
        }

        $this.refreshClosures = function () {
            grid.ajax.reload(null, false);
        };

        $this.init = function () {
            InitializeControl();
            LoadBAConversion();
            loadGrid();
        };
    }
    $(function () {
        var self = new index();
        self.init();

        $.fn.ProjectClosureIndex = self;
    });
}(jQuery));