(function ($) {
    function index() {
        var $this = this, grid;

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-event-table', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                ordering: false,
                "pageLength": 50,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search By Title"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "Event/index",
                    type: "Post"
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "20%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "title", data: "title", title: "Title", sortable: false, searchable: false, visible: true },
                        { name: "leaveDate", data: "leaveDate", title: "Leave Date", sortable: false, searchable: false, visible: true },
                        { name: "leaveType", data: "leaveType", title: "Leave Type", sortable: false, searchable: false, visible: true },                        
                        {
                            name: "isActive", data: "isActive", title: "Is Active", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center>';

                                actionButtons += '<div class="chk-box dis-block clearfix">';
                                if (row.isActive == true) {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isActive" name="isActive" value="' + row.id + '" checked/><span class="slider round"></span></label>';
                                }
                                else {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isActive" name="isActive" value="' + row.id + '" /><span class="slider round"></span></label>';
                                }
                                actionButtons += '<label for=isActive"></label>'
                                actionButtons += '</div>&nbsp;&nbsp;'
                                return actionButtons;
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = $("<a/>", {
                                    id: "editevent",
                                    title: "edit",
                                    href: domain + "Event/add/" + data.id,
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-edit",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";

                                if (dataRow.allowDelete) {
                                    actionButtons += $("<a/>", {
                                        href: domain + "Event/delete/" + data.id,
                                        id: "deleteevent",
                                        title: "delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-event",
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
                    $('.divoverlay').addClass('hide');
                    $('.switchBox').on('change', function () {
                        var switchElement = this;
                        $.get(domain + 'Event/ApprovedStatus', {
                            id: this.value
                        });
                    });
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

            $("#modal-delete-event").on('loaded.bs.modal', function () {

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