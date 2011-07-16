var webForm = {

    postback: function (obj) {
        var form = obj.closest('form');
        var es = obj.attr('id');
        var et = obj.attr('data-server-event');
        var er = obj.attr('data-server-event-arg');

        form.append($('<input/>').attr('type', 'hidden').attr('name', '__event_source').val(es))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__event_target').val(et))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__event_argument').val(er))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__scroll_x').val(window.pageXOffset))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__scroll_y').val(window.pageYOffset));

        for (var item in this) {
            if (typeof (this[item]) != 'function') {
                form.append($('<input/>').attr('type', 'hidden').attr('name', "@" + item).val(JSON.stringify(this[item])));
            }
        }

        form.submit();
        obj.attr('disabled', 'disabled');
    },

    init: function () {

        $('*[data-runat="server"]').click(function (e) {
            e.preventDefault();
            webForm.postback($(this));
        });

        $('*[data-runat="server-auto"]').change(function (e) {
            e.preventDefault();
            webForm.postback($(this));
        });
    }
}