/*global window, $*/

/* Add String.replaceBetween method in Javascript */
String.prototype.replaceBetween = function (start, end, value) {
    return this.substring(0, start) + value + this.substring(end);
};
/* String.replaceBetween end here */

var Global = {
    DomainName: "",
    DataServer: { multisearch: [], dataURL: "" },
    FilterType: { Contains: 0, Equals: 1, StartsWith: 2, LessThanOrEqual: 3, GreaterThanOrEqual: 4 },
    Filter: { text: "", value: "", type: "" },
    MessageType: { Success: 0, Error: 1, Warning: 2, Info: 3 }
};

Global.IsNull = function (o) { return typeof o === "undefined" || typeof o === "unknown" || o == null };
Global.IsNotNull = function (o) { return !Global.IsNull(o); };
Global.IsNullOrEmptyString = function (str) {
    return Global.IsNull(str) || typeof str === "string" && $.trim(str).length == 0
};
Global.IsNotNullOrEmptyString = function (str) { return !Global.IsNullOrEmptyString(str); };

if (typeof jQuery != 'undefined' && typeof jQuery.validator != 'undefined') {
    $.validator.unobtrusive.options = {
        errorElement: "label",
        errorClass: "error",
        errorPlacement: function (error, element) {
            var elm = $(element);
            if (elm.parent('.input-group').length || elm.parent('.input-group-custom').length) {
                error.insertAfter(elm.parent());
            }
            else if (elm.prop('type') === 'checkbox' || elm.prop('type') === 'radio') {
                error.appendTo(elm.closest(':not(input, label, .checkbox, .radio)').first());
            } else {
                error.insertAfter(elm);
            }
        },
        submitHandler: function (form) {
            form.submit();
        }
    };
}

Global.FormNAHelper = function (formElement, options, onBeforeSubmit, onSucccess, onError) {
    "use strict";
    var settings = {};
    settings = $.extend({}, settings, options);
    formElement.validate(settings.validateSettings);
    formElement.submit(function (e) {
        var submitBtn = formElement.find(':submit');
        if (formElement.validate().valid()) {
            return true;
        }
        e.preventDefault();
    });
    return formElement;
};

Global.ShowMessage = function (message, success, targetId) {
    var $target = $('#' + targetId);
    $target.empty().html(message).addClass('alert');

    if (success) {
        $target.addClass('alert-success').removeClass('alert-danger');
    }
    else {
        $target.addClass('alert-danger').removeClass('alert-success');
    }
    $target.show();
    $target.off('click').on('click', function () {
        $target.hide(500).empty().removeClass('alert-success').removeClass('alert-danger');
    });
}

Global.FormHelper = function (formElement, options, onBeforeSubmit, onSucccess, onError) {
    "use strict";
 
    var settings = {};
    settings = $.extend({}, settings, options);
    formElement.data('validator', null);
   
    formElement.validate(settings.validateSettings);
    $.validator.unobtrusive.parse(formElement);
 
    formElement.submit(function (e) {
        if (options && options.beforeSubmit) {
            if (!options.beforeSubmit()) {
                return false;
            }
        }
        var submitBtn = formElement.find(':submit');
        if (formElement.validate().valid()) {
            var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();
            submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
            submitBtn.prop('disabled', true);

            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formElement.serializeArray(),
                beforeSend: function (jqXHR, data) {
                    if (onBeforeSubmit) {
                        onBeforeSubmit(jqXHR, data);

                        if (!jqXHR.status) {
                            submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                            submitBtn.prop('disabled', false);
                        }
                    }
                },
                success: function (result) {

                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else {

                            if (settings.updateTargetId) {
                                $("#" + settings.updateTargetId).html(result);
                            }
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                    }
                },
                complete: function () {
                    submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                    submitBtn.prop('disabled', false);
                }
            });
        }
        e.preventDefault();
    });

    return formElement;
};

Global.FormHelperWithFiles = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    "use strict";
    var settings = {};
    settings = $.extend({}, settings, options);
    formElement.validate(settings.validateSettings);
    formElement.submit(function (e) {
        e.preventDefault();
        $("#loadingElement").hide();
        var formdata = new FormData();
        formElement.find('input[type="file"]:not(:disabled)').each(function (i, elem) {
            if (elem.files && elem.files.length > 0) {
                for (var i = 0; i < this.files.length; i++) {
                    var file = elem.files[i];
                    formdata.append(elem.getAttribute('name'), file);
                }
            }
        });

        $.each(formElement.serializeArray(), function (i, item) {
            formdata.append(item.name, item.value);
        });

        var submitBtn = formElement.find(':submit');
        if (formElement.validate().valid()) {
            var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();
            submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
            submitBtn.prop('disabled', true);

            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formdata,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).show();
                    }
                },
                success: function (result) {
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else if (settings.updateTargetId) {
                            $("#" + settings.updateTargetId).html(result.message || result.data || result);
                            $("#" + settings.updateTargetId).show();
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {
                    console.log(error);
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                    }
                },
                complete: function (result) {
                    submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                    submitBtn.prop('disabled', false);

                    if (onComplete === null || onComplete === undefined) {

                    } else {
                        onComplete(result);
                    }

                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).hide();
                    }
                }
            });
        }

        //e.preventDefault();
    });

    return formElement;
};

Global.FormValidationReset = function (formElement, validateOption) {
    $(formElement).validate(validateOption);
    return $(formElement);
};

Global.DropDownHelper = function (selectElement, options, callBack) {
    var settings = {};
    settings = $.extend({}, settings, options);
    $(selectElement).empty();
    var optionHtml = '';
    if (settings.optionalLabel) {
        optionHtml = '<option value="">' + settings.optionalLabel + '</option>';
    }
    $.get(settings.url, settings.data, function (result) {
        $.each(result, function (index, item) {
            optionHtml += '<option value="' + item[settings.dataValueField] + '">' + item[settings.dataTextField] + '</option>';
        });

        $(selectElement).html(optionHtml);
        if (callBack) { callBack(); }
    });
}

Global.GridHelper = function (gridElement, options) {
    if ($(gridElement).find("thead tr th").length > 1 || options.serverSide === true) {
        var settings = {};
        settings = $.extend({}, settings, options);
        return $(gridElement).DataTable(settings);
    }
};

Global.ReIndexList = function (list) {
    if (list.length) {
        var index = 0;
        list.each(function (i, g) {
            $(g).find(":input.reindex:not(:disabled)").each(function (h, ele) {

                var name = $(ele).attr("name");
                var start = name.indexOf("[");
                var end = name.indexOf("]") + 1;
                name = name.replaceBetween(start, end, "[" + index + "]");
                $(ele).attr("name", name);
            });
            index++
        });
    }
};

Global.GetUrlVars = function () {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

Global.DynamicFormPost = function (url, params, newWindow, delay) {
    delay = delay ? delay : 0;

    setTimeout(function () {
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("action", url);
        if (newWindow) {
            form.setAttribute("target", "_blank");
        }
        for (var item in params) {
            if (params.hasOwnProperty(item)) {
                if (params[item] && Array.isArray(params[item])) {
                    var valueArray = params[item];
                    for (var i = 0; i < valueArray.length ; i++) {
                        var input = document.createElement('input');
                        input.type = 'hidden';
                        input.name = item + '[' + i + ']';
                        input.value = valueArray[i];
                        form.appendChild(input);
                    }
                }
                else {
                    var input = document.createElement('input');
                    input.type = 'hidden';
                    input.name = item;
                    input.value = params[item];
                    form.appendChild(input);
                }
            }
        }
        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    }, delay);
}
