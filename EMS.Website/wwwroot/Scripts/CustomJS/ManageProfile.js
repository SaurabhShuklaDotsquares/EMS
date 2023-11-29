(function ($) {
    function Manageprofile() {
        var $this = this, form;



        function initializeForm() {
            form = new Global.FormValidationReset('#form1');

            form.on("change", "#chkOtherTech", function (event) {
                if (this.checked) {
                    $(this).next('label').css('color', 'green');
                    $("#OtherTechnology").show();
                }
                else {
                    $(this).next('label').css('color', 'black');
                    $("#OtherTechnology").val('');
                    $("#OtherTechnology").hide();
                }
            });

            form.find("#DOB").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            form.find("#MarraigeDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            form.find('.checkList input[name="Technology"]').each(
                function () {
                    var specializations = $(this).closest('.checkList').find('input.checkspl');

                    if (this.checked) {
                        if (specializations.filter(':checked').length === 0) {
                            specializations.next('label').css('color', 'red');
                        }
                        else {
                            specializations.next('label').css('color', 'green');
                            $(this).next('label').css('color', 'green');
                        }
                    }
                    else {
                        specializations.prop('disabled', true).prop('checked', false);
                    }
                }).on('change', function () {
                    var specializations = $(this).closest('.checkList').find('input.checkspl');

                    if (this.checked) {
                        specializations.prop('disabled', false);
                        if (specializations.filter(':checked').length === 0) {
                            specializations.next('label').css('color', 'red');
                        }
                        else {
                            specializations.next('label').css('color', 'green');
                            $(this).next('label').css('color', 'green');
                        }
                    }
                    else {
                        specializations.prop('disabled', true).prop('checked', false);
                        specializations.next('label').css('color', '');
                        $(this).next('label').css('color', '');
                    }
                });

            form.find('.checkList input[name="Domain"]').each(
                function () {
                    if (this.checked) {
                        $(this).next('label').css('color', 'green');
                    }
                }).on('change', function () {
                    if (this.checked) {
                        $(this).next('label').css('color', 'green');
                    }

                });

            form.on('change', '.chkSecialization input[type="radio"]', function () {
                $(this).closest('.chkSecialization').find('label').css('color', 'green');
                $(this).closest('.checkList').find('input[name="Technology"]+label').css('color', 'green');
            });

            form.on('change', '#IsInterestedPffaccount', function () {
                if ($(this).is(':checked')) {
                    $("#pf_uan_div").show();
                } else {
                    $("#pf_uan_div").hide();
                }
            })

            form.on('submit').on('submit', function (e) {
                e.preventDefault();
                if (form.valid()) {
                    var userTechs = [];
                    var errorMsg = false;
                    var selectedTechs = $('.checkList input[name="Technology"]:checked');
                    if (selectedTechs.length) {
                        selectedTechs.each(function () {
                            var specType = $(this).closest('.checkList').find('input.checkspl:checked').val();

                            if (!specType) {
                                errorMsg = ('Please choose specialization level for technology : "' + $(this).next('label').text().trim() + '"');
                                return false;
                            }
                            else {
                                userTechs.push({
                                    TechId: this.value,
                                    SpecTypeId: specType
                                });
                            }
                        });

                        if (errorMsg) {
                            Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                            //CustomAlerts.error("Error !!!", errorMsg);
                            return false;
                        }
                    }

                    var userDomain = [];
                    var selectedDomains = $('.checkList input[name="Domain"]:checked');
                    if (selectedDomains.length) {
                        selectedDomains.each(function () {
                            userDomain.push({
                                DomainId: this.value
                            });
                        });
                    }

                    var formdata = form.serializeArray();
                    if (userTechs.length) {

                        $.each(userTechs, function (i, item) {
                            formdata.push({ name: 'TechnologyList[' + i + '].TechId', value: item.TechId });
                            formdata.push({ name: 'TechnologyList[' + i + '].SpecTypeId', value: item.SpecTypeId });
                        });
                    }
                    if (userDomain.length) {
                        $.each(userDomain, function (i, item) {
                            formdata.push({ name: 'DomainExpert[' + i + '].DomainId', value: item.DomainId });
                        });
                    }

                    /*Add files of form */
                    var formdata1 = new FormData();
                    form.find('input[type="file"]:not(:disabled)').each(function (i, elem) {
                        if (elem.files && elem.files.length > 0) {
                            for (var i = 0; i < this.files.length; i++) {
                                var file = elem.files[i];
                                formdata1.append(elem.getAttribute('name'), file);
                            }
                        }
                    });

                    /* Append Serilization of Form  with FormData */
                    $.each(formdata, function (i, item) {
                        formdata1.append(item.name, item.value);
                    });


                    var submitBtn = $(form).find(':submit');
                    var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();

                    submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
                    submitBtn.prop('disabled', true);
                    $.ajax(form.attr("action"), {
                        type: "POST",
                        data: formdata1,
                        contentType: false,
                        processData: false,
                        success: function (data) {
                            if (data.success) {
                                Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                                $('html, body').animate({
                                    'scrollTop': $("#NotificationDiv").position().top
                                });
                            } else {
                                Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                            }
                            setTimeout(function () {
                                window.location.reload();}, 3000)
                            
                        },
                        complete: function (result) {
                            submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                            submitBtn.prop('disabled', false);

                        }

                    });




                    //debugger;
                    //$.post(form[0].action, formdata1,

                    //    function (data) {
                    //    submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                    //    submitBtn.prop('disabled', false);
                    //    if (data.success) {
                    //        Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                    //        $('html, body').animate({
                    //            'scrollTop': $("#NotificationDiv").position().top
                    //        });
                    //    } else {
                    //        Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                    //    }
                    //});



                }
            });
        }

        if (window.File && window.FileList && window.FileReader) {
            $("#files").on("change", function (e) {
                var files = e.target.files,
                    filesLength = files.length;
                $(".doc-list .remove-li").remove();
                for (var i = 0; i < filesLength; i++) {
                    var file = files[i];
                    var typeArr = file.name.split(".");
                    var fileIcon = 'fa fa-file-image-o';
                    if (file.name.indexOf('.ppt')!=-1) {
                        fileIcon = "fa fa-file-powerpoint-o";
                    } else if (file.name.indexOf('.doc') != -1) {
                        fileIcon = "fa fa-file-word-o";
                    }
                    else if (file.name.indexOf('.xls') != -1 || file.name.indexOf('.xlsx') != -1) {
                        fileIcon = "fa fa-file-excel-o";
                    }
                    else if (file.name.indexOf('.rar') != -1 || file.name.indexOf('.zip') != -1) {
                        fileIcon = "fa fa-file-zip-o";
                    }
                    else if (file.name.indexOf('.pdf') != -1) {
                        fileIcon = "fa fa-file-pdf-o";
                    }
                    else if (file.name.indexOf('.txt') != -1) {
                        fileIcon = "fa fa-file-txt-o";
                    }
                    $("<li class=\"remove-li\">" +
                        "<i class=\""+fileIcon+"\"></i>  " + file.name +
                        "<span><a href=\"javascript:;\" style=\"float: right;\" class=\"delete delete-remove\"> <i class=\"fa fa-trash\" style=\"color:red;font-size:17px;\"></i></a> </span></li>").prependTo(".doc-list");
                                     
                }
            });


            $(document).on('click', '.delete-remove', function () {                
                $(this).parent().parent(".remove-li").remove();
            })

        } else {
            alert("Your browser doesn't support to File API")
        }


        $(".delete-perm").click(function () {
            var id = $(this).data("id");
            if (confirm("Are you sure you want to delete this item?")) {
                $.ajax({
                    url: domain + 'User/DeleteDocumentFile',
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
        //if (window.File && window.FileList && window.FileReader) {
        //    $("#files").on("change", function (e) {
        //        //var files = e.target.files,
        //        //    filesLength = files.length;
        //        //var fragment = "";
        //        //for (var i = 0; i < filesLength; i++) {
        //        //    var fileName = files[i].name; // get file name
        //        //    var fileSize = files[i].size; // get file size 
        //        //    var fileType = files[i].type; // get file type

        //        //    // append li to UL tag to display File info
        //        //    fragment += "<li>" + fileName + " (<b>" + fileSize + "</b> bytes) - Type :" + fileType + "</li>";
        //        //}





        //        for (var i = 0; i < filesLength; i++) {
        //            var f = files[i]
        //            //debugger;
        //            //var fileName = files[i].name; // get file name
        //            //var fileSize = files[i].size; // get file size 
        //            //var fileType = files[i].type; // get file type
        //            var fileReader = new FileReader();
        //            fileReader.onload = (function (e) {
        //                //debugger;
        //                    var file = e.target;
        //                    $("<span class=\"pip\">" +
        //                        "<img class=\"imageThumb\" src=\"" + e.target.result + "\" title=\"" + fileName + "\"/>" +
        //                        "<br/><span class=\"remove\"><i class=\"fa fa-trash\"></i>&nbsp;Remove</span>" +
        //                        "</span>").insertAfter("#files");
        //                    $(".remove").click(function () {
        //                        $(this).parent(".pip").remove();
        //                    });


        //            });
        //            fileReader.readAsDataURL(f);
        //        }





        //        //var files = evt.target.files;

        //        //for (var i = 0, len = files.length; i < len; i++) {
        //        //    var file = files[i];

        //        //    var reader = new FileReader();

        //        //    reader.onload = (function (f) {
        //        //        return function (e) {
        //        //            //debugger;
        //        //            // Here you can use `e.target.result` or `this.result`
        //        //            // and `f.name`.
        //        //            $("<span class=\"pip\">" +
        //        //                "<img class=\"imageThumb\" src=\"" + e.target.result + "\" title=\"" + f.name + "\"/>" +
        //        //                "<br/><span class=\"remove\"><i class=\"fa fa-trash\"></i>&nbsp;Remove</span>" +
        //        //                "</span>").insertAfter("#files");
        //        //            $(".remove").click(function () {
        //        //                $(this).parent(".pip").remove();
        //        //            });
        //        //        };
        //        //    })(file);

        //        //    reader.readAsDataURL(file);
        //        //}



        //    });
        //} else {
        //    alert("Your browser doesn't support to File API")
        //}

        $this.init = function () {
            initializeForm();
        };
    }

    $(function () {
        var self = new Manageprofile();
        self.init();
    });
}(jQuery));
