(function ($) {
    function index() {
        var $this = this, grid;

        function initGridControlWithEvents() {
            $('.switchBox').on('change', function () {
                var switchElement = this;
                $.get(domain + 'Technology/UpdateStatus', {
                    id: this.value
                });
            });
        }

        function initializeModalWithForm() {
            $("#modal-add-Technology").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: []}
                }, null, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        $this.refreshTaskGrid();
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

        }

        function loadtaskgrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-TechnologyList', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": true,                
                "bAutoWidth": false,
                "bLengthChange": false,
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                    {
                        url: domain + "technology/index",
                        type: "Post",
                    },
               
               
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "81%", "targets": 2 },
                    { "width": "9%", "targets": 3 },
                    { "width": "6%", "targets": 4 }
                ],
                columns:
                    [
                       { name: "TechId", data: "techId", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "Title", data: "title", title: "Technology Name", sortable: false, searchable: false, visible: true },
                       {
                           name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                           render: function (data, type, dataRow, meta) {
                               var content = "";
                                content = content + '<div class="chk-box dis-block clearfix">';
                                if (dataRow.status == true) {
                                    content = content + '<label class="switch"><input type="checkbox" title="Active" class="switchBox" id="isactive" name="isactive" value="' + dataRow.techId + '" checked/><span class="slider round"></span></label>';
                                }
                                else {
                                    content = content + '<label class="switch"><input type="checkbox" title="InActive" class="switchBox" id="IsActive" name="IsActive" value="' + dataRow.techId + '" /><span class="slider round"></span></label>';
                                }
                                content = content + '<label for=IsActive"></label>'
                                content = content + '</div>'
                                return content;
                            }
                       },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, dataRow, meta) {
                               var actionButtons = $("<a/>", {
                                   id: "editTechnology",
                                   title: "edit",
                                   href: domain + "technology/add/" + data.techId,
                                   'data-toggle': "modal",
                                   'data-target': "#modal-add-Technology",
                                   'data-backdrop': "static",
                                   html: $("<i/>", {
                                       class: "glyphicon glyphicon-edit",
                                       style: "color:black"
                                   }),
                               }).get(0).outerHTML + "&nbsp; "

                               return actionButtons;
                           }
                       },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                },
                "fnDrawCallback": function (oSettings) {
                    initGridControlWithEvents();
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

        $this.refreshTaskGrid = function () {
            grid.ajax.reload();
        };

        $this.init = function () {
            loadtaskgrid();
            initializeModalWithForm();
        };
    }
    $(function () {
        var self = new index();
        self.init();
        $.fn.TaskGridIndex = self;
    });
}(jQuery));