$(window).on('load', function () {
    GetStudentProfileDetailsForSideBar();
    GetCartDetailsToNotificationList();
    GetStudentDashboardBoxDetails();

    var oTable = $('#recPendingTable').DataTable({
        "ajax": {
            "url": '/Dashboard/GetPendingExams?LoginId=' + $("#loginId").val(),
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamNo", "autoWidth": true },
            { "data": "PaperName", "autoWidth": true },
            { "data": "RegisterdDate", "autoWidth": true },
            {
                "data": "FirstPaperId", "Width": "50px", "render": function (data) {
                    if (data === '') {
                        var color = 'green';
                        return '<span style="color:' + color + '">Completed</span>';
                    } else {
                        return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" onclick="MoveToMcqPage(' + data + ')"><i class="fa fa-play-circle mr-2"></i>Start</button>';
                    }
                }
            },
            {
                "data": "SecondPaperId", "Width": "50px", "render": function (data) {
                    if (data === '') {
                        var color = 'green';
                        return '<span style="color:' + color + '">Completed</span>';
                    } else {
                        return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" onclick="MoveToSecondExam(' + data + ')"><i class="fa fa-play-circle mr-2"></i>Start</button>';
                    }
                }
            }
        ]
    });
})




//Side bar student details load
function GetStudentProfileDetailsForSideBar() {
    $.ajax({
        type: "Get",
        url: "/Dashboard/GetStudentProfileDetails?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $('#pStudentCollege').text(result.dataList[0].School);
                $('#astudentName').text(result.dataList[0].StudentFirstName + " " + result.dataList[0].StudentLastName);
                $('#spanNavBarName').text(result.dataList[0].StudentFirstName);
                $("#headerName").text("Welcome, " + result.dataList[0].StudentFirstName + " " + result.dataList[0].StudentLastName);
            }
        }
    })
}

//Cart notifications load
function GetCartDetailsToNotificationList() {
    $.ajax({
        type: "Get",
        url: "/Dashboard/GetCartDetailsToNotificationList?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $('#cartNotiList').empty();
                $('#spanCartCount').text(result.count);
                $.each(result.dataList, function (key, value) {
                    //alert(value[0].PaperName);
                    var SetData = $("#cartNotiList");

                    var Data = '<a href="/Dashboard/Cart" class="text-reset notification-item">' +
                        '<div class="media">' +
                        '<div class="avatar-xs mr-3">' +
                        '<span class="avatar-title bg-primary rounded-circle font-size-16">' +
                        '<i class="bx bx-cart"></i>' +
                        '</span>' +
                        '</div>' +
                        '<div class="media-body">' +
                        '<h6 class="mt-0 mb-1">Your order is placed</h6>' +
                        '<div class="font-size-12 text-muted">' +
                        '<p class="mb-1">' + value.ItemName + '</p>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</a>';

                    SetData.append(Data);
                });
            }
        }
    })
}

function GetStudentDashboardBoxDetails() {
    $.ajax({
        type: "Post",
        url: "/Dashboard/GetStudentDashboardBoxDetails?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $('#divExmPending').text(result.pendingExamsCount);
                $('#divExmCompleted').text(result.completedExamsCount);
                $('#divExmOver70Marks').text(result.over70ExamsCount);
                $('#divExmLess40Marks').text(result.less40ExamsCount);
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

function MoveToMcqPage(ExamId) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/MoveToMcqExam",
        data: { "ExamId": ExamId },
        success: function (result) {
            if (result.success) {
                if (result.hasMcq) {
                    window.location.href = url;
                } else {
                    window.location.href = url3;
                }
            }

            //window.open(url, '_blank');
        }
    })
}

function MoveToSecondExam(ExamId) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/MoveToSecondExam",
        data: { "ExamId": ExamId },
        success: function (result) {
            window.location.href = url2;
            //window.open(url2, '_blank');
        }
    })
}

$('#btnCompleteProfile').click(function () {
    window.location.href = url4;
})

$('#getStartedBtn').click(function () {
    window.location.href = url5;
})

