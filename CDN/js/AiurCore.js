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
});