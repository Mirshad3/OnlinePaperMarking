$(document).ready(function () {
    GetTutorProfileDetailsForSideBar();
    GetTutorNotifications();
    GetTutorDashboardBoxDetails();

    var oTable = $('#recPendingTable').DataTable({
        "ajax": {
            "url": '/TutorDashboard/GetNotAssignedExamsForTutor?LoginId=' + $("#loginId").val(),
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamNo", "autoWidth": true },
            { "data": "StudentName", "autoWidth": true },
            { "data": "ExamType", "autoWidth": true },
            { "data": "Subject", "autoWidth": true, "className": "text-left" },
            { "data": "Medium", "autoWidth": true },
            { "data": "Year", "autoWidth": true },
            {
                "data": "ExamID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-success" onclick="Approve(' + data + ')"><i class="fa fa-check mr-2"></i>Approve</button>';
                }
            }
        ]
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
                $("#headerName").text("Welcome, " + result.dataList[0].TutorFirstName + " " + result.dataList[0].TutorLastName);
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

//To redirect when notification click
function SelectNotification(AssignId) {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SelectNotification",
        data: { "AssignId": AssignId },
        success: function (result) {
            if (result.success) {
                window.location.href = "/" + result.controller + "/" + result.method;
            }
        }
    })
}

//Approve exams
function Approve(ExamId) {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/ApproveExam",
        data: { "ExamId": ExamId, "TutorLoginId": $("#loginId").val() },
        success: function (result) {
            if (result.success) {
                $('#recPendingTable').DataTable().ajax.reload();
                $('#recApprovedTable').DataTable().ajax.reload();
            }
        }
    })
}

function GetTutorDashboardBoxDetails() {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/GetTutorDashboardBoxDetails?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $('#divMarkingPending').text(result.pendingExamsCount);
                $('#divMarkingCompleted').text(result.completedExamsCount);
                $('#divMoneyToRecieve').text(result.moneyToBeRecieve);
                $('#divMoneyRecieved').text(result.moneyRecieved);
                $('#headerProfileCompletion').text(result.percentage + '%');
                $('#divProgressBar').empty();

                if (result.percentage < 100) {
                    $('#btnCompleteProfile').show();
                }

                var data = '<div class="progress-bar" role="progressbar" style="width: ' + result.percentage + '%" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100"></div>';
                $('#divProgressBar').append(data);
            }
        }
    })
}

$('#btnCompleteProfile').click(function () {
    window.location.href = url1;
})