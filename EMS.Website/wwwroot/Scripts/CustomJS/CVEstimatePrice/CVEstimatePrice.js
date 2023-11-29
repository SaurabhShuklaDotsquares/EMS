(function () {
    function Role() {
        var $this = this, formAddEdit;
        var viewModel;
        function BindRoleList() {
            $("#RoleId").html("");
            var DDLItems = "";
            if ($('#RoleCateGoryId').val() != "") {
                $.get(domain + 'Role/BindRoleList', { RoleCateGoryId: $('#RoleCateGoryId').val() }, function (data) {
                    if (data != null) {
                        $.each(data.roleList, function (i, Rolecat) {
                            DDLItems += "<option value='" + Rolecat.value + "'>" + Rolecat.text + "</option>";
                        });
                        $("#RoleId").html(DDLItems);
                        // $('#RoleId   option:nth-child(2)').attr("selected", "selected");

                    }
                    else {
                        DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                        $("#RoleId").html(DDLItems);
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#RoleId").html(DDLItems);
            }

        }
        //for rolecategory on change event to get Role

        $(document).on('change', '#RoleCateGoryId', function () {
            if ($('#RoleCateGoryId').val() != 1) {
                $(".techno").hide();
            }
            if ($('#RoleCateGoryId').val() > 0) {
                $('.validRoleCate').text('');
                $('#formcvmanageprice #RoleCateGoryId').valid();
            }
            
            $('#EntryLevelPrice').val('')
            $('#OneToTwoPrice').val('')
            $('#ThreeToSixPrice').val('')
            $('#SixToTenPrice').val('')
            $('#TenPlusPrice').val('')
            $("#RoleId").html("");
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
                    }
                });
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Select-" + "</option>";
                $("#RoleId").html(DDLItems);
            }
        });

        
        $(document).off('change', '#RoleId').on('change', '#RoleId', function () {
            if ($('#RoleId').val() > 0) {
                $('.validRole').text('');
                $('#formcvmanageprice #RoleId').valid();
            }
            if ($('#RoleId').val() == 25) {
                $(".techno").show();
                $('#TechnologyId').attr('required', 'required')
            }
            else {
                $(".techno").hide();
                $('#TechnologyId').removeAttr('required')
            }
            roleid = $('#RoleId').val()
            techid = ""

            $.get(domain + "CvManagePrice/EstimatePrice?RoleId=" + roleid + "&TechId=" + techid, function (data, status) {
                if (data != null && data != undefined && data != "") {
                    $("#container-result").html(data)
                }
            });
            
        });

        
            $(document).off('change', '#TechnologyId').on('change', '#TechnologyId', function () {
            if ($('#TechnologyId').val() > 0) {
                $('.validRoleTech').text('');
                $('#formcvmanageprice #TechnologyId').valid();
            }
            roleid = $('#RoleId').val()
            techid = $('#TechnologyId').val()

            $.get(domain + "CvManagePrice/EstimatePrice?RoleId=" + roleid + "&TechId=" + techid, function (data, status) {
                if (data != null && data != undefined && data != "") {
                    $("#container-result").html(data)
                }
            });
            

        });


        $(document).on('click', '#btn-submits', function () {
            if (!$('form').valid()) {
                return false;
            }
            SaveRecord();
            if (ValidateFloatValue()) {
                var data = $("#formcvmanageprice").serialize();
                $.ajax({
                    url: domain + 'CvManagePrice/SaveRecords',
                    type: 'POST',
                    data: data,
                    success: function (data) {
                        window.location.href = data.redirectUrl;
                    }
                });
            }

        })


        function SaveRecord() {

            if (ValidateFloatValue()) {

                var obj = new Object();
                obj.RoleCateGoryId = $('#RoleCateGoryId').val();
                obj.RoleId = $('#RoleId').val();
                obj.TechnologyId = $('#TechnologyId').val();
               
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
            if ($('#TechnologyId').val() == '') {
                $('.validRoleTech').text('* Required');
            }
            else {
                $('.validRoleTech').text('');
            }
            return true;


        }

        function InitializeForm() {

            formAddEdit = new Global.FormHelper($("#formcvmanageprice"), { updateTargetId: "validation-summary", validateSettings: { ignore: '' } },
                function beforeSubmit() {


                }, function onSucccess(result) {

                    var $tackCommentMessageDiv = $('#CVEstimatePriceMessageDiv');
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