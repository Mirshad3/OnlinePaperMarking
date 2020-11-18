$(document).ready(function () {

    var today = new Date();
    var firstDate = new Date(today.getFullYear(), today.getMonth(), 1);
    $('#dpFromDate').val(moment(firstDate).format("YYYY-MM-DD"));
    $('#dpToDate').val(moment(today).format("YYYY-MM-DD"));

    var oTable = $('#loggedDetailTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetLoggedDetails?FromDate=' + $('#dpFromDate').val() + '&ToDate=' + $('#dpToDate').val(),
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "Name", "autoWidth": true },
            { "data": "Email", "autoWidth": true },
            {
                "data": "UserType", "Width": "50px", "render": function (data) {
                    if (data === '2') {
                        return '<span>Student</span>';
                    } else {
                        return '<span>Tutor</span>';
                    }
                }
            },
            { "data": "LoggedDate", "autoWidth": true }
        ]
    });

})

$('#searchBtn').click(function () {
    $('#loggedDetailTable').DataTable().ajax.url('/AdminPanel/GetLoggedDetails?FromDate=' + $('#dpFromDate').val() + '&ToDate=' + $('#dpToDate').val()).load();
})