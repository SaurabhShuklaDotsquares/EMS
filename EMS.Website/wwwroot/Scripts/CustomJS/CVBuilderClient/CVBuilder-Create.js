(function ($) {
    function index() {
        var $this = this, grid, formAddEdit;

        function IntializeForm() {
            formAddEdit = new Global.FormHelper($("form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: [] }
            });

            CKEDITOR.replace('Title', { toolbar: "Basic" });
            attachEventCKEditor('Title');

            CKEDITOR.replace('ProfileSummary', { toolbar: "Basic" });
            attachEventCKEditor('ProfileSummary');

            CKEDITOR.replace('TechnicalSkills', { toolbar: "Basic" });
            attachEventCKEditor('TechnicalSkills');

            CKEDITOR.replace('WorkExperience', { toolbar: "Basic" });
            attachEventCKEditor('WorkExperience');

            CKEDITOR.replace('RolesAcross', { toolbar: "Basic" });
            attachEventCKEditor('RolesAcross');

            $('.selectdates').on('change', function (ids) {               
                var id = $(this).attr('id');  
                if (id == "ToDate_0") {
                    var fmonth = $("#FromMonth_0 :selected").val();
                    var fyear = $("#FromDate_0 :selected").val();
                    var tmonth = $("#ToMonth_0 :selected").val();
                    var tyear = $("#ToDate_0 :selected").val();
                    ValidateDateRange(fmonth, fyear, tmonth, tyear,0);
                }
            });
            $('.selectdatesUpdate').on('change', function (ids) {
                var lastChar = 0; var firstChar = '';
                var id = $(this).attr('id');
                var values = id.split('_');
                firstChar = values[0];
                lastChar = values[1];
                if (firstChar == "ToDate") {
                    var fmonth = $("#FromMonth_" + lastChar +" :selected").val();
                    var fyear = $("#FromDate_" + lastChar +" :selected").val();
                    var tmonth = $("#ToMonth_" + lastChar +" :selected").val();
                    var tyear = $("#ToDate_" + lastChar +" :selected").val();
                    ValidateDateRange(fmonth, fyear, tmonth, tyear, 0);
                }
            });
            
            function ValidateDateRange(fmonth, fyear, tmonth, tyear, lastChar) {                
                if (fyear != "Select" && tyear == "Present") {
                   
                }
                else
                {
                    if (fyear > tyear) {
                        $("#ToDate_" + lastChar).val('Present');
                        swal({
                            title: "Alert!",
                            text: "Please check month and year of To Date which should be greater than From Date.",
                            icon: "error",
                        });
                    }
                    else if (fyear == tyear) {
                        if (parseInt(fmonth) > parseInt(tmonth)) {
                            $("#ToMonth_" + lastChar).val(fmonth);
                            swal({
                                title: "Alert!",
                                text: "To always greater then From.",
                                icon: "error",
                            });
                        }
                    }
                }
            }
            function ValidateDateRangeSelected(firstChar, lastChar) {                
                var lastCharTop = lastChar - 1;
                var ftmonth = $("#ToMonth_" + lastCharTop + " :selected").val();
                var ftyear = $("#ToDate_" + lastCharTop + " :selected").val();
                var fmonth = $("#FromMonth_" + lastChar + " :selected").val();
                var fyear = $("#FromDate_" + lastChar + " :selected").val();

                if (ftyear > fyear) {
                    $("#FromDate_" + lastChar).val('Select');
                    swal({
                        title: "Alert!",
                        text: "Experience should be added into assending order as per the From and To date.",
                        icon: "error",
                    });
                }
                else if (ftyear == fyear) {
                    if (parseInt(ftmonth) > parseInt(fmonth)) {
                        $("#FromMonth_" + lastChar).val(ftmonth);
                        swal({
                            title: "Alert!",
                            text: "Experience should be added into assending order as per the From and To date.",
                            icon: "error",
                        });
                    }
                }
            }
            
            $('.selectdateAddmoreUpdate').on('change', function (ids) {

                var lastChar = 0; var firstChar = '';
                var id = $(this).attr('id');
                var values = id.split('_');
                firstChar = values[0];
                lastChar = values[1];

                if ("FromDate" == firstChar) {
                    ValidateDateRangeSelected(firstChar, lastChar);

                }
                if ("ToDate" == firstChar) {
                    var fmonth = $("#FromMonth_" + lastChar + " :selected").val();
                    var fyear = $("#FromDate_" + lastChar + " :selected").val();
                    var tmonth = $("#ToMonth_" + lastChar + " :selected").val();
                    var tyear = $("#ToDate_" + lastChar + " :selected").val();
                    ValidateDateRange(fmonth, fyear, tmonth, tyear, lastChar);
                }
            });
            $("#fromDate").datepicker({
               
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-60:+0",
                onClose: function (selectedDate) {
                    $("#toDate").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#toDate").datepicker({
               
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-60:+0",
                onClose: function (selectedDate) {
                    $("#fromDate").datepicker("option", "maxDate", selectedDate);
                }
            });
            

            $(document).on('click', '#addmore', function () {

                var innerHtml = "";
                var len = $('#tbodykra tr').length;
                innerHtml = "<tr id='trlen_" + len + "'>";
                innerHtml += "<td> " + (len + 1) + "</td><td style='display: none;'><input type='hidden' id='Id_ " + len + "' value='0' /></td><td><input type='text' name='Title_" + len + "' maxlength='40' class='form-control' id='Title_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
                innerHtml += " <td><select id = 'Level_" + len + "' class='form-control'><option value='3'>Beginner</option><option value='2'>Intermediate</option><option value='1'>Expert</option></select></td >";
                innerHtml += "<td> <a href='javascript:void (0);' class='RemoveRowKRA'><i class='text-danger fa fa-trash'></i></a></td > ";


                innerHtml += "</tr>"

                $('#tbodykra').append(innerHtml);


            })



            $(document).on('click', '.RemoveRowKRA', function () {                
                $(this).parent().parent().remove();
                RegenerateIdKRa();
            })
            function RegenerateIdKRa() {
                var tbl_len = $('#tbodykra tr').length;
                
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodykra tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodykra tr')[i]).children()[1]).find('input').attr('name', 'Id_' + i);
                    $($($('#tbodykra tr')[i]).children()[1]).find('input').attr('id', 'Id_' + i);
                    $($($('#tbodykra tr')[i]).children()[2]).find('input').attr('name', 'Title_' + i);
                    $($($('#tbodykra tr')[i]).children()[2]).find('input').attr('id', 'Title_' + i);
                    $($($('#tbodykra tr')[i]).children()[3]).find('input').attr('name', 'Level_' + i);
                    $($($('#tbodykra tr')[i]).children()[3]).find('input').attr('id', 'Level_' + i);
                }
            }

            $(document).on('click', '#addmoreCareerTimeline', function () {

                var innerHtml = "";
                var len = $('#tbodyCareerTimeline tr').length;
                innerHtml = "<tr id='trlen_" + len + "'>";
                innerHtml += "<td> " + (len + 1) + "</td><td><input type='text' name='Title_" + len + "' class='form-control' id='Title_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
                innerHtml += "<td><input type='number' name='KRAOrderno_" + len + "' class='form-control' id='KRAOrderno_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";
                innerHtml += "<td> <a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td > ";


                innerHtml += "</tr>"

                $('#tbodyCareerTimeline').append(innerHtml);


            })



            $(document).on('click', '.RemoveRow', function () {
                $(this).parent().parent().remove();
                RegenerateIdCareerTimeline();
            })
            function RegenerateIdCareerTimeline() {
                var tbl_len = $('#tbodyCareerTimeline tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodyCareerTimeline tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodyCareerTimeline tr')[i]).children()).find('input').attr('name', 'Title_' + i);
                    $($($('#tbodyCareerTimeline tr')[i]).children()).find('input').attr('id', 'Title_' + i);
                    $($($('#tbodyCareerTimeline tr')[i]).children()[2]).find('input').attr('name', 'KRAOrderno_' + i);
                    $($($('#tbodyCareerTimeline tr')[i]).children()[2]).find('input').attr('id', 'KRAOrderno_' + i);


                }
            }

            $(document).on('click', '#addmoreCertifications', function () {

                var innerHtml = "";
                var len = $('#tbodyCertifications tr').length;
                innerHtml = "<tr id='trlen_" + len + "'>";
                innerHtml += "<td> " + (len + 1) + "</td>";
                innerHtml += "<td style='display: none;'><input type='hidden' id='CImageURL_" + len + "' value='' /></td > "
                innerHtml += "<td><input type='text' name='CTitle_" + len + "' class='form-control' id='CTitle_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
                innerHtml += "<td><input type='text' name='KRAOrderno_" + len + "' class='form-control' id='KRAOrderno_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";
                innerHtml += "<td><input type='file' id='CertificationImage_" + len + "' class='custom-file-input' accept='image/png,image/jpeg,image/jpg' onchange='proCVFileImagePreview(event)'></td>";
                innerHtml += "<td><img src='' asp-append-version='true' style='border:1px solid gray; margin-bottom:5px;height:50px;width: 50px;float:right;' id='CIcon_" + len + "'/></td>";
                innerHtml += "<td> <a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td > ";


                innerHtml += "</tr>"

                $('#tbodyCertifications').append(innerHtml);


            })



            $(document).on('click', '.RemoveRow', function () {
                $(this).parent().parent().remove();
                RegenerateIdCertifications();
            })
            function RegenerateIdCertifications() {
                var tbl_len = $('#tbodyCertifications tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodyCertifications tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodyCertifications tr')[i]).children()[1]).find('input').attr('name', 'CImageURL_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[1]).find('input').attr('id', 'CImageURL_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[2]).find('input').attr('name', 'CTitle_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[2]).find('input').attr('id', 'CTitle_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[3]).find('input').attr('name', 'KRAOrderno_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[3]).find('input').attr('id', 'KRAOrderno_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[4]).find('input').attr('name', 'CertificationImage_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[4]).find('input').attr('id', 'CertificationImage_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[5]).find('input').attr('name', 'CIcon_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[5]).find('input').attr('id', 'CIcon_' + i);
                }
            }
            $(document).on('click', '#addmorePreviousExperience', function () {
                
                var monthsOption = [];
                var yearsOption = [];
                var fyearsOption = [];
                $.ajax
                    ({
                        url: domain + 'clientcv/GetYearsMonthsList',
                        type: 'POST',

                        success: function (result) {                            
                            $.each(result.monthsList, function () {
                                monthsOption.push("<option value='" + this['value'] + "'>" + this['text'] + "</option>");
                            });
                            $.each(result.yearsList, function () {
                                yearsOption.push("<option value='" + this['value'] + "'>" + this['text'] + "</option>");
                            });
                            $.each(result.fYearsList, function () {
                                fyearsOption.push("<option value='" + this['value'] + "'>" + this['text'] + "</option>");
                            });
                            var innerHtml = "";
                            var len = $('#tbodyPreviousExperience tr').length;
                            innerHtml = "<tr id='trlen_" + len + "'>";
                            innerHtml += "<td> " + (len + 1) + "</td><td><input type='text' name='OrganizationName_" + len + "' class='form-control' id='OrganizationName_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
                            innerHtml += "<td><input type='text' name='Designation_" + len + "' class='form-control' id='Designation_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";
                            innerHtml += "<td><div class='cv-select'><select id='FromMonth_" + len + "' class='form-control' >" + monthsOption + "</select></div></td>";
                            innerHtml += "<td><div class='cv-select'><select id='FromDate_" + len + "' class='form-control selectdateAddmore' >" + fyearsOption + "</select></div></td>";
                            
                            innerHtml += "<td><div class='cv-select'><select id='ToMonth_" + len + "' class='form-control'>" + monthsOption + "</select></div></td>";
                            innerHtml += "<td><div class='cv-select'><select id='ToDate_" + len + "' class='form-control selectdateAddmore' >" + yearsOption + "</select></div></td>";
                            
                            innerHtml += "<td> <a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td > ";


                            innerHtml += "</tr>"

                            $('#tbodyPreviousExperience').append(innerHtml);

                            $('.selectdateAddmore').on('change', function (ids) {
                                
                                var lastChar = 0; var firstChar = '';
                                var id = $(this).attr('id');
                                var values = id.split('_');
                                firstChar = values[0];
                                lastChar = values[1];

                                if ("FromDate" == firstChar) {
                                    ValidateDateRangeSelected(firstChar, lastChar);
                                    
                                } 
                                if ("ToDate" == firstChar) {
                                    var fmonth = $("#FromMonth_" + lastChar + " :selected").val();
                                    var fyear = $("#FromDate_" + lastChar + " :selected").val();
                                    var tmonth = $("#ToMonth_" + lastChar + " :selected").val();
                                    var tyear = $("#ToDate_" + lastChar + " :selected").val();
                                    ValidateDateRange(fmonth, fyear, tmonth, tyear, lastChar);
                                }
                            });

                        },
                        error: function (ex) {
                            alert("Whooaaa! Something went wrong.." + ex);
                        },
                    });
                
            });



            $(document).on('click', '.RemoveRow', function () {
                $(this).parent().parent().remove();
                RegenerateIdPreviousExperience();
            })
            function RegenerateIdPreviousExperience() {
                var tbl_len = $('#tbodyPreviousExperience tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodyPreviousExperience tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodyPreviousExperience tr')[i]).children()[1]).find('input').attr('name', 'OrganizationName_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[1]).find('input').attr('id', 'OrganizationName_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[2]).find('input').attr('name', 'Designation_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[2]).find('input').attr('id', 'Designation_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[3]).find('select').attr('name', 'FromMonth_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[3]).find('select').attr('id', 'FromMonth_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[4]).find('select').attr('name', 'FromDate_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[4]).find('select').attr('id', 'FromDate_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[5]).find('select').attr('name', 'ToMonth_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[5]).find('select').attr('id', 'ToMonth_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[6]).find('select').attr('name', 'ToDate_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[6]).find('select').attr('id', 'ToDate_' + i);


                }
            }

            $(document).on('click', '#addmoreEducation', function () {

                var innerHtml = "";
                var len = $('#tbodyEducation tr').length;
                innerHtml = "<tr id='trlen_" + len + "'>";
                innerHtml += "<td> " + (len + 1) + "</td><td><input type='text' name='EduTitle_" + len + "' class='form-control' id='EduTitle_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
                innerHtml += "<td><input type='text' name='EduUniversity_" + len + "' class='form-control' id='EduUniversity_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";
                innerHtml += "<td> <a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td > ";


                innerHtml += "</tr>"

                $('#tbodyEducation').append(innerHtml);


            })



            $(document).on('click', '.RemoveRow', function () {
                $(this).parent().parent().remove();
                RegenerateIdEducation();
            })
            function RegenerateIdEducation() {
                var tbl_len = $('#tbodyEducation tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodyEducation tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodyEducation tr')[i]).children()).find('input').attr('name', 'EduTitle_' + i);
                    $($($('#tbodyEducation tr')[i]).children()).find('input').attr('id', 'EduTitle_' + i);
                    $($($('#tbodyEducation tr')[i]).children()[2]).find('input').attr('name', 'EduUniversity_' + i);
                    $($($('#tbodyEducation tr')[i]).children()[2]).find('input').attr('id', 'EduUniversity_' + i);


                }
            }

            $(document).on('click', '#btn-submit', function () {               
                var isAgreeChecked = $('input:checkbox[name=IsAgree]:unchecked').val();
                if ($('#ExperienceType').val() == '') {
                    swal({
                        title: "Alert!",
                        text: "Experience Type is required.",
                        icon: "error",
                    });
                    $("#ExperienceType").focus();
                }                
                else if (isAgreeChecked) {
                    swal({
                        title: "Alert!",
                        text: "Please check I acknowledge that the above information is true and valid to the best of my knowledge.",
                        icon: "error",
                    });
                    $("#IsAgree").focus();
                }
                else if (ValidateCertifications()) {
                    swal({
                        title: "Alert!",
                        text: "Certification Icon is required.",
                        icon: "error",
                    });
                    $("#CTitle_0").focus();
                }
                else if (ValidateFloatValue()) {
                    document.getElementById("btn-submit").disabled = true; 
                    SaveRecord();
                }
                else {
                    swal({
                        title: "Alert!",
                        text: "Fill correct details in previous experience and also fill records in assending order for best view.",
                        icon: "error",
                    });
                    $("#tbodyPreviousExperience").focus();
                }
            });


            function SaveRecord() {   
                $('.loading-common,.loading-overlay').show();
                //var IndustryData = new Array();
                //var markedIndustryChk = document.getElementsByName('Industry');
                //for (var checkbox of markedIndustryChk) {
                //    if (checkbox.checked) {
                //        IndustryData.push(checkbox.value);
                //    }
                //}

                //var TechnologyData = new Array();
                //var markedTechnologyChk = document.getElementsByName('Technology');
                //for (var Tcheckbox of markedTechnologyChk) {
                //    if (Tcheckbox.checked) {
                //        TechnologyData.push(Tcheckbox.value);
                //    }
                //}
                var TechnologyData = new Array(); 
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
                            var tempobj = new Object();
                            tempobj.SpecTypeId = specType;
                            tempobj.TechId = this.value;
                            TechnologyData.push(tempobj);
                        }
                    });

                    if (errorMsg) {
                        swal({
                            title: "Alert!",
                            text: errorMsg,
                            icon: "error",
                        });
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
                var dataEducation = new Array();
                var tbl_len = $('#tbodyEducation tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.Title = $('#EduTitle_' + i).val();
                    tempobj.University = $('#EduUniversity_' + i).val();
                    dataEducation.push(tempobj);
                }
                var formData = new FormData();
                formData.append("Id", $('#Id').val());                
                formData.append("Title",CKEDITOR.instances['Title'].getData());
                formData.append("ProfileSummary",CKEDITOR.instances['ProfileSummary'].getData());
                formData.append("TechnicalSkills", CKEDITOR.instances['TechnicalSkills'].getData());
                formData.append("WorkExperience", CKEDITOR.instances['WorkExperience'].getData());                
                formData.append("RolesAcross", CKEDITOR.instances['RolesAcross'].getData());
                formData.append("Linkedin",$('#Linkedin').val());
                formData.append("Languages", $('#Languages').val());
                formData.append("ExperienceType", $('#ExperienceType').val());
                //formData.append("IndustryJson", JSON.stringify(IndustryData));
                formData.append("UserDomainJson", JSON.stringify(userDomain));
                formData.append("TechnologyJson", JSON.stringify(TechnologyData));
                formData.append("OtherIndustry",$("#OtherIndustry").val());
                formData.append("OtherTechnology", $("#OtherTechnology").val());
                formData.append("OtherTechnologyParent", $("#OtherTechnologyParent").val());
                formData.append("ProfilePicture", $('#ProfilePicture').val());
                formData.append("IsAgree", document.getElementById('IsAgree').checked);
                formData.append("Uid_User", $('#Uid_User').val());
                                
                var data = new Array();
                var tbl_len = $('#tbodykra tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.Id = $('#Id_' + i).val();
                    tempobj.Title = $('#Title_' + i).val();
                    tempobj.KRAOrderno = $('#Level_' + i + ' :selected').val();
                    data.push(tempobj);
                }
                formData.append("dataListJson", JSON.stringify(data));

                var dataEducation = new Array();
                var tbl_len = $('#tbodyEducation tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.Title = $('#EduTitle_' + i).val();
                    tempobj.University = $('#EduUniversity_' + i).val();
                    dataEducation.push(tempobj);
                }
                formData.append("EducationJson", JSON.stringify(dataEducation));
                               
                var dataCertifications = new Array();
                var tbl_len = $('#tbodyCertifications tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();                    
                    tempobj.Title = $('#CTitle_' + i).val();
                    tempobj.CertificationsNumber = $('#KRAOrderno_' + i).val();                    
                    tempobj.CertificationsURL = $('#CImageURL_' + i).val();
                    
                    var files = $('#CertificationImage_' + i).prop("files");
                    if (files.length > 0) {
                        tempobj.ImageIndex = i;
                    }
                    else {
                        tempobj.ImageIndex = "";
                    }
                    dataCertifications.push(tempobj);
                }                

                formData.append("CertificationsJson", JSON.stringify(dataCertifications));
                // Append array3 with files to FormData
                //$.each(dataCertifications, function (index, item) {
                //    formData.append("array3[" + index + "][name]", item.Title);
                //    formData.append("array3[" + index + "][age]", item.KRAOrderno);
                //    formData.append("array3[" + index + "][file]", item.CertificationImage);
                //});
                //formData.append("CertificationsJson", JSON.stringify(dataCertifications));
                
                var tbl_len = $('#tbodyCertifications tr').length;
                for (var i = 0; i < tbl_len; i++) {                    
                    //var fileInput = $('#CertificationImage_' + i)[0];
                    //var file = fileInput.files[0];
                    //formData.append("CertificationIMG", file);
                    var files = $('#CertificationImage_' + i).prop("files");
                    formData.append("CertificationIMG", files[0]);
                }
                var datatPreviousExperience = new Array();
                var tbl_len = $('#tbodyPreviousExperience tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.OrganizationName = $('#OrganizationName_' + i).val();
                    tempobj.Designation = $('#Designation_' + i).val();
                    tempobj.FromMonth = $('#FromMonth_' + i + ' :selected').text();
                    tempobj.FromDate = $('#FromDate_' + i + ' :selected').val();
                    //tempobj.FromDate = $('#FromDate_' + i).val();
                    tempobj.ToMonth = $('#ToMonth_' + i + ' :selected').text();
                    tempobj.ToDate = $('#ToDate_' + i + ' :selected').val();
                    //tempobj.ToDate = $('#ToDate_' + i).val();
                    datatPreviousExperience.push(tempobj);
                }
                formData.append("PreviousExperienceJson", JSON.stringify(datatPreviousExperience));
                
                var files = $('#ProfileImage').prop("files");
                formData.append("ProfileImage", files[0]);
                
                

                $.ajax({
                    url: domain + 'clientcv/SaveRecords',
                    type: 'POST',
                    dataType: 'json',
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function () {                        
                        swal({
                            title: "Alert!",
                            text: "Record saved successfully.",
                            icon: "success",
                        }).then(function () {
                            $('.loading-common,.loading-overlay').hide();
                            window.location.href = 'clientcv/index';
                        });
                    }
                });

            }
            $("#chkOtherIndustry").click(function () {
                if ($(this).prop('checked') == true) {
                    $("#divOtherIndustry").fadeIn();
                    $("#OtherIndustry").prop("required", true);
                } else {
                    $("#OtherIndustry").val('');
                    $("#divOtherIndustry").fadeOut();
                    $("#OtherIndustry").prop("required", false);
                }
            });
            $("#chkOtherTechnologyParent").click(function () {
                if ($(this).prop('checked') == true) {
                    $("#divOtherTechnologyParent").fadeIn();
                    $("#OtherTechnologyParent").prop("required", true);
                } else {
                    $("#OtherTechnologyParent").val('');
                    $("#divOtherTechnologyParent").fadeOut();
                    $("#OtherTechnologyParent").prop("required", false);
                }
            });
            $("#chkOtherTechnology").click(function () {
                if ($(this).prop('checked') == true) {
                    $("#divOtherTechnology").fadeIn();
                    $("#OtherTechnology").prop("required", true);
                } else {
                    $("#OtherTechnology").val('');
                    $("#divOtherTechnology").fadeOut();
                    $("#OtherTechnology").prop("required", false);
                }
            });
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

            function ValidateCertifications() {
                var tbl_len = $('#tbodyCertifications tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    
                    var Title = $('#CTitle_' + i).val();
                    var CertificationsURL = $('#CImageURL_' + i).val();
                    var files = $('#CertificationImage_' + i).prop("files");
                    if (Title != "") {
                        if (files.length == 0) {
                            if (CertificationsURL == "" || CertificationsURL == undefined) {
                                return true;
                            }
                        }
                       
                    }
                }
                return false;
            }

            function ValidateFloatValue() {                
                //if ($('#ExperienceType').val() == '') {
                //    swal({
                //        title: "Alert!",
                //        text: "Experience is required.",
                //        icon: "error",
                //    });
                //    return false;
                //}
                //var isAgreeChecked = $('input:checkbox[name=IsAgree]:unchecked').val();
                //if (isAgreeChecked) {
                //    swal({
                //        title: "Alert!",
                //        text: "Please check you are all information agree.",
                //        icon: "error",
                //    });
                //    return false;
                //}
                
                var tbl_len = $('#tbodyPreviousExperience tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    
                    var fromMonth = $('#FromMonth_' + i + ' :selected').val();
                    var fromDate = $('#FromDate_' + i).val();
                    var toMonth = $('#ToMonth_' + i + ' :selected').val();
                    var toDate = $('#ToDate_' + i).val();
                    if (i == "0") {
                        if (fromDate != "Select" && toDate == "Present") {
                            return true;
                        }
                        else {
                            if (fromDate > toDate) {
                                return false;
                            }
                            else if (fromDate == toDate) {
                                if (parseInt(fromMonth) > parseInt(toMonth)) {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {                       
                        
                        var lastCharTop = i - 1;
                        var ftmonth = $("#ToMonth_" + lastCharTop + " :selected").val();
                        var ftyear = $("#ToDate_" + lastCharTop + " :selected").val();
                        var fmonth = $("#FromMonth_" + i + " :selected").val();
                        var fyear = $("#FromDate_" + i + " :selected").val();

                        if (fromDate != "Select" && toDate == "Present") {
                            return true;
                        }
                        else {

                            if (ftyear > fyear) {
                                return false;
                            }
                            else if (ftyear == fyear) {
                                if (parseInt(ftmonth) > parseInt(fmonth)) {
                                    return false;
                                }
                            }


                            if (fromDate > toDate) {
                                return false;
                            }
                            else if (fromDate == toDate) {
                                if (parseInt(fromMonth) > parseInt(toMonth)) {
                                    return false;
                                }
                            }
                        }
                    }                    
                }                              

                return true;
            }
        }



        function attachEventCKEditor(instance) {
            CKEDITOR.on('instanceReady', function (e) {
                e.editor.document.on('keyup', function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }




        $this.init = function () {
            IntializeForm();
        };
    }

    $(function () {
        var self = new index();
        self.init();



    });
}(jQuery));