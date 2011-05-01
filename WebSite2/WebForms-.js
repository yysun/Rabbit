var webForm = {

    postback: function (obj) {
        var form = obj.closest('form');
        var es = obj.attr('id');
        var et = obj.attr('data-server-event');
        var er = obj.attr('data-server-event-arg');

        form.append($('<input/>').attr('type', 'hidden').attr('name', '__event_source').val(es))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__event_target').val(et))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__event_argument').val(er))
                .submit();

        obj.attr('disabled', 'disabled');
    },

    init: function () {

        $('*[data-runat="server"]').click(function (e) {
            e.preventDefault();
            webForm.postback($(this));
        });

        $('*[data-autopostback="true"]').change(function (e) {
            e.preventDefault();
            webForm.postback($(this));
        });
    }
}