var sendValidationEmail = function (mailAddress, id) {
    $('#' + id).attr('disabled', 'disabled');
    $('#' + id).html('Sending. Please wait...');
    $.post('/Account/SendEmail', { email: mailAddress }, function (data) {
        if (data.code === 0) {
            $('#' + id).attr('disabled', 'disabled');
            $('#' + id).html('Email Sent!');
        } else {
            alert(data.message);
            $('#' + id).html('Error.');
        }
    });
};

var SetPrimaryEmail = function (mailAddress) {
    $.post('/Account/SetPrimaryEmail', { email: mailAddress }, function (data) {
        if (data.code === 0) {
            window.location.reload();
        } else {
            alert(data.message);
        }
    });
};

var DeleteEmail = function (mailAddress) {
    $.post('/Account/DeleteEmail', { email: mailAddress }, function (data) {
        if (data.code === 0) {
            window.location.reload();
        } else {
            alert(data.message);
        }
    });
};

var DeleteGrant = function (appId) {
    $.post('/Account/DeleteGrant', { appId: appId }, function (data) {
        if (data.code === 0) {
            window.location.reload();
        } else {
            alert(data.message);
        }
    });
};

var UnbindAccount = function (provider) {
    $.post('/Account/UnBindAccount', { provider: provider }, function (data) {
        if (data.code === 0) {
            window.location.reload();
        } else {
            alert(data.message);
        }
    });
};