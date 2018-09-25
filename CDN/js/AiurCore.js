$(document).ready(function () {
    // Activate clipboard tool
    new ClipboardJS('[data-clipboard-text]');

    // Activate tooltip tool
    $('[data-toggle="tooltip"]').tooltip();

    //Seems useless
    $.fn.top = function () {
        $('html, body').animate({
            scrollTop: $(this).offset().top + 'px'
        }, 'medium');
        return this;
    }

    //Aiur Scroll to top
    $(document).scroll(function () {
        var scrollDistance = $(this).scrollTop();
        if (scrollDistance > 100) {
            $('.aiur-scroll-to-top').fadeIn();
        } else {
            $('.aiur-scroll-to-top').fadeOut();
        }
    });

    $(document).on('click', 'a.aiur-scroll-to-top', function (event) {
        $('html, body').animate({ scrollTop: 0 }, 1000, 'easeInOutExpo');
        event.preventDefault();
    });

    var convertToImage = function (key, callback) {
        $.get('https://oss.aiursoft.com/api/viewonefile?filekey=' + key, callback);
    };

    // Convert file key to file url
    $('*[data-file-key]').each(function (index, element) {
        var key = $(element).attr('data-file-key');
        convertToImage(key, function (data) {
            if (data.code === 0) {
                $(element).attr('src', data.file.internetPath);
                return;
            }
            if (data.code === -4) {
                $.get('/Auth/Update', function (userdata) {
                    console.log('Updated user data: ' + userdata.Value.headImgFileKey);
                    convertToImage(userdata.Value.headImgFileKey, function (latestImage) {
                        $(element).attr('src', latestImage.file.internetPath);
                        return;
                    });
                });
            }
        });
    });
});