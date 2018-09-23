var sendValidationEmail = function (mailAddress, id) {
    $('#' + id).attr('disabled', 'disabled');
    $('#' + id).html('Sending. Please wait...');
    $.get('/Account/SendEmail?email=' + mailAddress, function (data) {
        if (data.code === 0) {
            $('#' + id).attr('disabled', 'disabled');
            $('#' + id).html('Email Sent!');
        } else {
            alert(data.message);
            $('#' + id).html('Error.');
        }
    });
};

var DeleteEmail = function (mailAddress) {
    $.get('/Account/DeleteEmail?email=' + mailAddress, function (data) {
        if (data.code === 0) {
            window.location.reload();
        } else {
            alert(data.message);
        }
    });
};

var DeleteGrant = function (appId) {
    $.get('/Account/DeleteGrant?appId=' + appId, function (data) {
        if (data.code === 0) {
            window.location.reload();
        } else {
            alert(data.message);
        }
    });
};
