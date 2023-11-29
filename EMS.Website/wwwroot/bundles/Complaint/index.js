(function ($) {
    function index() {
        var $this = this, grid;

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

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
                        type: "Post"                       
                    },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "12%", "targets": 5 },
                    { "width": "13%", "targets": 6 },
                    { "width": "6%", "targets": 7 },
                ],
                columns:
                    [
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "Name", data: "name", title: "EMPLOYEE NAME", sortable: false, searchable: false, visible: true },
                       { name: "ComplaintType", data: "complaintType", title: "Complaint Type", sortable: false, searchable: false, visible: true },
                       { name: "PriorityType", data: "priorityType", title: "Priority", sortable: false, searchable: false, visible: true },
                       { name: "AreaofImprovement", data: "areaofImprovement", title: "Area of Improvement", sortable: false, searchable: false, visible: true },
                       { name: "AddedBy", data: "addedBy", title: "Added By", sortable: false, searchable: false, visible: true },
                       { name: "AddedDate", data: "addedDate", title: "Added Date", sortable: false, searchable: false, visible: true },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, dataRow, meta) {
                             var actionButtons = $("<a/>", {
                                   id: "editcomplaint",
                                   title: "edit",
                                   href: domain + "complaint/add/" + data.id,
                                   html: $("<i/>", {
                                       class: "glyphicon glyphicon-edit",
                                       style: "color:black"
                                   }),
                               }).get(0).outerHTML + "&nbsp; ";

                             if (dataRow.allowDelete) {
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