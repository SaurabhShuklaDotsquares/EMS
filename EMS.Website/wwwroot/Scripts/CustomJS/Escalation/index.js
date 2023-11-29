/*global jQuery, Global,secureDomain */
(function () {
    function ManageEscalation() {
        $this = this;

        function LoadEscalationGrid() {

            grid = new Global.GridHelper('#grid-escalationlist', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                "pageLength": 25,
                "bFilter": true,
                "bAutoWidth": false,
                "language": {
                    searchPlaceholder: "Search By Project"
                },
                ajax:
                {
                    url: domain + "escalation/Index",
                    type: "POST",
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "28%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "15%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "width": "5%", "targets": 6 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Project", data: "project", title: "Project", sortable: false, searchable: false, visible: true },
                        { name: "Escalation Date", data: "escalationDate", title: "Escalation Date", sortable: false, searchable: false, visible: true },
                        { name: "Escalation Type", data: "type", title: "Escalation Type", sortable: false, searchable: false, visible: true },
                        {
                            name: "isActive", data: "isActive", title: "Active", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data == "FALSE") {
                                    return '<center><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i></center>';
                                } else {
                                    return '<center><i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i></center>';
                                }
                            }
                        },
                        { name: "CreatedDate", data: "createdDate", title: "Created Date", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center>';
                                actionButtons += $("<a/>", {
                                    id: "addEdit",
                                    class: "",
                                    href: domain + "Escalation/AddEdit/?id=" + row.id,
                                    html: $("<i/>", {
                                        class: "fa fa-pencil",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp;",
                                }).get(0).outerHTML + "&nbsp; ";
                                return actionButtons;
                            }
                        }

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData.isSubmitted) {
                        $('td', nRow).css({ 'background-color': '#f1f1f1', 'color': 'black' });
                    }
                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e18f00');
                    $('.pagination .active a').css('border-color', '#e18f00');
                }
            });
        }

        $this.init = function () {
            LoadEscalationGrid();
        }
    }

    $(function () {
        var self = new ManageEscalation;
        self.init();
    });
}(jQuery));
