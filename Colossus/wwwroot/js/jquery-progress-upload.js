(function ($) {
    $.fn.progressloader = function (url, formname, callback) {
        var createElements = function (initor) {
            var maxSize = initor.attr('data-max-file-size');

            var label = '<p>Only ' + maxSize + ' max.</p>';

            var progressbar = '\
            <div id="progress" class="progress mb-3">\
            <div id="progressbar" role="progressbar"></div>\
            </div>'

            var buttons = '\
            <div class="btn-group" role="group">\
                <button type="button" id="uploadButton"></button>\
                <a type="button" id="copyButton" data-toggle="tooltip" data-original-title="copied!" data-trigger="click"></a>\
                <a type="button" id="openButton"></a>\
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

        var getElements = function () {
            var uploader = {
                progress: $('#progress'),
                progressbar: $('#progressbar'),
                uploadButton: $('#uploadButton'),
                copyButton: $('#copyButton'),
                openButton: $('#openButton'),
                message: $('#message'),
                address: $('#address'),
            };
            return uploader;
        }

        var setClass = function (uploader) {
            uploader.progressbar.css('width', '0%');
            uploader.progressbar.addClass('progress-bar progress-bar-striped progress-bar-animated');
            uploader.uploadButton.addClass('btn btn-success');
            uploader.uploadButton.html('Upload');
            uploader.copyButton.addClass('btn btn-warning');
            uploader.copyButton.html('Copy link');
            uploader.openButton.addClass('btn btn-primary');
            uploader.openButton.html('Open');
            uploader.message.addClass('text-danger');
            uploader.address.addClass('form-control mb-3');
            uploader.address.prop('disabled', true);
            if ($.fn.tooltip) {
                $('[data-toggle="tooltip"]').tooltip();
            }
            return uploader;
        };

        var init = function (uploader) {
            uploader.copyButton.hide();
            uploader.openButton.hide();
            uploader.address.hide();
            uploader.uploadButton.hide();
            uploader.progress.hide();
        }

        var ready = function () {
            var uploader = getElements();
            uploader.copyButton.hide();
            uploader.openButton.hide();
            uploader.progressbar.css('width', '0%');
            uploader.progress.show();
            uploader.uploadButton.show();
            uploader.uploadButton.removeAttr('disabled');
            uploader.message.html('');
            uploader.address.hide();
        }

        var uploading = function () {
            var uploader = getElements();
            uploader.uploadButton.attr('disabled', 'true');
            uploader.uploadButton.attr('value', 'Uploading...');
            $.ajax({
                url: url,
                type: 'POST',
                enctype: 'multipart/form-data',
                data: new FormData($(formname)[0]),
                cache: false,
                contentType: false,
                processData: false,
                xhr: function () {
                    var myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) {
                        myXhr.upload.addEventListener('progress', function (e) {
                            if (e.lengthComputable) {
                                uploader.progressbar.css('width', 100 * e.loaded / e.total + '%');
                                uploader.message.html(Math.round(e.loaded / e.total * 100) + "%");
                                if (e.loaded == e.total) {
                                    processing(uploader);
                                }
                            }
                        }, false);
                    }
                    return myXhr;
                },
                success: finish
            });
        }

        var processing = function (uploader) {
            uploader.message.html('Please be patient while we are verifying your file...');
        }

        var finish = function (e) {
            ready();
            var uploader = getElements();
            uploader.address.val(e.value);
            uploader.copyButton.show();
            uploader.openButton.show();
            uploader.copyButton.attr('data-clipboard-text', e.value);
            uploader.openButton.attr('href', e.value);
            uploader.address.show();
            if (callback) {
                callback(e)
            }
        }

        createElements(this);
        var uploader = getElements();
        setClass(uploader);
        init(uploader);
        this.on('change', ready);
        uploader.uploadButton.on('click', uploading);
    }
}(jQuery));