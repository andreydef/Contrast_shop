// Script to preview the images
$(function () {

    /* Preview selected image */
    function readUrl(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $("img#imgpreview")
                    .attr("src", e.target.result)
                    .width(200)
                    .height(200);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    // Function, after page reloading for output the image
    $("#imageUpload").change(function () {
        readUrl(this);
    });

    /*-----------------------------------------------------------*/

    /* Dropzone js*/

    Dropzone.options.dropzoneForm = {
        acceptedFiles: "images/*",
        init: function () {
            this.on("complete",
                function (file) {
                    if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                        location.reload();
                    }
                });
            this.on("sending",
                function (file, xhr, formData) {
                    formData.append("id", @Model.Id); /* the script works because it's connected to the site 
                                                           and that's where the Razor implementation is present */
         });
        }
    };
    /*-----------------------------------------------------------*/
});