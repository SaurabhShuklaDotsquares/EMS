
base = {};
enumUtil = {};
base.appPath = "";
enumUtil.MessageType = {
    Success: 0,
    Error: 1,
    Info: 2,
    Warning: 3,
    InvalidPassword: 4

}
base.haseValue = function (p) {
    if (p == '' || p == undefined || p == null || p == '0' || p == 'undefined') {
        return false;
    }
    else {
        return true;
    }
}
base.getUrl = function (controllerName, actionName, id) {
    return base.appPath + '/' + controllerName + '/' + actionName + (id == undefined || id == null ? '' : ('/' + id));
}
base.ClearOption = function (p) {
    if (base.haseValue(p.ctrlId)) {
        if (base.haseValue(p.caption)) {
            var ctrlId = p.ctrlId.split(',');
            var captions = p.caption.split(',');
            for (var i = 0; i < ctrlId.length; i++) {
                $(ctrlId[i]).empty();

                var caption = base.haseValue(p.caption) ? captions[i] : 'Please Select'
                $(ctrlId[i]).children().remove();
                $(ctrlId[i]).append($("<option>").val("").text(caption));
            }
        }

    }
}
base.Cascading = function (p) {
    var ctrlId = p.ctrlId;
    var formId = p.formId;
    var fullctrlId = base.haseValue(formId) ? ('#' + formId + ' #' + ctrlId) : '#' + ctrlId;
    var optionalIds = p.optionalIds;
    var clearId = p.clearId;
    var controllerName = p.controllerName;
    var actionName = p.actionName;
    var parmVal = p.parmVal;
    var SelectedValue = p.SelectedValue;
    var ismultiSelect = base.haseValue(p.ismultiSelect) ? p.ismultiSelect : false;
    var ctrlcaption = base.haseValue(p.ctrlCaption) ? p.ctrlCaption : 'Please Select'
    var clearCaption = base.haseValue(p.clearCaption) ? p.clearCaption : 'Please Select'
    if (parmVal != "") {
        // Do something after 1 second 

        $.ajaxSetup({ async: false })
        $.get(base.getUrl(controllerName, actionName), { id: parmVal }, function (data) {
            switch (data.MessageType) {
                case enumUtil.MessageType.Success:
                    !(ismultiSelect) ? base.ClearOption({ ctrlId: fullctrlId, caption: ctrlcaption }) : $(fullctrlId).empty();
                    if (base.haseValue(clearCaption)) {
                        base.EmptyOption({ ctrlId: clearId, caption: clearCaption })
                    }
                    if (base.haseValue(optionalIds)) {

                        var optionalIdsInfo = optionalIds.split(',');

                        for (var i = 0; i <= optionalIdsInfo.length; i++) {
                            var optionalId = base.haseValue(formId) ? ('#' + formId + ' #' + optionalIdsInfo[i]) : '#' + optionalIdsInfo[i];
                            $(optionalId).empty();
                            $(optionalId).append($("<option>").val('').text('Please Select'));
                            $.each(data.Object, function () {
                                $(optionalId).append($("<option>").val(this.Value).text(this.Text));
                            });
                        }

                    }

                    $.each(data.Object, function () {
                        $(fullctrlId).append($("<option>").val(this.Value).text(this.Text));
                    });
                    if (base.haseValue(SelectedValue)) {
                        $(fullctrlId).val(SelectedValue).trigger('change');
                    }
                    break;
                case enumUtil.MessageType.Error:
                    alert(data.message)
                    break;
                default:
            }
        });

    }
    else {
        base.EmptyOption({ ctrlId: clearId, caption: clearCaption })
    }
    //  base.ClearOption({ ctrlId: clearId, caption: p.clearCaption })
}


function GetDocTypeDDL() {
    base.Cascading({
        ctrlId: '',
        formId: '',
        controllerName: '',
        actionName: '',
        parmVal: '',


    })
}
function AppendDDL(p) {
    selectedDedType = []
    var ctrlId = p.ctrlId;
    var options = p.options;
    var excludeIds = p.excludeIds
    var tNo = p.tNo
    var TypeIds = p.TypeIds;
    var currentIds = p.currentIds;
    $('#Tbody_' + tNo + ' tr').each(function () {
        var Id = $(this).attr('id').replace('tr_', '')
        selectedDedType.push($('#DeductinTypeId_' + Id).val())

    })
    var firstDdl = $('#Tbody_' + tNo + ' tr').find("select")[0]
    var firstDdlId = $(firstDdl).attr('id')
    $('#' + firstDdlId + ' option').each(function () {
        if (!(selectedDedType.indexOf($(this).val()) > -1)) {
            $('#' + ctrlId).append($("<option>").val($(this).val()).text($(this).text()));

        }
    }
    );

}
function AddRow(p, tNo) {
    var len = $('#Tbody_' + tNo + ' tr').length;
    var innerHtml = "";
    var currentIds = len + 1;
    var TypeId = p;
    if (CheckValidate({ len: len, typeId: p })) {
        var trIds = 'tr_' + p + "_" + currentIds;
        innerHtml = "<tr id='tr_" + p + "_" + currentIds + "' style='height:80px;'>";
        innerHtml += "<td> " + (len + 1) + "</td><td><select  onchange = 'Validate(this)' class='form-control ddlDeductinType' data-val-required=' * required' id='DeductinTypeId_" + p + "_" + currentIds + "' name='DeductinTypeId_" + p + "_" + currentIds + "'><option value=''>Please Select</option></td>";
        innerHtml += "<td><input style='" + backColorEMR +"' "+EMPreadOnly+" "+EMRequired+" type='number' oninput='validateNumber(this);' name='ClaimedByEmployee_" + p + "_" + currentIds + "' class='form-control' id='ClaimedByEmployee_" + p + "_" + currentIds + "'></td>";
        innerHtml += "<td><input style='" + backColorAC + "' " + AcreadOnly + " " + AcRequired +" type='number'oninput='validateNumber(this);' name='GivenByEmployer_" + p + "_" + currentIds + "' id='GivenByEmployer_" + p + "_" + currentIds + "' class='form-control'></td>";
        innerHtml += "<td><input style='" + backColorEMR + "' " + EMPreadOnly + " " + EMRequired +" type='text' name='EmployeeRemark_" + p + "_" + currentIds + "' class='form-control' id='EmployeeRemark_" + p + "_" + currentIds + "'></td>";
        innerHtml += "<td><input style='" + backColorAC + "' " + AcreadOnly + " " + AcRequired + " type='text' name='EmployerRemark_" + p + "_" + currentIds + "' class='form-control' id='EmployerRemark_" + p + "_" + currentIds + "'></td>";
        if (TypeId == 3 || TypeId == 4) {
            innerHtml += "<td><input style='" + backColorEMR + "' " + EMPreadOnly + " " + EMRequired + " type='text' name='EmployeePanNumber_" + p + "_" + currentIds + "' class='form-control' id='EmployeePanNumber_" + p + "_" + currentIds + "'><span style = 'color: red; font-weight: bold; font-size: smaller;' id='EmployeePanNumberValidate_" + p + "_" + currentIds + "' name = 'EmployeePanNumberValidate_" + p + "_" + currentIds + "' ></span ></td > ";
        }
        else {
            innerHtml += "<td></td>";
        }
        innerHtml += "<td><input multiple type='file' " + FileRequired+" name='file_" + p + "_" + currentIds + "' class='form-control' id='file_" + p + "_" + currentIds + "'><span style = 'color: red; font-weight: bold; font-size: smaller;' id='EmployeePanNumberValidateBrowse_" + p + "_" + currentIds + "' name = 'EmployeePanNumberValidateBrowse_" + p + "_" + currentIds + "' ></span ></td>";
        innerHtml += "<td style='width: 5pc;'> <a href='javascript:void (0);'style='font-size: 18px;' class='RemoveRow' onclik ='RemoveRow(this)'><i onclik ='RemoveRow(this)' class='text-danger fa fa-trash'></i></a></td> ";
        innerHtml += "</tr>"
        $('#Tbody_' + tNo).append(innerHtml);
        AppendDDL({ ctrlId: 'DeductinTypeId_' + p + '_' + currentIds, tNo: tNo, TypeIds: p, currentIds: currentIds })

    }

}

function CheckValidate(p) {
    
    var len = p.len;
    var typeId = p.typeId
    var optionLength = $('#DeductinTypeId_' + typeId + '_' + 1 + ' option').length
    var ret = true;
    if ((optionLength-1) == len) {
        ret=  false
    }
    var currentSelect = base.haseValue($('#DeductinTypeId_' + typeId + '_' + len).val())
    if (!currentSelect) {
        //$('#DeductinTypeId_' + typeId + '_' + len).attr("style", "border:1px solid red !important");
        alertify.error('Please select DEDUCTIONTYPE.');
    }
    return ($('#tr_' + typeId + '_' + len + ' select').valid() && $('#tr_' + typeId + '_' + len + ' input').valid()) && ret && currentSelect
}
 
$(document).on('click', '.RemoveRow', function () {
    var trId = $(this).parent().parent().attr('id').split('_')[1]
    $(this).parent().parent().remove();
    RegenerateId(trId);
})

$(document).off('click', '.btnRemoveRow').on('click', '.btnRemoveRow', function () {
    
    var Id = $(this).attr('data-id');

    if (Id > 0) {
        $.ajax({
            url: "empInvestment/DeleteInvestment",
            method: 'post',
            data: { Id: Id },
            success: function (data) {
                console.log(data);
                if (data.isSuccess) {
                    alertify.dismissAll();
                    alertify.success(data.data);
                }
                else {
                    alertify.dismissAll();
                    alertify.error(data.data);
                    return false;
                }
            }
        });
    }
    else {
        alertify.error('some error occurred.');
    }

});

$(document).off('change', '.ddlDeductinType').on('change', '.ddlDeductinType', function () {

    
    var ddlDeductinTypeVal = $(this).val()
    var Id = $(this).attr('id');
    if (ddlDeductinTypeVal != "") {


        $("#ClaimedByEmployee_" + Id.split(',')[1] + "_" + Id.split(',')[2] + "").attr('required', 'required');
        $("#EmployeeRemark_" + Id.split(',')[1] + "_" + Id.split(',')[2] + "").attr('required', 'required');



        if (ddlDeductinTypeVal == 22 || ddlDeductinTypeVal == 23 || ddlDeductinTypeVal == 24) {
            if ($("#EmployeePanNumber_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").val() == "") {
                $("#EmployeePanNumberValidate_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("Require PAN Number")
            }
            
            $("#EmployeePanNumber_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").attr('required', 'required')
            $("#EmployeePanNumberValidateBrowse_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("Require File")
            $("#file_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").attr('required', 'required')
        }
        else {
            $("#EmployeePanNumberValidate_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("")
            $("#EmployeePanNumber_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").removeAttr('required', 'required')
            $("#EmployeePanNumberValidateBrowse_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("")
            $("#file_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").removeAttr('required', 'required')
        }
    }
    else {
        $("#EmployeePanNumberValidate_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("")
        $("#EmployeePanNumber_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").removeAttr('required', 'required')
        $("#EmployeePanNumberValidateBrowse_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("")
        $("#file_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").removeAttr('required', 'required')
        $("#ClaimedByEmployee_" + Id.split(',')[1] + "_" + Id.split(',')[2] + "").removeAttr('required', 'required');
        $("#EmployeeRemark_" + Id.split(',')[1] + "_" + Id.split(',')[2] + "").removeAttr('required', 'required');
    }
});

$(document).off('change', '.CheckPannumber').on('change', '.CheckPannumber', function () {
    //debugger;
    var PanNumber = $(this).val()
    var Id = $(this).attr('id')
    var regex = /([A-Z]){5}([0-9]){4}([A-Z]){1}$/;

    if (regex.test(PanNumber.toUpperCase())) {
        $("#EmployeePanNumberValidate_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("")
        return true;
    } else {
        alertify.error('Enter valid pan number');
        $("#EmployeePanNumberValidate_" + Id.split('_')[1] + "_" + Id.split('_')[2] + "").text("Require PAN Number")
        return false;
    }
});

$(document).off('click', '#btn-submit-tdsfrm').on('click', '#btn-submit-tdsfrm', function () {
    
    if (!$('form').valid()) {
        return false;
    }
    else
    {
        var form = $('#frmTDS')[0];
        var data = new FormData($('form#frmTDS')[0]);
        $('.loading-common,.loading-overlay').show()
        $.ajax({
            method: 'post',
            data: data,
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (data) {
                console.log(data);
                if (data.isSuccess) {
                    alertify.dismissAll();
                    //alertify.success(data.message);
                    setTimeout(function () {
                        window.location.href = "empInvestment/index";
                    }, 6000);

                }
                else {
                    alertify.dismissAll();
                    alertify.error(data.message);
                    if (data.data != '' && data.data != null) {
                        if (data.data == 'filetxt') {
                            $('#filetxtvalidate').text(data.message);
                        }
                        else {
                            $('#EmployeePanNumberValidateBrowse_' + data.data.split('_')[1] + '_' + data.data.split('_')[2] + '').text(data.message);
                        }
                    }
                    $('.loading-common,.loading-overlay').hide()
                    return false;
                }
            }
        });

    }
});



$(document).off('change', '#btnLockUnlock').on('change', '#btnLockUnlock', function (e) {
    
    var IsLockUnlock = $("#btnLockUnlock").is(":checked")
    var d = IsLockUnlock == true ? 'Unlock.': 'Lock.';
    var CnfrmMsg = confirm('Are you sure, You would like to' + " " + (IsLockUnlock == true ? 'Unlock' : 'Lock') + " this record.")
    var Id = $("#hdnUID").val()
    var AssesmentYearId = $("#hdnAssesmentYearId").val()
    
    if (IsLockUnlock) {
        IsLockUnlock = false;
    }
    else {
        IsLockUnlock = true;
    }
    e.preventDefault();
    if (CnfrmMsg) {
        $.ajax({
            url: "empInvestment/UpdateLockUnlock",
            type: "Post",
            data: { Uid: Id, LockUnlock: IsLockUnlock, AssesmentYearId: AssesmentYearId },
            success: function (data) {
                
                if (data.value) {
                    
                    if (!IsLockUnlock) {
                        $("#btnLockUnlocktxt").text('Click here to lock')
                        $("#btnLockUnlocktxt").css("font-weight", "bold")
                        $("#btnLockUnlocktxt").css("font-size", "smaller")
                    }
                    else {
                        $("#btnLockUnlocktxt").text('Click here to unlock')
                        $("#btnLockUnlocktxt").css("font-weight", "bold")
                        $("#btnLockUnlocktxt").css("font-size", "smaller")
                    }
                    var Message = (!IsLockUnlock) ? "Investment Detail UnLock Successfully." : "Investment Detail Lock Successfully.";
                    alertify.success(Message);
                }
                else {
                    alertify.error('Some error.');
                }
            }
        })
    }
    else {
        $('.switchBox').each(function (index, element) {
            if ($(element).data('bootstrapSwitch')) {
                $(element).off('switch-change');
                $(element).bootstrapSwitch('destroy');
            }

            $(element).bootstrapSwitch()
                .on('switch-change', function () {
                    //e.preventDefault();
                    var switchElement = this;
                    if ($("#btnLockUnlock").prop('checked') == true) {
                        $('#btnLockUnlock').val(true)
                    }
                    else {
                        $('#btnLockUnlock').val(false)
                    }

                });
        });

        
        $('.switch-left').addClass('fa fa-lock')
        $('.switch-left').text('')
        $('.switch-left').css('font-size', '20px');
        $('.switch-left').css('padding', '6px');
        $('.switch-right').addClass('fa fa-unlock');
        $('.switch-right').text('');
        $('.switch-right').css('font-size', '20px');
        $('.switch-right').css('padding', '6px');
        $('.has-switch').css('float', 'right');
        $('.has-switch').css('margin-right', '20px');
        $('.has-switch').css('position', 'absolute');
    }
    
   

});

function RegenerateId(tbdno) {
    var tbdyId = '#Tbody_' + tbdno;
    var tbl_len = $(tbdyId+' tr').length;
    for (var i = 1; i <= tbl_len; i++) {
        $($($(tbdyId+' tr')[i]).children()[0]).text((i + 1));
        $($(tbdyId + ' tr')[i-1]).children().find('select').each(function () {

            $(this).attr('id', 'DeductinTypeId_' + tbdno + '_' + i)
            $(this).attr('name', 'DeductinTypeId_' + tbdno + '_' + i)
        })
        $($(tbdyId + ' tr')[i-1]).children().find('input').each(function () {

            var currentIds = $(this).attr('id');
            var currentIndInfo = currentIds.split('_')
            currentIndInfo[2] = i;
            $(this).attr('id', currentIndInfo.join('_'))
            $(this).attr('name', currentIndInfo.join('_'))
        })

    }

}
function Validate(p) {
    var ids = $(p).attr('id');
    var val = $(p).val();
    var idInfo = ids.split('_');
    var typeID = idInfo[1];
    var len = idInfo[2];
    if (base.haseValue(val)) {

        
        $('#file_' + typeID + '_' + len).attr('required', 'required')
        if (isAcountUser=='False') {
            $('#GivenByEmployer_' + typeID + '_' + len).attr('readonly', 'readonly')
            $('#EmployerRemark_' + typeID + '_' + len).attr('readonly', 'readonly')
            $('#ClaimedByEmployee_' + typeID + '_' + len).attr('required', 'required')
            $('#EmployeeRemark_' + typeID + '_' + len).attr('required', 'required')
        }
        else {
            //$('#GivenByEmployer_' + typeID + '_' + len).attr('required', 'required')
            //$('#EmployerRemark_' + typeID + '_' + len).attr('required', 'required')
            //$('#ClaimedByEmployee_' + typeID + '_' + len).attr('readonly', 'readonly')
            //$('#EmployeeRemark_' + typeID + '_' + len).attr('readonly', 'readonly')
            $('#ClaimedByEmployee_' + typeID + '_' + len).attr('required', 'required')
            $('#EmployeeRemark_' + typeID + '_' + len).attr('required', 'required')
        }

    }
    else {
        $('#ClaimedByEmployee_' + typeID + '_' + len).removeAttr('required')
        $('#GivenByEmployer_' + typeID + '_' + len).removeAttr('required')
        $('#EmployeeRemark_' + typeID + '_' + len).removeAttr('required')
        $('#EmployerRemark_' + typeID + '_' + len).removeAttr('required')
        $('#file_' + typeID + '_' + len).removeAttr('required')
    }
    return true;
}

 

function validateNumber(elem) {
    var validNumber = new RegExp(/^\d*\.?\d*$/);
    var lastValid = $(elem).val();
    if (validNumber.test(elem.value)) {
        return true;
    } else {
        $(elem).val(null)
    }
}
$("#modal-delete-Doc").on('loaded.bs.modal', function () {

    var modal = $("#modal-delete-Doc");
    var form  = new Global.FormHelper($(this).find("form"),
        {
            updateTargetId: "validation-summary",
            validateSettings: { ignore: '' }
        }, null, function (result) {
            if (result.isSuccess) {
                $('#li_' + result.data).remove();
                modal.modal('hide');
                Global.ShowMessage(result.message, true, 'MessageDiv');
            }
            else {
                modal.modal('hide');
                Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
            }
        });
}).on('hidden.bs.modal', function () {
   // window.location.href = true;
    $(this).removeData('bs.modal');
    $(this).find('.modal-content').empty();
});
$("#modal-investment-mail").on('loaded.bs.modal', function () {
    
    var modal = $("#modal-investment-mail");
    var form = new Global.FormHelper($(this).find("form"),
        {
            updateTargetId: "validation-summary",
            validateSettings: { ignore: '' }
        }, null, function (result) {
            if (result.value) {
               
                modal.modal('hide');
                alertify.success('Mail send successfully.');
                //Global.ShowMessage(result.message, true, 'MessageDiv');
            }
            else {
                modal.modal('hide');
                //alertify.success(result.value || result.value || result, false, 'MessageDiv');
                alertify.error('some error occurred.');
            }
        });
}).on('hidden.bs.modal', function () {
    // window.location.href = true;
    $(this).removeData('bs.modal');
    $(this).find('.modal-content').empty();
});

$('#modal-edit-TDS')
    .on('hidden.bs.modal', function () {
        window.location.reload();
    });



$(document).ready(function (e) {

    
    $('.switchBox').each(function (index, element) {
        if ($(element).data('bootstrapSwitch')) {
            $(element).off('switch-change');
            $(element).bootstrapSwitch('destroy');
        }

        $(element).bootstrapSwitch()
            .on('switch-change', function () {
                e.preventDefault();
                var switchElement = this;
                if ($("#btnLockUnlock").prop('checked') == true) {
                    $('#btnLockUnlock').val(true)
                }
                else {
                    $('#btnLockUnlock').val(false)
                }

            });
    });

    $('.switch-left').addClass('fa fa-lock')
    $('.switch-left').text('')
    $('.switch-left').css('font-size', '20px');
    $('.switch-left').css('padding', '6px');
    $('.switch-right').addClass('fa fa-unlock');
    $('.switch-right').text('');
    $('.switch-right').css('font-size', '20px');
    $('.switch-right').css('padding', '6px');
    $('.has-switch').css('float', 'right');
    $('.has-switch').css('margin-right', '20px');
    $('.has-switch').css('position', 'absolute');
});

