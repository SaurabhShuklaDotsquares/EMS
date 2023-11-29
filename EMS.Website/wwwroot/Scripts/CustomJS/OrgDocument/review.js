(function ($) {
    function Review() {
        var $this = this, grid, selectedDoc;

        function Intializecontrol() {
            var modal = $("#modal-orgApprovedDoc");
            modal.on('loaded.bs.modal', function () {

                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null, function (result) {

                    var $MessageDiv = $('#MessageDiv');

                    if (Global.IsNotNullOrEmptyString(result.message)) {
                        $MessageDiv.addClass('alert-success').removeClass('alert-danger');
                        $MessageDiv.empty().html(result.message);
                    }
                    else if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                        $MessageDiv.addClass('alert-danger').removeClass('alert-success');
                        $MessageDiv.empty().html(result.errorMessage);
                    }

                    $MessageDiv.show();
                    window.setTimeout(function () {
                        $MessageDiv.html('');
                        $MessageDiv.hide();
                    }, 5000);

                    modal.modal('hide');

                    loadOrgDocGrid();

                });

                form.find('#selectedDoc').html(selectedDoc);

            })
                .on('hidden.bs.modal', function () {
                    modal.removeData('bs.modal');
                    modal.find('.modal-content').empty();
                });

            grid.on('click', 'a.btn-link', function () {
                selectedDoc = $(this).closest('tr').find('td:eq(1)').html();
            });
        }

        function loadOrgDocGrid() {
            grid = new Global.GridHelper('#grid-orgdocument', {
                serverSide: true,
                destroy: true,
                "pageLength": 20,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "orgdocument/review",
                        type: "Post"
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "1%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "20%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       {
                           name: "Document", data: "document", title: "Document", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return '<a class="ablue" target="_blank" href="' + row.documentPath + '">' + data + " v" + row.version + '</a>'
                           }
                       },
                       {
                           name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return row.status + (row.approvedDate ? "<br>" + row.approvedDate : "")
                           }
                       },
                       { name: "Reviewers", data: "reviewers", title: "Approvers", sortable: false, searchable: false, visible: true },
                       {
                           name: "Departments", data: "departments", title: "Access to", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               var accessTo = '';
                               accessTo += row.departments ? '<b>Departments</b> : ' + row.departments : '';
                               accessTo += row.roles ? (accessTo != '' ? '<br>' : '') + '<b>Roles</b> : ' + row.roles : '';

                               return accessTo;
                           }
                       },
                       {
                           name: "Action", data: "allowApproval", title: "Action", sortable: false, searchable: false,
                           render: function (data, type, row, meta) {
                               var actions = '';
                               if (row.allowApproval && !row.selfApproved) {
                                   actions = '<a title="Approve document" class="btn btn-default btn-sm" data-backdrop="static" data-target="#modal-orgApprovedDoc" data-toggle="modal", href="' + domain + "orgdocument/OrgDocApprove/" + row.id + '"><i class="fa fa-check-square-o"></i> Approve</a>';
                               }
                               actions += ' <a class="btn btn-default btn-sm" href="' + domain + 'orgdocument/addedit/' + row.id + '"><i class="fa fa-pencil-square-o"></i> Edit</a>';

                               return actions;
                           }
                       }
                    ],
                "fnDrawCallback": function (oSettings) {
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

        $this.init = function () {
            loadOrgDocGrid();
            Intializecontrol();
        };
    }
    $(function () {
        var self = new Review();
        self.init();
    });
}(jQuery));