(function () {
    function Role() {
        var $this = this, formAddEdit;
        var viewModel;
        function BindRoleList() {
            $("#RoleId").html("");
            $("#DesignationId").html("");
            var DDLItems = "";
            if ($('#RoleCateGoryId').val() != "") {
                $.get(domain + 'Role/BindRoleList', { RoleCateGoryId: $('#RoleCateGoryId').val() }, function (data) {
                    if (data != null) {
                        $.each(data.roleList, function (i, Rolecat) {
                            DDLItems += "<option value='" + Rolecat.value + "'>" + Rolecat.text + "</option>";
                        });
                        $("#RoleId").html(DDLItems);
                        // $('#RoleId   option:nth-child(2)').attr("selected", "selected");
                        BindDesignationList();

                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#RoleId").html(DDLItems);
                        $("#DesignationId").html(DDLItems);
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#RoleId").html(DDLItems);
                $("#DesignationId").html(DDLItems);
            }

        }

        function BindDesignationList() {
            $("#DesignationId").html("");
            var DDLItems = "";
            if ($('#RoleId').val() != "") {
                $.get(domain + 'Role/BindDesignationList', { RoleId: $('#RoleId').val() }, function (data) {
                    if (data != null) {
                        $.each(data.designationList, function (i, Designation) {
                            DDLItems += "<option value='" + Designation.value + "'>" + Designation.text + "</option>";
                        });
                        $("#DesignationId").html(DDLItems);
                        //$('#DesignationId   option:nth-child(2)').attr("selected", "selected");

                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#DesignationId").html(DDLItems);
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#DesignationId").html(DDLItems);
            }

        }


        //for rolecategory on change event to get Role

        $(document).on('change', '#RoleCateGoryId', function () {
            
            if ($('#RoleCateGoryId').val() > 0) {
                $('.validRoleCate').text('');
            }
            $("#RoleId").html("");
            $("#DesignationId").html("");
            var DDLItems = "";
            if ($('#RoleCateGoryId').val() != "") {
                $.get(domain + 'Role/BindRoleList', { RoleCateGoryId: $('#RoleCateGoryId').val() }, function (data) {
                    if (data != null) {
                        $.each(data.roleList, function (i, Rolecat) {
                            DDLItems += "<option value='" + Rolecat.value + "'>" + Rolecat.text + "</option>";
                        });
                        $("#RoleId").html(DDLItems);
                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#RoleId").html(DDLItems);
                        $("#DesignationId").html(DDLItems);
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#RoleId").html(DDLItems);
                $("#DesignationId").html(DDLItems);
            }
        });




        //for role on change event to get desi

        $(document).on('change', '#RoleId', function () {
            
            if ($('#RoleId').val() > 0) {
                $('.validRole').text('');
            }
            $("#DesignationId").html("");
            var DDLItems = "";
            if ($('#RoleId').val() != "") {
                $.get(domain + 'Role/BindDesignationList', { RoleId: $('#RoleId').val() }, function (data) {
                    if (data != null) {
                        $.each(data.designationList, function (i, Designation) {
                            DDLItems += "<option value='" + Designation.value + "'>" + Designation.text + "</option>";
                        });
                        $("#DesignationId").html(DDLItems);
                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#DesignationId").html(DDLItems);
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#DesignationId").html(DDLItems);
            }
        });


        //for role on change event to get desi

        $(document).on('change', '#DesignationId', function () {
            
            if ($('#DesignationId').val() > 0) {
                $('.validDesi').text('');
            }
            $.get(domain + 'KRA/GetKRAData', { DesignationId: $('#DesignationId').val() }, function (data) {
                if (data != null) {
                    $('#tbodykra').html('');
                    var innerHtml = "";
                    var len = $('#tbodykra tr').length;
                    for (var i = 0; i < data.dataList.length; i++) {
                        innerHtml = "<tr id='trlen_" + len + "'>";
                        innerHtml += "<td> " + (len + 1) + "</td>";
                        innerHtml += "<td><input type='text' name='Title_" + len + "' value='" + data.dataList[i].title + "' class='form-control' id='Title_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td > ";

                        innerHtml += "<td><input type='number' name='KRAOrderno_" + len + "' value='" + data.dataList[i].kraOrderno + "'  class='form-control' id='KRAOrderno_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";                       

                        if (len == 0) {
                            /*innerHtml += "<td><a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td> ";*/

                            innerHtml += "<td><a class='btn' id='addmore' href='javascript: void (0);'>Add More</a></td>";
                        }
                        //else if (len-1) {
                        //    innerHtml += "<td><a class='btn' id='addmore' href='javascript: void (0);'>Add More</a></td>";
                        //}
                        else {
                            innerHtml += "<td><a href='javascript:void (0);' class='RemoveRow'><i class='text-danger fa fa-trash'></i></a></td> ";
                        }
                        innerHtml += "</tr>"
                        $('#tbodykra').append(innerHtml);
                        len++;

                    }
                }
                else {

                    $('#tbodykra').html('');
                    var innerHtml = "";
                    var len = $('#tbodykra tr').length;
                    innerHtml = "<tr id='trlen_" + len + "'>";
                    innerHtml += "<td> " + (len + 1) + "</td>";
                    innerHtml += "<td><input type='text' name='Title_" + len + "' class='form-control' id='Title_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td > ";

                    innerHtml += "<td><input type='number' name='KRAOrderno_" + len + "'  class='form-control' id='KRAOrderno_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td >";
                    innerHtml += "<td><a class='btn' id='addmore' href='javascript: void (0);'>Add More</a></td>";
                    innerHtml += "</tr>"
                    $('#tbodykra').append(innerHtml);
                }
            });

        });





        $(document).on('click', '#addmore', function () {
          
            var innerHtml = "";
            var len = $('#tbodykra tr').length;
            innerHtml = "<tr id='trlen_" + len + "'>";
            innerHtml += "<td> " + (len + 1) + "</td><td><input type='text' name='Title_" + len + "' class='form-control' id='Title_" + len + "'/><span asp-validation-for='Value' class='text-danger' id='validt_" + len + "'></span></td>";
            innerHtml += "<td><input type='number' name='KRAOrderno_" + len + "' class='form-control' id='KRAOrderno_" + len + "' /><span asp-validation-for='Value' class='text-danger' id='valido_" + len + "'></span></td>";
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
                $($($('#tbodykra tr')[i]).children()[2]).find('input').attr('name', 'KRAOrderno_' + i);
                $($($('#tbodykra tr')[i]).children()[2]).find('input').attr('id', 'KRAOrderno_' + i);


            }
        }

        $(document).on('click', '#btn-submit', function () {
            SaveRecord();

        })


        function SaveRecord() {

            if (ValidateFloatValue()) {

                var obj = new Object();
                obj.RoleCateGoryId = $('#RoleCateGoryId').val();
                obj.RoleId = $('#RoleId').val();
                obj.DesignationId = $('#DesignationId').val();

                var data = new Array();
                var tbl_len = $('#tbodykra tr').length;
                for (var i = 0; i < tbl_len; i++) {
                    var tempobj = new Object();
                    tempobj.Title = $('#Title_' + i).val();
                    tempobj.KRAOrderno = $('#KRAOrderno_' + i).val();
                    data.push(tempobj);
                }
                obj.dataList = data;
                var jsondata = JSON.stringify(obj);
                $.post('KRA/SaveRecords', { jsondata: jsondata }, function (result) {
                    
                    var $tackCommentMessageDiv = $('#KRAMessageDiv');
                    if (result.isSuccess) {
                        if (Global.IsNotNullOrEmptyString(result.message)) {
                            $tackCommentMessageDiv.addClass('alert-success').removeClass('alert-danger');
                            $tackCommentMessageDiv.empty().html(result.message);
                        }
                    }
                    else {
                        if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                            $tackCommentMessageDiv.addClass('alert-danger').removeClass('alert-success');
                            $tackCommentMessageDiv.empty().html(result.errorMessage);
                        }
                    }
                    $tackCommentMessageDiv.show();
                    window.setTimeout(function () {
                        $tackCommentMessageDiv.html('');
                        $tackCommentMessageDiv.hide();
                        window.location.href = window.location.href;
                    }, 3000);

                });
            }
        }



        function ValidateFloatValue() {
           
            if ($('#RoleCateGoryId').val() == '') {
                $('.validRoleCate').text('* Required');
            }
            else {
                $('.validRoleCate').text('');
            }
            if ($('#RoleId').val() == '') {
                $('.validRole').text('* Required');
            }
            else {
                $('.validRole').text('');
            }
            if ($('#DesignationId').val() == '') {
                $('.validDesi').text('* Required');
            }
            else {
                $('.validDesi').text('');
            }
            var c = 0, b = 0;
            var tbl_len = $('#tbodykra tr').length;
            for (var i = 0; i < tbl_len; i++) {
                if ($('#Title_' + i).val() == "") {
                    $('#validt_' + i).text('The Value field is required.')
                    c = c + 1;
                }
                else {
                    $('#validt_' + i).text('')
                }
                if ($('#KRAOrderno_' + i).val() == "") {
                    $('#valido_' + i).text('The Value field is required.')
                    b = b + 1;
                }
                else {
                    $('#valido_' + i).text('')
                }
            }
            if (c != 0 || b != 0) {
                return false;
            }

            return true;


        }


        function InitializeForm() {

            formAddEdit = new Global.FormHelper($("#form1"), { updateTargetId: "validation-summary", validateSettings: { ignore: '' } },
                function beforeSubmit() {


                }, function onSucccess(result) {

                    var $tackCommentMessageDiv = $('#KRAMessageDiv');
                    if (Global.IsNotNullOrEmptyString(result.message)) {
                        $tackCommentMessageDiv.addClass('alert-success').removeClass('alert-danger');
                        $tackCommentMessageDiv.empty().html(result.message);
                    }
                    else if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                        $tackCommentMessageDiv.addClass('alert-danger').removeClass('alert-success');
                        $tackCommentMessageDiv.empty().html(result.errorMessage);
                    }
                    $tackCommentMessageDiv.show();
                    window.setTimeout(function () {
                        $tackCommentMessageDiv.html('');
                        $tackCommentMessageDiv.hide();
                        window.location.href = window.location.href;
                    }, 3000);


                });


        }


        $this.init = function () {
            BindRoleList();
            InitializeForm();
        }
    }

    $(function () {
        var self = new Role();
        self.init();
    })

}(jQuery))