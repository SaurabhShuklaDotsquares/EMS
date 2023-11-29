(function ($) {

    function Index() {
        $this = this;

        $('#btnsearch').click(function () {
            EstimateHostingPackageGrid();
        })

        //Method to Load EstimateDocument Grid
        function EstimateHostingPackageGrid() {
            var data1 = { packagename: $('#packagename').val() };
            var EstimateDocGrid = new Global.GridHelper('#grid-estimatehostingpackage', {
                serverSide: true,
                destroy: true,
                //"bAutoWidth": false,
                "pageLength": 20,
                "bFilter": false,
                ajax:
                {
                    url: domain + "estimatehostingpackage",
                    type: "POST",
                    data: data1,
                },
                order: [2, "desc"],
                columns:
                    [
                        { name: "id", data: "id", title: "id", sortable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false},
                        { name: "Name", data: "name", title: "Package Name"},
                        { name: "PackageDetail", data: "packageDetail", title: "Package Detail"},
                        { name: "EstimateTechnologyName", data: "estimateTechnologyName", title: "Estimate Technologies Name" },
                        { name: "CountryName", data: "countryName", title: "Country Name" },
                        {
                            name: "HostingCost", data: "hostingCost", title: "Hosting Cost", render: function (data, type, dataRow, meta) {
                                return `${data} ${dataRow.hostingCostType}`
                            }
                        },
                        {
                            name: "SetupCost", data: "setupCost", title: "Setup Cost", render: function (data, type, dataRow, meta) {
                                return `${data} ${dataRow.setupCostType}`
                            }
                        },
                        {
                            name: "IsActive", data: "isActive", title: "Active", render: function (data, type, dataRow, meta) {
                                if (data) {;
                                    return `<i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i>`;
                                }
                                else {
                                    return `<i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i>`;
                                }
                            }
                        },
                        {
                            name: "action", data: null, title: "action", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return '<a class="fa fa-edit" href="' + domain + 'estimatehostingpackage/manage/' + dataRow.id + '"></a>';
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
            })
            return EstimateDocGrid;
        }


        function InitializeModal() {

        }

        $this.init = function () {

            InitializeModal()
            EstimateHostingPackageGrid();

        }
    }
    $(function () {
        var self = new Index();
        self.init();
    })

})(jQuery);