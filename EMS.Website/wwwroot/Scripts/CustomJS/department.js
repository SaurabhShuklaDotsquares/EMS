(function ($) {
    //Get department acccording  to filter
    function Department() {
        var $this = this;
        $('#btn_search').click(function () {
            LoadDepartmentList();
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
                $.get(domain + 'Department/UpdateStatus', {
                    id: this.value
                }, function (result) {

                });
            });
        }
        function clearData() {
            $('#txt_search').val('');
        }
        function LoadDepartmentList() {

            var data1 = { deptName: $('#txt_search').val() };

            var departmentGrid = new Global.GridHelper('#grid-department', {
                serverSide: true,
                destroy: true,
                paging: false,
                "bAutoWidth": false,
                "bInfo": false,
                "bFilter": false,
                ajax: {
                    url: domain + 'department/index',
                    type: 'POST',
                    data: data1
                },
                order: [[0, 'desc']],
                "columnDefs": [
              { "width": "0%", "targets": 0 },
              { "width": "70%", "targets": 2 },
              { "width": "8%", "targets": 3 },
              { "width": "6%", "targets": 4 },
              { "width": "10%", "targets": 5 },

                ],
                columns: [
                    {
                        name: 'deptId',
                        data: 'deptId',
                        title: "deptid",
                        sortable: false,
                        searchable: false,
                        visible: false
                    },
                    {
                        name: 'rowId',
                        data: 'rowId',
                        title: "#",
                        sortable: false,
                        searchable: false
                    },

                    {
                        name: 'name',
                        data: 'name',
                        title: "Name",
                        sortable: false,
                        searchable: false
                    },
                    {
                        name: 'deptcode',
                        data: 'deptcode',
                        title: "Deptcode",
                        sortable: false,
                        searchable: false
                    },
                    {
                        name: 'isActive',
                        data: 'isActive',
                        title: 'Status',
                        sortable: false,
                        searchable: false,
                        render: function (data, type, dataRow, meta) {
                            var content = '';
                            content = content + '<div class="chk-box dis-block clearfix">';
                            if (dataRow.isActive === true) {
                                content = content + '<label class="switch"><input type="checkbox" title="Active" class="switchBox" id="IsActive" name="IsActive" value="' + dataRow.deptId + '" checked/><span class="slider round"></span></label>';
                            }
                            else {
                                content = content + '<label class="switch"><input type="checkbox" title="InActive" class="switchBox" id="IsActive" name="IsActive" value="' + dataRow.deptId + '" /><span class="slider round"></span></label>';
                            }
                            content = content + '<label for=IsActive"></label>'
                            content = content + '</div>'
                            return content;
                        }
                    },
                    {
                        name: 'action',
                        data: null,
                        title: "Action",
                        sortable: false,
                        searchable: false,
                        render: function (data, type, dataRow, meta) {
                            return '<a  class="fa fa-edit" data-toggle="modal" data-target="#modal-action-department" href="' + domain + 'Department/AddEditDepartment?deptId=' + dataRow.deptId + '" ></a>'
                        }
                    }

                ],
                "fnDrawCallback": function () {
                    /*initGridControlWithEvents();*/
                },

            });
            clearData();
            return departmentGrid;


        }


        function initilizeModel() {

            $(document).delegate("#btn-submit", "click", function () {
                var form1 = new Global.FormHelper($("#frm-department"));
    

            });

            $("#modal-action-department").on("hidden.bs.modal", function (e) {
                $(this).removeData("bs.modal");
            });

        }

        $this.init = function () {
            initilizeModel();
            LoadDepartmentList();

        };
    }
    $(function () {
        var self = new Department();
        self.init();
    });

}(jQuery));