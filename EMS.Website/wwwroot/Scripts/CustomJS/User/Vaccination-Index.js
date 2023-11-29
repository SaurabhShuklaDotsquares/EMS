(function ($) {
    function VaccinationIndex() {
        var $this = this;
        function loadVaccinationFileGrid() {

            var vaccinationDocGrid = new Global.GridHelper('#grid-vaccination-report', {
                serverSide: true,
                destroy: true,
                "bAutoWidth": false,
                "pageLength": 50,
                "bFilter": false,
                "paging": false,
                ajax:
                {
                    url: domain + "report/vaccinationDetails",
                    type: "POST",
                },
                order: [1, "desc"],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "27%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                ],
                columns:
                    [
                        { name: "id", data: "id", title: "id", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },

                        {
                            name: "employeeName", data: "employeeName", title: "Employee Name", sortable: true, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return dataRow.employeeName + '<br>Team: <b>' + dataRow.projectManagerName + '</b>';
                            }
                        },
                        {
                            name: "phoneNumber", data: "phoneNumber", title: "Phone Number", sortable: false, searchable: false, visible: true,
                        },
                        {
                            name: "email", data: "email", title: "Email", sortable: false, searchable: false, visible: true,
                        },
                        {
                            name: "vaccinationStatus", data: "vaccinationStatus", title: "Vaccination Status", sortable: false, searchable: false, visible: true,
                        },

                        {
                            name: "certificate", data: "certificate", title: "Vaccination Certificate", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var Link = "";
                                if (dataRow.certificate != null) {
                                    Link += '<a download style="color:#101ee5;text-decoration:underline;" href="Upload/Vaccination_Files/' + dataRow.certificate + '" target="_blank">' + dataRow.certificate + '</a></br>';
                                }

                                return Link

                            }
                        },

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
            return vaccinationDocGrid;
        }

        $this.init = function () {
            loadVaccinationFileGrid();
        };
    }

    $(function () {
        var self = new VaccinationIndex();
        self.init();
    });
}(jQuery));