

// custome file form helper
FormHelperWithCustomFileCollection = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    //debugger
    var settings = {};
    settings = $.extend({}, settings, options);
    $.validator.unobtrusive.parse(formElement);
    if (settings.validateSettings !== null && settings.validateSettings !== undefined) {
        formElement.validate(settings.validateSettings);
    }
    formElement.off("submit").submit(function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();

        //debugger
        var _filecollection = null;
        var formdata = new FormData();

        // set custome file collection
        if (options && options.setCustomFileCollection) {
            var filetag = {};
            _filecollection = options.setCustomFileCollection(filetag);
            if (_filecollection != null) {
                for (var i = 0; i < _filecollection.length; i++) {
                    var file = _filecollection[i];
                    formdata.append(filetag.key, file);
                }
            }
        }

        // before form data set
        if (options && options.beforeFormDataSet) {
            options.beforeFormDataSet();
        }

        $.each(formElement.serializeArray(), function (i, item) {
            formdata.append(item.name, item.value);
        });

        if (options && options.updateFormData) {
            var updateformdata = options.updateFormData(formdata);
            if (updateformdata !== null && updateformdata !== undefined) {
                formdata = updateformdata;
            }
        }


        var submitBtn = formElement.find(':submit');//formElement.find('.btn-primary');
        if (formElement.validate().valid() && formElement.valid()) {
            $('.loading-common,.loading-overlay').show()
            if (options && options.beforeSubmit) {
                if (!options.beforeSubmit()) {
                    return false;
                }
            }

            submitBtn.find('i').removeClass("fas fa-chevron-circle-rightt");
            submitBtn.find('i').addClass("fa fa-spinner fa-spin");
            submitBtn.prop('disabled', true);
            submitBtn.find('span').html('Submitting..');

            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formdata,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).show();
                        submitBtn.hide();
                    }
                },
                success: function (result) {
                    //debugger
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else {
                            if (settings.updateTargetId) {
                                var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                                $("#" + settings.updateTargetId).html(datatresult);
                            }
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                        $("#loadingElement").hide();
                    }
                },
                complete: function (result) {
                    if (onComplete === null || onComplete === undefined) {
                        if (settings.loadingElementId !== null || settings.loadingElementId !== undefined) {
                            $("#" + settings.loadingElementId).hide();
                        }
                        //submitBtn.removeClass("spinning");
                        //submitBtn.addClass(submitBtn.attr("data-visible-class"));
                        //submitBtn.find('span').text(submitBtn.attr("data-text"));
                        ////submitBtn.prop('disabled', false);
                        //submitBtn.removeAttr('disabled');
                        submitBtn.find('i').removeClass("fa fa-spinner fa-spin");
                        submitBtn.find('i').addClass("fas fa-chevron-circle-right");
                        submitBtn.find('span').html('Submit');
                        submitBtn.prop('disabled', false);
                    } else {
                        onComplete(result);
                    }

                }
            });
        }

        e.preventDefault();
    });
    return formElement;
}


