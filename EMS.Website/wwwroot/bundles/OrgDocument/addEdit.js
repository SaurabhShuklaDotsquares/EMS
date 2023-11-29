(function () {
    function AddEdit() {
        var $this = this, form;

        function InitializeForm() {

            form = new Global.FormHelperWithFiles($("form#orgdocumentForm"), {
                updateTargetId: "validation-summary", validateSettings: { ignore: '' }
            });

            if ($('#HighLevelChanges').length) {

                CKEDITOR.replace('HighLevelChanges', { toolbar: "Basic" });
                attachEventCKEditor('HighLevelChanges');
            }

            form.on("change", "#DocType", function () {
                var typeId = parseInt(this.value);
                var docMaster = $("#OrgDocumentMasterId");
                docMaster.empty();
                docMaster.append("<option value='" + "" + "'>" + "-Select-" + "</option>");

                if (typeId) {
                    $.get(domain + 'orgdocument/getdocumentmaster/' + typeId, function (response) {
                        if (response.isSuccess) {
                            $.each(response.data, function (i, item) {
                                docMaster.append("<option value='" + item.value + "'>" + item.text + "</option>");
                            });
                        }
                    });
                }
            });

            form.on("change", "#OrgDocumentMasterId", function () {
                var masterId = parseInt(this.value);
                var baselineDoc = $("#currentBaselineDoc");
                baselineDoc.empty();
                form.find("input[name='RoleIds[]']").prop('checked', false);
                form.find("input[name='DepartmentIds[]']").prop('checked', false);

                if (masterId) {
                    $.get(domain + 'orgdocument/getdocumentbymasterid/' + masterId, function (response) {
                        if (response.isSuccess && response.data) {
                            var data = response.data;
                            baselineDoc.html('<div class="alert alert-info"><h5>Current Baseline Document</h5><a href="' + (domain + data.DocumentPath) + '">' + data.orgDocumentName + ' v' + data.ver + '</a><br><b>Added by</b> : ' + data.createBy + ' <b>Approved on</b> : ' + data.approvedDate + '</div>');

                            if (data.roleIds && data.roleIds.length) {
                                $.each(data.roleIds, function (i,v) {
                                    form.find("input[name='RoleIds[]'][value='" + v + "']").prop('checked', true);
                                });
                            }

                            if (data.departmentIds && data.departmentIds.length) {
                                $.each(data.departmentIds, function (i,v) {
                                    console.log(v);
                                    form.find("input[name='DepartmentIds[]'][value='" + v + "']").prop('checked', true);
                                });
                            }
                        }
                    });
                }
            });

            form.on("change", "#allRoles", function () {
                console.log(this.checked)
                form.find("input[name='RoleIds[]']").prop('checked', this.checked);
            });

            form.on("change", "#allDepartments", function () {
                form.find("input[name='DepartmentIds[]']").prop('checked', this.checked);
            });

            form.on("change", "input[name='DepartmentIds[]']", function () {
                var departments = form.find("input[name='DepartmentIds[]']");

                form.find("#allDepartments").prop('checked', departments.length == departments.filter(':checked').length);
            });

            form.on("change", "input[name='RoleIds[]']", function () {
                var roles = form.find("input[name='RoleIds[]']");

                form.find("#allRoles").prop('checked', roles.length == roles.filter(':checked').length);
            });
        }

        function attachEventCKEditor(instance) {
            CKEDITOR.on('instanceReady', function (e) {
                e.editor.document.on('keyup', function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }

        $this.init = function () {
            InitializeForm();
        }
    }

    $(function () {
        var self = new AddEdit();
        self.init();
    })

}(jQuery));