/*global jQuery, Global,secureDomain */

(function () {
    function ManageRooms() {
        $this = this;
        var startDate = '', endDate = '';
        function initializeForm() {

        }

        function LoadRoomsGrid() {
            var manageUserGrid = new Global.GridHelper('#grid-Rooms', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": true,
                ajax: {
                    url: domain + 'MeetingRoom/GetRooms',
                    type: 'POST',
                },
                order: [[0, 'desc']],
                "columnDefs": [
                 //{ "width": "0%", "targets": 0 },
                 //{ "width": "5%", "targets": 1 },
                ],
                columns: [
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                    { name: 'roomId', data: 'roomId', title: "roomId", sortable: false, searchable: false, visible: false },
                    { name: 'roomName', data: 'roomName', title: "Meeting Room", sortable: true, searchable: false },
                    { name: 'officeName', data: 'officeName', title: "Building Name", sortable: true, searchable: false },
                    {
                        name: 'action', data: null, title: "Color", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            return '<div class="' + full.colorClass + '"></div>';
                        }
                    },
                    {
                        name: 'action', data: null, title: "Action", className: "text-center", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            return '<a class="trans-btn"  href="' + domain + 'MeetingRoom/ManageRoom/' + full.roomId + '" "><i class="fa fa-edit"></i></a>';
                        }
                    }
                ],

                //"fnInitComplete": function (oSettings, json) {
                //    if (startDate == undefined) {
                //        startDate = '';
                //    }
                //    if (endDate == undefined) {
                //        endDate = '';
                //    }
                //    var html = '<input type="button" id="searchDate" class="btn btn-warning pull-right" value="Search" />';
                //    html = html + '<div class="col-md-3 pull-right margin-right10"><input type="text" name="EndDate" id="EndDate" class="form-control searchfilter" placeholder="End Date" value="' + endDate + '" /></div>';
                //    html = html + '<div class="col-md-3 pull-right"><input type="text" name="StartDate" id="StartDate" class="form-control searchfilter" placeholder="Start Date" value="' + startDate + '" /></div>';
                //    if (json.totalLeave != undefined && json.totalLeave != 0) {
                //        html = html + '<div class="col-md-4 pull-right text-right"><label style="padding: 10px 0; margin-bottom:0; font-weight:bold;">Total Applied Leave: ' + json.totalLeave + (json.totalLeave == 1 ? ' Day' : ' Days') + '</label></div>'
                //    }
                //    $('.dataTables_wrapper > div.row:first > div:last').html(html);
                //    //console.log(json);
                //}
            });
            return manageUserGrid;
        }



        $this.init = function () {
            initializeForm();
            LoadRoomsGrid();
            load();
        }
    }

    $(function () {
        var self = new ManageRooms;
        self.init();
    });

}(jQuery));
