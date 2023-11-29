(function ($) {
    function index() {
        var $this = this, grid, formAddEdit;
        function initializeModalWithForm() {

            $("#modal-add-device").on('loaded.bs.modal', function () {

                var modal = $(this);

                modal.find('#StartTime').datetimepicker({
                    format: 'DD/MM/YYYY hh:mm A',
                    ignoreReadonly: true
                });

                modal.find(".multiple").select2({
                    placeholder: "---Select Accessory---",
                    allowClear: true
                });

                formAddEdit = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: {
                        ignore: [],
                        messages: {
                            StartTime: '',
                            Description: '',
                            AssignUid: ''
                        }
                    }
                }, null, function (result) {
                    var MessageDiv = $("#MessageDiv");
                    Global.ShowMessage(result.message || result.errorMessage, result.isSuccess, 'MessageDiv');
                    if (result.isSuccess) {                        
                        modal.modal('hide');
                        grid.ajax.reload();
                    }
                })

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-status-device").on('loaded.bs.modal', function () {
                var modal = $("#modal-status-device");
                formAddEdit = new Global.FormHelper($(this).find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: {
                        ignore: [],
                        rules: {
                            Priority: { priorityvalidate: true }
                        }
                    }
                }, null, function (result) {

                    var MessageDiv = $("#MessageDiv");
                    Global.ShowMessage(result.message || result.errorMessage, result.isSuccess, 'MessageDiv');
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload();
                    }

                });
            }).on('hidden.bs.modal', function () {
                $('.divoverlay').addClass('hide');
                $(this).removeData('bs.modal');
            });
        }



        function loadDevicegrid() {
            $('.divoverlay').removeClass('hide');
            deviceId = $("#DeviceList").val();
            userId = $("#UserList").val();
            status = $("#DeviceStatus").val();
            grid = new Global.GridHelper('#grid-deviceList', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "device/index",
                        type: "Post",
                        data: { 'deviceId': deviceId, 'userId': userId, 'status': status }
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "1%", "targets": 0 },
                    { "width": "1%", "targets": 1, "className": "rowCenterText" },
                    { "width": "10%", "targets": 2 },
                    { "width": "17%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "12%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "14%", "targets": 7 },
                    { "width": "14%", "targets": 8 },
                    { "width": "5%", "targets": 9 },
                    { "width": "5%", "targets": 10 },
                    { "width": "1%", "targets": 11 }
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "S.NO", sortable: false, searchable: false, visible: true },
                       { name: "Device", data: "device", title: "Device", sortable: false, searchable: false, visible: true },
                       { name: "Accessory", data: "accessory", title: "Accessory", sortable: false, searchable: false, visible: true },
                       { name: "Sim", data: "sim", title: "Sim", sortable: false, searchable: false, visible: true },
                       { name: "User", data: "user", title: "User", sortable: false, searchable: false, visible: true },
                       { name: "AssignBy", data: "assignBy", title: "Assign By", sortable: false, searchable: false, visible: true },
                       { name: "StartTime", data: "startTime", title: "Allocate Time", sortable: false, searchable: false, visible: true },
                       { name: "EndTime", data: "endTime", title: "Submitted Time", sortable: false, searchable: false, visible: true },
                       { name: "Received", data: "received", title: "Received", sortable: false, searchable: false, visible: true },
                       {
                           name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               if (row.status == "Assigned") {
                                   return $("<a />", {
                                       id: "changeStatus",
                                       title: "Update Status",
                                       text: row.status,
                                       href: domain + "device/updateStatus/" + row.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-status-device",
                                       'data-backdrop': "static",
                                       style: 'font-weight: bold',
                                   }).get(0).outerHTML
                               }
                               return row.status;
                           }
                       },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, dataRow, meta) {
                               var actionButtons = "";
                               if (data.displayEdit && data.status == "Assigned") {
                                   actionButtons += $("<a/>", {
                                       id: "editdevice",
                                       title: "edit",
                                       href: domain + "device/add/" + data.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-add-device",
                                       'data-backdrop': "static",
                                       html: $("<i/>", {
                                           class: "glyphicon glyphicon-edit",
                                           style: "color:black"
                                       }),
                                   }).get(0).outerHTML + "&nbsp; "
                               }
                               return actionButtons;
                           }
                       },

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData["status"] == "Assigned") {
                        $('td', nRow).css('background-color', 'Yellow');
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

        function initializeControl() {

            $("#btnReset").on("click", function () {
                $("#DeviceList").val(''),
                $("#UserList").val(''),
                $("#DeviceStatus").val(''),
                loadDevicegrid();
            });

            $("#btnSearch").on("click", function () {
                loadDevicegrid();
            });
        }

        $this.init = function () {
            loadDevicegrid();
            initializeModalWithForm();
            initializeControl();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));