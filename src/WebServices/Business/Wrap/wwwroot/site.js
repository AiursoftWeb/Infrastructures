$(document).ready(function () {
    var detectAppear = function () {
        var urlInput = $(this);
        var urlValue = urlInput.val();
        if (urlValue) {
            $('#record-name-part').removeClass('d-none');
        } else {
            $('#record-name-part').addClass('d-none');
        }
    }

    $('#url').on('input', detectAppear);

    detectAppear();
});