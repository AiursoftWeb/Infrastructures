$(document).ready(function () {
    // Activate clipboard tool
    new ClipboardJS('[data-clipboard-text]');

    // Activate tooltip tool
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="tooltip"]').on('click', function () {
        setTimeout(function () {
            $('[data-toggle="tooltip"]').tooltip('hide');
        }, 2000);
    });

    //Seems useless
    // $.fn.top = function () {
    //     $('html, body').animate({
    //         scrollTop: $(this).offset().top + 'px'
    //     }, 'medium');
    //     return this;
    // }

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
});