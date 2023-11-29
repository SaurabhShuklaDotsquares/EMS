(function ($) {
    function index() {
        var $this = this, grid, filter = {};

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-lessonlearnt-table', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                ordering: false,
                "pageLength": 10,
                //searching: false,
                "language": {
                    searchPlaceholder: "Search By Name"
                },
                ajax:
                {
                    url: domain + "lessonlearnt/index",
                    type: "Post",
                    data: { searchName: $('#txt_search').val() },
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "15%", "targets": 3 },
                    { "width": "15%", "targets": 4 },
                    { "width": "5%", "targets": 5 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false },
                        { name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false },
                        { name: "WhatLearnt", data: "whatLearnt", title: "What Lesson Learned ?", sortable: false, searchable: false },
                        { name: "CreatedByName", data: "createdByName", title: "Created By", sortable: false, searchable: false },
                        { name: "CreatedDate", data: "createdDate", title: "Created Date", sortable: false, searchable: false },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = '';
                                if (dataRow.allowEdit) {
                                    actionButtons += $("<a/>", {
                                        id: "editlessonlearnt",
                                        title: "edit",
                                        href: domain + "lessonlearnt/addedit/" + data.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-edit-lessonlearnt",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                if (dataRow.allowDelete) {
                                    actionButtons += $("<a/>", {
                                        href: domain + "lessonlearnt/delete/" + data.id,
                                        id: "deletelessonlearnt",
                                        title: "delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-lessonlearnt",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-trash",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML;
                                }
                                return actionButtons;
                            }
                        },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
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
        }

        function intializeModalWithForm() {


            $("#modal-add-edit-lessonlearnt").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelperWithFiles(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload(null, false);
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });


            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-delete-lessonlearnt").on('loaded.bs.modal', function () {

                var modal = $(this);

                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload(null, false);
                            Global.ShowMessage(result.message, true, 'MessageDiv');
                        }
                        else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                        }
                    });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            //$('#btn_search').click(function () {
            //    //filter = { searchName :$('#txt_search').val() };
            //    //Global.search['value'] = $('#txt_search').val();
            //    //grid.draw();
            //    intializeGrid();
            //})

        }

        $this.init = function () {
            intializeGrid();
            intializeModalWithForm();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));