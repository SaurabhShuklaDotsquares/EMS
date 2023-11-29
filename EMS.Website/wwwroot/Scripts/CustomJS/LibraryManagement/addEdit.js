(function () {
    var layoutList = [];
    var oldLayoutList = [];
    var designTypeCount = 0;
    var currentUserRoleIsDesigner = $("#currentUserRoleIsDesigner").val();

    $("#LibraryTypeId option[value='6']").remove();

    if ($("#layoutTypes").val().trim().length > 0) {
        layoutList = $("#layoutTypes").val().split(',');
        oldLayoutList = layoutList;
    }

    $(document).on("change", ".design_layout", function () {
        if ($(this).val().length > 0) {
            layoutList.push($(this).val());
        } else {
            var last = layoutList[layoutList.length - 1];
            layoutList.pop(layoutList.indexOf(last));
        }
    });

    //$(document).on("keyup", "#Title", function () {
    //    if ($(this).val() != "") {
    //        $.ajax({
    //            url: "LibraryManagement/IsLibraryExist",
    //            data: { "text": $(this).val() },
    //            type: 'Get',
    //            success: function (result) {
    //                if (result == "1") {
    //                    //$("#Title").removeClass("valid");
    //                    //$("#Title").addClass("invalid error");
    //                    $("#UniqueTitle-error").show();
    //                } else {
    //                    $("#UniqueTitle-error").hide();
    //                }
    //            }
    //        });
    //    }
    //});

    function readURL(input) {
        var temFileCount = parseInt($(".tempFile").length);
        var imgNo = parseInt($("#image_no").val()) - temFileCount;
        libraryType = $('.library-type:checked').val();
        var maxAllowedfiles = 10;
        var message = "10 max file upload limit.";
        if (input.files && input.files.length > 0) {
            $(".tempFile").remove();
            var newNo = imgNo + input.files.length;
            var j = 0;
            if (imgNo < maxAllowedfiles) {

                $("#upload-error").hide();
                for (var i = imgNo; i < newNo; i++) {
                    if (input.files && input.files[j]) {
                        var selectedFileCount = input.files.length;
                        var reader = new FileReader();
                        reader.onload = (function (j) {
                            var fileName = j;
                            return function (e) {
                                var imageCount = parseInt(fileName.index) + 1;
                                if (imageCount <= maxAllowedfiles) {
                                    var imageExtensions = ["jpg", "png", "jpeg", "gif"];
                                    var OtherExtensions = ["pdf", "docx", "doc", "ppt", "xlsx", "xls", "doc", "csv", "psd", "html", "rar", "zip","txt"];
                                    var extension = fileName.name.split('.').pop().toLowerCase();
                                    if (extension == 'exe') {
                                        alert(".exe type not allowed")
                                        return false
                                    }
                                    if ($.inArray(extension, imageExtensions) > -1) {
                                        $("#previewImage").append("<div class='col-md-2 tempFile'><img id='img" + fileName.index + "' class='preview_image' src='" + e.target.result + "'><br><label><input type='radio' name='CoverImage' value='" + fileName.index + "' /> Is Banner?</label></div>");
                                       
                                    } else {
                                        if ($.inArray(extension, OtherExtensions) > -1) {
                                            $("#previewImage").append("<div class='col-md-2 tempFile'><img id='img" + fileName.index + "' class='preview_doc' src='/images/Filetypes/" + extension + ".png'></div>");
                                           
                                        }
                                        else {
                                            $("#previewImage").append("<div class='col-md-2 tempFile'><img id='img" + fileName.index + "' class='preview_doc' src='/images/document.svg'></div>");
                                           
                                        }
                                    }
                                    $("#image_no").val(selectedFileCount);
                                } else {
                                    $("#upload-error").text(message);
                                    $("#upload-error").show();
                                }
                            };
                        })(input.files[j]);
                        input.files[j].index = i;
                        reader.readAsDataURL(input.files[j]);
                        j++;
                    }
                }
            } else {
                $("#upload-error").text(message);
                $("#upload-error").show();
            }
        }
    }
    function readComponentURL(input) {
        var temFileCount = parseInt($(".tempFile").length);
        var imgNo = parseInt($("#image_no").val()) - temFileCount;
        libraryType = $('.library-type:checked').val();
        var maxAllowedfiles = 10;
        var message = "10 max file upload limit.";
        if (input.files && input.files.length > 0) {
            $(".tempFile").remove();
            var newNo = imgNo + input.files.length;
            var j = 0;
            if (imgNo < maxAllowedfiles) {

                $("#upload-error").hide();
                for (var i = imgNo; i < newNo; i++) {
                    if (input.files && input.files[j]) {
                        var selectedFileCount = input.files.length;
                        var reader = new FileReader();
                        reader.onload = (function (j) {
                            var fileName = j;
                            return function (e) {
                                var imageCount = parseInt(fileName.index) + 1;
                                if (imageCount <= maxAllowedfiles) {
                                    //var imageExtensions = ["jpg", "png", "jpeg", "gif"];
                                    var OtherExtensions = ["pdf", "docx", "doc", "xlsx", "xls", "doc", "txt"];
                                    var extension = fileName.name.split('.').pop().toLowerCase();
                                    if (extension == 'exe') {
                                        alert(".exe type not allowed")
                                        return false
                                    }
                                    if ($.inArray(extension, imageExtensions) > -1) {
                                        
                                        $("#previewDescImage").append("<div class='col-md-2 tempFile'><img id='img" + fileName.index + "' class='preview_desc_doc' src='" + e.target.result + "'><br><label><input type='radio' name='CoverImage' value='" + fileName.index + "' /> Is Banner?</label></div>");
                                    } else {
                                        if ($.inArray(extension, OtherExtensions) > -1) {
                                           
                                            $("#previewDescImage").append("<div class='col-md-2 tempFile'><img id='img" + fileName.index + "' class='preview_desc_doc' src='/images/Filetypes/" + extension + ".png'></div>");
                                        }
                                        else {
                                            
                                            $("#previewDescImage").append("<div class='col-md-2 tempFile'><img id='img" + fileName.index + "' class='preview_desc_doc' src='/images/document.svg'></div>");
                                        }
                                    }
                                    $("#image_no").val(selectedFileCount);
                                } else {
                                    $("#upload-error").text(message);
                                    $("#upload-error").show();
                                }
                            };
                        })(input.files[j]);
                        input.files[j].index = i;
                        reader.readAsDataURL(input.files[j]);
                        j++;
                    }
                }
            } else {
                $("#upload-error").text(message);
                $("#upload-error").show();
            }
        }
    }

    $("input:checkbox[name=TechnologyParent]").change(function () {
        var technologyParentId = $(this).val();
        var techCount = $("#techCount").val();
        var numberChecked = $('input:checkbox[name=TechnologyParent]:checked').length;
        if (numberChecked == 0)
            $("#techVisibility").hide();
        if ($(this).is(":checked")) {
            $("#techVisibility").show();
            $.ajax({
                url: domain + 'LibraryManagement/GetTechnologyByParent',
                type: 'POST',
                data: { technologyParentId: technologyParentId },
                success: function (data) {
                    //console.log(data);
                    //console.log(JSON.parse(data));
                    var result = JSON.parse(data);
                    var techList = '';
                    $.each(result.data, function (index, item) {
                        techList += `<div class="col-md-3 parentTechId_${item.TechnologyParentId}"> <div class="chk pull-left"> 
                                        <input type="checkbox" name="Technology" id="chkTechnology_${item.TechId}" data-id="${item.TechId}" value="${item.TechId}">
                                        <label for="chkTechnology_${item.TechId}">&nbsp;${item.Title}</label>
                                        </div>
                                    </div>`;
                        techCount++;
                    });
                    $("#techIds").append(techList);
                    if (numberChecked > 0)
                        $("input:checkbox[name=Technology]:first").prop("required", true);
                }
            });
        } else {
            $(".parentTechId_" + technologyParentId).remove();
        }
    });

    $("#IntegrationHours").keydown(function (event) {
        if (event.shiftKey == true) {
            event.preventDefault();
        }

        if ((event.keyCode >= 48 && event.keyCode <= 57) ||
            (event.keyCode >= 96 && event.keyCode <= 105) ||
            event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
            event.keyCode == 39 || event.keyCode == 44 || event.keyCode == 46 || event.keyCode == 190) {

        } else {
            event.preventDefault();
        }

        //if ($(this).val().indexOf('.') !== -1 && event.keyCode == 190)
        //    event.preventDefault();
        //if a decimal has been added, disable the "."-button

    });

    $('#EstimatedHours').change(function () {
        if ($(this).val() == "" || $(this).val() == null || $(this).val().length==0) {
            $('#EstimatedMsg').show();
        }
        else {
            $('#EstimatedMsg').hide();
        }
    });
    $('#ReDevelopmentHours').change(function () {
        if ($(this).val() == "" || $(this).val() == null || $(this).val().length==0) {
            $('#ReDevelopmentMsg').show();
        }
        else {
            $('#ReDevelopmentMsg').hide();
        }
    });

    $("#LibraryFiles").change(function () {
        readURL(this);
    });
    $("#ComponentFiles").change(function () {
        readComponentURL(this);
    });

    $(".IsLive").click(function () {
        var current = $(this).val();
        if (current == "true") {
            $("#liveUrl").prop("required", true);
            //$("#cLiveUrl").fadeIn();
        }
        else {
            $("#liveUrl").removeAttr("required");
            // $("#cLiveUrl").fadeOut();
            $("#liveUrl").val("");
        }
    });

    $(".library-type").click(function () {
        var current = $(this).val();
        console.log(current);
        $("#selectLayoutList-error").hide();
        $("#selectComponentList-error").hide();
        $("#selectTemplateList-error").hide();
        //$(".new-design-type").fadeOut();
        //$(".industryRow").fadeOut();
        $(".liveRow").hide();
        $(".ndaRow").hide();
        $(".teamRow").hide();

        $("#txt_sales_kit_type").hide();
        $("#txt_csv_type").hide();
        $("#SalesKitId").val("");
        $("#CvsId").val("");

        $("input:radio[name=IsNDA]:first").removeAttr("required");
        $("input:radio[name=IsLive]:first").removeAttr("required");
        $("input:radio[name=Team]:first").removeAttr("required");
        //$(".new-design-type").remove();
        if (current == 2) {
            $("#FileUpload").fadeOut();
            var oldDesigns = $("#designTypeCount").val();
            if (oldDesigns == 0) {
                $(".new-design-type").fadeIn();
            }
            $("#liveURl").fadeIn();
            //$("#descriptionSpan").hide();
            $("#techParentSpan").hide();
            $(".industryRow").fadeIn();
            $("input:checkbox[name=Industry]").prop("checked", false);
            $("input:checkbox[name=TechnologyParent]:first").prop("required", false);
            //$("#Description").prop("required", false);
            $("#componentType").fadeOut();
            $("#templateType").fadeOut();
            $("#designType").fadeIn();
            $("#txt_crmuser").fadeOut();
            $("#div_liveURL").fadeOut();
            $("#divBATL").fadeIn();
            $('.ba-row').hide();
            $('.tl-row').hide();
            $("#layoutType").fadeOut();
            $("input:radio[name=DesignTypeId]:first").prop("required", true);
            $("input:checkbox[name=LayoutType]:first").prop("required", true);
            //$("input:radio[name=IsNDA]:first").prop("required", false);
            //$("input:radio[name=IsLive]:first").prop("required", false);
            $("#selectTemplateList").removeAttr("required");
            $("#selectTemplateList option:selected").remove();
            $(".fs-option").removeClass("selected");
            $(".fs-label").text('Select some options');
            $(".only-design-type").fadeIn();
            $("input:radio[name=IsNDA]").removeAttr("checked");
            $("input:radio[name=Team]").removeAttr("checked");
            $("input:radio[name=IsLive]").removeAttr("checked");
            $("label[for='Users']").text("Author");
        }
        else if (current == 1 || current == 3) {
            $("#componentType").hide();
            $("#FileUpload").fadeIn();
            $("#liveURl").fadeIn();
            $(".liveRow").show();
            $(".ndaRow").show();
            $(".teamRow").show();
            $(".industryRow").show();
            $("input:radio[name=IsNDA]:first").prop("required", true);
            $("input:radio[name=IsLive]:first").prop("required", true);
            $("input:radio[name=Team]:first").prop("required", true);
            $("#techParentSpan").show();
            $("input:checkbox[name=TechnologyParent]:first").prop("required", true);
            $(".only-design-type").fadeOut();
            $("#componentType").fadeOut();
            $("#templateType").fadeOut();
            $("#designType").fadeOut();
            $("#txt_crmuser").fadeIn();
            $("#div_liveURL").fadeIn();
            $("#divBATL").fadeIn();
            $('.ba-row').show();
            $('.tl-row').show();
            $("#layoutType").fadeOut();
            $("#selectComponentList").removeAttr("required");
            $("#selectTemplateList option:selected").remove();
            $(".fs-option").removeClass("selected");
            $(".fs-label").text('Select some options');
            $("input:radio[name=DesignTypeId]").removeAttr("checked");
            $("input:radio[name=DesignTypeId]:first").removeAttr("required");
            $("#selectLayoutList").removeAttr("required");
            $("#selectLayoutList option:selected").remove();
            $("label[for='Users']").text("Developer");
        }
        else if (current == 4) {
            $("#techParentSpan").show();
            $("#liveURl").fadeIn();
            //$("#descriptionSpan").show();
            $("input:checkbox[name=TechnologyParent]:first").prop("required", true);
            //$("#Description").prop("required", true);
            $("#FileUpload").fadeIn();
            $("input:checkbox[name=Industry]").prop("checked", false);
            $("#componentType").fadeIn();
            $("#templateType").fadeOut();
            $("#designType").fadeOut();
            $("#txt_crmuser").fadeOut();
            $("#div_liveURL").fadeOut();
            $("#divBATL").fadeIn();
            $('.ba-row').hide();
            $('.tl-row').show();
            $("#layoutType").fadeOut();
            $(".industryRow").fadeIn();
            $("#selectComponentList").prop("required", true);
            $("#selectTemplateList").removeAttr("required");
            $("input:radio[name=DesignTypeId]").removeAttr("checked");
            $("input:radio[name=DesignTypeId]:first").removeAttr("required");
            $("#selectLayoutList").removeAttr("required");
            $("#selectLayoutList option:selected").remove();
            $(".fs-option").removeClass("selected");
            $(".fs-label").text('Select some options');
            $(".only-design-type").fadeOut();

            $("input:radio[name=IsNDA]:first").removeAttr("required");
            $("input:radio[name=IsLive]:first").removeAttr("required");
            $("input:radio[name=IsTeam]:first").removeAttr("required");
            $("input:radio[name=IsNDA]").removeAttr("checked");
            $("input:radio[name=Team]").removeAttr("checked");
            $("input:radio[name=IsLive]").removeAttr("checked");
            $("label[for='Users']").text("Author");
            //$("#cLiveUrl").fadeOut();
        }
        else if (current == 6) {
            $("#techParentSpan").show();
            $("#liveURl").fadeIn();
            //$("#descriptionSpan").show();
            $("input:checkbox[name=TechnologyParent]:first").prop("required", true);
            //$("#Description").prop("required", true);
            $("#FileUpload").fadeIn();
            $(".industryRow").show();
            $("#componentType").fadeOut();
            $("#templateType").fadeIn();
            $("#designType").fadeOut();
            $("#txt_crmuser").fadeIn();
            $("#div_liveURL").fadeIn();
            $("#divBATL").fadeIn();
            $('.ba-row').show();
            $('.tl-row').show();
            $("#layoutType").fadeOut();
            $("#selectComponentList").removeAttr("required");
            $("#selectTemplateList").prop("required", true);
            $("input:radio[name=DesignTypeId]").removeAttr("checked");
            $("input:radio[name=DesignTypeId]:first").removeAttr("required");
            $("#selectLayoutList").removeAttr("required");
            $("#selectLayoutList option:selected").remove();
            $(".fs-option").removeClass("selected");
            $(".fs-label").text('Select some options');
            $(".only-design-type").fadeOut();
            $("input:radio[name=IsNDA]").removeAttr("checked");
            $("input:radio[name=IsLive]").removeAttr("checked");
            $("label[for='Users']").text("Author");
        }
        else if (current == 5) {
            $(".industryRow").show();
            $("#liveURl").fadeIn();
            $("#techParentSpan").hide();
            //$("#descriptionSpan").show();
            //$("#Description").prop("required", true);
            //$("input:checkbox[name=TechnologyParent]:first").prop("required", true);
            $("#FileUpload").fadeIn();
            $(".only-design-type").fadeOut();
            $("#componentType").fadeOut();
            $("#templateType").fadeOut();
            $("#designType").fadeOut();
            $("#txt_crmuser").fadeOut();
            $("#div_liveURL").fadeOut();
            $("#divBATL").fadeIn();
            $('.ba-row').show();
            $('.tl-row').show();
            $("#layoutType").fadeOut();
            $("#selectComponentList").removeAttr("required");
            $("#selectTemplateList option:selected").remove();
            $(".fs-option").removeClass("selected");
            $(".fs-label").text('Select some options');
            $("input:radio[name=DesignTypeId]").removeAttr("checked");
            $("input:radio[name=DesignTypeId]:first").removeAttr("required");
            $("#selectLayoutList").removeAttr("required");
            $("#selectLayoutList option:selected").remove();
            $("input:radio[name=IsNDA]").removeAttr("checked");
            $("input:radio[name=IsLive]").removeAttr("checked");
            $("label[for='Users']").text("Author");
        }
        else if (current == 7) {
            $("#txt_sales_kit_type").fadeIn();
            $("#divIndustry").fadeIn();
            $("#componentType").fadeOut();
            $("#techParentSpan").fadeIn();
            $("#liveURl").fadeOut();
            $("#divBATL").fadeOut();
            $("#txt_csv_type").fadeOut();
            $("#txt_crmuser").fadeOut();
            $("#designType").fadeOut();  
            $("input:radio[name=DesignTypeId]").removeAttr("checked");
            $("input:radio[name=DesignTypeId]:first").removeAttr("required");
        }
        else if (current == 8) {
            $("#txt_sales_kit_type").fadeOut();
            $("#liveURl").fadeOut();
            $("#componentType").fadeOut();
            /*$("#divIndustry").fadeOut();*/
            $("#divBATL").fadeOut();
            $("#txt_csv_type").fadeIn();
            $("#techParentSpan").fadeIn();
            $("#txt_crmuser").fadeOut();          
            $("#designType").fadeOut(); 
            $("input:radio[name=DesignTypeId]").removeAttr("checked");
            $("input:radio[name=DesignTypeId]:first").removeAttr("required");
        }
    });

    $("#modal-add-component").on("hidden.bs.modal", function (event) {
        var componentCount = $("#componentCount").val();
        var id = $("#modal-add-component").data("newComponentTypeId");
        var name = $("#modal-add-component").data("newComponentTypeName");
        $("#modal-add-component").removeData("newComponentTypeName");
        $("#modal-add-component").removeData("newComponentTypeId");
        if (id != 'null' && id != 0 && typeof id !== 'undefined') {
            var o = new Option(id, name);
            $(o).html(name);
            $("#selectComponentList").append($("<option></option>")
                .attr("value", id)
                .text(name));
            $("#componentList").find(".fs-options").append(`<div class="fs-option g0" data-value="${id}" data-index="${componentCount}"><span class="fs-checkbox"><i></i></span><div class="fs-option-label">${name}</div></div>`);
            componentCount++;
            $("#componentCount").val(componentCount);
        }
    });

    $("#modal-add-template").on("hidden.bs.modal", function (event) {
        var templateCount = $("#templateCount").val();
        var id = $("#modal-add-template").data("newTemplateTypeId");
        var name = $("#modal-add-template").data("newTemplateTypeName");
        $("#modal-add-template").removeData("newTemplateTypeName");
        $("#modal-add-template").removeData("newTemplateTypeId");
        if (id != 'null' && id != 0 && typeof id !== 'undefined') {
            var o = new Option(id, name);
            $(o).html(name);
            $("#selectTemplateList").append($("<option></option>")
                .attr("value", id)
                .text(name));
            $("#templateList").find(".fs-options").append(`<div class="fs-option g0" data-value="${id}" data-index="${templateCount}"><span class="fs-checkbox"><i></i></span><div class="fs-option-label">${name}</div></div>`);
            templateCount++;
            $("#templateCount").val(templateCount);
        }
    });

    $("#modal-add-layout").on("hidden.bs.modal", function (event) {
        var layoutCount = $("#layoutCount").val();
        var id = $("#modal-add-layout").data("newLayoutTypeId");
        var name = $("#modal-add-layout").data("newLayoutTypeName");
        $("#modal-add-layout").removeData("newLayoutTypeName");
        $("#modal-add-layout").removeData("newLayoutTypeId");
        if (id != 'null' && id != 0 && typeof id !== 'undefined') {
            var o = new Option(id, name);
            $(o).html(name);
            //$("#selectLayoutList").append($("<option></option>")
            //    .attr("value", id)
            //    .text(name));
            //$("#layoutList").find(".fs-options").append(`<div class="fs-option g0" data-value="${id}" data-index="${layoutCount}"><span class="fs-checkbox"><i></i></span><div class="fs-option-label">${name}</div></div>`);
            $(".design_layout").append($("<option></option>")
                .attr("value", id)
                .text(name));
            layoutCount++;
            $("#layoutCount").val(layoutCount);
        }
    });
    $("#modal-action-saleskit").on('shown.bs.modal', function (e) {
        if ($("#IsChild").is(":checked")) {
            $("#parentSalesKit").removeClass("hide");
        }
        if ($("#IsChild").is(":unchecked")) {
            $("#parentSalesKit").addClass("hide");
            $("#ParentId").val("");
        }
        $("#IsChild").on("change", function () {
            if ($("#IsChild").is(":checked")) {
                $("#parentSalesKit").removeClass("hide");
            }
            else {
                $("#parentSalesKit").addClass("hide");
                $("#ParentId").val("");
            }
        });
    }).on("hidden.bs.modal", function (e) {
        $(this).removeData("bs.modal");

    });
    $("#modal-action-cvs").on("hidden.bs.modal", function (e) {
        $(this).removeData("bs.modal");

    });
    $(document).delegate("#btn-submit", "click", function () {
        var salesKitForm = new Global.FormHelper($("#frm-saleskit"));
        var cvsForm = new Global.FormHelper($("#frm-cvs"));
    });
    $('#CRMUserId').keypress(function (event) {
        if (event.which != 8 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault();
        }
    });

    $(".addDesignType").click(function () {
        var myVal = $('.design_layout:last').val();
        if (!$(".new-design-type").is(":visible")) {
            $(".new-design-type").fadeIn();
        } else {
            if (myVal != null && myVal.length > 0) {
                //$(".design_layout").attr("disabled", "disabled");
                var clone = $(".design-type-0").clone();

                //*************************Comenting below code as it's giving error in uploading more then one file :: Start********************************
                //******************* Also it's looks like it has no use**************************************
                //var a = clone.find("select").children("option:selected").val();
                //$(".design-type-clones").find('select').attr('disabled', 'disabled');
                //$(".design-type-clones").find('select').find("option[value='" + a + "']").attr('disabled', 'disabled');
                //*************************Comenting below code as it's giving error in uploading more then one file :: End********************************

                //for (var i = 0; i < layoutList.length; i++) {
                //    clone.find("option[value='" + layoutList[i] + "']").attr('disabled', 'disabled');
                //}

                for (var i = 0; i < layoutList.length; i++) {
                    if (clone.find("option[value='" + layoutList[i] + "']").text() != "Others") {
                        clone.find("option[value='" + layoutList[i] + "']").attr('disabled', 'disabled');
                    }

                }
                clone.find(".design_layout").removeAttr("disabled");
                designTypeCount = designTypeCount + 1;

                clone.find(".DesignFiles_DesignUnitType1").attr({
                    id: "DesignFiles_" + designTypeCount + "__DesignUnitType",
                    name: "DesignFiles[" + designTypeCount + "].DesignUnitType",
                    class: "DesignFiles_" + designTypeCount + "__DesignUnitType1"
                });
                clone.find(".DesignFiles_DesignUnitType2").attr({
                    id: "DesignFiles_" + designTypeCount + "__DesignUnitType",
                    name: "DesignFiles[" + designTypeCount + "].DesignUnitType",
                    class: "DesignFiles_" + designTypeCount + "__DesignUnitType2"
                });


                clone.appendTo(".design-type-clones");

                clone.attr("class", "design-type-" + designTypeCount + " new-design-type");
                clone.find("select[id=DesignFiles_0__DesignLayoutType]").attr("name", "DesignFiles[" + designTypeCount + "].DesignLayoutType");
                clone.find("select[id=DesignFiles_0__DesignLayoutType]").attr("id", "DesignFiles_" + designTypeCount + "__DesignLayoutType");



                clone.find("input[id=DesignFiles_0__Image_File]").attr({
                    id: "DesignFiles_" + designTypeCount + "__Image_File",
                    name: "DesignFiles[" + designTypeCount + "].Image.File",
                });
                clone.find("input[id=DesignFiles_0__PSD_File]").attr({
                    id: "DesignFiles_" + designTypeCount + "__PSD_File",
                    name: "DesignFiles[" + designTypeCount + "].PSD.File",
                });
                clone.find(".remove-design-type").attr("data-id", designTypeCount);
                clone.find(".remove-design-type").css("display", "block");
                $("#DesignFiles_" + designTypeCount + "__Image_File").val("");
                $("#DesignFiles_" + designTypeCount + "__PSD_File").val("");

                $("#DesignFiles_" + designTypeCount + "__DesignLayoutType").val("");

                var sIndex = clone.find('select').val();
                clone.find('select').append(clone.find("select option")
                    .remove().sort(function (a, b) {
                        var at = $(a).text(),
                            bt = $(b).text();
                        return (at > bt) ? 1 : ((at < bt) ? -1 : 0);
                    }));
                clone.find('select').val(sIndex);
                clone.find("input[class=DesignFiles_DesignUnitType1]").prop("checked", true);
            } else {
                $("form").submit();
            }
        }
    })

    $("#chkOtherIndustry").click(function () {
        if ($(this).prop('checked') == true) {
            $("#divOtherIndustry").fadeIn();
            $("#OtherIndustry").prop("required", true);
        } else {
            $("#OtherIndustry").val('');
            $("#divOtherIndustry").fadeOut();
            $("#OtherIndustry").prop("required", false);
        }
    })

    $("#chkOtherTechnologyParent").click(function () {
        if ($(this).prop('checked') == true) {
            $("#divOtherTechnologyParent").fadeIn();
            $("#OtherTechnologyParent").prop("required", true);
        } else {
            $("#OtherTechnologyParent").val('');
            $("#divOtherTechnologyParent").fadeOut();
            $("#OtherTechnologyParent").prop("required", false);
        }
    })

    $("#chkOtherTechnology").click(function () {
        if ($(this).prop('checked') == true) {
            $("#divOtherTechnology").fadeIn();
            $("#OtherTechnology").prop("required", true);
        } else {
            $("#OtherTechnology").val('');
            $("#divOtherTechnology").fadeOut();
            $("#OtherTechnology").prop("required", false);
        }
    })

    $(document).on("click", ".remove-design-type", function () {
        var id = parseInt($(this).data("id"));
        //layoutList.push($(this).val());
        //$(".design-type-clones select")
        //    .append($("<option></option>")
        //        .attr("value", $(".design-type-" + id).find("select").val())
        //        .text($(".design-type-" + id).find("select option:selected").html()));
        layoutList.pop(layoutList.indexOf($(".design-type-" + id).find("select").val()));

        var a = $(".design-type-" + id).find("select").children("option:selected").val();
        // console.log("option[value=" + a + "]");
        $(".design-type-clones").find('select').children("option[value=" + a + "]").removeAttr('disabled');

        //console.log(layoutList);
        //console.log($(".design-type-" + id).find("select").val());
        $(".design-type-" + id).remove();
        $(".new-design-type").last().find("select").removeAttr("disabled");
        //var layoutId = $(".design-type-" + id).find("select").val();
        //if (layoutId.length > 0) {
        //    layoutList.pop(layoutList.indexOf(layoutId));
        //}
        var k = 0;
        $("fieldset.new-design-type").each(function (i, obj) {
            $(this).addClass("design-type-" + i);
            if (id == i) {
                k = id + 1;
            }
            if (k > id) {
                var currentId = (i + 1);
                $(this).removeClass("design-type-" + currentId);
                var clone = $(this);
                clone.find("select[id=DesignFiles_" + currentId + "__DesignLayoutType]").attr("name", "DesignFiles[" + i + "].DesignLayoutType");
                clone.find("select[id=DesignFiles_" + currentId + "__DesignLayoutType]").attr("id", "DesignFiles_" + i + "__DesignLayoutType");


                clone.find(".DesignFiles_" + currentId + "__DesignUnitType1").attr({
                    id: "DesignFiles_" + i + "__DesignUnitType",
                    name: "DesignFiles[" + i + "].DesignUnitType",
                    class: "DesignFiles_" + i + "__DesignUnitType1"
                });
                clone.find(".DesignFiles_" + currentId + "__DesignUnitType2").attr({
                    id: "DesignFiles_" + i + "__DesignUnitType",
                    name: "DesignFiles[" + i + "].DesignUnitType",
                    class: "DesignFiles_" + i + "__DesignUnitType2"
                });


                clone.find("input[id=DesignFiles_" + currentId + "__Image_File]").attr({
                    id: "DesignFiles_" + i + "__Image_File",
                    name: "DesignFiles[" + i + "].Image.File",
                });
                clone.find("input[id=DesignFiles_" + currentId + "__PSD_File]").attr({
                    id: "DesignFiles_" + i + "__PSD_File",
                    name: "DesignFiles[" + i + "].PSD.File",
                });
                clone.find(".remove-design-type").attr("data-id", i);
                clone.find(".remove-design-type").css("display", "block");
                $("#DesignFiles_" + i + "__Image_File").val("");
                $("#DesignFiles_" + i + "__PSD_File").val("");
                $("#DesignFiles_" + i + "__DesignLayoutType").val("");
            }
        });
    })


    $(document).on("click", ".FileUploaderSizeLimit", function () {
        var filesize = 0;
        $(".FileUploaderSizeLimit").each(function () {
            filesize += $(this)[0].files[0] == null ? 0 : ($(this)[0].files[0].size / 1024 / 1024);
        });
        if (filesize > 4096) {
            CustomAlerts.warning("Warning !!!", "You have exceeded maximum allowed size 4 GB");
            return false;
        }
    });


    //by default designer selected behalf on login user.
    if (currentUserRoleIsDesigner == "True") {
        $('#LibraryTypeId')
            .val('2')
            .trigger('change');

        $('.library-type')
            .val('2')
            .trigger('click');
    }


}(jQuery));

function DeleteLibraryFile(id, isLayout) {
    if (window.confirm("Are you sure to delete this file permanantely?")) {
        //var sIndex = $('.design-type-clones select:last').val();
        //$(".design-type-clones select")
        //    .append($("<option></option>")
        //        .attr("value", $("#hdesign" + id).val())
        //        .text($("#hdesign" + id).data('name')));

        ////var myVal = $('.design_layout:last').val();
        ////console.log(sIndex);
        //$('.design-type-clones select:last').append($(".design-type-clones select:last option")
        //    .remove().sort(function (a, b) {
        //        var at = $(a).text(),
        //            bt = $(b).text();
        //        return (at > bt) ? 1 : ((at < bt) ? -1 : 0);
        //    }));
        //$('.design-type-clones select:last').val(sIndex);
        //$("#designLayout" + id).remove();
        $.ajax({
            url: domain + 'LibraryManagement/DeleteLibraryFile',
            type: 'POST',
            data: { id: id },
            success: function (data) {
                if (data == true) {
                    if (isLayout == false) {
                        $("#image_no").val(parseInt($("#image_no").val()) - 1);
                        $("#imageid" + id).remove();
                    } else {
                        var sIndex = $('.design-type-clones select:last').val();
                        $(".design-type-clones select")
                            .append($("<option></option>")
                                .attr("value", $("#hdesign" + id).val())
                                .text($("#hdesign" + id).data('name')));

                        //var myVal = $('.design_layout:last').val();
                        //console.log(sIndex);
                        $('.design-type-clones select:last').append($(".design-type-clones select:last option")
                            .remove().sort(function (a, b) {
                                var at = $(a).text(),
                                    bt = $(b).text();
                                return (at > bt) ? 1 : ((at < bt) ? -1 : 0);
                            }));
                        $('.design-type-clones select:last').val(sIndex);
                        $("#designLayout" + id).remove();
                    }
                }
            }
        });
    }
}
function DeleteLibraryComponentFile(id, isLayout) {
    if (window.confirm("Are you sure to delete this file permanently?")) {
        //var sIndex = $('.design-type-clones select:last').val();
        //$(".design-type-clones select")
        //    .append($("<option></option>")
        //        .attr("value", $("#hdesign" + id).val())
        //        .text($("#hdesign" + id).data('name')));

        ////var myVal = $('.design_layout:last').val();
        ////console.log(sIndex);
        //$('.design-type-clones select:last').append($(".design-type-clones select:last option")
        //    .remove().sort(function (a, b) {
        //        var at = $(a).text(),
        //            bt = $(b).text();
        //        return (at > bt) ? 1 : ((at < bt) ? -1 : 0);
        //    }));
        //$('.design-type-clones select:last').val(sIndex);
        //$("#designLayout" + id).remove();
        $.ajax({
            url: domain + 'LibraryManagement/DeleteLibraryComponentFile',
            type: 'POST',
            data: { id: id },
            success: function (data) {
                if (data == true) {
                    if (isLayout == false) {
                        $("#image_no").val(parseInt($("#image_no").val()) - 1);
                        $("#imageid" + id).remove();
                    } else {
                        var sIndex = $('.design-type-clones select:last').val();
                        $(".design-type-clones select")
                            .append($("<option></option>")
                                .attr("value", $("#hdesign" + id).val())
                                .text($("#hdesign" + id).data('name')));

                        //var myVal = $('.design_layout:last').val();
                        //console.log(sIndex);
                        $('.design-type-clones select:last').append($(".design-type-clones select:last option")
                            .remove().sort(function (a, b) {
                                var at = $(a).text(),
                                    bt = $(b).text();
                                return (at > bt) ? 1 : ((at < bt) ? -1 : 0);
                            }));
                        $('.design-type-clones select:last').val(sIndex);
                        $("#designLayout" + id).remove();
                    }
                }
            }
        });
    }
}