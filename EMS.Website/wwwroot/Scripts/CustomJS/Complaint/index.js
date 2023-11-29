(function ($) {
    function index() {
        var $this = this, grid;
        $("#btnSearch").on("click", function () {
            if ($('#Internal').prop('checked')) {
                $('#Internal').val(1)
            }
            else {
                $('#Internal').val(0)
            }

            if ($('#Client').prop('checked')) {
                $('#Client').val(2)
            }
            else {
                $('#Client').val(0)
            }
            intializeGrid();
        })

        $("#btnReset").on("click", function () {
            $('#ProjectId').val('0')
            $('#EmployeeId').val('0')
            $('#Internal').prop('checked', false)
            $('#Internal').val(0)
            $('#Client').prop('checked', false)
            $('#Client').val(0),
            $('#PriorityId').val(0)
            intializeGrid();
        })

        function intializeGrid() {

            $('.loading-common,.loading-overlay').show()

            grid = new Global.GridHelper('#grid-complaints-table', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                ordering: false,
                "pageLength": 50,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search By Name"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "complaint/index",
                    type: "Post",
                    data: {
                        ProjectId: $('#ProjectId').val(),
                        EmployeeId: $('#EmployeeId').val(),
                        InternalTypeId: $('#Internal').val(),
                        ClientTypeId: $('#Client').val(),
                        PriorityId: $('#PriorityId').val()
                    }
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "18%", "targets": 2 },
                    { "width": "8%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "12%", "targets": 5 },
                    { "width": "11%", "targets": 6 },
                    { "width": "9%", "targets": 7 },
                    { "width": "10%", "targets": 8 },
                    { "width": "1%", "targets": 9 }
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Name", data: "name", title: "EMPLOYEE NAME", sortable: false, searchable: false, visible: true },
                        {
                            name: "project",
                            data: "project",
                            title: "Project",
                            sortable: false,
                            searchable: false,
                            visible: true
                        },
                        { name: "ComplaintType", data: "complaintType", title: "Complaint Type", sortable: false, searchable: false, visible: true },
                        {
                            name: "PriorityType",
                            data: "priorityType",
                            title: "Priority",
                            sortable: false,
                            searchable: false,
                            visible: true
                        },
                        { name: "ClientComplaint", data: "clientComplaint", title: "Complaint / TL or PL Explanation", sortable: false, searchable: false, visible: true },
                        { name: "AreaofImprovement", data: "areaofImprovement", title: "Area of Improvement", sortable: false, searchable: false, visible: true },
                        { name: "AddedBy", data: "addedBy", title: "Added By", sortable: false, searchable: false, visible: true },
                        { name: "AddedDate", data: "addedDate", title: "Added Date", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "";
                                if (dataRow.allowDelete) {
                                    actionButtons = $("<a/>", {
                                        id: "editcomplaint",
                                        title: "edit",
                                        href: domain + "complaint/add/" + data.id,
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";


                                    actionButtons += $("<a/>", {
                                        href: domain + "complaint/delete/" + data.id,
                                        id: "deletecomplaint",
                                        title: "delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-complaint",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-trash",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML;
                                }
                                return actionButtons;
                            }
                        },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                },
                "fnDrawCallback": function (oSettings) {
                    $('.loading-common,.loading-overlay').hide();
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

        function intializeModalWithForm() {

            $("#modal-delete-complaint").on('loaded.bs.modal', function () {

                var modal = $(this);

                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload(null, false);
                            Global.ShowMessage(result.message, true, 'MessageDiv');
                        }
                        else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                        }
                    });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        $this.init = function () {
            intializeGrid();
            intializeModalWithForm();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));