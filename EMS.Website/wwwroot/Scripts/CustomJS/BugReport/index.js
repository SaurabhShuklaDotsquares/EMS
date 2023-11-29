(function ($) {
    function index() {
        var $this = this, grid;

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-bugReport-table', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "ordering": false,
                "language": {
                    searchPlaceholder: "Search By Name"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                    {
                        url: domain + "bugreport/index",
                        type: "Post",
                        data: { "StatusFilterId": $("#StatusFilterId option:selected").val() }
                    },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "10%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "25%", "targets": 3 },
                    { "width": "15%", "targets": 4, visible: isApprover },
                    { "width": "15%", "targets": 5 },
                    { "width": "8%", "targets": 6 },
                    { "width": "10%", "targets": 7 }
                ],
                columns:
                    [
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       {
                           name: "Id", data: "id", title: "Bug Id", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return '<span>' + "Bug-Id : " + data + '</span><br>';
                           }
                       },
                       { name: "Module", data: "module", title: "Module/Section", sortable: false, searchable: false, visible: true },
                       { name: "Description", data: "description", title: "Description", sortable: false, searchable: false, visible: true },
                       { name: "AddedBy", data: "addedBy", title: "Added By", sortable: false, searchable: false },
                       { name: "AddedDate", data: "addedDate", title: "Added Date", sortable: false, searchable: false, visible: true },
                       {
                           name: "Attachment", data: "attachment", title: "Attachment", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               if (data && data != "" && data != undefined) {
                                   return "<a class='ablue' href='" + domain + data + "' target='_blank'> Download </a>";
                               }
                               return "";
                           }
                       },
                       {
                           name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {                              
                               if (data == "Pending" && row.isAllowed) {
                                   return '<a class="ablue" style="text-decoration: underline;cursor:pointer !important;" data-toggle="modal" data-target="#modal-approvestatus" href="' + domain + 'bugreport/status/' + row.id + '">' + data + '</a>';
                               }
                               else if (data == "Inprocess" && row.isAllowed) {
                                   return '<a class="ablue" style="text-decoration: underline;cursor:pointer !important;" data-toggle="modal" data-target="#modal-approvestatus" href="' + domain + 'bugreport/status/' + row.id + '">' + data + '</a>';
                               }
                               else if (data == "Pending" && !row.isAllowed) {
                                   return '<span style="color:#FF8C00;">' + data + '</span>';
                               }
                               else if (data == "Completed") {
                                   return '<span style="color:green;">' + data + '</span>&nbsp;<a style="text-decoration: underline;cursor:pointer !important;" data-toggle="tooltip" title="' + row.comment + '"><i class="fa fa-info-circle" style="color:black"><i/></a>';
                               }
                               else if (data == "Rejected") {
                                   return '<span style="color:red;">' + data + '</span>&nbsp;<a style="text-decoration: underline;cursor:pointer !important;" data-toggle="tooltip" title="' + row.comment + '"><i class="fa fa-info-circle" style="color:black"><i/></a>';
                               }
                               else {
                                   return '<span style="color:#FF8C00;">' + data + '</span>&nbsp;<a style="text-decoration: underline;cursor:pointer !important;" data-toggle="tooltip" title="' + row.comment + '"><i class="fa fa-info-circle" style="color:black"><i/></a>';
                               }
                           }
                       },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    $('[data-toggle="tooltip"]').tooltip();

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
                       
            $("#modal-approvestatus").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null,function (result) {                    
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload();
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            var params = Global.GetUrlVars();            
            if (params.addnew) {
                $("#btnAddNew").click();
            }

            $("#StatusFilterId").on('change', function () {
                intializeGrid();
            })
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