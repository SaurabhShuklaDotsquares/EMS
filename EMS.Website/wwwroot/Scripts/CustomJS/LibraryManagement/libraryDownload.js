
(function ($) {

    $(function () {

        $(".chosen").chosen();

        if (roleid != null && roleid != "" && roleid != "0") {
            $("#RoleId").val(roleid).change();
        }
        if (userid != null && userid != "") {
            $("#UserLoginId").val(userid).change();
            userid = null;
            roleid = null;
        }
    });

}(jQuery));
$("#RoleId").change(function () {

    if ($(this).val() != "") {
        //GetRoleUsers($(this).val());

        $("#UserLoginId").val('').trigger('chosen:updated');;

        if (userid == null || userid == "") {
            GetRoleDownloadPermission($(this).val(), null);
        }
    } else {
        $(':input').val('');

        //$("#UserLoginId option").remove();
        //$(".users").find(".fs-options .fs-option").remove();
        //$(".users").find(".fs-label").text('--select--');
    }
});

$("#UserLoginId").change(function () {

    if ($(this).val() != "") {
        $("#RoleId").val("")
        GetRoleDownloadPermission(null, $(this).val());
    } else {
        GetRoleDownloadPermission($("#RoleId").val(), null);
    }
});

function GetRoleUsers(id) {

    $.ajax({
        url: domain + 'LibraryManagement/GetRoleUsers',
        type: 'POST',
        data: { roleId: id },
        success: function (data) {

            var result = JSON.parse(data);
            var select = '<option value="">--Select--</option>';
            var divOptions = '';
            $.each(result.data, function (index, item) {
                select += "<option value=" + item.Uid + ">" + item.Name + "</option>";
                divOptions += `<div class="fs-option g0" data-value="${item.Uid}" data-index="${index}"><span class="fs-checkbox"><i></i></span><div class="fs-option-label">${item.Name}</div></div>`;
            });
            $("#UserLoginId").html(select);
            $(".users").find(".fs-options").append(divOptions);
            if (userid != null) {
                $("#UserLoginId").val(userid).change();
                userid = null;
                roleid = null;
            }
        }
    });
}

function GetRoleDownloadPermission(roleId, userId) {

    console.log(roleId, userId);
    $.ajax({
        url: domain + 'LibraryManagement/GetRoleDownloadPermission',
        type: 'POST',
        data: { roleId: roleId, userId: userId },
        success: function (data) {
            var result = JSON.parse(data);
            if (result.data.length > 0) {
                $.each(result.data, function (index, item) {
                    $("#type" + item.LibraryFileTypeId).val(item.MaximumDownloadInDay);
                    $("#typeMonth" + item.LibraryFileTypeId).val(item.MaximumDownloadInMonth);
                });
            } else {
                $(".dayType").val("");
                $(".monthType").val("");
            }
        }
    });
}