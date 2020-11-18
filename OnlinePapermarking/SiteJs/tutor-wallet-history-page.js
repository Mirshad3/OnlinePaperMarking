$(document).ready(function () {
    
        GetTutorProfileDetailsForSideBar();
        GetTutorNotifications();

        var oTable = $('#recPendingTable').DataTable({
            "ajax": {
                "url": '/TutorDashboard/GetTutorWalletHistory?LoginId=' + $("#loginId").val() + '&IsPaid=' + false,
                "type": "get",
                "datatype": "json"
            },
            "columns": [
                { "data": "ExamNo", "autoWidth": true },
                { "data": "PaperName", "autoWidth": true },
                { "data": "TutorCompletedDate", "autoWidth": true },
                { "data": "PriceForTutor", "autoWidth": true },
                { "data": "CalPrice", "autoWidth": true, "visible": false }
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
                    .column(4)
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0);



                // Total over this page
                pageTotal = api
                    .column(4, { page: 'current' })
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0);

                //alert(total);
                //alert(pageTotal);

                // Update footer
                $(api.column(3).footer()).html(
                    'Rs.' + pageTotal + ' ( Rs.' + total + ' total)'
                );
                //$(api.column(4).footer()).html(total);
            }
        });

    var bTable = $('#recApprovedTable').DataTable({
        "ajax": {
            "url": '/TutorDashboard/GetTutorWalletHistory?LoginId=' + $("#loginId").val() + '&IsPaid=' + true,
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamNo", "autoWidth": true },
            { "data": "PaperName", "autoWidth": true },
            { "data": "TutorCompletedDate", "autoWidth": true },
            { "data": "PaidDate", "autoWidth": true },
            { "data": "PriceForTutor", "autoWidth": true },
            { "data": "CalPrice", "autoWidth": true, "visible": false }
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
                .column(5)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);



            // Total over this page
            pageTotal = api
                .column(5, { page: 'current' })
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            //alert(total);
            //alert(pageTotal);

            // Update footer
            $(api.column(4).footer()).html(
                'Rs.' + pageTotal + ' ( Rs.' + total + ' total)'
            );
            //$(api.column(4).footer()).html(total);
        }
    });

        
});

//Side bar tutor details load
function GetTutorProfileDetailsForSideBar() {
    $.ajax({
        type: "Get",
        url: "/TutorDashboard/GetTutorProfileDetailsForSideBar?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $("#sideBarWorkingDetails").empty();
                $("#sideBarWorkingDetails").append(result.dataList[0].Occupation + ", " + result.dataList[0].School_University + "</br>Colombo District");
                $("#sideBarName").text(result.dataList[0].TutorFirstName + " " + result.dataList[0].TutorLastName);
                $("#upFirstName").text(result.dataList[0].TutorFirstName);
            }
        }
    })
}

//Get tutor's notifications
function GetTutorNotifications() {
    $.ajax({
        type: "Get",
        url: "/TutorDashboard/GetTutorNotifications?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $("#notificationCount").text(result.dataCount);
                $("#notificationList").empty();
                $.each(result.dataList, function (key, value) {
                    var Data = "<a onclick='SelectNotification(" + value.AssignID + ")' class='text-reset notification-item'>" +
                        "<div class='media'>" +
                        //"<div class='avatar-xs mr-3'>" +
                        //"<span class='avatar-title bg - primary rounded - circle font - size - 16'><i class='bx bx-cart'></i></span>" +
                        //"</div>" +
                        "<div class='media-body'>" +
                        "<h6 class='mt-0 mb-1'>" + value.NotificationHeader + "</h6><div class='font-size-12 text-muted'><p class='mb-1'>" + value.NotificationDetail + "</p></div>" +
                        "</div>" +
                        "</div>" +
                        "</a>";

                    $("#notificationList").append(Data);
                })
            }
        }
    })
}