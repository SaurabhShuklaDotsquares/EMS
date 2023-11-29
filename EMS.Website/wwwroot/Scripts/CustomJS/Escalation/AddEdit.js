(function ($) {
    'use strict';
    function Escalation() {
        var $this = this;
        var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
        function InitializeForm() {

            //var form = new Global.FormHelper($("form"), {
            //    updateTargetId: "validation-summary",
            //    validateSettings: { ignore: '' }
            //});

            $('#modal-conclusion').on('loaded.bs.modal', function (e) {
                new Global.FormHelperWithFiles($("#frm-Manage-Conclusion"), {
                    updateTargetId: "validation-summary",
                    validateSettings: {
                        ignore: []
                    }
                }, function onSuccess(result) {
                    //debugger;
                    if (result.message) {
                        $('.gallery').html('');
                        $("#frm-Manage-Conclusion")[0].reset();
                        // alert(result.message);
                    }
                    if (result.errorMessage) {
                        // alert(result.errorMessage);
                    }
                });
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                //LoadEstimateDocGrid();
            })

            var manageEscalationForm = new Global.FormHelperWithFiles($("#frm-Manage-Escalation"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: []
                }
            }, function onSuccess(result) {
                //console.log(result);
                if ($('#SendEmail').is(':checked')) {
                    $.ajax({
                        url: domain + 'Escalation/SendEmail',
                        type: 'Get',
                        success: function (data) {
                            $("#modal-email").modal("show");
                            $("#modal-email .modal-content").html(data);
                            $("#email").val(result.data.emails);
                            $("#values").val(JSON.stringify(result.data.values));
                        }
                    });
                } else {
                    window.location.href = result.redirectUrl;
                }
                if (result.message) {
                    $('.gallery').html('');
                    $("#frm-Manage-Escalation")[0].reset();
                    // alert(result.message);
                }
                if (result.errorMessage) {
                    // alert(result.errorMessage);
                }
            });

            $(".datepicker").datepicker({
                dateFormat: "dd/mm/yy"
            });

            addSelect2("select2");
            //addSelect2("projectSelect", "--Select Projects--");
            addSelect2("userSelect", "--Select Users--");
            CKEDITOR.replace('EscalationDetails');
            attachEventCKEditor('EscalationDetails');
            CKEDITOR.replace('RootCauseAnalysisDesctiption');
            attachEventCKEditor('RootCauseAnalysisDesctiption');
        }


        function addSelect2(selector, placeHolder, isMultiple) {
            $("." + selector).select2({
                placeholder: placeHolder,
                allowClear: true,
                width: '100%'
            });
        }

        function attachEventCKEditor(instance) {
            CKEDITOR.instances[instance].on("instanceReady", function (e) {
                e.editor.document.on("keyup", function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }

        var imagesPreview = function (file, placeToInsertImagePreview) {
            if (file) {
                var reader = new FileReader();
                reader.onload = function (event) {
                    $($.parseHTML('<img>')).attr('src', event.target.result).appendTo(placeToInsertImagePreview);
                }
                reader.readAsDataURL(file);
            }
        };

        $(".delete").click(function () {
            var id = $(this).data("id");
            if (confirm("Are you sure you want to delete this item?")) {
                $.ajax({
                    url: domain + 'Escalation/DeleteDocumentFile',
                    type: 'POST',
                    data: { id: id },
                    success: function (data) {
                        $("#img" + id).remove();
                    }
                });
                return true;
            }
            return false;
        });

        $('#gallery-photo-add').on('change', function (evt) {
            //debugger;
            for (var i = 0; i < evt.target.files.length; i++) {
                var ext = evt.target.files[i].name.split('.').pop().toLowerCase();
                if ($.inArray(ext, ['gif', 'png', 'jpg', 'jpeg']) == -1) {
                    // //debugger;
                    $('#gallery-photo-add').val('');
                    $('div.gallery').empty().html();
                    alert("Invalid extensions, allowed extensions are: " + _validFileExtensions.join(", "));
                    return false;
                }
                if (Math.round(evt.target.files[i].size / (1024 * 1024)) > 2) {
                    $('#gallery-photo-add').val('');
                    $('div.gallery').empty().html();
                    alert('Max Upload image size is 2MB only');
                    return false;
                }
                else {
                    $('div.gallery').empty().html();
                    imagesPreview(evt.target.files[i], 'div.gallery');
                }
            };
        });

        function LoadEscalationGrid() {
            var uid = $("#Uid").val();
            var grid = new Global.GridHelper('#grid-conclusionlist', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                "pageLength": 25,
                "bFilter": true,
                "bAutoWidth": false,
                "language": {
                    searchPlaceholder: "Search By username"
                },
                ajax:
                {
                    url: domain + "escalation/GetEscalationConclusion",
                    type: "POST",
                    data: { escalationId: $("#Id").val() }
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "65%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "5%", "targets": 3 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Name", data: "name", title: "User Name", sortable: false, searchable: false, visible: true },
                        { name: "Resolution", data: "resolution", title: "Resolution", sortable: false, searchable: false, visible: true },
                        { name: "CreatedDate", data: "createdDate", title: "Created Date", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center>';
                                if (uid == row.uid) {
                                    actionButtons += $("<a/>", {
                                        id: "addEdit",
                                        class: "",
                                        href: domain + "Escalation/AddEditConclusion/?id=" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-conclusion",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-pencil",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp;",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
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

        if (window.File && window.FileList && window.FileReader) {
            $("#files").on("change", function (e) {
                var files = e.target.files,
                    filesLength = files.length;
                for (var i = 0; i < filesLength; i++) {
                    var f = files[i]
                    var fileReader = new FileReader();
                    fileReader.onload = (function (e) {
                        var file = e.target;
                        $("<span class=\"pip\">" +
                            "<img class=\"imageThumb\" src=\"" + e.target.result + "\" title=\"" + file.name + "\"/>" +
                            "<br/><span class=\"remove\"><i class=\"fa fa-trash\"></i>&nbsp;Remove</span>" +
                            "</span>").insertAfter("#files");
                        $(".remove").click(function () {
                            $(this).parent(".pip").remove();
                        });

                        // Old code here
                        /*$("<img></img>", {
                          class: "imageThumb",
                          src: e.target.result,
                          title: file.name + " | Click to remove"
                        }).insertAfter("#files").click(function(){$(this).remove();});*/

                    });
                    fileReader.readAsDataURL(f);
                }
            });
        } else {
            alert("Your browser doesn't support to File API")
        }



        $this.init = function () {
            InitializeForm();
            LoadEscalationGrid();
        }
    }
    $(function () {
        var self = new Escalation();
        self.init();
    });
}(jQuery));