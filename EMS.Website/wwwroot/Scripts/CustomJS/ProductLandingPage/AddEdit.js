(function ($) {
    'use strict';
    function AddEdit() {
        var $this = this, form, fineUploader, landingPageId, redirectUrl;

        function initialize() {

            form = new Global.FormHelper($("form"), {
                updateTargetId: "validation-summary", validateSettings: { ignore: '' }
            }, null, function (result) {
                if (result.isSuccess) {

                    landingPageId = result.data;
                    redirectUrl = result.redirectUrl;

                    if (landingPageId && fineUploader.getUploads({ status: qq.status.SUBMITTED }).length) {
                        fineUploader.uploadStoredFiles();
                    }
                    else {
                        window.location.href = redirectUrl;
                    }

                } else {
                    form.find("#validation-summary").html(result);
                }
            });

            landingPageId = parseInt(form.find("#Id").val());

            CKEDITOR.inline('HighlightText', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                }, ],
                removeButtons: 'Anchor,Styles,Font,SpecialChar'
            });
            attachEventCKEditor('HighlightText');

            CKEDITOR.inline('TechnologyDetail', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                },
                {
                    "name": 'insert',
                    "groups": ['insert']
                }],
                removeButtons: 'Anchor,Styles,Font,SpecialChar,Smiley,PageBreak,Iframe,HorizontalRule',
            });
            attachEventCKEditor('TechnologyDetail');

            CKEDITOR.inline('AboutProduct', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                },
                {
                    "name": 'insert',
                    "groups": ['insert']
                },
                ],
                removeButtons: 'Anchor,Styles,Font,SpecialChar,Smiley,PageBreak,Iframe,HorizontalRule',
                filebrowserImageUploadUrl: domain + 'ProductLandingPage/UploadImage'
            });
            attachEventCKEditor('AboutProduct');

            CKEDITOR.inline('Feature1', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                }, ],
                removeButtons: 'Anchor,Styles,Font,SpecialChar,Smiley,PageBreak,Iframe,HorizontalRule',
            });
            attachEventCKEditor('Feature1');

            CKEDITOR.inline('Feature2', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                }, ],
                removeButtons: 'Anchor,Styles,Font,SpecialChar'
            });
            attachEventCKEditor('Feature2');

            CKEDITOR.inline('Feature3', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                }, ],
                removeButtons: 'Anchor,Styles,Font,SpecialChar'
            });
            attachEventCKEditor('Feature3');

            CKEDITOR.inline('ServiceDetail', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                },
                {
                    "name": 'insert',
                    "groups": ['insert']
                }],
                removeButtons: 'Anchor,Styles,Font,SpecialChar,Smiley,PageBreak,Iframe,HorizontalRule',
                filebrowserImageUploadUrl: domain + 'ProductLandingPage/UploadImage'
            });
            attachEventCKEditor('ServiceDetail');

            CKEDITOR.inline('Testimonials', {
                toolbarGroups: [
                {
                    "name": "styles",
                    "groups": ["styles"]
                },
                {
                    "name": "basicstyles",
                    "groups": ["basicstyles"]
                },
                {
                    "name": "links",
                    "groups": ["links"]
                },
                {
                    "name": 'paragraph',
                    "groups": ['align']
                },
                {
                    "name": 'insert',
                    "groups": ['insert']
                }],
                removeButtons: 'Anchor,Styles,Font,SpecialChar,Smiley,PageBreak,Iframe,HorizontalRule',
                filebrowserImageUploadUrl: domain + 'ProductLandingPage/UploadImage'
            });
            attachEventCKEditor('Testimonials');

            form.on("click", "#screenshot-list .remove", function (e) {

                var li = $(this).closest("li");

                if (confirm("Are you sure ?\nWant to remove this image.")) {
                    li.remove();
                }
            });

            form.on("click", "#Save", function (e) {
                e.preventDefault();
                if (confirm("Are you sure ?\nDetails will be published and further changes will be locked.")) {
                    form.find("#IsDraft").val(false);
                    indexScreenshots();
                    form.submit();
                }
            });

            form.on("click", "#Draft", function (e) {
                e.preventDefault();
                form.find("#IsDraft").val(true);
                indexScreenshots();
                form.submit();
            });
        }

        function initFineuploader() {
            fineUploader = new qq.FineUploader({
                element: document.getElementById('fine-uploader-manual-trigger'),
                template: 'qq-template-manual-trigger',
                request: {
                    endpoint: domain + 'ProductLandingPage/UploadScreenshot'
                },
                thumbnails: {
                    placeholders: {
                        waitingPath: 'scripts/plugin/fine-uploader/placeholders/waiting-generic.png',
                        notAvailablePath: 'scripts/plugin/fine-uploader/placeholders/not_available-generic.png'
                    }
                },
                autoUpload: false,
                callbacks: {
                    onProgress: function (id) {
                        $('.divoverlay').removeClass('hide');
                    },
                    onUpload: function (id) {

                        var fileContainer = this.getItemByFileId(id)
                        var captionInput = fileContainer.querySelector('.caption')
                        var captionText = captionInput.value

                        this.setParams({ caption: captionText, id: landingPageId }, id)
                    },
                    onAllComplete: function () {
                        $('.divoverlay').addClass('hide');
                        window.location.href = redirectUrl;
                    }
                }
            });
        }

        function indexScreenshots() {
            var imageList = $("#screenshot-list").children("li");
            if (imageList.length) {
                Global.ReIndexList(imageList);
            }
        }

        function attachEventCKEditor(instance) {
            CKEDITOR.on('instanceReady', function (e) {
                e.editor.document.on('keyup', function () {
                    console.log('edited')
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }

        $this.init = function () {
            initialize();
            initFineuploader();
        }
    }

    $(function () {
        var self = new AddEdit();
        self.init();
    });
}(jQuery));