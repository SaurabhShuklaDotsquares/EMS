(function ($) {
    var timer;
    $(document).on({
        ajaxStart: function () {
            timer && clearTimeout(timer);
            timer = setTimeout(function () {
                $('.divoverlay').removeClass('hide');
            }, 1500);
        },
        ajaxStop: function () {
            $('.divoverlay').addClass('hide');
            clearTimeout(timer);
        }

    });

    function index() {
        $this = this;
        function loadLibraryDwnloadPermission() {
            var AdditionalJson;
            LibraryDwnloadPermissiongrid = new Global.GridHelper('#grid-LibraryDwnloadPermission', {
                serverSide: true,
                // destroy: true,
                "pageLength": 25,
                "bFilter": true,
                searchDelay: 800,
                "ordering": false,
                "bAutoWidth": false,
                "bLengthChange": true,
                ajax:
                {
                    url: domain + "librarymanagement/ManageLibraryDownloadPermissionList",
                    type: "Post",
                    cache: false
                    // data: GetFilter()
                },
                "columnDefs": [
                    { "width": "4%", "targets": 0 },
                    { "width": "8%", "targets": 1 },
                    { "width": "8%", "targets": 2 },
                    { "width": "30%", "targets": 3 },
                    { "width": "30%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    //{ "width": "10%", "targets": 6 },
                    //{ "width": "5%", "targets": 7 }
                ],
                columns:
                    [
                        {
                            name: "rowIndex", data: "rowIndex", title: "S. No", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "RoleName", data: "roleName", title: "Role Name", sortable: false, searchable: true, visible: true
                        },
                        {
                            name: "UserName", data: "userName", title: "User Name", sortable: false, searchable: true, visible: true
                            //,render: function (data, type, row, meta) {
                            //    return "<span style='font-weight:bold;'>" + data + "</span>" +
                            //        "<div style='font-size:11px;'>Generated Date : " + row.createdDate + "</div>" +
                            //        "<div style='font-size:11px;'>Last Activity : " + row.modifyDate + "</div>";
                            //}
                        },
                        {
                            name: "Maximum Download in a Day", title: "Maximum Download in a Day", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {

                                var html = '<table style="width:100%;text-align: center;margin-left:-10px;"><tbody><tr>';
                                //var length = 0;
                                //$.each(meta.settings.json, function (j, x) {
                                //    length++;
                                //});

                                var w = (100 / row.fileTypes.length) + '%';
                                $.each(row.fileTypes, function (j, x) {
                                    var chk = 0;

                                    $.each(row.libraryFileTypeList, function (i, v) {

                                        if (x.id == v.libraryFileTypeId) {
                                            html += "<td style='width:" + w + ";'>" + v.maximumDownloadInDay + "</td>";
                                            chk = 1;
                                        }
                                    });
                                    if (chk == 0) {
                                        html += "<td style='width:" + w + ";'></td>";
                                    }
                                });
                                html += '</tr></tbody></table>';
                                return html;

                            }
                        },
                        {
                            name: "Maximum Download in a Month", title: "Maximum Download in a Month", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {

                                var html = '<table style="width:100%;text-align: center;margin-left:-10px;"><tbody><tr>';
                                // var length = 0;
                                //$.each(meta.settings.json, function (j, x) {
                                //    length++;
                                //});

                                var w = (100 / row.fileTypes.length) + '%';
                                AdditionalJson = meta.settings.json;
                                $.each(row.fileTypes, function (j, x) {
                                    var chk = 0;

                                    $.each(row.libraryFileTypeList, function (i, v) {

                                        if (x.id == v.libraryFileTypeId) {
                                            html += "<td style='width:" + w + ";'>" + v.maximumDownloadInMonth + "</td>";
                                            chk = 1;
                                        }
                                    });
                                    if (chk == 0) {
                                        html += "<td style='width:" + w + ";'></td>";
                                    }
                                });
                                html += '</tr></tbody></table>';
                                return html;

                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "&nbsp; ";

                                actionButtons += $("<a/>", {
                                    id: "EditLibPermission",
                                    title: "Edit Permissions",
                                    href: domain + "LibraryManagement/LibraryDownloadPermission/" + data.id,
                                    html: $("<i/>", {
                                        class: "fa fa-edit curser",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";
                                if (data.userLoginId != null) {
                                    actionButtons += $("<a/>", {
                                        id: "DeleteLibPermission",
                                        title: "Delete",
                                        href: domain + "LibraryManagement/Delete/" + data.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-LibDownloadPermission",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-trash",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; "
                                }
                                return actionButtons;
                            }
                        }

                    ],
                "headerCallback": function (thead, data, start, end, display, json) {

                    if (data.length > 0) {
                        var th1 = $(thead).find('th').eq(3);
                        var th2 = $(thead).find('th').eq(4);
                        var th3 = $(thead).find('th').eq(2);
                        var th4 = $(thead).find('th').eq(1);
                        var th5 = $(thead).find('th').eq(0);

                        th1.addClass('paddingRemove');
                        th2.addClass('paddingRemove');

                        th3.addClass('txtcenter');
                        th4.addClass('txtcenter');
                        th5.addClass('txtcenter');
                        var th = '<table  style="width:100%;"><thead><tr>';
                        //var length = 0;
                        //$.each(AdditionalJson, function (j, x) {
                        //    length++;
                        //});
                        if (data.length > 0) {
                            var w = (100 / data[0].fileTypes.length) + '%';
                            for (var i = 0; i < data[0].fileTypes.length; i++) {
                                th += '<th style="width:' + w + ';text-align: center;padding: 0px !important;">' + data[0].fileTypes[i].name + '</th>';
                            }
                        }
                        ////$.each(AdditionalJson, function (i,v) {
                        //for (var i = 0; i < length - 4; i++) {
                        //    th += '<th style="width:' + w + ';text-align: center;">' + AdditionalJson.name + '</th>';
                        //}


                        th += '</tr></thead></table>';
                        th1.html("<div style='text-align: center;'>Maximum Download in a Day</div> <br />" + th);
                        th2.html("<div style='text-align: center;'>Maximum Download in a Month</div> <br />" + th);
                    }
                },
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    //if (aData["status"] == "Completed") {
                    //    $(nRow).addClass("Completed_border");
                    //    //$('td', nRow).css('background-color', '#9DFF9D');
                    //    //$('td', nRow).css('color', 'black');
                    //}
                    if (aData.userName == "") {
                        $(nRow).addClass("background-aqua");
                    }
                },
                "fnDrawCallback": function (oSettings) {

                    $('.pagination .active a').css('background-color', '#ff8c15');
                    $('.pagination .active a').css('border-color', '#e99701');
                }

                //"fnInitComplete": function (oSettings, json) {

                //    //if (oSettings.fnRecordsDisplay() > 0) {
                //    //    homeVM.hasPostSales(true);
                //    //    homeVM.showReportFilters(true);
                //    //} else {
                //    //    homeVM.hasPostSales(false);
                //    //}
                //    //homeVM.postSalesLoaded(true);
                //    //$('#totalPostSalesActivity').html(json.recordsTotal);
                //}

            }
            );
        }

        $("#modal-delete-LibDownloadPermission").on("loaded.bs.modal", function (e) {
            var modal = $(this);
            new Global.FormHelper(modal.find("form"), { updateTargetId: "error-ModalMessage" }, null, function (result) {
                if (result.isSuccess) {
                    modal.modal('hide');
                    window.location.href = result.redirectUrl;
                }
                else {
                    modal.find('#error-ModalMessage').attr('class', 'alert alert-danger').html(result.message || result.errorMessage).show();
                }
            });
        }).on("hidden.bs.modal", function () {
            $(this).removeData('bs.modal');
            $(this).find('.modal-content').empty();
        });

        $this.init = function () {
            loadLibraryDwnloadPermission();
        };
    }
    $(function () {
        var self = new index();
        self.init();
        self.HomeIndex = self;
    });
}(jQuery));