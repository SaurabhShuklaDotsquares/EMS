(function () {
    function AdditionalSupport() {
        var $this = this, grid, isDirectorRole;
        //function loadGrid() {
        //    $('.divoverlay').removeClass('hide');
        //    grid = new Global.GridHelper('#grid-additional-support', {
        //        serverSide: true,
        //        destroy: true,
        //        ordering: false,
        //        "pageLength": 15,
        //        "bFilter": false,
        //        "bAutoWidth": false,
        //        "bLengthChange": false,
        //        ajax:
        //        {
        //            url: domain + "project/additionalsupport",
        //            type: "POST",
        //            data: getFilters()
        //        },
        //        "columnDefs": [
        //            { "width": "2%", "targets": 0 },
        //            { "width": "10%", "targets": 1 },
        //            { "width": "15%", "targets": 2 },
        //            { "width": "12%", "targets": 3 },
        //            { "width": "12%", "targets": 4 },
        //            { "width": "12%", "targets": 5 },
        //            { "width": "12%", "targets": 6 },
        //            { "width": "12%", "targets": 7 },
        //            { "width": "12%", "targets": 8 },
        //            { "width": "12%", "targets": 9 }
        //        ],
        //        columns:
        //            [
        //                { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
        //                { name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true },
        //                { name: "Description", data: "description", title: "Description", sortable: false, searchable: false, visible: true },
        //                { name: "StartEndDate", data: "startEndDate", title: "Period", sortable: false, searchable: false, visible: true },
        //                { name: "AssignedUsers", data: "assignedUsers", title: "Assigned Developers", sortable: false, searchable: false, visible: true },
        //                {
        //                    name: "RequestedBy", data: "requestedBy", title: "Requested By", sortable: false, searchable: false, visible: true,
        //                    render: function (data, type, row, meta) {
        //                        return "<b>" + data + "</b><br>" + row.createDate
        //                    }
        //                },
        //                { name: "TLName", data: "pmName", title: "Project Manager", sortable: false, searchable: false, visible: true },
        //                { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
        //                {
        //                    name: "UpdateDate", data: "updateDate", title: "Processed On", sortable: false, searchable: false, visible: true,
        //                    render: function (data, type, row, meta) {
        //                        return data ? ("<b>" + data + "</b><br>" + row.updateComment) : ""
        //                    }
        //                },
        //                {
        //                    name: "Action", data: null, title: "Action", sortable: false, searchable: false,
        //                    render: function (data, type, row, meta) {

        //                        var actionButtons = '';
        //                        if (!isDirectorRole && row.updateAllowed) {
        //                            actionButtons += $("<a/>", {
        //                                id: "update",
        //                                class: "btn btn-default btn-sm",
        //                                title: "Update Status",
        //                                href: domain + "project/requestadditionalsupport/" + row.id,
        //                                'data-toggle': "modal",
        //                                'data-target': "#modal-additionalSupport",
        //                                html: $("<i/>", {
        //                                    class: "fa fa-check",
        //                                    style: "color:black"
        //                                }).get(0).outerHTML + "&nbsp; Process",
        //                            }).get(0).outerHTML;
        //                        }

        //                        if (row.editAllowed) {
        //                            actionButtons += $("<a/>", {
        //                                id: "ADDEDIT",
        //                                class: "btn btn-default btn-sm",
        //                                title: "EDIT",
        //                                href: domain + "project/requestadditionalsupport/" + row.id,
        //                                'data-toggle': "modal",
        //                                'data-target': "#modal-additionalSupport",
        //                                html: $("<i/>", {
        //                                    class: "fa fa-edit",
        //                                    style: "color:black"
        //                                }).get(0).outerHTML + "&nbsp; Edit",
        //                            }).get(0).outerHTML;
        //                        }

        //                        return actionButtons;
        //                    }
        //                },
        //            ],
        //        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

        //            switch (aData.status) {
        //                case "Approved":
        //                    $(nRow).css({ 'background-color': '#9efb9e', 'color': 'black' });
        //                    break;
        //                case "UnApproved":
        //                    $(nRow).css({ 'background-color': '#ffb6b6', 'color': 'black' });
        //                    break;
        //                default:
        //                    $(nRow).css({ 'background-color': '#fcf383', 'color': 'black' });
        //                    break;
        //            }
        //        },
        //        "fnDrawCallback": function (oSettings) {
        //            $('.divoverlay').addClass('hide');
        //            if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
        //                $('.dataTables_paginate').hide();
        //            }
        //            else {
        //                $('.dataTables_paginate').show();
        //            }
        //            $('.pagination .active a').css('background-color', '#e99701');
        //            $('.pagination .active a').css('border-color', '#e99701');
        //        }
        //    });
        //}

        function loadGrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-additional-support', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "ordering": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "project/additionalsupport",
                        type: "POST",
                        data: getGridFilters()
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "4%", "targets": 0 },
                    { "width": "12%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "9%", "targets": 3 },
                    { "width": "9%", "targets": 4 },
                    { "width": "9%", "targets": 5 },
                    { "width": "9%", "targets": 6 },
                    { "width": "9%", "targets": 7 },
                    { "width": "14%", "targets": 8 },
                    { "width": "10%", "targets": 9, visible: !isDirectorRole }
                ],
                columns:
                    [
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true },
                       { name: "Description", data: "description", title: "Description", sortable: false, searchable: false, visible: true },
                       { name: "StartEndDate", data: "startEndDate", title: "Period", sortable: false, searchable: false, visible: true },
                       { name: "AssignedUsers", data: "assignedUsers", title: "Assigned Developers", sortable: false, searchable: false, visible: true },
                       {
                           name: "RequestedBy", data: "requestedBy", title: "Requested By", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return "<b>" + data + "</b><br>" + row.createDate
                           }
                       },
                       { name: "TLName", data: "pmName", title: "Project Manager", sortable: false, searchable: false, visible: true },
                       { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                       {
                           name: "UpdateDate", data: "updateDate", title: "Processed On", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return data ? ("<b>" + data + "</b><br>" + row.updateComment) : ""
                           }
                       },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, row, meta) {
                               console.log(row);
                               var actionButtons = '';
                               if (!isDirectorRole && row.updateAllowed) {
                                   actionButtons += $("<a/>", {
                                       id: "update",
                                       class: "btn btn-default btn-sm",
                                       title: "Update Status",
                                       href: domain + "project/requestadditionalsupport/" + row.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-additionalSupport",
                                       html: $("<i/>", {
                                           class: "fa fa-check",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; Process",
                                   }).get(0).outerHTML;
                               }

                               if (row.editAllowedPM || row.editAllowedForOtherUser) {
                                   actionButtons += $("<a/>", {
                                       id: "ADDEDIT",
                                       class: "btn btn-default btn-sm",
                                       title: "EDIT",
                                       href: domain + "project/requestadditionalsupport/" + row.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-additionalSupport",
                                       html: $("<i/>", {
                                           class: "fa fa-edit",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; Edit",
                                   }).get(0).outerHTML;
                               }
                             


                               return actionButtons;
                           }
                       },

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    switch (aData.status) {
                        case "Approved":
                            $(nRow).css({ 'background-color': '#9efb9e', 'color': 'black' });
                            break;
                        case "UnApproved":
                            $(nRow).css({ 'background-color': '#ffb6b6', 'color': 'black' });
                            break;
                        default:
                            $(nRow).css({ 'background-color': '#fcf383', 'color': 'black' });
                            break;
                    }
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
                }
            });
        }

        function getGridFilters() {
            return {
                projectId: $("#ProjectId").val(),
                status: $("#Status").val(),
                pmUid: $("#PMUid").val()
            };
        }
        //function getFilters() {
        //    return {
        //        projectId: $("#ProjectId").val(),
        //        status: $("#Status").val(),
        //        pmUid: $("#PMUid").val()
        //    };
        //}
        function initialize() {

            isDirectorRole = isDirector == "true";
          
            $("#modal-additionalSupport").on('loaded.bs.modal', function (e) {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find('form'), null, null,
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            //ReloadGrid();
                            loadGrid();
                            Global.ShowMessage(result.message, true, 'MainNotificationMessage');

                        } else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'NotificationMessage');
                        }
                    });

                form.find("#StartDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    onSelect: function (selectedDate) {
                        form.find("#EndDate").datepicker("option", "minDate", selectedDate);
                    }
                });

                form.find("#EndDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    onSelect: function (selectedDate) {
                        form.find("#StartDate").datepicker("option", "maxDate", selectedDate);
                    }
                });

                form.find("#AssignedUserIds").select2({
                    placeholder: "-- Select Assigned Developers --",
                    allowClear: true,
                    width: '100%'
                });

            }).on("hidden.bs.modal", function (e) {
                $(this).removeData("bs.modal").find('.modal-content').empty();
            });

            $("#btnSearch").on("click", function () {
                //ReloadGrid(true);
                loadGrid();
            });

            var queryString = Global.GetUrlVars();
            if (queryString && queryString.request && queryString.request.trim() !== "") {
                $("#modal-additionalSupport").modal({
                    remote: domain + "project/requestadditionalsupport/" + queryString.request.trim()
                });
            }
        }

        //function ReloadGrid(moveToFirstPage) {
        //    if (moveToFirstPage) {
        //        grid.ajax.reload();
        //    }
        //    else {
        //        grid.ajax.reload(null, false);
        //    }
        //}

        $this.init = function () {
            loadGrid();
            initialize();

        }
    }
    $(function () {
        var self = new AdditionalSupport();
        self.init();
    });
}(jQuery))