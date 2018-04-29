var sendValidationEmail = function (mailAddress, id) {
    $.get('/Account/SendEmail?email=' + mailAddress, function (data) {
        if (data.code == 0) {
            $('#' + id).attr('disabled', 'disabled');
            $('#' + id).html('Email Sent to ' + mailAddress + '!');
        }
    });
}