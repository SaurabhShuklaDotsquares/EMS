(function () {
    "use strict"

    function OnlyView() {
        var $this = this;

        function InitializeForm() {

            openFile();
        }

        // open file
        function openFile() {

            //debugger
            var fileName = $("#FileName").val();
            var contentType = $("#ContentType").val();
            var data = $("#Data").val();

            // Convert Base64 string to Byte Array.
            var bytes = Base64ToBytes(data);
            var fileExt = getFileExtension(fileName);
            if (fileExt == "pdf") {

                // pdf
                if (bytes != null) {
                    $('#pdf-container').show();
                    let pdfViewer = new PDFjsViewer($('.pdfpages'));
                    pdfViewer.loadDocument({ data: bytes }).then(function () {
                        pdfViewer.setZoom(1.5);// 150%
                    });
                }
            }
            else if (fileExt == "docx") {

                // docx
                if (bytes != null) {
                    $('#word-container').show();

                    //Convert BLOB to File object.
                    var doc = new File([bytes], contentType);

                    //If Document not NULL, render it.
                    if (doc != null) {
                        //Set the Document options.
                        var docxOptions = Object.assign(docx.defaultOptions, {
                            useMathMLPolyfill: true
                        });
                        //Reference the Container DIV.
                        var container = document.querySelector("#word-container");

                        //Render the Word Document.
                        docx.renderAsync(doc, container, null, docxOptions);
                    }
                }
            }
            else {
                swal({
                    text: "invalid file.",
                    icon: "error",
                });
            }
        }

        // extention
        function getFileExtension(filename) {
            return filename.split('.').pop().toLowerCase();
        }
        // convert to bytes
        function Base64ToBytes(base64) {
            var s = window.atob(base64);
            var bytes = new Uint8Array(s.length);
            for (var i = 0; i < s.length; i++) {
                bytes[i] = s.charCodeAt(i);
            }
            return bytes;
        };
        // disable context menus
        function disableContextMenu() {

            // menu
            Object.getOwnPropertyNames(console).filter(function (property) {
                return typeof console[property] == 'function';
            }).forEach(function (verb) {
                console[verb] = function () { return 'Sorry, for security reasons...'; };
            });
            window.addEventListener('devtools-opened', () => {
                // do some extra code if needed or ...
                // maybe even delete the page, I still like to add redirect just in case
                window.location.href += "#";
                window.document.head.innerHTML = "";
                window.document.body.innerHTML = "Unauthorized access found on page, kindly please <a style='text-decoration: underline;cursor: pointer;color: blue;' onclick='window.location.reload();'>click here</a> to reload the document.";
            });
            window.addEventListener('devtools-closed', () => {
                // do some extra code if needed
            });
            let verifyConsole = () => {
                var before = new Date().getTime();
                var after = new Date().getTime();
                if (after - before > 100) { // user had to resume the script manually via opened dev tools
                    window.dispatchEvent(new Event('devtools-opened'));
                } else {
                    window.dispatchEvent(new Event('devtools-closed'));
                }
                setTimeout(verifyConsole, 100);
            }
            verifyConsole();

            // keys and click
            document.oncontextmenu = function () {
                return false;
            }
            document.onkeydown = function (e) {
                if (e.keyCode == 123) {
                    return false;
                }
                if (e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
                    return false;
                }
                if (e.ctrlKey && e.shiftKey && e.keyCode == 'C'.charCodeAt(0)) {
                    return false;
                }
                if (e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
                    return false;
                }
                if (e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
                    return false;
                }
            }

        }


        $this.init = function () {
            disableContextMenu();
            InitializeForm();
        }

    }

    $(function () {
        var self = new OnlyView();
        self.init();
    })

}(jQuery));


