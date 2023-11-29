(function ($) {
    //Get department acccording  to filter
    function PriceList() {
        var $this = this;
        $('#btn_search').click(function () {
            LoadPriceList();
        });
        $("#txt_search").keypress(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                return false;
            }
        })




        function clearData() {
            $('#txt_search').val('');
        }
        function LoadPriceList() {
            $('.loading-common,.loading-overlay').show()
            var data1 = { deptName: $('#txt_search').val() };
            var priceGrid = new Global.GridHelper('#grid-priceList', {
                serverSide: true,
                destroy: true,
                "bPaginate": false,
                "bInfo": false,
                "bFilter": false,
                "searching": true,
                language: {
                    searchPlaceholder: "Enter Technology Name"
                },
                ajax: {
                    url: domain + 'CvManagePrice/EstimatePriceList',
                    type: 'POST',
                    data: data1
                },
                order: [[0, 'desc']],
                "columnDefs": [

                    {
                        "width": "5%",
                        "targets": 0
                    },
                    {
                        "width": "5%",
                        "targets": 1
                    },
                    {
                        "width": "15%",
                        "targets": 2
                    },
                    {
                        "width": "15%",
                        "targets": 3
                    },
                    {
                        "width": "15%",
                        "targets": 4
                    },
                    {
                        "width": "15%",
                        "targets": 5
                    },
                    {
                        "width": "15%",
                        "targets": 6
                    },
                    {
                        "width": "15%",
                        "targets": 7
                    },
                ],
                columns: [

                    {
                        name: 'rowId',
                        data: 'rowId',
                        title: "#",
                        sortable: false,
                        searchable: false
                    },
                    {
                        name: 'roleid',
                        data: 'roleid',
                        title: "roleid",
                        sortable: false,
                        searchable: false,
                        visible: false
                    },
                    {
                        name: 'techid',
                        data: 'techid',
                        title: "techid",
                        sortable: false,
                        searchable: true,
                        visible: false
                    },
                    {
                        name: 'techname',
                        data: 'techname',
                        title: "Technology",
                        sortable: false,
                        searchable: true
                    },
                    {
                        name: 'entryLEVEL',
                        data: 'entryLEVEL',
                        title: "Entry Level",
                        sortable: false,
                        searchable: false
                    },

                    {
                        name: 'onetoTWO',
                        data: 'onetoTWO',
                        title: "1-2 Years",
                        sortable: false,
                        searchable: false
                    },
                    {
                        name: 'threetoSIX',
                        data: 'threetoSIX',
                        title: "3-6 Years",
                        sortable: false,
                        searchable: false
                    },
                    {
                        name: 'sixtoTEN',
                        data: 'sixtoTEN',
                        title: "6-10 Years",
                        sortable: false,
                        searchable: false
                    },
                    {
                        name: 'tenPLUS',
                        data: 'tenPLUS',
                        title: "10+ Years",
                        sortable: false,
                        searchable: false
                    },
                    {
                        name: 'action',
                        data: null,
                        title: "Action",
                        sortable: false,
                        searchable: false,
                        class: "text-center",


                        render: function (data, type, dataRow, meta) {
                            return '<a  class="fa fa-edit" data-toggle="modal" data-target="#modal-price-list" href="' + domain + 'CvManagePrice/AddEditPriceList?RoleId=' + dataRow.roleid + '&TechnologyId=' + dataRow.techid + '"></a>'
                        }
                    }

                ],
                "fnDrawCallback": function () {
                    $('#grid-priceList_filter').css('float', 'left')
                    $('.loading-common,.loading-overlay').hide()
                },

            });
            clearData();

            return priceGrid;

        }




        $('#txt_search').on('change', function () {
            var table = $('#grid-priceList').DataTable();
            //table.column(3).search($(this).val(), true).draw();
            table.columns().every(function () {
                var dataTableColumn = this;

                $(this.footer()).find('input').on('keyup change', function () {
                    dataTableColumn.search(this.value).draw();
                });
            });
        });

        function initilizeModel() {

            $(document).delegate("#btn-submit", "click", function () {
                var form1 = new Global.FormHelper($("#formcvmanageprices"));

            });

            $("#modal-price-list").on("hidden.bs.modal", function (e) {
                $(this).removeData("bs.modal");
            });

        }

        $this.init = function () {
            initilizeModel();
            LoadPriceList();
        };
    }
    $(function () {
        var self = new PriceList();
        self.init();
    });

}(jQuery));