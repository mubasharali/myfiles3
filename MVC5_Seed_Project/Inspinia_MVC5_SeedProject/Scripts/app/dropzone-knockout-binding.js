$(document).ready(function () {

    ko.bindingHandlers.dropzone = {
        init: function (element, valueAccessor) {
            var value = ko.unwrap(valueAccessor());

            var options = {
                maxFileSize: 15,
                autoProcessQueue: false,
                uploadMultiple: true,
                parallelUploads: 100,
                maxFiles: 100,
                addRemoveLinks: true,
                clickable: '#dropzonePreview',
                init: function () {
                    var myDropzone = this;
                    this.element.querySelector("button[type=submit]").addEventListener("click", function (e) {

                        e.preventDefault();
                        e.stopPropagation();
                        var form = $(this).closest("#form");
                        if (form.valid() == true) {
                            if (myDropzone.getQueuedFiles().length > 0) {
                                myDropzone.processQueue();
                            } else {
                                myDropzone.uploadFiles([]); //send empty 
                            }
                        }
                    });
                    this.on("success", function (file, responseText) {
                       // var responseText = file.id // or however you would point to your assigned file ID here;
                       // console.log(responseText); // console should show the ID you pointed to
                        console.log(responseText);
                    });
                }
            };

            $.extend(options, value);

            $(element).addClass('dropzone');
            new Dropzone(element, options); // jshint ignore:line
        }
    };
});