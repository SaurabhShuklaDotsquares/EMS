(function ($) {
    function index() {
        var $this = this, grid, formAddEdit, closureSummary, localeOpts, pmUid, closureFilterType;

        function getfilter(d) {
            var dateRange = $('#ReportDateRange').val();
            var dateFrom = '', dateTo = '';

            if (dateRange && dateRange != '') {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }
            d = d || {};
            d.TextSearch = $("#txtSearch").val();
            d.DateFrom = dateFrom;
            d.DateTo = dateTo;
            d.BA = $("#Uid_BA").val();
            d.TL = $("#Uid_TL").val();
            d.PMUid = pmUid || $("#PMUid").val() || null;
            d.CRMStatusId = $("#CRMStatusId").val();
            d.filterType = closureFilterType || null;
            return d;
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
                    url: domain + "projectclosure/ReportCOVID19",
                    type: "Post",
                    data: function (d) {
                        getfilter(d);
                    }
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "17%", "targets": 1 },
                    { "width": "24%", "targets": 2 },
                    //{ "width": "8%", "targets": 3, },
                    { "width": "8%", "targets": 3, },
                    { "width": "13%", "targets": 4 },
                    //{ "width": "8%", "targets": 6, },
                    //{ "width": "8%", "targets": 7, },
                    //{ "width": "8%", "targets": 5, },
                    { "width": "25%", "targets": 5, },
                    //{ "width": "5%", "targets": 10, "className": "rowCenterText" },
                ],
                columns:
                    [
                        {
                            name: "rowIndex", data: "rowIndex", title: "S.No", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "ClientName", data: "clientName", title: "CLIENT", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var client = "";
                                client += "<a class='ablue' data-toggle='modal' data-target='#modal-history-projectClosure' href='" + domain + "projectclosure/history/" + row.projectId + "'>CRM ID: [" + row.crmProjectId + "]</a> (" + row.crmStatus + ")<br/>" +
                                    data + (row.pCountry ? "[" + row.pCountry + "]" : "");
                                client += (row.hasConverted ? "<div><label class='label label-success converted'>Converted</label></div>" : "");
                                client += (row.hasDeadResponse ? "<div><label class='label label-danger converted'>Dead Response</label></div>" : "");
                                return client;
                            }
                        },
                        {
                            name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var project = "";
                                project += "<div style='font-weight:600 !important; margin: 0;'>" + (!row.hasConverted && !row.hasDeadResponse ? "<a class='ablue' data-toggle='modal' data-target='#modal-status-projectClosure' href='projectclosure/UpdateProjectStatus/" + row.id + "' >" + data + "</a>" : data) + "<div>" +
                                    "<div style='font-size:11px;font-weight: normal;'>Generated Date : " + row.createdDate + "</div>" +
                                    "<div style='font-size:11px;font-weight: normal;'>Last Activity : " + row.modityDate + "</div>";
                                return project;
                            }
                        },
                        //{ name: "StartEndDate", data: "startEndDate", title: "CRM Start/ End Date", sortable: false, searchable: false },
                        { name: "ClosingDate", data: "closingDate", title: "Closing Date", sortable: false, searchable: false, visible: true },
                        //{ name: "Estimate", data: "estimate", title: "Estimate", sortable: false, searchable: false, visible: true },
                        //{
                        //    name: "Invoice", data: "invoice", title: "Invoice", sortable: false, searchable: false, visible: true,
                        //    render: function (data, type, row, meta) {
                        //        if (row.bucketHours) {
                        //            return (data != "-" ? data + " /<br>" : "") + "Bucket: " + row.bucketHours;
                        //        }
                        //        return data;
                        //    }
                        //},
                        //{
                        //    name: "BA", data: "ba", title: "BA/ TL NAME", sortable: false, searchable: false, visible: true,
                        //    render: function (data, type, row, meta) {
                        //        var baTLNames = "";
                        //        if (row.ba) {
                        //            baTLNames += row.ba + (row.baId == row.addedBy ? "&nbsp; <img border='0' src='./Styles/images/Plus.png' alt='' title='Request Added By BA' height='15px'>" : "");
                        //        }

                        //        if (row.tl) {
                        //            baTLNames += (baTLNames != "" ? " / " : "") + "<div>" + row.tl + (row.tlId == row.addedBy ? "&nbsp; <img border='0' src='./Styles/images/Plus.png' alt='' title='Request Added By TL' height='15px'>" : "") + "</div>";
                        //        }
                        //        return baTLNames;
                        //    }
                        //},
                        {
                            name: "PM", data: "pm", title: "Team", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var baTLNames = "";
                                if (row.ba) {
                                    baTLNames += "<div style='font-size:11px;font-weight: normal;'>BA : "+row.ba + (row.baId == row.addedBy ? "&nbsp; <img border='0' src='./Styles/images/Plus.png' alt='' title='Request Added By BA' height='15px'>" : "")+"</div>";
                                }

                                if (row.tl) {
                                    baTLNames += "<div style='font-size:11px;font-weight:normal;'>TL : " + row.tl + (row.tlId == row.addedBy ? "&nbsp; <img border='0' src='./Styles/images/Plus.png' alt='' title='Request Added By TL' height='15px'>" : "") + "</div>";
                                }
                                var pm = row.pm != "" ? "PM : "+row.pm : "";
                                return pm + (baTLNames != "" ? "<br/>" + baTLNames:"");
                            }
                        },
                        {
                            name: "Comments", data: "comments", title: "Review Comments", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var reviews = "";

                                if (row.comments) {
                                    reviews += row.promisingPercentage ? "<strong>Promising: </strong>" + row.promisingPercentage : "";
                                    reviews += row.nextStartDate ? "<br><strong>May start again on : </strong>" + row.nextStartDate : "";
                                    reviews += row.developers ? "<br><strong>Number of Developers : </strong>" + row.developers : "";
                                    reviews += "<br><strong>Comments : </strong>" + (row.comments.length <= 55 ? row.comments : (row.comments.substring(0, 50) + "... <a class='ablue' data-toggle='tooltip' title='" + row.comments + "'>more</a>"));
                                }
                                return reviews;
                            }
                        },
                        //{
                        //    name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                        //    render: function (data, type, dataRow, meta) {

                        //        var actionButtons = "";

                        //        if (data.isPending && data.allowPendingEdit) {
                        //            actionButtons += $("<a/>", {
                        //                id: "approveProjectClosure",
                        //                title: "Approve",
                        //                'class': "btn btn-default btn-sm btn-block",
                        //                href: domain + "projectclosure/add/" + data.id,
                        //                html: "<i class='fa fa-check'></i> Approve",
                        //            }).get(0).outerHTML;

                        //            if (!data.hasConverted) {
                        //                actionButtons += $("<button/>", {
                        //                    id: "declineProjectClosure",
                        //                    title: "Decline",
                        //                    'class': "btn btn-warning btn-sm btn-block btn-decline",
                        //                    'data-id': data.id,
                        //                    html: "<i class='fa fa-times'></i> Decline",
                        //                }).get(0).outerHTML;
                        //            }
                        //        }
                        //        else {
                        //            var actionButtons = $("<a/>", {
                        //                id: "viewDetail",
                        //                title: "View Detail",
                        //                href: domain + "projectclosure/detail/" + data.id,
                        //                'data-toggle': "modal",
                        //                'data-target': "#modal-detail-projectClosure",
                        //                'data-backdrop': "static",
                        //                html: $("<i/>", {
                        //                    class: "glyphicon glyphicon-eye-open",
                        //                    style: "color:black"
                        //                }),
                        //            }).get(0).outerHTML + "&nbsp; ";

                        //            if (data.allowEdit) {
                        //                actionButtons += $("<a/>", {
                        //                    id: "editProjectClosure",
                        //                    title: "edit",
                        //                    href: domain + "projectclosure/add/" + data.id,
                        //                    html: $("<i/>", {
                        //                        'class': "glyphicon glyphicon-edit",
                        //                        style: "color:black"
                        //                    }),
                        //                }).get(0).outerHTML + "&nbsp; ";
                        //            }

                        //            if (!data.isPending && !data.hasConverted && !data.hasDeadResponse && data.allowReview) {
                        //                actionButtons += $("<a/>", {
                        //                    id: "reviewProjectClosure",
                        //                    title: "Review",
                        //                    href: domain + "projectclosure/addeditreview/" + data.id,
                        //                    'data-toggle': "modal",
                        //                    'data-target': "#modal-review-projectClosure",
                        //                    'data-backdrop': "static",
                        //                    html: $("<i/>", {
                        //                        class: "fa fa-thumbs-o-up",
                        //                        style: "color:black"
                        //                    }),
                        //                }).get(0).outerHTML + "&nbsp; ";
                        //            }
                        //            actionButtons += $("<a/>", {
                        //                id: "DownloadPDF",
                        //                title: "Download PDF",
                        //                href: domain + "projectclosure/DownloadPDF/" + data.id,
                        //                target: "_blank",
                        //                html: $("<i/>", {
                        //                    class: "fa fa-file-pdf-o",
                        //                    style: "color:black"
                        //                }),
                        //            }).get(0).outerHTML
                        //        }

                        //        return actionButtons;
                        //    }
                        //},
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    //if (aData.isPending) {
                    //    $(nRow).addClass('darkred')
                    //}
                    //else if (aData.hasConverted) {
                    //    $(nRow).css('background-color', '#dbfbdb');
                    //}
                    //else if (!aData.hasReview) {
                    //    $(nRow).css('background-color', '#99f3f0');
                    //}
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
                    $('[data-toggle="tooltip"]').tooltip();
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

            closureSummary = $("#div_baconversion");

            var start = moment().subtract(29, 'days');
            var end = moment();
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
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, rangeChangeCB);

            rangeChangeCB(start, end);

            $("#modal-history-projectClosure").on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

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
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#btnSearch").on("click", function () {
                pmUid = null;
                closureFilterType = null;
                loadGrid();
                LoadSummary();
            });

            $("#btnReset").on("click", function () {
                $("#txtSearch").val('');
                //$("#chaseStatus").val('');
                $("#ReportDateRange").val('');
                $("#Uid_BA").val('');
                $("#Uid_TL").val('');
                $("#CRMStatusId").val('');
                //$("#ProjectStatus").val('');
                pmUid = null;
                closureFilterType = null;
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

            $("#grid-projectcloser").on("click", ".btn-decline", function () {
                var id = $(this).data('id');
                if (id) {
                    if (confirm("Are you sure?\nCRM Status will be reverted and Record will be deleted.")) {
                        $('.divoverlay').removeClass('hide');
                        $.post(domain + "projectclosure/decline/" + id, function (result) {
                            if (result) {
                                if (result.isSuccess) {
                                    grid.ajax.reload(null, false);
                                }
                                Global.ShowMessage(result.message || result.errorMessage, result.isSuccess, 'MessageDiv');
                            }

                            $('.divoverlay').addClass('hide');
                        });
                    }
                }
            });

            closureSummary.on("click", "td.filter", function () {
                var td = $(this);
                pmUid = td.closest("tr").find("#PMId").val();
                closureFilterType = td.data("type");

                if (pmUid && closureFilterType) {
                    if (td.hasClass("active")) {
                        td.removeClass("active");
                        pmUid = null;
                        closureFilterType = null;
                    }
                    else {
                        closureSummary.find("td.filter.active").removeClass("active");
                        td.addClass("active");
                    }
                    grid.ajax.reload();
                }
            });
        }

        function LoadSummary() {
            closureSummary.empty();
            var data = getfilter();
            $.ajax({
                url: domain + "projectclosure/projectclosuresummary",
                type: 'POST',
                data: data,
                success: function (result) {
                    closureSummary.html(result);
                },
                error: function (ex) {

                }
            });
        }

        $this.refreshClosures = function () {
            grid.ajax.reload(null, false);
        };

        $this.init = function () {
            InitializeControl();
            LoadSummary();
            loadGrid();
        };
    }
    $(function () {
        var self = new index();
        self.init();

        $.fn.ProjectClosureReport = self;
    });
}(jQuery));