(function ($) {
    $.fn.progressloader = function () {

        var createElements = function (initor) {
            var maxSize = initor.attr('data-max-file-size');

            var label = '<p>Only ' + maxSize + ' max.</p>';

            var progressbar = '\
            <div class="progress mb-3">\
            <div id="progress" role="progressbar"></div>\
            </div>'

            var buttons = '\
            <div class="btn-group" role="group">\
                <button type="button" id="uploadButton"></button>\
                <button type="button" id="copyButton"></button>\
                <button type="button" id="openButton"></button>\
            </div>';

            var message = '<p id="message"></p>';

            var address = '<input class="" type="text" id="address" />'

            initor.after(address);
            initor.after(message);
            initor.after(buttons);
            initor.after(progressbar);
            initor.after(label);
            return initor;
        }

        var setClass = function (uploader) {
            uploader.progress.css('width', '0%');
            uploader.progress.addClass('progress-bar progress-bar-striped progress-bar-animated');
            uploader.uploadButton.addClass('btn btn-success');
            uploader.uploadButton.html('Upload');
            uploader.copyButton.addClass('btn btn-secondary');
            uploader.copyButton.html('Copy link');
            uploader.openButton.addClass('btn btn-primary');
            uploader.openButton.html('Open');
            uploader.message.addClass('text-danger');
            uploader.address.addClass('form-control mb-3');
            uploader.address.prop('disabled', true);
            return uploader;
        };

        createElements(this);
        var uploader = {
            progress : $('#progress'),
            uploadButton : $('#uploadButton'),
            copyButton : $('#copyButton'),
            openButton : $('#openButton'),
            message : $('#message'),
            address : $('#address'),
        }
        setClass(uploader);



    }
}(jQuery));