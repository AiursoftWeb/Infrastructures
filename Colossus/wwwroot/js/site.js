$(document).ready(function () {
    //$('.dropify').progressloader('/Home/Upload','#fileform');
    var settings = {
        onInit: function (elements) {

        },

        onGetFile: function (elements) {

        },

        onStartSubmitting: function (elements) {

        },

        onProcessing: function (elements) {

        },

        onFinish: function (elements, data) {
            alert(data.value)
        }
    };
    $('.dropify').progressloader(settings);
    $('.dropify').dropify();
});