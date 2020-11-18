$(document).ready(function () {
    
    var today = new Date();
    var firstDate = new Date(today.getFullYear(), today.getMonth(), 1);
    $('#dpFromDate').val(moment(firstDate).format("YYYY-MM-DD"));
    $('#dpToDate').val(moment(today).format("YYYY-MM-DD"));

    var oTable = $('#paymentTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetStudentPayments?FromDate=' + $('#dpFromDate').val() + '&ToDate=' + $('#dpToDate').val(),
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "OrderNo", "autoWidth": true },
            { "data": "PaymentDate", "autoWidth": true },
            { "data": "StudentName", "autoWidth": true },
            { "data": "Email", "autoWidth": true },
            { "data": "ItemName", "autoWidth": true },
            { "data": "UnitPrice", "autoWidth": true },
            { "data": "Price", "autoWidth": true, "visible":false },
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
                .column(6)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            

            // Total over this page
            pageTotal = api
                .column(6, { page: 'current' })
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            //alert(total);
            //alert(pageTotal);

            // Update footer
            $(api.column(5).footer()).html(
                'Rs.' + pageTotal + ' ( Rs.' + total + ' total)'
            );
            //$(api.column(4).footer()).html(total);
        }
    });

})

$('#searchBtn').click(function () {
    $('#paymentTable').DataTable().ajax.url('/AdminPanel/GetStudentPayments?FromDate=' + $('#dpFromDate').val() + '&ToDate=' + $('#dpToDate').val()).load();
})