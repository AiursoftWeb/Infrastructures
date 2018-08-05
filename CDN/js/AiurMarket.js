$(document).ready(function () {
    $(document).scroll(function () {
        var scrollDistance = $(this).scrollTop();
        if (scrollDistance > 100) {
            $('.scroll-to-top').fadeIn();
        } else {
            $('.scroll-to-top').fadeOut();
        }
    });
    $(document).on('click', 'a.scroll-to-top', function (event) {
        $('html, body').animate({ scrollTop: 0 }, 'fast');
        event.preventDefault();
    });
});