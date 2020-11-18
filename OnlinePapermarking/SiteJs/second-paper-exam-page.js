$(document).ready(function () {
    $.fn.dataTableExt.sErrMode = 'throw';

    $('#errorTable').on('error.dt', function (e, settings, techNote, message) {
        window.location.href = "/";
    });

    var oTable = $('#errorTable').DataTable({
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

    GetSecondPaperTime();
})



$("#btnUpload").click(function () {
    if (window.FormData !== undefined) {
        var fileUpload = $("#imgInp").get(0);
        var files = fileUpload.files;

        // Create FormData object
        var fileData = new FormData();

        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        fileData.append("LoginId", $("#loginId").val());
        fileData.append("ExamId", $("#examId").val());
        
        $.ajax({
            type: "Post",
            url: "/Exam/UploadSecondPaperAnswers",
            contentType: false, // Not to set any content header
            processData: false,
            data: fileData,
            success: function (result) {
                if (result.success) {
                    if (result.hasThirdPaper) {
                        if (result.hasMcq) {
                            if (result.isThirdPaperCompleted == true) {
                                $("#modalP").text("Upload Success. Your paper assigned to a our tutor successfully.");
                                $("#popunBtn").trigger("click");
                                $("#isRedirect").val('true');
                            } else {
                                $("#modalP").text("Upload Success. Please complete your third paper to assign this paper to a tutor.");
                                $("#popunBtn").trigger("click");
                                $("#isRedirect").val('false');
                                $('.final-countdown-start').slideDown(0);
                                $('.upload-main-card,.paper-upload-section').hide();
                                $('.final-countdown-upload').hide();
                                $('#cd_start').show();
                                $('#cd_seconds').val(result.thirdPaperTime);
                                $('#headerPaperName').text(result.paperName);
                                $.APP.resetTimer();
                            }
                        } else {
                            if (result.isThirdPaperCompleted == true && result.isFirstPaperCompleted == true) {
                                $("#modalP").text("Upload Success. Your paper assigned to a our tutor successfully.");
                                $("#popunBtn").trigger("click");
                                $("#isRedirect").val('true');
                            } else if (result.isThirdPaperCompleted == false) {
                                $("#modalP").text("Upload Success. Please complete your third paper to assign this paper to a tutor.");
                                $("#popunBtn").trigger("click");
                                $("#isRedirect").val('false');
                                $('.final-countdown-start').slideDown(0);
                                $('.upload-main-card,.paper-upload-section').hide();
                                $('.final-countdown-upload').hide();
                                $('#cd_start').show();
                                $('#cd_seconds').val(result.thirdPaperTime);
                                $('#headerPaperName').text(result.paperName);
                                $.APP.resetTimer();
                            } else if (result.isFirstPaperCompleted == false) {
                                $("#modalP").text("Upload Success. Please complete your first paper to assign this paper to a tutor.");
                                $("#popunBtn").trigger("click");
                                $("#isRedirect").val('true');
                            }
                        }
                    } else {
                        if (result.hasMcq || (result.hasMcq == false && result.isFirstPaperCompleted == true)) {
                            $("#modalP").text("Upload Success. Your paper assigned to a our tutor successfully.");
                            $("#popunBtn").trigger("click");
                            $("#isRedirect").val('true');
                        } else if (result.hasMcq == false && result.isFirstPaperCompleted == false) {
                            $("#modalP").text("Upload Success. Please complete your first paper to assign this paper to a tutor.");
                            $("#popunBtn").trigger("click");
                            $("#isRedirect").val('true');
                        }
                    }
                }
            }
        })
    }
});

$("#modalOkBtn").click(function () {
    if ($("#isRedirect").val() === 'true') {
        window.location.href = "/Dashboard/Exams";
    }
})

$('#goToMapBtn').click(function () {
    window.open('/Exam/MapPage?ExamId=' + $('#examId').val());
})

function GetSecondPaperTime() {
    $.ajax({
        type: "Post",
        url: "/Exam/GetSecondPaperTime",
        data: { "ExamId": $("#examId").val() },
        success: function (result) {
            if (result.success) {
                $('#cd_seconds').val(result.time);
            } else {

            }
        }
    })
}