(function () {
    function Role() {
        var $this = this,formAddEdit;       
        var viewModel;
        $('#RoleCateGoryId   option:nth-child(2)').attr("selected", "selected");


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
                        $('#RoleId   option:nth-child(2)').attr("selected", "selected");
                        BindDesignationList();
                        //BindList();
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
                        $('#DesignationId   option:nth-child(2)').attr("selected", "selected");
                        BindList();
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

        function BindList() {
            $('.divoverlay').removeClass('hide');
            var roleId = $("#RoleId").val();
            var designationId = $("#DesignationId").val();
            $.ajax({
                //url: domain + 'Role/MenuAccess/' + roleId + designationId,
                url: domain + 'Role/MenuAccess/',
                data: { roleId: roleId, designationId: designationId},
                type: 'GET',
                datatype: 'application/json',
                success: function (result) {
                    $('.divoverlay').addClass('hide');
                    $(".menuaccuss_editable").empty().html(result);
                },
                error: function (ex) {
                    alert("Whooaaa! Something went wrong.." + ex);
                },
            });
        }




     

        //for rolecategory on change event to get Role

        $(document).on('change', '#RoleCateGoryId', function () {
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


        function InitializeForm() {
            formAddEdit = new Global.FormHelper($("#form1"), { updateTargetId: "validation-summary", validateSettings: { ignore: '' } },
            function beforeSubmit() {               
            }, function onSucccess(result) {             
                var $tackCommentMessageDiv = $('#RoleMessageDiv');
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
                }, 3000);
            });

            $(document).on('change', '#DesignationId', function () {
                BindList();
            });
        }


        $this.init = function () {
            BindRoleList();
           // BindDesignationList();
            /*BindList();*/           
            InitializeForm();
        }
    }

    $(function () {
        var self = new Role();
        self.init();
    })

}(jQuery))