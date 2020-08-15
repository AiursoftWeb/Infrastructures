$(document).ready(function () {
    var tempHtmlStorage = "";
    var hovering = false;
    $('*[data-hover-video]').each(function () {
        var placeholder = $(this);
        var innerPlace = $(this).find("[data-place]");
        placeholder.hover(function () {
            if (hovering) return;
            hovering = true;
            tempHtmlStorage = innerPlace.html();
            innerPlace.html('<video id="preview" loop="loop" autoplay="autoplay" muted="muted" style="width:100%" src="' + placeholder.attr('data-hover-video') + '"></video>');
            setTimeout(function () {
                document.getElementById('preview').addEventListener('loadedmetadata', function () {
                    this.currentTime = 0;
                }, false);
            }, 1);
        },
            function () {
                hovering = false;
                innerPlace.html(tempHtmlStorage);
            });
    });

    $('[type=search]').on('input', () => {
        var searchTerm = $('[type=search]').val();
        if (!searchTerm) {
            $('#auto-complete').html('');
        }
        $.get(`/suggestion/${searchTerm}`, data => {
            $('#auto-complete').html('');
            data.forEach(suggestion => {
                $('#auto-complete').append(`<option value="${suggestion}" />`);
            });
        });
    });
});
