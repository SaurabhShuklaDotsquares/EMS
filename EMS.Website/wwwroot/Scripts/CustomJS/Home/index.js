(function ($) {
    var timer;
    $(document).on({
        ajaxStart: function () {
            timer && clearTimeout(timer);
            timer = setTimeout(function () {
                $('.divoverlay').removeClass('hide');
            }, 1500);
        },
        ajaxStop: function () {
            $('.divoverlay').addClass('hide');
            clearTimeout(timer);
        }

    });

    function index() {
        var $this = this, postSaleActivitygrid, PreSaleActivitygrid, ExpectedPreSaleActivity, Invoicegrid, taskgrid, homeVM = new HomeViewModel();

        function HomeViewModel() {
            var self = this;
            self.hasInvoices = ko.observable(false);
            self.hasExpectedPreSales = ko.observable(false);
            self.hasFutureOccupancyPreSales = ko.observable(false);
            self.hasPreSales = ko.observable(false);
            self.hasPostSales = ko.observable(false);
            self.hasToDos = ko.observable(false);
            self.showReportFilters = ko.observable(false);

            self.invoiceLoaded = ko.observable(false);
            self.expectedPreSalesLoaded = ko.observable(false);
            self.preSalesLoaded = ko.observable(false);
            self.postSalesLoaded = ko.observable(false);
            self.toDosLoaded = ko.observable(false);

            self.showThoughts = ko.pureComputed(function myfunction() {
                return self.invoiceLoaded() && self.preSalesLoaded() && self.postSalesLoaded() && self.postSalesLoaded() &&
                    !self.hasInvoices() && !self.hasPreSales() && !self.hasPostSales() && !self.hasToDos()
                    && self.expectedPreSalesLoaded() && !self.hasExpectedPreSales();
            }, self);
        }

        function loadHome() {
            if (isDirector == "false") {

                loadPostSales();
                loadExpectedPreSales();
                loadPreSales();

                if (isPMUser == "false") {
                    loadInvoice();
                }
                else {
                    homeVM.invoiceLoaded(true);
                }

                loadTaskList();
            }
            else {
                homeVM.invoiceLoaded(true);
                homeVM.expectedPreSalesLoaded(true);
                homeVM.preSalesLoaded(true);
                homeVM.postSalesLoaded(true);
                homeVM.toDosLoaded(true);
            }
        }

        function loadPostSales() {

            postSaleActivitygrid = new Global.GridHelper('#grid-homePostSaleActivity', {
                serverSide: true,
                destroy: true,
                "pageLength": 20,
                "bFilter": false,
                "ordering": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "home/projectClosure",
                    type: "Post",
                    cache: false,
                    data: GetFilter()
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "10%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "20%", "targets": 3 },
                    { "width": "15%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "5%", "targets": 7 }
                ],
                columns:
                    [
                        {
                            name: "rowIndex", data: "rowIndex", title: "S.NO", sortable: false, searchable: false, visible: true

                        },
                        {
                            name: "ClientID", data: "crmProjectId", title: "CRM ID", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return "<span style='font-weight:bold;'>" + data + "</span>";
                            }
                        },
                        {
                            name: "ClientName", data: "clientName", title: "CLIENT NAME", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                //return data + "[" + row.pCountry + "]";
                                var client = data + (row.pCountry ? " [" + row.pCountry + "]" : "");
                                //if (row.hasDeadResponse) {
                                //    client += "<div style='margin-top: 5px;'><div class='label label-danger converted' style='padding-left: 0;'><label>Dead Response: " + row.deadResponseDate +"</label></div></div>";
                                //}
                                return client;
                            }
                        },
                        {
                            name: "ProjectName", data: "projectName", title: "PROJECT NAME", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return "<span style='font-weight:bold;'>" + data + "</span>" +
                                    "<div style='font-size:11px;'>Generated Date : " + row.createdDate + "</div>" +
                                    "<div style='font-size:11px;'>Last Activity : " + row.modifyDate + "</div>";
                            }
                        },
                        { name: "EngagementDate", data: "engagementDate", title: "ENGAGEMENT DATE", sortable: false, searchable: false, visible: true },
                        {
                            name: "BA", data: "ba", title: "BA NAME", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data && row.baId == row.addedBy) {
                                    return data + "&nbsp; " + "<img border='0' src='./Styles/images/Plus.png' alt='' ToolTip='Request Added By BA' height='15px'>";
                                }
                                return data;
                            }
                        },
                        {
                            name: "TL", data: "tl", title: "TEAM LEADER NAME", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data && row.tlId == row.addedBy) {
                                    return data + "&nbsp; " + "<img border='0' src='./Styles/images/Plus.png' alt='' ToolTip='Request Added By TL' height='15px'>";
                                }
                                return data;
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "&nbsp; ";
                                if (data.allowUpdateStatus || data.hasDeadResponse) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "Chase",
                                        href: domain + "projectclosure/chase/" + data.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-chase-projectClosure",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                actionButtons += $("<a/>", {
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
                                }).get(0).outerHTML + "&nbsp; "

                                return actionButtons;
                            }
                        }
                    ],

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    if (aData["status"] == "4") {
                        $(nRow).addClass('darkgreen_border FillCellBasicStyleForGrid');
                    } else if (aData["status"] == "3") {
                        $(nRow).addClass('escalated_border FillCellBasicStyleForGrid');
                    } else if (aData["clientQuality"] == "1") {
                        $(nRow).addClass('lightyellow_border FillCellBasicStyleForGrid');
                    } else if (aData["clientQuality"] == "2") {
                        $(nRow).addClass('green_border FillCellBasicStyleForGrid');
                    } else if (aData["clientQuality"] == "3") {
                        $(nRow).addClass('lightred_border FillCellBasicStyleForGrid');
                    } else {
                        $(nRow).addClass('away_border');
                    }

                    //aData["status"] == "4" ? $(nRow).addClass('darkgreen') : aData["status"] == "3" ? $(nRow).addClass('escalated') : aData["clientQuality"] == "1" ? $(nRow).addClass('lightyellow') : aData["clientQuality"] == "2" ? $(nRow).addClass('green') : aData["clientQuality"] == "3" ? $(nRow).addClass('lightred') : $(nRow).addClass('away');
                },

                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $("#grid-homePostSaleActivity_wrapper").find('.dataTables_paginate').hide();
                    }
                    else {
                        $("#grid-homePostSaleActivity_wrapper").find('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                },

                "fnInitComplete": function (oSettings, json) {
                    if (oSettings.fnRecordsDisplay() > 0) {
                        homeVM.hasPostSales(true);
                        homeVM.showReportFilters(true);
                    } else {
                        homeVM.hasPostSales(false);
                    }
                    homeVM.postSalesLoaded(true);
                    $('#totalPostSalesActivity').html(json.recordsTotal);
                }

            });
        }

        function loadExpectedPreSales() {

            ExpectedPreSaleActivity = new Global.GridHelper('#grid-homeExpectedPreSaleActivity', {
                serverSide: true,
                destroy: true,
                "ordering": false,
                "pageLength": 25,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "home/ExpectedNewWork",
                    type: "Post",
                    data: GetFilter()
                },
                "columnDefs": [

                    { "width": "8%", "targets": 0 },
                    { "width": "25%", "targets": 1 },
                    { "width": "5%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "13%", "targets": 8 }
                ],
                columns:
                    [
                        {
                            name: "client", data: "client", title: "Lead/Client", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var client = "CRM Lead Id : " + (dataRow.crmid != "" ? "<b>" + dataRow.crmid + "</b>" : "N/A") + '</br>';
                                client += 'EMS Lead Id : ' + dataRow.leadId + '</br>';

                                client += dataRow.clientId != "" ? "<b>" + dataRow.client + "</b></br>" :
                                    (dataRow.allowClientAndDelete ? "<a data-toggle='modal' style='text-decoration:underline;cursor:pointer;' data-target='#modal-createSelectClient' href='" + domain + "estimate/AddClient/" + dataRow.leadId + "'>Client: N/A</a></br>" : "");
                                return client += dataRow.newClient;
                            }
                        },
                        {
                            name: "title", data: "title", title: "Title", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var title = '<img title="' + dataRow.abroadPM + '" src="' + domain + 'images/icons/' + dataRow.abroadPM + '.jpg" alt=""/> ';
                                title += dataRow.allowConclusionAndEdit ? '<a data-toggle="modal" data-target="#modal-conclusion" style="text-decoration:underline;text-transform:uppercase;font-weight: bold;" href="' + domain + 'estimate/Conclusion/' + dataRow.leadId + '">' + dataRow.leadTitle + '</a>' : '<span style="text-transform:uppercase;font-weight: bold;">' + dataRow.leadTitle + '</span>';

                                return title += ' ' + (dataRow.remark != "" ? "<a href='javascript:void(0)' class='remarkTitle' title='" + dataRow.remark + "'><img src='" + domain + "images/icons/Comments-icon.png' alt='R' /></a>" : "")
                                    + '</br><div style="font-size: 11px;">Generated on: ' + dataRow.generatedDate + '</br>Last Activity: ' + dataRow.modifiedDate + ' </br> <a target="_black" style="text-decoration:underline;" href="' + domain + 'upload/estimatedocument/' + dataRow.proposalDocument + '">' + dataRow.proposalDocument + '</a></div>';
                            }
                        },
                        { name: "estimateTime", data: "estimateTimeinDay", title: "ESTIMATED TIME", sortable: false, searchable: false, visible: true },
                        {
                            name: "owners", data: "owners", title: "Estimate/Comm. Owners", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var rData = dataRow.ownersName + (dataRow.nextChaseDate != "" ? "</br><b>Next Chase On :</b> " + dataRow.nextChaseDate : "");

                                if (dataRow.showConversionDate && dataRow.conversionDate != "") {
                                    rData += "<div style='margin-bottom:5px;margin-top:5px;'><label class='label future-occupancy-label-color converted'><b>Conversion Date : </b>" + dataRow.conversionDate + "</label></div>";
                                }

                                return rData;
                                //<div><label class="label label-danger converted">Dead Response</label></div>
                                //(dataRow.conversionDate != "" ? "</br><b>Conversion Date :</b> " + dataRow.conversionDate : "");
                            }
                        },
                        { name: "technologies", data: "technologies", title: "technologies", sortable: false, searchable: false, visible: true },
                        {
                            name: "source", data: "source", title: "Source", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return (dataRow.source != "" ? "<b>" + dataRow.source + "</b></br>" + dataRow.assignedDate : "");
                            }
                        },
                        {
                            name: "status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                if (dataRow.allowAction) {
                                    return '<a style="text-decoration: underline" data-toggle="modal" data-target="#modal-leadStatus" href="' + domain + 'estimate/LeadStatus/' + dataRow.leadId + '">'
                                        + dataRow.status.replace("Team", dataRow.source).replace("Out of India PM", dataRow.source + ' - ' + dataRow.abroadPM) + '</a>';
                                }
                                else {
                                    return dataRow.status.replace("Team", dataRow.source).replace("Out of India PM", dataRow.source + ' - ' + dataRow.abroadPM);
                                }
                            }
                        },
                        {
                            name: "lastConversation", data: "lastConversation", title: "Last Conv.", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                if (dataRow.lastConversation == "" || !(dataRow.showLastConversationInDiv && dataRow.lastConversation.substring(dataRow.lastConversation.length - 3) == "...")) {
                                    return dataRow.lastConversation;
                                } else {
                                    var lastConversation = '<lable>' + $(dataRow.lastConversationFull).text().substring(0, 100) + '</lable>';

                                    lastConversation += '<div class="tooltipfortable">...more<span class="tooltiptextfortable">' + dataRow.lastConversationFull + '</span></div>';
                                    return lastConversation;
                                }
                            }
                        },
                        {
                            name: "action", data: null, title: "Action", sortable: false, searchable: false, visible: true,
                            render: function (data, item, row, meta) {

                                var actionButtons = "";

                                if (data.allowAction) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "Chase",
                                        href: domain + "estimate/LeadStatus/" + data.leadId,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-leadStatus",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                actionButtons += $("<a/>", {
                                    id: "viewDetail",
                                    title: "View History",
                                    href: domain + "estimate/EstimateHistory/" + row.leadId,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-lead-estimateHistory",
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-eye-open",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";

                                if (row.crmid != "" && row.crmid != "0") {
                                    actionButtons += $("<a/>", {
                                        id: "viewCRMNotes",
                                        title: "CRM Notes",
                                        href: domain + "estimate/LeadNotes/" + row.crmid,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-lead-crmnotes",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-th-list",
                                            style: "color:black"
                                        })
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                if (data.allowConclusionAndEdit) {
                                    actionButtons += $("<a/>", {
                                        id: "editLead",
                                        title: "Edit",
                                        href: domain + "estimate/AddEditLead/" + row.leadId,
                                        html: $("<i/>", {
                                            class: "fa fa-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }



                                return actionButtons;
                            }
                        },
                    ],

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    if (aData["status"] == "Chase Request") {
                        $(nRow).addClass('freeuser_border FillCellBasicStyleForGrid');
                    }
                    if (aData["status"] == "Do Not Chase") {
                        $(nRow).addClass('donotchase_border FillCellBasicStyleForGrid');
                    }
                    if (aData["status"] == "Action Required From (Out of India PM)") {
                        $(nRow).addClass('freeuser_border FillCellBasicStyleForGrid');
                    }
                    if (aData["status"] == "Escalated") {
                        $(nRow).addClass('escalated_border FillCellBasicStyleForGrid');
                    }
                    if (aData["status"] == "Converted" || aData["status"] == "Almost Converted") {
                        $(nRow).addClass('darkgreen_border FillCellBasicStyleForGrid');
                    }

                },

                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $("#grid-homeExpectedPreSaleActivity_wrapper").find('.dataTables_paginate').hide();
                    }
                    else {
                        $("#grid-homeExpectedPreSaleActivity_wrapper").find('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                },

                "fnInitComplete": function (oSettings, json) {
                    if (oSettings.fnRecordsDisplay() > 0) {
                        homeVM.hasExpectedPreSales(true);
                        homeVM.showReportFilters(true);
                    } else {
                        homeVM.hasExpectedPreSales(false);
                    }
                    if (json.futureOccupancy > 0) {
                        homeVM.hasFutureOccupancyPreSales(true);
                    } else {
                        homeVM.hasFutureOccupancyPreSales(false);
                    }
                    homeVM.expectedPreSalesLoaded(true);
                    $('#totalExpectedPreSalesActivity').html(json.recordsTotal);
                    $('#totalFutureOccupancyPreSalesActivity').html(json.futureOccupancy);
                }
            });
        }

        function loadPreSales() {

            PreSaleActivitygrid = new Global.GridHelper('#grid-homePreSaleActivity', {
                serverSide: true,
                destroy: true,
                "ordering": false,
                "pageLength": 25,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "home/estimate",
                    type: "Post",
                    data: GetFilter()
                },
                "columnDefs": [

                    { "width": "8%", "targets": 0 },
                    { "width": "25%", "targets": 1 },
                    { "width": "5%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "13%", "targets": 8 }
                ],
                columns:
                    [
                        {
                            name: "client", data: "client", title: "Lead/Client", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var client = "CRM Lead Id : " + (dataRow.crmid != "" ? "<b>" + dataRow.crmid + "</b>" : "N/A") + '</br>';
                                client += 'EMS Lead Id : ' + dataRow.leadId + '</br>';

                                client += dataRow.clientId != "" ? "<b>" + dataRow.client + "</b></br>" :
                                    (dataRow.allowClientAndDelete ? "<a data-toggle='modal' style='text-decoration:underline;cursor:pointer;' data-target='#modal-createSelectClient' href='" + domain + "estimate/AddClient/" + dataRow.leadId + "'>Client: N/A</a></br>" : "");

                                //client += dataRow.clientId != "" ? "ClientId:<b>" + dataRow.clientId + "</b></br><b>" + dataRow.client + "</b></br>" :
                                //    (dataRow.allowClientAndDelete ? "<a data-toggle='modal' style='text-decoration:underline;cursor:pointer;' data-target='#modal-createSelectClient' href='" + domain + "estimate/AddClient/" + dataRow.leadId + "'>ClientID:N/A</a></br>" : "");

                                return client += dataRow.newClient;
                            }
                        },
                        {
                            name: "title", data: "title", title: "Title", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var title = '<img title="' + dataRow.abroadPM + '" src="' + domain + 'images/icons/' + dataRow.abroadPM + '.jpg" alt=""/> ';
                                title += dataRow.allowConclusionAndEdit ? '<a data-toggle="modal" data-target="#modal-conclusion" style="text-decoration:underline;text-transform:uppercase;font-weight: bold;" href="' + domain + 'estimate/Conclusion/' + dataRow.leadId + '">' + dataRow.leadTitle + '</a>' : '<span style="text-transform:uppercase;font-weight: bold;">' + dataRow.leadTitle + '</span>';

                                return title += ' ' + (dataRow.remark != "" ? "<a href='javascript:void(0)' class='remarkTitle' title='" + dataRow.remark + "'><img src='" + domain + "images/icons/Comments-icon.png' alt='R' /></a>" : "")
                                    + '</br><div style="font-size: 11px;">Generated on: ' + dataRow.generatedDate + '</br>Last Activity: ' + dataRow.modifiedDate + ' </br> <a target="_black" style="text-decoration:underline;" href="' + domain + 'upload/estimatedocument/' + dataRow.proposalDocument + '">' + dataRow.proposalDocument + '</a></div>';
                            }
                        },
                        { name: "estimateTime", data: "estimateTimeinDay", title: "ESTIMATED TIME", sortable: false, searchable: false, visible: true },
                        {
                            name: "owners", data: "owners", title: "Estimate/Comm. Owners", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return dataRow.ownersName + '</br>' + (dataRow.nextChaseDate != "" ? "<b>Next Chase On :</b> " + dataRow.nextChaseDate : "");
                            }
                        },
                        { name: "technologies", data: "technologies", title: "technologies", sortable: false, searchable: false, visible: true },
                        {
                            name: "source", data: "source", title: "Source", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return (dataRow.source != "" ? "<b>" + dataRow.source + "</b></br>" + dataRow.assignedDate : "");
                            }
                        },
                        {
                            name: "status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                if (dataRow.allowAction) {
                                    return '<a style="text-decoration: underline" data-toggle="modal" data-target="#modal-leadStatus" href="' + domain + 'estimate/LeadStatus/' + dataRow.leadId + '">'
                                        + dataRow.status.replace("Team", dataRow.source).replace("Out of India PM", dataRow.source + ' - ' + dataRow.abroadPM) + '</a>';
                                }
                                else {
                                    return dataRow.status.replace("Team", dataRow.source).replace("Out of India PM", dataRow.source + ' - ' + dataRow.abroadPM);
                                }
                            }
                        },
                        {
                            name: "lastConversation", data: "lastConversation", title: "Last Conv.", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                if (dataRow.lastConversation == "" || !(dataRow.showLastConversationInDiv && dataRow.lastConversation.substring(dataRow.lastConversation.length - 3) == "...")) {
                                    return dataRow.lastConversation;
                                } else {
                                    var lastConversation = '<lable>' + $(dataRow.lastConversationFull).text().substring(0, 100) + '</lable>';

                                    lastConversation += '<div class="tooltipfortable">...more<span class="tooltiptextfortable">' + dataRow.lastConversationFull + '</span></div>';
                                    return lastConversation;
                                }
                            }
                        },
                        {
                            name: "action", data: null, title: "Action", sortable: false, searchable: false, visible: true,
                            render: function (data, item, row, meta) {

                                var actionButtons = "";

                                if (data.allowAction) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "Chase",
                                        href: domain + "estimate/LeadStatus/" + data.leadId,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-leadStatus",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }


                                actionButtons += $("<a/>", {
                                    id: "viewDetail",
                                    title: "View History",
                                    href: domain + "estimate/EstimateHistory/" + row.leadId,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-lead-estimateHistory",
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-eye-open",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";

                                if (row.crmid != "" && row.crmid != "0") {
                                    actionButtons += $("<a/>", {
                                        id: "viewCRMNotes",
                                        class: "clsViewNote",
                                        title: "CRM Notes",
                                        //href: domain + "estimate/LeadNotes/" + row.crmid,
                                        'data-id': row.crmid,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-lead-crmnotes",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-th-list",
                                            style: "color:black"
                                        })
                                    }).get(0).outerHTML + "&nbsp; ";
                                }


                                if (data.allowConclusionAndEdit) {
                                    actionButtons += $("<a/>", {
                                        id: "editLead",
                                        title: "Edit",
                                        href: domain + "estimate/AddEditLead/" + row.leadId,
                                        html: $("<i/>", {
                                            class: "fa fa-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }



                                return actionButtons;
                            }
                        },
                    ],

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    if (aData["status"] == "Chase Request") {
                        $(nRow).addClass('freeuser_border FillCellBasicStyleForGrid');
                    }
                    if (aData["status"] == "Do Not Chase") {
                        $(nRow).addClass('donotchase_border');
                    }
                    if (aData["status"] == "Action Required From (Out of India PM)") {
                        $(nRow).addClass('freeuser_border FillCellBasicStyleForGrid');
                    }
                    if (aData["status"] == "Escalated") {
                        $(nRow).addClass('escalated_border FillCellBasicStyleForGrid');
                    }
                    if (aData["status"] == "Converted" || aData["status"] == "Almost Converted") {
                        $(nRow).addClass('darkgreen_border FillCellBasicStyleForGrid');
                    }

                },

                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $("#grid-homePreSaleActivity_wrapper").find('.dataTables_paginate').hide();
                    }
                    else {
                        $("#grid-homePreSaleActivity_wrapper").find('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                },

                "fnInitComplete": function (oSettings, json) {
                    if (oSettings.fnRecordsDisplay() > 0) {
                        homeVM.hasPreSales(true);
                        homeVM.showReportFilters(true);
                    } else {
                        homeVM.hasPreSales(false);
                    }
                    homeVM.preSalesLoaded(true);
                    $('#totalPreSalesActivity').html(json.recordsTotal);
                }
            });
        }

        function loadInvoice() {
            Invoicegrid = new Global.GridHelper('#grid-invoice', {
                serverSide: true,
                destroy: true,
                "ordering": false,
                "pageLength": 20,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "home/invoicelist",
                    type: "Post"
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0, visible: false },
                    { "width": "10%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "5%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "width": "15%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "5%", "targets": 8 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "S.NO", sortable: false, searchable: false, visible: false },
                        { name: "CRMProjectId", data: "crmProjectId", title: "Client", sortable: false, searchable: false, visible: true },
                        { name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true },
                        { name: "InvoiceNumber", data: "invoiceNumber", title: "Invoice Number", sortable: false, searchable: false, visible: true },
                        { name: "InvoiceDate", data: "invoiceDate", title: "Date", sortable: false, searchable: false, visible: true },
                        { name: "BAName", data: "baName", title: "Ba Name", sortable: false, searchable: false, visible: true },
                        { name: "TLName", data: "tlName", title: "TL Name", sortable: false, searchable: false, visible: true },
                        { name: "InvoiceStatus", data: "invoiceStatus", title: "Status", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "&nbsp; ";
                                if (data.allowUpdateStatus) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "Chase",
                                        href: domain + "invoice/chaseinvoice/" + dataRow.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-chaseInvoice",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                actionButtons += $("<a/>", {
                                    id: "viewDetail",
                                    title: "View Detail",
                                    href: domain + "invoice/viewinvoice/" + data.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-ViewInvoice",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "fa fa-eye blue",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML;

                                return actionButtons;
                            }
                        },

                    ],

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData.class == "darkred") {
                        $(nRow).addClass("darkred_border FillCellBasicStyleForGrid");
                    }
                    else if (aData.class == "lightred") {
                        $(nRow).addClass("lightred_border FillCellBasicStyleForGrid");
                    }
                    else if (aData.class == "lightyellow") {
                        $(nRow).addClass("lightyellow_border FillCellBasicStyleForGrid");
                    }
                },

                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $("#grid-invoice_wrapper").find('.dataTables_paginate').hide();
                    }
                    else {
                        $("#grid-invoice_wrapper").find('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                },

                "fnInitComplete": function (oSettings, json) {
                    if (oSettings.fnRecordsDisplay() > 0) {
                        homeVM.hasInvoices(true);
                    }
                    homeVM.invoiceLoaded(true);
                    $('#totalUnpaidInvoiceList').html(json.recordsTotal);
                }
            });
        }

        function loadTaskList() {
            taskgrid = new Global.GridHelper('#grid-taskList', {
                serverSide: true,
                destroy: true,
                "ordering": false,
                //"pageLength": 10,
                "paging": false,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "home/taskList",
                    type: "Post"
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "8%", "targets": 2 },
                    { "width": "20%", "targets": 3 },
                    { "width": "17%", "targets": 4 },
                    { "width": "12%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "6%", "targets": 8 },
                    { "width": "9%", "targets": 9 },
                    { "width": "6%", "targets": 10 }
                ],
                columns:
                    [
                        { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "S.NO", sortable: false, searchable: false, visible: false },
                        { name: "TaskId", data: "taskId", title: "Task Id", sortable: false, searchable: false, visible: true },
                        {
                            name: "Source", data: null, title: "Task", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                if (data.source === "MOM") {
                                    //return data.taskName + "<a target='_target'" + "href='" + domain + "minutesOfMeeting/viewtask/" + data.meetingId +
                                    // "?tl=true&st=" + data.status + "'><sup style='color:#112eca;font-weight:bold;'><b>MOM: " + data.updatedDate + "</b></sup></a>";
                                    return "<a style='color:#112eca;font-size:11px;' target='_blank'" + "href='" + domain + "minutesOfMeeting/viewtask/" + data.meetingId +
                                        "?tl=true&st=" + data.meetingStatus + "'><b>MOM# " + data.updatedDate + "</b></a><br/>" + data.taskName;
                                }
                                else {
                                    return data.taskName;
                                }
                            }
                        },
                        { name: "AssignTo", data: "assignTo", title: "Assign To", sortable: false, searchable: false, visible: true },
                        { name: "AssignBy", data: "assignBy", title: "Assign By", sortable: false, searchable: false, visible: true },
                        { name: "UpdatedDate", data: "updatedDate", title: "Add Date", sortable: false, searchable: false, visible: true },
                        { name: "TaskEndDate", data: "taskEndDate", title: "End Date", sortable: false, searchable: false, visible: true },
                        { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                        { name: "Priority", data: "priority", title: "Priority", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "&nbsp; ";
                                if (data.allowChase) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "Chase",
                                        href: domain + "task/chase/" + data.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-chase-task",
                                        'data-backdrop': "black",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; "
                                }

                                actionButtons += $("<a/>", {
                                    id: "commenttask",
                                    title: "Comment",
                                    href: domain + "task/comment/" + data.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-comment-task",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "fa fa-comments blue",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp;"

                                return actionButtons;
                            }
                        },

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData["status"] == "Completed") {
                        $(nRow).addClass("Completed_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', '#9DFF9D');
                        //$('td', nRow).css('color', 'black');
                    }
                    else if (aData["status"] == "Process") {
                        $(nRow).addClass("Process_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', 'yellow');
                        //$('td', nRow).css('color', 'black');
                    }
                    else if (aData["status"] == "Pending") {
                        $(nRow).addClass("Pending_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', '#FF3333');
                        //$('td', nRow).css('color', 'white');
                    }
                    else if (aData["status"] == "Closed") {
                        $(nRow).addClass("Closed_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', 'Orange');
                        //$('td', nRow).css('color', 'white');
                    }
                },
                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $("#grid-taskList_wrapper").find('.dataTables_paginate').hide();
                    }
                    else {
                        $("#grid-taskList_wrapper").find('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');

                },
                "fnInitComplete": function (oSettings, json) {
                    if (oSettings.fnRecordsDisplay() > 0) {
                        homeVM.hasToDos(true);
                    }
                    homeVM.toDosLoaded(true);
                    $('#totalToDoList').html(json.recordsTotal);
                }
            });
        }

        function InitializeControl() {

            ko.applyBindings(homeVM, $("#HomeView")[0]);

            $("#selectbaId").on('change', function () {
                loadPostSales();
                loadExpectedPreSales();
                loadPreSales();
            });

            $("input[name='reportingDays']").on('change', function () {
                loadPostSales();
                loadExpectedPreSales();
                loadPreSales();
            });

            $("#modal-lead-estimateHistory").on('loaded.bs.modal', function () {
            }).on('hidden.bs.modal', function () {
                $('.divoverlay').addClass('hide');
                $(this).removeData('bs.modal');
            });

            $("#modal-lead-crmnotes").on("hidden.bs.modal", function () {
                $(this).removeData("bs.modal");
                $(this).find('.modal-content').empty();
            });

            $(document).off('click', '.clsViewNote').on('click', '.clsViewNote', function () {
                $('#hdnSelectedNote').val($(this).attr('data-id'));
                /*   setTimeout(function () {*/
                var recipient = domain + "estimate/LeadNotes/" + parseInt($('#hdnSelectedNote').val()); // Extract info from data-* attributes

                $('#modal-lead-crmnotes .modal-content').load(recipient, function () {

                });
                /*  }, 500)*/

            });


            $("#modal-leadStatus").on('loaded.bs.modal', function () {
            }).on('hidden.bs.modal', function () {
                $('.divoverlay').addClass('hide');
                $(this).removeData('bs.modal');
            });          

        }

        function showFilters() {
            $("#reportFilters").removeClass("hidden");
            $("#thoughts").addClass("hidden");
        }

        function scrollToDiv(aid) {
            var dTag = $("div[name='" + aid + "']");
            $('html,body').animate({ scrollTop: dTag.offset().top }, 'slow');
        }

        $("#lnk_to_do_list").click(function () {
            scrollToDiv('to_do_list');
        });

        $("#lnk_unpaid_invoice_grid").click(function () {
            scrollToDiv('unpaid_invoice_grid');
        });

        $("#lnk_post_sale_activity").click(function () {
            scrollToDiv('post_sale_activity');
        });

        $("#lnk_pre_sale_activity").click(function () {
            scrollToDiv('pre_sale_activity');
        });

        $("#lnk_expected_pre_sale_activity").click(function () {
            scrollToDiv('expected_pre_sale_activity');
        });

        $("#lnk_future_sale_activity").click(function () {
            scrollToDiv('pre_sale_activity');
        });




        function GetFilter() {
            var data = {
                BAUser: $("#selectbaId").val(),
                reportingDays: $("input[name='reportingDays']:checked").val()
            };
            return data;
        }

        $this.RefreshExpectedPreSales = function () {
            ExpectedPreSaleActivity.ajax.reload();
        };

        $this.RefreshPreSales = function () {
            PreSaleActivitygrid.ajax.reload();
        };

        $this.RefreshPostSales = function () {
            postSaleActivitygrid.ajax.reload();
        };

        $this.RefreshTaskGrid = function () {
            taskgrid.ajax.reload();
        };
        //function LoadLeaveBalance() {
        //    $.ajax({
        //        url: domain + "LeaveBalance/Index",
        //        contentType: 'application/html; charset=utf-8',
        //        type: 'GET',
        //        dataType: 'html',
        //        success: function (result) {
        //            $('#LeaveBalance').html(result);
        //        },
        //        error: (function (xhr, status) {
        //            alert(status);
        //        })
        //    });
        //}
        $this.init = function () {
            InitializeControl();
            loadHome();
            //LoadLeaveBalance();
        };

    }

    $(function () {
        var self = new index();
        self.init();
        $.fn.HomeIndex = self;
    });

}(jQuery));