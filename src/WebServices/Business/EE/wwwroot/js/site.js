var subscribe = function (id) {
    $.post('/Course/Subscribe/' + id, function (data) {
        if (data.code === 0) {
            $('#subscribe')
                .removeClass('btn-primary')
                .addClass('btn-danger')
                .attr("href", "javascript:unsubscribe(" + id + ")")
                .html('UnSubscribe');
        } else {
            alert(data.message);
        }
    });
}

var unsubscribe = function (id) {
    $.post('/Course/UnSubscribe/' + id, function (data) {
        if (data.code === 0) {
            $('#subscribe')
                .removeClass('btn-danger')
                .addClass('btn-primary')
                .attr("href", "javascript:subscribe(" + id + ")")
                .html('Subscribe');
        } else {
            alert(data.message);
        }
    });
}

var follow = function (id) {
    $.post('/Profile/Follow/' + id, function(data) {
        if (data.code === 0) {
            $('#follow')
                .removeClass('btn-primary')
                .addClass('btn-danger')
                .attr('href', 'javascript:unFollow("' + id + '")')
                .html('Unfollow');
        } else {
            alert(data.message);
        }
    });
}

var unFollow = function (id) {
    $.post('/Profile/UnFollow/' + id, function(data) {
        if (data.code === 0) {
            $('#follow')
                .removeClass('btn-danger')
                .addClass('btn-primary')
                .attr('href', 'javascript:follow("' + id + '")')
                .html('follow');
        } else {
            alert(data.message);
        }
    });
}