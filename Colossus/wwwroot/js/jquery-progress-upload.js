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

    }
};

(function ($) {
    $.fn.progressloader = function (settings) {
        var createElements = function (initor) {
            var progressbar = '\
            <div id="progress">\
                <div id="progressbar" role="progressbar"></div>\
            </div>';

            var message = '<p id="message"></p>';
            initor.after(message);
            initor.after(progressbar);
        }

        var getElements = function () {
            var elements = {
                progress: $('#progress'),
                progressbar: $('#progressbar'),
                message: $('#message')
            };
            return elements;
        }

        var setClass = function (elements) {
            elements.progress.addClass('progress mb-3 mt-3');
            elements.progressbar.css('width', '0%');
            elements.progressbar.addClass('progress-bar progress-bar-striped progress-bar-animated');
            elements.message.addClass('text-danger');
        }

        //Occurs when page loads.
        var init = function (elements) {
            elements.progress.hide();
            if (settings.onInit) {
                settings.onInit(elements);
            }
        }

        //Occurs when user put in a file
        var getFile = function () {
            var elements = getElements();
            elements.progressbar.css('width', '0%');
            elements.progress.show();
            elements.message.html('0%');
            if (settings.onGetFile) {
                settings.onGetFile(elements);
            }
        }

        var startSubmitting = function () {
            event.preventDefault();
            var elements = getElements();
            if (settings.onStartSubmitting) {
                settings.onStartSubmitting(elements);
            }
            $.ajax({
                url: form.attr('action'),
                type: form.attr('method'),
                enctype: form.attr('enctype'),
                data: new FormData(form[0]),
                cache: false,
                contentType: false,
                processData: false,
                xhr: function () {
                    var myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) {
                        myXhr.upload.addEventListener('progress', function (e) {
                            if (e.lengthComputable) {
                                elements.progressbar.css('width', 100 * e.loaded / e.total + '%');
                                elements.message.html(Math.round(e.loaded / e.total * 100) + "%");
                                if (e.loaded == e.total) {
                                    processing(elements);
                                }
                            }
                        }, false);
                    }
                    return myXhr;
                },
                success: finish
            });
        }

        var processing = function (elements) {
            if (settings.onProcessing) {
                settings.onProcessing(elements);
            }
            elements.message.html('Please be patient while we are verifying your file...');
        }

        var finish = function (data) {
            var elements = getElements();
            getFile();
            if (settings.onFinish) {
                settings.onFinish(elements, data);
            }
        }

        createElements(this);
        var elements = getElements();
        var form = this.parents().filter("form").first();
        setClass(elements);
        init(elements);
        this.on('change', getFile);
        form.submit(startSubmitting);
    }
}(jQuery))