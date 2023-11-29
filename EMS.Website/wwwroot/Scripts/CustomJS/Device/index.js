(function ($) {
    function Index() {
        var $this = this, grid;

        function loadGrid() {

            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-device-history', {
                serverSide: true,
                destroy: true,
                ordering: false,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "device/index",
                    type: "POST",
                    data: getFilters()
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "10%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "12%", "targets": 3 },
                    { "width": "12%", "targets": 4 },
                    { "width": "7%", "targets": 5 },
                    { "width": "10%", "targets": 6 }
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        {
                            name: "DeviceName", data: "deviceName", title: "Device", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {

                                if (row.simDetails == "" && row.serialNumber && row.serialNumber != undefined) {
                                    return data + "<br>" + "S/N : " + row.serialNumber;
                                }
                                else if (row.simDetails == "") {
                                    return data;
                                }
                                else {
                                    if (row.serialNumber && row.serialNumber != undefined) {
                                        return data + "<br>" + row.simDetails + "<br>" + "S/N : " + row.serialNumber;
                                    }
                                    else {
                                        return data + "<br>" + row.simDetails;
                                    }
                                }
                            }
                        },
                        { name: "Condition", data: "condition", title: "Condition", sortable: false, searchable: false, visible: true },
                        {
                            name: "PrevDeviceUser", data: "prevDeviceUser", title: "Previous Assigned Person", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data) {
                                    return data + "<br>" + row.prevDeviceUserPeriod
                                }
                                else {
                                    return data;
                                }
                            }
                        },
                        {
                            name: "AssignedTo", data: "assignedTo", title: "New Assigned Person", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return data + "<br>" + row.assignedDate
                            }
                        },
                        { name: "AssignedBy", data: "assignedBy", title: "Assigned By", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '';

                                if (row.recieveAllowed) {
                                    actionButtons += $("<a/>", {
                                        id: "addEdit",
                                        class: "btn btn-default btn-sm",
                                        href: domain + "device/addreturndevice/" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-return-device",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-pencil",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Receive",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                if (row.recieveAllowed) {
                                    actionButtons += $("<a/>", {
                                        id: "addEditCondition",
                                        class: "btn btn-default btn-sm",
                                        href: domain + "device/addEditAssignDevice/" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-assign-device",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-edit",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Edit",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                return actionButtons;
                            }
                        },

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData.isSubmitted) {
                        $('td', nRow).css({ 'background-color': '#9dff9d', 'color': 'black' });
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

        function getFilters() {
            return {
                historyType: $("#HistoryType").val(),
                deviceType: $("#DeviceType").val(),
                deviceId: $("#DeviceId").val(),
                userId: $("#UserId").val(),
            };
        }

        function initialize() {

            $("#modal-assign-device").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, null,
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "NotificationMessage");
                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });

                form.find("#AssignedDateTime").datepicker({
                    dateFormat: "dd/mm/yy",
                    maxDate: new Date()
                });

                fillDeviceDropDown(form);

                form.on("change", "#DeviceType", function () {
                    fillDeviceDropDown(form);
                });

                form.on("change", "#DeviceId", function () {
                    var spnQuanityError = form.find('#spnQuanityError');
                    spnQuanityError.html('');

                    if (!$(this).find("option:selected").data('availablestock')) {
                        spnQuanityError.html("Stock is not avaliable for selected device");
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-return-device").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, null,
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "divMessage");
                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });

                // Date in YYYY-MM-DD
                var parts = form.find("#AssignedDate").val().split('-');
                // JavaScript counts months from 0 so parts[1] will be parts[1] - 1
                var assignedDate = new Date(parts[0], parts[1] - 1, parts[2]);

                form.find("#ReturnDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    maxDate: new Date(),
                    minDate: assignedDate
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#btnSearch").on("click", function () {
                loadGrid();
            });

            var deviceHistory = $("#deviceHistory");
            deviceHistory.on("change", "#DeviceType", function () {
                fillDeviceDropDown(deviceHistory, "All Devices");
            });

        }

        function fillDeviceDropDown(parent, defaultText) {
            var deviceType = parent.find('#DeviceType').val();
            var ddlDevice = parent.find('#DeviceId');

            ddlDevice.html("<option value>" + (defaultText || "Select Device") + "</option>");
            if (deviceType != "") {
                $.ajax({
                    url: domain + 'device/getdevicenamelist',
                    type: 'GET',
                    data: { deviceType: deviceType },
                    success: function (data) {
                        var name = '';
                        name = name + "<option value>" + (defaultText || "Select Device") + "</option>";
                        $(data).each(function (index, item) {
                            name = name + "<option data-availablestock=" + item.availableStock + " value=" + item.id + ">" + item.name + " [Stock: " + item.availableStock + "]" + "</option>";
                        });
                        ddlDevice.html(name);
                    }
                });
            }
        }

        $this.init = function () {
            loadGrid();
            initialize();
        };
    }

    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));