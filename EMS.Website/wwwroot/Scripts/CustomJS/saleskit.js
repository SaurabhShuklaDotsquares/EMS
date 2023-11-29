(function () {

    function SalesKit() {
        $this = this;

        $("#btn_search").click(function () {
            LoadSalesKitGrid();
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
                $.get(domain + 'SalesKit/UpdateStatus', {
                    id: this.value
                });
            });
        }

        function clearData() {
            $('#txt_search').val('');

        }

        function LoadSalesKitGrid() {
            var data1 = { searchName: $('#txt_search').val() };

            var salesKitGrid = new Global.GridHelper('#grid-saleskit', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                stateSave: true,

                ajax:
                {
                    url: domain + "SalesKit/Index",
                    type: "POST",
                    data: data1
                },
                order: [[0, 'desc']],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "50%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "5%", "targets": 6}
                ],
                columns:
                    [
                        { name: "salesKit_ID", data: "salesKit_ID", title: "ID", sortable: false, searchable: false, visible: false },
                        { name: "rowId", data: "rowId", title: "#", sortable: false, searchable: false, searchable: false, visible: true },
                        { name: "salesKit_Name", data: "salesKit_Name", title: "Sales Kit Name", sortable: true, searchable: true, visible: true },
                        { name: "displayOrder", data: "displayOrder", title: "Display Order", sortable: true, searchable: true, visible: true },
                        { name: "parentName", data: "parentName", title: "Sales Kit Type", sortable: false, searchable: false, visible: true },
                        {
                            name: "isActive", data: "isActive", title: "status ", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var content = '';
                                content = content + '<div class="chk-box dis-block clearfix">';
                                if (dataRow.isActive === true) {
                                    content = content + '<label class="switch"><input type="checkbox" title="Active" class="switchBox" id="isActive" name="isActive" value="' + dataRow.salesKit_ID + '" checked/><span class="slider round"></span></label>';
                                }
                                else {
                                    content = content + '<label class="switch"><input type="checkbox" title="InActive" class="switchBox" id="IsActive" name="IsActive" value="' + dataRow.salesKit_ID + '" /><span class="slider round"></span></label>';
                                }
                                content = content + '<label for=IsActive"></label>'
                                content = content + '</div>'
                                return content;
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                return '<a  class="fa fa-edit" data-toggle="modal" data-target="#modal-action-saleskit" href="' + domain + 'SalesKit/AddEdit?id=' + dataRow.salesKit_ID + '" ></a>'
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
            return salesKitGrid;
        }

        function InitializeModel() {
            $(document).delegate("#btn-submit", "click", function () {
                var form = new Global.FormHelper($("#frm-saleskit"));
            });
            $("#modal-action-saleskit").on('shown.bs.modal', function (e) {
                if ($("#IsChild").is(":checked")) {
                    $("#parentSalesKit").removeClass("hide");
                }
                if ($("#IsChild").is(":unchecked")) {
                    $("#parentSalesKit").addClass("hide");
                    $("#ParentId").val("");
                }
                $("#IsChild").on("change", function () {
                    if ($("#IsChild").is(":checked")) {
                        $("#parentSalesKit").removeClass("hide");
                    }
                    else {
                        $("#parentSalesKit").addClass("hide");
                        $("#ParentId").val("");
                    }
                });
            }).on("hidden.bs.modal", function (e) {
                $(this).removeData("bs.modal");

            });


        }

        $this.init = function () {
            InitializeModel();
            LoadSalesKitGrid();

        }
    }

    $(function () {
        var self = new SalesKit;
        self.init();
    });

}(jQuery));

