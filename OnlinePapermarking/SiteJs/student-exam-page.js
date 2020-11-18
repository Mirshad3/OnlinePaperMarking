$(document).ready(function () {

    GetStudentProfileDetailsForSideBar();
    GetCartDetailsToNotificationList();

    $.fn.dataTableExt.sErrMode = 'throw';

    $('#recPendingTable').on('error.dt', function (e, settings, techNote, message) {
        window.location.href = "/";
    });

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

    var bTable = $('#recApprovedTable').DataTable({
        "ajax": {
            "url": '/Dashboard/GetCompletedExams?LoginId=' + $("#loginId").val(),
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "FirstPaperMarks", "autoWidth": true, "visible": false },
            { "data": "FirstPaperPercentMarks", "autoWidth": true, "visible": false },
            { "data": "SecondPaperMarks", "autoWidth": true, "visible": false },
            { "data": "SecondPaperPercentMarks", "autoWidth": true, "visible": false },
            { "data": "ExamNo", "autoWidth": true },
            { "data": "PaperName", "autoWidth": true },
            { "data": "RegisterdDate", "autoWidth": true },
            {
                "data": "TutorStatus", "Width": "50px", "render": function (data, type, row, meta) {

                    if (data === 'Pending') {
                        return '<span class="pr-2 text-danger">Marking Pending</span>';
                    } else if (data === 'Completed') {
                        return '<span class="pr-2 text-success">Marking Completed</span>';
                    } else {
                        return '<span class="pr-2 text-danger">' + data + '</span>';
                    }
                }
            },
            {
                "data": "ExamID", "Width": "50px", "render": function (data, type, row, meta) {
                    
                    if (row.TutorStatus == 'Pending') {
                        return '<span class="pr-2 text-danger">' + row.FirstPaperMarks + '/40</span>' + '</br> <button type="button" class="btn btn-outline-info waves-effect waves-light" data-toggle="modal" data-target=".view-paper1" onclick="ViewFirstPaper(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                    } else if (row.TutorStatus == 'Completed') {
                        return '<span class="pr-2 text-success">' + row.FirstPaperMarks + '/40</span>' + '</br> <button type="button" class="btn btn-outline-info waves-effect waves-light" data-toggle="modal" data-target=".view-paper1" onclick="ViewFirstPaper(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                    } else {
                        return '<span class="pr-2 text-danger">0.00/40 (00%)</span>' + '</br> <button type="button" class="btn btn-outline-info waves-effect waves-light" data-toggle="modal" data-target=".view-paper1" onclick="ViewSecondPaper(' + data + ')"><i class="fa fa-eye" aria-hidden="true"></i>View</button>';
                    }
                }
            },
            {
                "data": "ExamID", "Width": "50px", "render": function (data, type, row, meta) {
                    
                    if (row.TutorStatus == 'Pending') {
                        return '<span class="pr-2 text-danger">' + row.SecondPaperMarks + '/60</span>' + '</br> <button type="button" class="btn btn-outline-info waves-effect waves-light" data-toggle="modal" data-target=".view-paper2" onclick="ViewSecondPaper(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                    } else if (row.TutorStatus == 'Completed') {
                        return '<span class="pr-2 text-success">' + row.SecondPaperMarks + '/60</span>' + '</br> <button type="button" class="btn btn-outline-info waves-effect waves-light" data-toggle="modal" data-target=".view-paper2" onclick="ViewSecondPaper(' + data + ')"><i class="fa fa-eye mr-2""></i>View</button>';
                    } else {
                        return '<span class="pr-2 text-danger">0.00/60 (00%)</span>' + '</br> <button type="button" class="btn btn-outline-info waves-effect waves-light" data-toggle="modal" data-target=".view-paper1" onclick="ViewSecondPaper(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                    }
                }
            },
            {
                "data": "TotalMarks", "Width": "50px", "render": function (data, type, row, meta) {

                    if (row.TutorStatus == 'Pending') {
                        return '<span class="pr-2 text-danger">' + data + '</span>';
                    } else if (row.TutorStatus == 'Completed') {
                        return '<span class="pr-2 text-success">' + data + '</span>';
                    } else {
                        return '<span class="pr-2 text-danger">' + data + '</span>';
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

function MoveToMcqPage(ExamId) {
        
    $.ajax({
        type: "Post",
        url: "/Dashboard/MoveToMcqExam",
        data: { "ExamId": ExamId},
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

function ViewFirstPaper(ExamId) {
    $('#examId').val(ExamId);
    $.ajax({
        type: "Post",
        url: "/Dashboard/CheckHasMcq",
        data: { "ExamId": ExamId },
        success: function (result) {
            if (result.success) {
                if (result.hasMcq) {

                    $.ajax({
                        type: "Post",
                        url: "/Dashboard/GetMcqPaperAnswers",
                        data: { "ExamId": ExamId },
                        success: function (result) {
                            $(':radio,:checkbox').prop("checked", false);
                            $('.btnSpan').removeClass("mcq-incorrect");
                            $('.btnSpan').removeClass("mcq-correct");
                            $('#radioBtnList').show();
                            $('#uploadedDetails').hide();
                            if (result.success) {
                                $.each(result.dataList, function (key, value) {
                                    if (value.CorrectAnswer === value.StudentAnswer) {
                                        $("#" + value.StudentQuestionNumber + "-" + value.StudentAnswer).prop("checked", true);
                                    } else {
                                        $("#" + value.StudentQuestionNumber + "-" + value.StudentAnswer).prop("checked", true);
                                        $("#sp-" + value.StudentQuestionNumber + "-" + value.StudentAnswer).addClass("mcq-incorrect");
                                        $("#sp-" + value.CorrectQuestionNumber + "-" + value.CorrectAnswer).addClass("mcq-correct");
                                    }
                                });
                            }

                            $('#headerPaper1Name').text(result.paperName);
                        }
                    })
                } else {
                    $.ajax({
                        type: "Post",
                        url: "/Dashboard/GetFirstPaperTutorComment",
                        data: { "ExamId": ExamId },
                        success: function (result) {
                            $('#radioBtnList').hide();
                            $('#uploadedDetails').show();
                            $('#headerPaper1Name').text(result.paperName);
                            $('#txtAreaPaper1Comment').val(result.comment);
                        }
                    })
                }

            }
        }
    })

    //alert(hasMcq);

    
    
}

$(':radio,:checkbox').click(function () {
    return false;
});

function ViewSecondPaper(ExamId) {
    $('#examId').val(ExamId);
    $.ajax({
        type: "Post",
        url: "/Dashboard/GetSecondPaperTutorComment",
        data: { "ExamId": ExamId },
        success: function (result) {
            $('#txtAreaPaper2Comment').val(result.comment);
            $('#headerPaper2Name').text(result.paperName);
        }
    })
}

$("#downloadBtn1").click(function () {
    DownloadFirstPaperMarkedSheets($('#examId').val());
})

$("#downloadBtn").click(function () {
    DownloadSecondPaperMarkedSheets($('#examId').val());
})

//Download attach files in tutor completed second paper
function DownloadFirstPaperMarkedSheets(ExamId) {
    debugger
    var ajax = new XMLHttpRequest();
    ajax.open("Post", "/TutorDashboard/DownloadFirstPaperMarkedSheets?ExamId=" + ExamId, true);
    ajax.responseType = "blob";
    ajax.onreadystatechange = function () {
        if (this.readyState == 4) {
            debugger;
            var blob = new Blob([this.response], { type: "application/octet-stream" });
            var fileName = "Answers.zip";
            saveAs(blob, fileName);

        }
    };
    ajax.send(null);
}

//Download attach files in tutor completed second paper
function DownloadSecondPaperMarkedSheets(ExamId) {
    debugger
    var ajax = new XMLHttpRequest();
    ajax.open("Post", "/TutorDashboard/DownloadSecondPaperMarkedSheets?ExamId=" + ExamId, true);
    ajax.responseType = "blob";
    ajax.onreadystatechange = function () {
        if (this.readyState == 4) {
            debugger;
            var blob = new Blob([this.response], { type: "application/octet-stream" });
            var fileName = "Answers.zip";
            saveAs(blob, fileName);

        }
    };
    ajax.send(null);
}

function ViewMyAnswers() {
    window.open('/Dashboard/ViewMyAnswers?ExamId=' + $('#examId').val());
}

function ViewMyAnswersForFirstPaper() {
    window.open('/Dashboard/ViewAnswerImagesForFirstPaper?ExamId=' + $('#examId').val());
}

function ViewTutorUploads() {
    window.open('/Dashboard/ViewTutorUploads?ExamId=' + $('#examId').val());
}

function ViewTutorUploadsForFirstPaper() {
    window.open('/Dashboard/ViewTutorUploadsForFirstPaper?ExamId=' + $('#examId').val());
}
