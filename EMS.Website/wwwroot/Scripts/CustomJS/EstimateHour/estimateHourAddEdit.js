(function () {
    $(document).ready(function () {
        if ($('#EstimatedHours').val() == "0") {
            $('#EstimatedHours').val('');
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
                url: domain + 'EstimateHour/GetTechnologyByParent',
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
                    //$("#techIds").html(techList);
                    if (numberChecked > 0)
                        $("input:checkbox[name=Technology]:first").prop("required", true);
                }
            });
        } else {
            $(".parentTechId_" + technologyParentId).remove();
        }
    });
    $("#selectFileList").change(function () {
        $("#EstimateHourFileNameTypeId").val($("#selectFileList").val());
    });
    $("#modal-add-FileName").on("hidden.bs.modal", function (event) {
        var FileNameCount = $("#FileNameCount").val();
        var id = $("#modal-add-FileName").data("newFileNameId");
        var name = $("#modal-add-FileName").data("newFileName");
        $("#modal-add-FileName").removeData("newFileName");
        $("#modal-add-FileName").removeData("newFileNameId");
        if (id != 'null' && id != 0 && typeof id !== 'undefined') {
            var o = new Option(id, name);
            $(o).html(name);
            //$("#selectFileList").append($("<option></option>")
            //    .attr({ "value": id, "data-index": id })
            //    .text(name));
            //alert(id);
            //setTimeout($("#selectFileList").val(id).fSelect(),1000);
            $("#selectFileList").val(id);
            $("#FileNameList").find(".fs-options").append(`<div class="fs-option g0 selected" data-value="${id}" data-index="${id}"><span class="fs-checkbox"><i></i></span><div class="fs-option-label">${name}</div></div>`);
            var Fileoption = $('#FileNameList').find('.fs-option[data-value=' + id + ']');
            $('#FileNameList').find('.fs-label').text(Fileoption.find('.fs-option-label').text());

            $("#EstimateHourFileNameTypeId").val(id);
            
            FileNameCount++;
            $("#FileNameCount").val(FileNameCount);
        }
    });

    $("input:checkbox[name=IsFreeBie]").change(function () {
        if ($(this).is(":checked")) {
            $("#EstimatedHours").val('0');
            $("#EstimatedHours").attr('disabled', 'disabled');
        }
        else {
            $("#EstimatedHours").removeAttr('disabled');
            $("#EstimatedHours").val('');

        }
        });

        $('#Crmid').keypress(function (event) {
        if (event.which != 8 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault();
        }
    });

    $("#EstimatedHours").keydown(function (event) {
        if (event.shiftKey == true) {
            event.preventDefault();
        }

        if ((event.keyCode >= 48 && event.keyCode <= 57) ||
            (event.keyCode >= 96 && event.keyCode <= 105) ||
            event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
            event.keyCode == 39 || event.keyCode == 44 || event.keyCode == 46 || event.keyCode == 190) {
            return true;
        } else {
            event.preventDefault();
        }
    });

    $('#ConversionDate').datepicker({
        defaultDate: "+1w",
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        numberOfMonths: 1,
        //minDate: 0
    });

}(jQuery));