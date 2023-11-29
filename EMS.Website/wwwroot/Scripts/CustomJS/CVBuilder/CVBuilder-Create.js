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
                            text: "From date is more than todate.",
                            icon: "error",
                        });
                    }
                    else if (fyear == tyear) {
                        if (parseInt(fmonth) > parseInt(tmonth)) {
                            $("#ToMonth_" + lastChar).val(fmonth);
                            swal({
                                title: "Alert!",
                                text: "From month is more than to month.",
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
                        text: "From date is more than todate.",
                        icon: "error",
                    });
                }
                else if (ftyear == fyear) {
                    if (parseInt(ftmonth) > parseInt(fmonth)) {
                        $("#FromMonth_" + lastChar).val(ftmonth);
                        swal({
                            title: "Alert!",
                            text: "From month is more than to month.",
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
                //defaultDate: "+1w",
                //dateFormat: "mm/yy",
                //changeMonth: true,
                //changeYear: true,
                //yearRange: "-60:+0" ,

                //numberOfMonths: 1,
                //minDate: 0,
                //maxDate: "+29"
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-60:+0",
                onClose: function (selectedDate) {
                    $("#toDate").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#toDate").datepicker({
                //defaultDate: "+1w",
                //dateFormat: "mm/yy",
                //changeMonth: true,
                //changeYear: true,
                //yearRange: "-60:+0"
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
                innerHtml += "<td> " + (len + 1) + "</td><td><input type='text' name='Title_" + len + "' class='form-control' id='Title_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
                //innerHtml += "<td><input type='number' name='KRAOrderno_" + len + "' class='form-control' id='KRAOrderno_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";
                innerHtml += "<td> <a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td > ";


                innerHtml += "</tr>"

                $('#tbodykra').append(innerHtml);


            })



            $(document).on('click', '.RemoveRow', function () {
                $(this).parent().parent().remove();
                RegenerateId();
            })
            function RegenerateId() {
                var tbl_len = $('#tbodykra tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodykra tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodykra tr')[i]).children()).find('input').attr('name', 'Title_' + i);
                    $($($('#tbodykra tr')[i]).children()).find('input').attr('id', 'Title_' + i);
                    //$($($('#tbodykra tr')[i]).children()[2]).find('input').attr('name', 'KRAOrderno_' + i);
                    //$($($('#tbodykra tr')[i]).children()[2]).find('input').attr('id', 'KRAOrderno_' + i);


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
                RegenerateId();
            })
            function RegenerateId() {
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
                innerHtml += "<td> " + (len + 1) + "</td><td><input type='text' name='CTitle_" + len + "' class='form-control' id='CTitle_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
                innerHtml += "<td><input type='text' name='KRAOrderno_" + len + "' class='form-control' id='KRAOrderno_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";
                innerHtml += "<td> <a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td > ";


                innerHtml += "</tr>"

                $('#tbodyCertifications').append(innerHtml);


            })



            $(document).on('click', '.RemoveRow', function () {
                $(this).parent().parent().remove();
                RegenerateId();
            })
            function RegenerateId() {
                var tbl_len = $('#tbodyCertifications tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodyCertifications tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodyCertifications tr')[i]).children()).find('input').attr('name', 'CTitle_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()).find('input').attr('id', 'CTitle_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[2]).find('input').attr('name', 'KRAOrderno_' + i);
                    $($($('#tbodyCertifications tr')[i]).children()[2]).find('input').attr('id', 'KRAOrderno_' + i);


                }
            }
            $(document).on('click', '#addmorePreviousExperience', function () {
                
                var monthsOption = [];
                var yearsOption = [];
                var fyearsOption = [];
                $.ajax
                    ({
                        url: domain + 'CVBuilder/GetYearsMonthsList',
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
                            innerHtml += "<td><div class='cv-select'><select id='FromMonth_" + len + "' class='form-control' >" + monthsOption + "</select><select id='FromDate_" + len + "' class='form-control selectdateAddmore' >" + fyearsOption + "</select></div></td>";
                            //innerHtml += "<td><select id='FromDate_" + len + "' class='form-control'>" + yearsOption + "</select></td>";
                            innerHtml += "<td><div class='cv-select'><select id='ToMonth_" + len + "' class='form-control'>" + monthsOption + "</select><select id='ToDate_" + len + "' class='form-control selectdateAddmore' >" + yearsOption + "</select></div></td>";
                            //innerHtml += "<td><select id='ToDate_" + len + "' class='form-control'>" + yearsOption + "</select></td>";

                            
                            //innerHtml += "<td><input asp-for='FromDate_" + len + "' id='fromDate_" + len + "' type='text' class='form-control' placeholder='MM/YYYY'  readonly='readonly'/></td>";
                            //innerHtml += "<td><input asp-for='toDate_" + len + "' id='toDate_" + len + "' type='text' class='form-control' placeholder='MM/YYYY'  readonly='readonly'/></td>";
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

                            //$("#fromDate_" + len).datepicker({
                            //    dateFormat: "dd/mm/yy",
                            //    changeMonth: true,
                            //    changeYear: true,
                            //    yearRange: "-60:+0",
                            //    onClose: function (selectedDate) {
                            //        debugger;
                            //        var values = len - 1;
                            //        var lastDate = "";
                            //        if (values == 0) {
                            //            lastDate = $("#toDate").val();
                            //        }
                            //        else {
                            //            lastDate = $("#toDate_" + values).val();
                            //        }
                                    
                            //        $("#fromDate_" + len).datepicker("option", "minDate", lastDate);
                            //        $("#toDate_" + len).datepicker("option", "minDate", selectedDate);
                            //    }
                            //});
                            //$("#toDate_" + len).datepicker({
                            //    dateFormat: "dd/mm/yy",
                            //    changeMonth: true,
                            //    changeYear: true,
                            //    yearRange: "-60:+0",
                            //    onClose: function (selectedDate) {
                            //        $("#fromDate_" + len).datepicker("option", "maxDate", selectedDate);
                            //    }
                            //});                          
                           
                        },
                        error: function (ex) {
                            alert("Whooaaa! Something went wrong.." + ex);
                        },
                    });
                //var monthsOption = [];
                //var sel = document.querySelector("select");
                //for (var i = 0, n = sel.options.length; i < n; i++) { // looping over the options
                //    if (sel.options[i].value) //month.push(sel.options[i].value);
                //    monthsOption.push("<option value='" + sel.options[i].value + "'>" + sel.options[i].value + "</option>");
                //}
            });



            $(document).on('click', '.RemoveRow', function () {
                $(this).parent().parent().remove();
                RegenerateId();
            })
            function RegenerateId() {
                var tbl_len = $('#tbodyPreviousExperience tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    $($($('#tbodyPreviousExperience tr')[i]).children()[0]).text((i + 1));
                    $($($('#tbodyPreviousExperience tr')[i]).children()).find('input').attr('name', 'OrganizationName_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()).find('input').attr('id', 'OrganizationName_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[2]).find('input').attr('name', 'Designation_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[2]).find('input').attr('id', 'Designation_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[3]).find('select').attr('id', 'FromMonth_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[4]).find('select').attr('id', 'FromDate_' + i);
                    $($($('#tbodyPreviousExperience tr')[i]).children()[5]).find('select').attr('id', 'ToMonth_' + i);
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
                RegenerateId();
            })
            function RegenerateId() {
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
                //if ($('#ExperienceType').val() == '') {
                //    swal({
                //        title: "Alert!",
                //        text: "Experience is required.",
                //        icon: "error",
                //    });
                //}
                //else {
                //    SaveRecord();
                //}
                if (ValidateFloatValue()) {
                    
                    SaveRecord();
                }
                else {
                    swal({
                        title: "Alert!",
                        text: "Fill correct details in all field.",
                        icon: "error",
                    });
                }
            });


            function SaveRecord() {                
                // if (ValidateFloatValue()) {
                var IndustryData = new Array();
                var markedIndustryChk = document.getElementsByName('Industry');
                for (var checkbox of markedIndustryChk) {
                    if (checkbox.checked) {
                        IndustryData.push(checkbox.value);
                        //alert(checkbox.value);
                    }
                }
                var TechnologyData = new Array();
                var markedTechnologyChk = document.getElementsByName('Technology');
                for (var Tcheckbox of markedTechnologyChk) {
                    if (Tcheckbox.checked) {
                        TechnologyData.push(Tcheckbox.value);                      
                    }
                }

                var obj = new Object();
                obj.Id = $('#Id').val();
                obj.Title = CKEDITOR.instances['Title'].getData();
                obj.ProfileSummary = CKEDITOR.instances['ProfileSummary'].getData();
                obj.TechnicalSkills = CKEDITOR.instances['TechnicalSkills'].getData();
                obj.WorkExperience = CKEDITOR.instances['WorkExperience'].getData();
                //obj.RolesAcross = $('#RolesAcross').val();
                obj.RolesAcross = CKEDITOR.instances['RolesAcross'].getData();
                obj.Linkedin = $('#Linkedin').val();
                obj.Languages = $('#Languages').val();
                obj.ExperienceType = $('#ExperienceType').val();                
                obj.Industry = IndustryData;
                obj.Technology = TechnologyData;
                obj.OtherIndustry=$("#OtherIndustry").val();
                obj.OtherTechnology = $("#OtherTechnology").val()
                obj.OtherTechnologyParent=$("#OtherTechnologyParent").val();

                var data = new Array();
                var tbl_len = $('#tbodykra tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.Title = $('#Title_' + i).val();
                    //tempobj.KRAOrderno = $('#KRAOrderno_' + i).val();
                    data.push(tempobj);
                }
                obj.dataList = data;

                var dataEducation = new Array();
                var tbl_len = $('#tbodyEducation tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.Title = $('#EduTitle_' + i).val();
                    tempobj.University = $('#EduUniversity_' + i).val();
                    dataEducation.push(tempobj);
                }
                obj.Education = dataEducation;

                var dataCertifications = new Array();
                var tbl_len = $('#tbodyCertifications tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.Title = $('#CTitle_' + i).val();
                    tempobj.KRAOrderno = $('#KRAOrderno_' + i).val();
                    dataCertifications.push(tempobj);
                }
                obj.Certifications = dataCertifications;

                
                var datatPreviousExperience = new Array();
                var tbl_len = $('#tbodyPreviousExperience tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.OrganizationName = $('#OrganizationName_' + i).val();
                    tempobj.Designation = $('#Designation_' + i).val();
                    tempobj.FromMonth = $('#FromMonth_' + i + ' :selected').text();
                    tempobj.FromDate = $('#FromDate_' + i).val();
                    tempobj.ToMonth = $('#ToMonth_' + i + ' :selected').text();
                    tempobj.ToDate = $('#ToDate_' + i).val();
                    datatPreviousExperience.push(tempobj);
                }
                obj.PreviousExperience = datatPreviousExperience;

                var jsondata = JSON.stringify(obj);
                $.post('CVBuilder/SaveRecords', { jsondata: jsondata }, function (result) {

                    swal({
                        title: "Alert!",
                        text: "Record saved successfully.",
                        icon: "success",
                    }).then(function () {
                        window.location.href = 'CVBuilder/index';
                    });
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

           

            function ValidateFloatValue() {                
                if ($('#ExperienceType').val() == '') {
                    swal({
                        title: "Alert!",
                        text: "Experience is required.",
                        icon: "error",
                    });
                    return false;
                }
                
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
                //if (IndustryData == null || IndustryData.length == 0) {
                //    swal({
                //        title: "Alert!",
                //        text: "Industry is required.",
                //        icon: "error",
                //    });
                //    return false;
                //}
                //if (TechnologyData == null || TechnologyData.length == 0) {
                //    swal({
                //        title: "Alert!",
                //        text: "Technology Category is required.",
                //        icon: "error",
                //    });
                //    return false;
                //}                

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