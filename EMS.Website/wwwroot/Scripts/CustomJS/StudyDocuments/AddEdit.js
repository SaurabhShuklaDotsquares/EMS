(function () {
    "use strict"

    function AddEdit() {
        var $this = this, form, filecollection = [], reg = (/\.(docx|DOCX|pdf|PDF)$/i), regexDigit = /([0-9]+)/g;
        // var regex = /\[([0-9]+)\]/g, replace [222] by [0] value in pattern

        function InitializeForm() {

            // technology ddl
            $("#TechnologyId").select2();

            // form 
            form = new FormHelperWithCustomFileCollection($("form#frmstudydocuments"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: ''
                },
                beforeSubmit: function () {
                    //debugger

                    // for file
                    if ($(".clssdf").length == 0) {
                        $('.loading-common,.loading-overlay').hide()
                        swal({
                            text: "Please choose any document(s).",
                            icon: "info",
                        });
                        return false;
                    }
                    // for file display name
                    if (isUniqueDisplayName() == false) {

                        swal({
                            text: "Display name should be unique.",
                            icon: "info",
                        });
                        return false;
                    }
                    return true;
                },
                setCustomFileCollection: function (r) {
                    r.key = "documents";
                    r.val = filecollection;
                    return filecollection;
                }
            });

            //multi image
            $('#documents').on("change", function () {
                //debugger

                // last length
                var fileIndex = $(".clssdf").length;

                for (var i = 0; i < this.files.length; i++) {

                    if (!reg.test(this.files[i].name)) {
                        swal({
                            text: "\"" + this.files[i].name + "\" Invalid file format, only .docx and .pdf files are allowed.",
                            icon: "info",
                        });
                        $(this).val("");
                        break;
                    }

                    var objectURL = URL.createObjectURL(this.files[i]);

                    // table tr
                    var _tr = $("<tr/>", { class: "clssdf" });

                    var _tdRowNo = $("<td/>", { style: "width:10%;text-align:center;", class: "clsresetindex", "data-rowno": "", text: (fileIndex + 1) });

                    var _tdFile = $("<td/>", { style: "width: 40%;padding-left: 15px;" });
                    var _hdnFileName = $("<input/>", { type: "hidden", id: "studyDocumentFiles_" + fileIndex + "__FileName", name: "studyDocumentFiles[" + fileIndex + "].FileName", value: this.files[i].name, class: "clsresetindex" });
                    var _aFileName = $("<a/>", { href: objectURL, target: "_blank", text: this.files[i].name, style: "color:blue;" });
                    _tdFile.append(_hdnFileName);
                    _tdFile.append(_aFileName);

                    var _tdDisplayName = $("<td/>", { style: "width: 40%;padding-left: 15px;" });
                    var _lbDisplayName = $("<label/>", { text: "File Display Name*" });
                    var _inputFilDisplayName = $("<input/>", { type: "text", id: "studyDocumentFiles_" + fileIndex + "__DisplayName", name: "studyDocumentFiles[" + fileIndex + "].DisplayName", "data-val": "true", "data-val-maxlength": "maximum 100 characters", "data-val-maxlength-max": "100", "data-val-required": "*required", class: "form-control valid clsresetindex clsdisplayname", "aria-invalid": "false" });
                    var _lbErrorDisplayName = $("<label/>", { id: "studyDocumentFiles_" + fileIndex + "__DisplayName-error", class: "error clsresetindex", for: "studyDocumentFiles_" + fileIndex + "__DisplayName" })
                    var _sErrorDisplayName = $("<span/>", { name: "studyDocumentFiles[" + fileIndex + "].DisplayName", class: "text-red field-validation-valid clsresetindex", "data-valmsg-replace": true });
                    _tdDisplayName.append(_lbDisplayName);
                    _tdDisplayName.append(_inputFilDisplayName);
                    _tdDisplayName.append(_lbErrorDisplayName);
                    _tdDisplayName.append(_sErrorDisplayName);

                    var _tdRemove = $("<td/>", { style: "width:10%;text-align:center;" });
                    var _aRemove = $("<a/>", { href: "javascript:void(0)", style: "color:blue;", "data-filename": this.files[i].name, text: "Remove" });
                    _tdRemove.append(_aRemove);

                    _tr.append(_tdRowNo);
                    _tr.append(_tdFile);
                    _tr.append(_tdDisplayName);
                    _tr.append(_tdRemove);

                    $("#tblsdf").append(_tr);
                    // table tr

                    //add in form data
                    filecollection.push(this.files[i]);

                    // last length + 1
                    fileIndex = fileIndex + 1;
                }
                showHideViewSFDFile();

                // reflect form validation
                var form = $("#frmstudydocuments")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse(form);
                $(form).data('unobtrusiveValidation');
                $(form).validate($(form).data("unobtrusiveValidation").options)
                $(form).valid();
            });

            $(document).on("click", "[data-filename]", function () {
                //debugger

                $this = $(this);
                var filename = $this.data("filename");
                if (filecollection.length > 0) {
                    var index = filecollection.findIndex(x => x.name === filename)
                    if (index != -1) {
                        filecollection.splice(index, 1);
                    }
                }
                $this.closest("tr.clssdf").remove();
                showHideViewSFDFile();

                //reset index
                resetIndexsOfSDF();
            });

            $("#viewsdf").on("click", function () {
                $("#dvsdf").toggle("slow");
            });

            // description ck-editor
            CKEDITOR.replace('Description', { toolbar: "Basic" });
            attachEventCKEditor('Description');
            // 
            showHideViewSFDFile();
        }
        // show/hide view files
        function showHideViewSFDFile() {
            //debugger
            var $this = $(".clssdf");
            if ($this.length > 0) {
                $("#viewsdf").show("slow");
                $("#dvsdf").show("slow");
            }
            else {
                $("#dvsdf").hide("slow");
                $("#viewsdf").hide("slow");
            }
            $("#documents").val("");
        }
        // editor
        function attachEventCKEditor(instance) {
            CKEDITOR.on('instanceReady', function (e) {
                e.editor.document.on('keyup', function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }
        // reset index of models
        function resetIndexsOfSDF() {
            //debugger

            $(".clssdf").each(function (index, val) {
                //debugger

                var $val = $(val);

                // all reset for assigned class
                // hidden
                var $clsresetindex = $val.find(".clsresetindex");
                $clsresetindex.each(function ($clsresetindex_i, $clsresetindex_val) {

                    var $ele = $($clsresetindex_val);
                    var name = $ele.attr("name");
                    var id = $ele.attr("id");
                    var _for = $ele.attr("for");
                    var _data_valmsg_for = $ele.attr("data-valmsg-for");
                    var data_rowno = $ele.attr("data-rowno");

                    if (id != null && id != undefined && id.length > 0) {
                        id = id.replace(regexDigit, index);// replace 222 by 0 value in pattern
                        $ele.attr("id", id);
                    }
                    if (name != null && name != undefined && name.length > 0) {
                        name = name.replace(regexDigit, index);// replace 222 by 0 value in pattern                
                        $ele.attr("name", name);
                    }
                    if (_for != null && _for != undefined && _for.length > 0) {
                        _for = _for.replace(regexDigit, index);// replace 222 by 0 value in pattern                
                        $ele.attr("for", _for);
                    }
                    if (_data_valmsg_for != null && _data_valmsg_for != undefined && _data_valmsg_for.length > 0) {
                        _data_valmsg_for = _data_valmsg_for.replace(regexDigit, index);// replace 222 by 0 value in pattern                
                        $ele.attr("data-valmsg-for", _data_valmsg_for);
                    }
                    if (data_rowno != null && data_rowno != undefined) {
                        $ele.text(index + 1);
                    }
                });
            });
        }
        // 1 id only have unique display name
        function isUniqueDisplayName() {
            //debugger
            var matchCount = 0;
            for (var i = 0; i < $(".clsdisplayname").length; i++) {

                matchCount = 0;
                for (var j = 0; j < $(".clsdisplayname").length; j++) {

                    if ($(".clsdisplayname")[i].value.toLowerCase() == $(".clsdisplayname")[j].value.toLowerCase()) {

                        matchCount++;
                    }
                    // match found except self
                    if (matchCount >= 2) {
                        return false;
                    }
                }
            }
            return true;
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


