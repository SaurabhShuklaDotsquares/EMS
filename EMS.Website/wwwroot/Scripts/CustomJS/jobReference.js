/*global jQuery, Global,secureDomain */
(function ($) {
    function JobReference() {
        var $this = this, formJobReference;

        function initializeForm() {
            $("#modal-viewdetail-add-edit").on('loaded.bs.modal', function (e) {
                $(this).on('keyup keypress', function (e) {
                    var code = e.keyCode || e.which;
                    if (code == 13) {
                        e.preventDefault();
                        return false;
                    }
                });

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }

        function LoadReferencesList() {
            var grid = new Global.GridHelper('#grid-references', {
                serverSide: true,
                destroy: true,
                "pageLength": 1000,
                "bFilter": false,
                pagging: false,
                ajax: {
                    url: domain + 'jobreference/viewreferences',
                    type: 'POST',
                    data: { currentOpeningId: $("#hdncurrentOpening").val() }
                },
                order: [[0, 'desc']],
                columns: [
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                    { name: 'name', data: 'name', title: "Name", sortable: false, searchable: false },
                    { name: 'email', data: 'email', title: "Email", sortable: false, searchable: false },
                    { name: 'post', data: 'post', title: "Post", sortable: false, searchable: false },
                    { name: 'mobileNo', data: 'mobileNo', title: "Mobile No.", sortable: false, searchable: false },
                     { name: 'referedBy', data: 'referedBy', title: "Referred By", sortable: false, searchable: false },
                     { name: 'status', data: 'status', title: "Status", sortable: false, searchable: false }
                     ,
            {
                name: 'action', data: null, title: "Action", sortable: false, searchable: false, render: function (data, type, dataRow, meta) {

                    return '<a href="' + domain + 'jobreference/viewdetail/' + dataRow.id + '" data-toggle="modal" data-target="#modal-viewdetail" > <img src="' + domain + '/images/view_icon.png" title="View Detail" alt="View Detail" width="16" height="16" border="0"></a>';
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
                }
            });
            return grid;
        }

        $this.init = function () {
            LoadReferencesList();
        };
    }
    $(function () {
        var self = new JobReference();
        self.init();
    });
}(jQuery));

