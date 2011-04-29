var webForm = {
    init: function () {
        $('*[data-runat="server"]').click(function (e) {
            e.preventDefault();
            var form = $(this).closest('form');
            var es = $(this).attr('id');
            var et = $(this).attr('data-server-event');
            var er = $(this).attr('data-server-event-arg');

            form.append($('<input/>').attr('type', 'hidden').attr('name', '__event_source').val(es))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__event_target').val(et))
                .append($('<input/>').attr('type', 'hidden').attr('name', '__event_argument').val(er))
                .submit();

            $(this).attr('disabled', 'disabled');
        });
    }
}