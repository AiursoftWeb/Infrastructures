//User just came.
var init = function () {
    $('#copyButton').hide();
    $('#openButton').hide();
    $('#address').hide();
    $('#uploadButton').hide();
    $('.progress').hide();
    $('#uploadButton').attr('value', 'Upload to share');
}

//User just put a file.
var ready = function () {
    $('#copyButton').hide();
    $('#openButton').hide();
    $('.progress-bar').css('width', '0%');
    $('.progress').show();
    $('#uploadButton').show();
    $('#uploadButton').removeAttr('disabled');
    $('#uploadButton').attr('value', 'Upload to share');
    $('#message').html('');
    $('#address').hide();
}

//User uploading the file.
var uploading = function () {
    $('#uploadButton').attr('disabled', 'true');
    $('#uploadButton').attr('value', 'Uploading...');
    $.ajax({
        url: '/Home/Upload',
        type: 'POST',
        enctype: 'multipart/form-data',
        data: new FormData($('#fileform')[0]),
        cache: false,
        contentType: false,
        processData: false,
        xhr: function () {
            var myXhr = $.ajaxSettings.xhr();
            if (myXhr.upload) {
                myXhr.upload.addEventListener('progress', function (e) {
                    if (e.lengthComputable) {
                        $('.progress-bar').css('width', 100 * e.loaded / e.total + '%');
                        $('#message').html(Math.round(e.loaded / e.total * 100) + "%");
                        if (e.loaded == e.total) {
                            processing();
                        }
                    }
                }, false);
            }
            return myXhr;
        },
        success: finish
    });
}

//User uploaded the file and verifing
var processing = function () {
    $('#message').html('Please be patient while we are verifying your file...');
}

//File validated!
var finish = function (e) {
    ready();
    $('#address').val(e.value);
    $('#copyButton').show();
    $('#openButton').show();
    $('#copyButton').attr('data-clipboard-text', e.value);
    $('#openButton').attr('href', e.value);
    $('#address').show();
}

$(':file').on('change', ready);
$('#uploadButton').on('click', uploading);
init();