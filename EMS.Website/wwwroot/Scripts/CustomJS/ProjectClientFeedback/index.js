(function () {
    function ProjectClientFeedback() {
        var $this = this, grid;
        function LoadProjectClientFeedbackGrid() {
            var filter = { projectId: $('#ProjectList').val() }
            grid = new Global.GridHelper('#grid-projectclientfeedback', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                //"ordering": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "ProjectClientFeedback/Index",
                    type: "Post",
                    data: filter

                },

                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "20%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "8%", "targets": 3 },
                    { "width": "13%", "targets": 4 },
                    { "width": "20%", "targets": 5 },
                    { "width": "20%", "targets": 6 },
                    { "width": "2%", "targets": 7 },

                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "ProjectName", data: "projectName", title: "Project Name", sortable: true, searchable: false, visible: true },
                        { name: "clientName", data: "clientName", title: "Client Name", sortable: true, searchable: false, visible: true },
                        { name: "status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                        { name: "CommentDate", data: "commentDate", title: "Comment Date", sortable: false, searchable: false, visible: true },
                        { name: "comment", data: "comment", title: "Meet Requirement", sortable: false, searchable: false, visible: true },
                        { name: "valuesAbout", data: "valuesAbout", title: "Values About Dotsquares", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = $("<a/>", {
                                    class: "viewDetail",
                                    title: "View Detail",
                                    href: domain + "ProjectClientFeedback/Detail/" + dataRow.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-projectClientFeedback-detail",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-eye-open",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp;"
                                if (dataRow.isEdit) {
                                    actionButtons += $("<a/>", {
                                        class: "viewDetail",
                                        title: "Edit client feedback",
                                        href: domain + "ProjectClientFeedback/AddEditFeedback/" + dataRow.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-AddEditFeedback-detail",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML;
                                }

                                return actionButtons;
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
            return grid;
        }

        function Initialize() {
            $("#modal-projectClientFeedback-detail").on('loaded.bs.modal', function () {
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-AddEditFeedback-detail").on('loaded.bs.modal', function () { 
                var modal = $(this);
                var form = new Global.FormHelperWithFiles(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                },function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload(null, false);
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });

                form.find('.delete-document').click(function (e) {
                    var $button = $(this);
                    var isConfirm = confirm('Are you want to sure to delete this document?');
                    if (isConfirm) {
                        $.post($button.data('href'), function (result) {

                            if (result.isSuccess) {
                                $button.parents('li').remove();
                            }
                            else {
                                alert(result.message);
                            }

                        });
                    }
                });

                form.find("#Commentdate").datepicker({
                    dateFormat: "dd/mm/yy",
                    maxDate: new Date()
                });
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        $('#btn_search').click(function () {
            LoadProjectClientFeedbackGrid();
        })
        $this.init = function () {

            LoadProjectClientFeedbackGrid();
            Initialize();

        }
    }

    $(function () {
        var self = new ProjectClientFeedback();
        self.init();
    })

}(jQuery))
