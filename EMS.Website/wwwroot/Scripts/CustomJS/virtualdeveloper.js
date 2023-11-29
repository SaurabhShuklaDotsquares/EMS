﻿(function () {

    function VirtualDeveloper() {
        $this = this;

        $("#btn_search").click(function () {
            LoadVirtualDeveloperGrid();
        });
        $("#txt_search").keypress(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                return false;
            }
        })
        function initGridControlWithEvents() {
            $('.switchBox').on('change', function () {
                var switchElement = this;
                $.get(domain + 'VirtualDeveloper/UpdateStatus', {
                    id: this.value
                });
            });
        }

        function clearData() {
            $('#txt_search').val('');
            $('#ddl_pmUid').val('0');
        }

        function LoadVirtualDeveloperGrid() {
            var data1 = { searchName: $('#txt_search').val(), ddl_pmUID: $('#ddl_pmUid').val() };

            var virtualDeveloerGrid = new Global.GridHelper('#grid-virtualDeveloper', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                stateSave: true,

                ajax:
                    {
                        url: domain + "VirtualDeveloper/Index",
                        type: "POST",
                        data: data1
                    },
                order: [[0, 'desc']],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "50%", "targets": 2 },
                    { "width": "30%", "targets": 3 },
                    { "width": "5%", "targets": 4 },
                    { "width": "5%", "targets": 5 }
                ],
                columns:
                    [
                        { name: "virtualDeveloper_ID", data: "virtualDeveloper_ID", title: "ID", sortable: false, searchable: false, visible: false },
                        { name: "rowId", data: "rowId", title: "#", sortable: false, searchable: false, searchable: false, visible: true },
                        { name: "virtualDeveloper_Name", data: "virtualDeveloper_Name", title: "Developer Name", sortable: false, searchable: false, visible: true },
                        { name: "emailid", data: "emailid", title: "emailId", sortable: false, searchable: false, visible: true },
                        {
                            name: "isactive", data: "isactive", title: "status ", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var content = '';
                                content = content + '<div class="chk-box dis-block clearfix">';
                                if (dataRow.isactive === true) {
                                    content = content + '<label class="switch"><input type="checkbox" title="Active" class="switchBox" id="isactive" name="isactive" value="' + dataRow.virtualDeveloper_ID + '" checked/><span class="slider round"></span></label>';
                                }
                                else {
                                    content = content + '<label class="switch"><input type="checkbox" title="InActive" class="switchBox" id="IsActive" name="IsActive" value="' + dataRow.virtualDeveloper_ID + '" /><span class="slider round"></span></label>';
                                }
                                content = content + '<label for=IsActive"></label>'
                                content = content + '</div>'
                                return content;
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                return '<a  class="fa fa-edit" data-toggle="modal" data-target="#modal-action-virtualDeveloper" href="' + domain + 'VirtualDeveloper/AddEditDeveloper?id=' + dataRow.virtualDeveloper_ID + '" ></a>'
                            }
                        }

                    ],
                "fnDrawCallback": function (oSettings) {
                    initGridControlWithEvents();
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
            return virtualDeveloerGrid;
        }

        function InitializeModel() {
            $(document).delegate("#btn-submit", "click", function () {
                var form = new Global.FormHelper($("#frm-virtualDeveloper"));

            });
            $("#modal-action-virtualDeveloper").on("hidden.bs.modal", function (e) {
                $(this).removeData("bs.modal");
              
            });
        }

        $this.init = function () {
            InitializeModel();
            LoadVirtualDeveloperGrid();

        }
    }

    $(function () {
        var self = new VirtualDeveloper;
        self.init();
    });

}(jQuery));

