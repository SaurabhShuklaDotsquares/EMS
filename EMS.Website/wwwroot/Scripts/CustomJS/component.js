(function ($) {

    function Component() {
        $this = this;
        var isSuperAdmin = $('#hdnIsSuperAdmin').val() == "True" ? true : false;
        var uid = $('#CreatedByUid').val();
        $('#btnSearch').click(function () {
            ComponentGrid();
        })
        $('#component-Image-gallery').on('click', '#myPager a', function (e) {
            e.preventDefault();
            $.ajax({
                type: "GET",
                url: this.href + '&type=' + $('#hdnInputType').val(),
                chase: false,
                success: function (result) {
                    $('#component-Image-gallery').html(result);
                    $('.colorboxGallery').colorbox({
                        width: "60%", height: "80%", scrolling: false, rel: 'component-Image-gallery', current: "{current} of {total}", title: function () {
                            var htmlfileurl = document.getElementById('downloadhtml');
                            var psdImageUrl = document.getElementById('PsdImageUrl');
                            var downloadUrl = '';
                            if ($(this).attr('has-image')) {
                                downloadUrl = '<a download href="' + htmlfileurl + '"> &nbsp;&nbsp;<i title="Download HTML" class="fa fa-download"></i>&nbsp;&nbsp; <strong>Download HTML</strong></a>' + '';
                            }

                            if ($(this).attr('has-html')) {
                                downloadUrl += '<a download href="' + psdImageUrl + '"> &nbsp;&nbsp; <i title="Download PSD" class="fa fa-download"></i>&nbsp;&nbsp; <strong>Download PSD </strong> </a> ' + '';
                            }
                            return downloadUrl;

                        }
                    });
                    var maxHeight = Math.max.apply(null, $('.wireframe-item').map(function () {
                        return $(this).innerHeight() + 10;
                    }).get());
                    $('.wireframe-item').css('height', maxHeight);
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');

                }
            })
        })

        $('#ComponentCategoryId').change(function () {
            if ($('#type').val() == '1') {
                var txtSearch = $("#txtSearch").val();
                var type = $("#type").val();
                ChangeDesigndata($(this).val(), txtSearch, type);
            }
            if ($('#type').val() == '2') {
                var txtSearch = $("#txtSearch").val();
                var type = $("#type").val();
                ChangeDesigndata($(this).val(), txtSearch, type);
            }
            else {
                ComponentGrid();
            }

        })

        function ChangeDesigndata(categoryId, txtSearch, type) {
            if (categoryId == "") {
                categoryId = 0;
            }
            if (type == "") {
                type = 1;
            }

            $('#hdnInputType').val(type);


            $('#div-grid-component').css("display", "none");
            $('#component-Image-gallery').css("display", "");
            $.ajax({
                type: "GET",
                url: domain + "Component/GetComponentDesignImages/" + categoryId + '?txtSearch=' + txtSearch + '&type=' + $('#hdnInputType').val(),
                success: function (result) {

                    $('#component-Image-gallery').html(result);
                    $('.colorboxGallery').colorbox({
                        width: "60%", height: "80%", scrolling: false, rel: 'component-Image-gallery', current: "{current} of {total}", title: function () {
                            var htmlfileurl = document.getElementById('downloadhtml');
                            var psdImageUrl = document.getElementById('PsdImageUrl');
                            var downloadUrl = '';
                            if ($(this).attr('has-image')) {
                                downloadUrl = '<a download href="' + htmlfileurl + '"> &nbsp;&nbsp;<i title="Download HTML" class="fa fa-download"></i>&nbsp;&nbsp; <strong>Download HTML</strong></a>' + '';
                            }

                            if ($(this).attr('has-html')) {
                                downloadUrl += '<a download href="' + psdImageUrl + '"> &nbsp;&nbsp; <i title="Download PSD" class="fa fa-download"></i>&nbsp;&nbsp; <strong>Download PSD </strong> </a> ' + '';
                            }
                            return downloadUrl;

                        }
                    });
                    var maxHeight = Math.max.apply(null, $('.wireframe-item').map(function () {
                        return $(this).innerHeight() + 10;
                    }).get());
                    $('.wireframe-item').css('height', maxHeight);
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                }
            })
        }

        $('#type').change(function () {
            if ($(this).val() == "1") {
                ChangeDesigndata($('#ComponentCategoryId').val(), "", $(this).val());

            }
            if ($(this).val() == "2") {
                ChangeDesigndata($('#ComponentCategoryId').val(), "", $(this).val());

            }
            else {
                $('#div-grid-component').css("display", "");
                $('#component-Image-gallery').css("display", "none");
                document.getElementById("btnSearch").disabled = false;
                ComponentGrid();
            }
        })

        //Method to Load Component Grid
        function ComponentGrid() {
            var data1 = { txtSearch: $('#txtSearch').val(), ComponentCategoryId: $('#ComponentCategoryId').val(), type: $('#type').val() }
            if (data1.type == "1") {
                categoryId = data1.ComponentCategoryId;

                ChangeDesigndata(categoryId, data1.txtSearch, data1.type);
            }
            if (data1.type == "2") {
                categoryId = data1.ComponentCategoryId;
                ChangeDesigndata(categoryId, data1.txtSearch);
            }
            else {
                var ComponentGrid = new Global.GridHelper('#grid-component', {
                    serverSide: true,
                    destroy: true,
                    "bAutoWidth": false,
                    "pageLength": 50,
                    "bFilter": false,
                    ajax:
                        {
                            url: domain + "Component/Index",
                            type: "POST",
                            data: data1,
                        },
                    order: [[4, "desc"]],
                    "columnDefs": [
                            { "width": "0%", "targets": 0 },
                            { "width": "3%", "targets": 1 },
                            { "width": "10%", "targets": 2 },
                            { "width": "10%", "targets": 3 },
                            { "width": "15%", "targets": 4 },
                            { "width": "15%", "targets": 5 },
                            { "width": "15%", "targets": 6 },
                            { "width": "5%", "targets": 7 },
                            { "width": "5%", "targets": 8 }


                    ],
                    columns:
                        [
                            { name: "id", data: "id", title: "id", sortable: false, searchable: false, visible: false },

                            { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },

                            {
                                name: "title", data: null, title: "title", sortable: true, searchable: false, visible: true,

                                render: function (data, type, dataRow, meta) {


                                    var mockups = "";
                                    if (dataRow.imageName != null && dataRow.imageName != "") {
                                        mockups += '<span>' + data.title + '</span><a class="colorbox" title="component image"  href="Upload/ComponentImage/' + data.imageName + '"></a></br>';
                                    }
                                    else {

                                    }
                                    if (dataRow.mockupDocument != null) {
                                        mockups += '<span>' + data.title + '</span><a  style="color:#101ee5;text-decoration:underline;" href="Upload/ComponentImage/' + data.imageName + '" target="_blank">' + data.imageName + '</a></br>';
                                    }
                                    return mockups;
                                },
                            },
                            { name: "category", data: "category", title: "category", sortable: true, searchable: false, visible: true, },
                            { name: "description", data: "description", title: "description", sortable: true, searchable: false, visible: true, },
                            {
                                name: "dataUrl", data: "dataUrl", title: "Html / JS URL", sortable: true, searchable: false, visible: true,
                                render: function (data, type, dataRow, meta) {
                                    var Link = "";
                                    if (dataRow.dataUrl != null) {
                                        Link += '<a  style="color:#101ee5;text-decoration:underline;" href="' + dataRow.dataUrl + '" target="_blank">' + dataRow.dataUrl + '</a></br>';
                                    }
                                    return Link;
                                }
                            },
                            {
                                name: "imageName", data: "imageName", title: "Html / Design / Js Document", sortable: true, searchable: false, visible: true,
                                render: function (data, type, dataRow, meta) {
                                    var Link = "";

                                    if (dataRow.dataUrl != null) {
                                        Link += '<a  style="color:#101ee5;text-decoration:underline;" href="Upload/ComponentImage/' + dataRow.imageName + '" download>' + dataRow.imageName + '</a></br>';
                                    }
                                    else {
                                        Link += '<a  style="color:#101ee5;text-decoration:underline;" href="Upload/ComponentImage/' + data + '" download >' + data + '</a></br>';
                                    }
                                    return Link;
                                }
                            },

                             {
                                 name: "created", data: null, title: "Created By", sortable: false, searchable: false, visible: true,
                                 render: function (data, type, dataRow, meta) {
                                     return dataRow.created + "</br> <b>uploaded By</b>: " + dataRow.createdByUid;
                                 }
                             },

                            {
                                name: "action", data: null, title: "action", sortable: false, searchable: false, visible: true,
                                render: function (data, type, dataRow, meta) {
                                    if (isSuperAdmin) {
                                        return '<a data-toggle="modal" data-target="#modal-AddEditComponent" class="fa fa-edit" href="' + domain + 'component/AddEditComponent/' + dataRow.id + '"></a> <a   title="delete component" class="fa fa-trash deletecomponent" style="font-size: 15px"   href="' + domain + 'component/DeleteComponent/' + dataRow.id + '"></a>';
                                    }
                                    else {
                                        return '<a data-toggle="modal" data-target="#modal-AddEditComponent" class="fa fa-edit" href="' + domain + 'component/AddEditComponent/' + dataRow.id + '"></a>';
                                    }

                                }
                            }


                        ],

                    "fnDrawCallback": function (oSettings) {
                        $(".deletecomponent").click(function () {
                            return confirm('Are you sure you want to delete ?');
                        });
                        if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {

                            $('.dataTables_paginate').hide();
                        }
                        else {
                            $('.dataTables_paginate').show();
                        }

                        $('.pagination .active a').css('background-color', '#e99701');
                        $('.pagination .active a').css('border-color', '#e99701');
                        $('.colorbox').colorbox({ width: "60%", height: "80%" });
                    }
                })
            }
            return ComponentGrid;
        }

        //Method to delete document using ajax
        function DeleteDocument(item) {
            var componentId = item;
            if (confirm("Are you sure to delete this component ?")) {
                var data1 = { componentId: $('#Id').val(), documentId: componentId.attr('id') }
                $.ajax({
                    type: "POST",
                    url: domain + "Component/DeleteDocument/",
                    data: data1,
                    success: function (result) {
                        if (result.isSuccess) {
                            var messageBox = result.updateTargetId;
                            var message = result.message;
                            componentId.closest('.divUploadedFile').html('');
                            $('#' + messageBox).attr('class', 'alert alert-success');
                            $('#' + messageBox).html(message);
                            $('#' + messageBox).show()
                        }
                        else {
                            var messageBox = result.updateTargetId;
                            var message = result.message;
                            $('#' + messageBox).attr('class', 'alert alert-danger');
                            $('#' + messageBox).html(message);
                            $('#' + messageBox).show()
                        }
                    }

                })
            }
            return false;
        }

        function InitializeModal() {
            $('#modal-AddEditComponent').on('loaded.bs.modal', function (e) {
                new Global.FormHelperWithFiles($('#componentForm'),
                    { updateTargetId: "error-ModalMessage" },

                    function onSuccess(result) {
                        $("#ModelSubmit").css("display", "block").find('span').html(result.message);                                                      
                        $("#Title").val('');
                        $("#Tags").val('');
                        $("#Description").val('');
                        $(".ComponentCategoryIdC").val("");
                        $('#DesignImageName').val('');
                        $('#PsdImageName').val('');
                        $('#ImageName').val('');
                        ComponentGrid();
                        $(this).removeData('bs.modal');
                    });
                $("#divDesignImages").show();

                $('#lnkDelMockupDoc').click(function () {
                    var lnkDel = $(this);
                    DeleteDocument(lnkDel);
                })
                $(".radiobtn").click(function () {
                    var values = $(this).val();
                    if (values == "1") {
                        $("#DataUrlId").hide();
                        $("#divDesignImages").show();
                        $("#divpsdimage").show();
                    }
                    if (values == "2") {
                        $("#DataUrlId").hide();
                        $("#divDesignImages").show();
                        $("#divpsdimage").show();
                    }
                    if (values == "3") {
                        $("#DataUrlJSlabel").show();
                        $("#DataUrlId").show();
                        $("#DataUrlhtmllabel").hide();
                        $("#divDesignImages").hide();
                        $("#divpsdimage").hide();
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
            })
        }

        $this.init = function () {
            InitializeModal();
            ComponentGrid();
        }
    }
    $(function () {
        var self = new Component();
        self.init();

    })

})(jQuery);