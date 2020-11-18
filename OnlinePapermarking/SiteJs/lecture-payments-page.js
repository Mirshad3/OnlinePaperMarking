$(document).ready(function () {

    var today = new Date();
    var firstDate = new Date(today.getFullYear(), today.getMonth(), 1);

    $('#dpPendingFromDate').val(moment(firstDate).format("YYYY-MM-DD"));
    $('#dpPendingToDate').val(moment(today).format("YYYY-MM-DD"));
    $('#dpPaidFromDate').val(moment(firstDate).format("YYYY-MM-DD"));
    $('#dpPaidToDate').val(moment(today).format("YYYY-MM-DD"));

    var oTable = $('#recPendingTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetPendingLecturePayments?FromDate=' + $('#dpPendingFromDate').val() + '&ToDate=' + $('#dpPendingToDate').val(),
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamID", "autoWidth": true },
            { "data": "ExamNo", "autoWidth": true },
            { "data": "TutorCompletedDate", "autoWidth": true },
            { "data": "TutorName", "autoWidth": true },
            { "data": "TutorEmail", "autoWidth": true },
            { "data": "PaperName", "autoWidth": true },
            { "data": "PriceForTutor", "autoWidth": true },
            { "data": "Price", "autoWidth": true, "visible":false },
        ],
        'columnDefs': [
            {
                'targets': 0,
                'checkboxes': {
                    'selectRow': true
                }
            }
        ],
        'select': {
            'style': 'multi'
        },
        "footerCallback": function (row, data, start, end, display) {
            var api = this.api(), data;

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ?
                    i.replace(/[\$,]/g, '') * 1 :
                    typeof i === 'number' ?
                        i : 0;
            };

            // Total over all pages
            total = api
                .column(7)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);



            // Total over this page
            pageTotal = api
                .column(7, { page: 'current' })
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            //alert(total);
            //alert(pageTotal);

            // Update footer
            $(api.column(6).footer()).html(
                'Rs.' + pageTotal + ' ( Rs.' + total + ' total)'
            );
            //$(api.column(4).footer()).html(total);
        }
    });

    var bTable = $('#recApprovedTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetPaidLecturePayments?FromDate=' + $('#dpPaidFromDate').val() + '&ToDate=' + $('#dpPaidToDate').val(),
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamNo", "autoWidth": true },
            { "data": "TutorCompletedDate", "autoWidth": true },
            { "data": "TutorName", "autoWidth": true },
            { "data": "TutorEmail", "autoWidth": true },
            { "data": "PaperName", "autoWidth": true },
            { "data": "PaidDateForTutor", "autoWidth": true },
            { "data": "PriceForTutor", "autoWidth": true },
            { "data": "Price", "autoWidth": true, "visible": false },
        ],
        "footerCallback": function (row, data, start, end, display) {
            var api = this.api(), data;

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ?
                    i.replace(/[\$,]/g, '') * 1 :
                    typeof i === 'number' ?
                        i : 0;
            };

            // Total over all pages
            total = api
                .column(7)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);



            // Total over this page
            pageTotal = api
                .column(7, { page: 'current' })
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            //alert(total);
            //alert(pageTotal);

            // Update footer
            $(api.column(6).footer()).html(
                'Rs.' + pageTotal + ' ( Rs.' + total + ' total)'
            );
            //$(api.column(4).footer()).html(total);
        }
    });

    $('#submitBtn').click(function () {
        var rows_selected = oTable.column(0).checkboxes.selected();
        var isFirst = true;
        // Iterate over all selected checkboxes
        $.each(rows_selected, function (index, rowId) {
            Submit(rowId, isFirst);
            isFirst = false;
        });

        $('#recPendingTable').DataTable().ajax.url('/AdminPanel/GetPendingLecturePayments?FromDate=' + $('#dpPendingFromDate').val() + '&ToDate=' + $('#dpPendingToDate').val()).load();
    })
})

$('#searchPendingBtn').click(function () {
    $('#recPendingTable').DataTable().ajax.url('/AdminPanel/GetPendingLecturePayments?FromDate=' + $('#dpPendingFromDate').val() + '&ToDate=' + $('#dpPendingToDate').val()).load();
})

$('#searchPaidBtn').click(function () {
    $('#recApprovedTable').DataTable().ajax.url('/AdminPanel/GetPaidLecturePayments?FromDate=' + $('#dpPaidFromDate').val() + '&ToDate=' + $('#dpPaidToDate').val()).load();
})

function Submit(ExamId, isFirst) {
    $.ajax({
        type: "POST",
        url: "/AdminPanel/SubmitPaid?ExamId=" + ExamId + "&LoginId=" + $('#loginId').val(),
        success: function (response) {
            if (response.success) {
                $('#recPendingTable').DataTable().ajax.url('/AdminPanel/GetPendingLecturePayments?FromDate=' + $('#dpPendingFromDate').val() + '&ToDate=' + $('#dpPendingToDate').val()).load();
                $('#recApprovedTable').DataTable().ajax.url('/AdminPanel/GetPaidLecturePayments?FromDate=' + $('#dpPaidFromDate').val() + '&ToDate=' + $('#dpPaidToDate').val()).load();
                if (isFirst) {
                    Notification("Success", "Submitted Successfully", "success");
                }
            }
            else {
                Notification("Error", response.message, "danger");
            }
        }
    });
}

function Notification(title, message, type) {
    //Right Top Notification
    $.notify({
        title: '<strong>' + title + '</strong>',
        message: "<br>" + message + "",
        icon: 'fa fa-check',
        //url: '/Dashboard/Cart',
        target: ''
    }, {
        // settings
        element: 'body',
        //position: null,
        type: "" + type + "",
        allow_dismiss: false,
        //newest_on_top: false,
        showProgressbar: false,
        placement: {
            from: "top",
            align: "right"
        },
        offset: 20,
        spacing: 10,
        z_index: 1031,
        delay: 1000,
        timer: 4000,
        url_target: '',
        mouse_over: null,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutRight'
        },
        onShow: null,
        onShown: null,
        onClose: null,
        onClosed: null,
        icon_type: 'class',
    });
}

