$(document).ready(function () {
    $('#comment_form').jqDynaForm();
    $('#comment_submit').click(function () {
        var json = $('#comment_form').jqDynaForm('get');
        $.get('Forum/ForumAjax.aspx', json, function (data) {
            var el = $(data).appendTo(".comments");
            if (!el.hasClass("comment-error")) {
                $('#comment_form').jqDynaForm('set', { name: '', email: '', message_body: '' });
                $('.comment-error').remove();
            }
        }, "html");

    });
});
