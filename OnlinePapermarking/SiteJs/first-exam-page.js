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

    GetFirstPaperTime();
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
            url: "/Exam/UploadFirstPaperAnswers",
            contentType: false, // Not to set any content header
            processData: false,
            data: fileData,
            success: function (result) {
                if (result.success) {
                    if (result.isSecondPaperCompleted) {
                        $("#modalP").text("Upload Success. Your paper assigned to a our tutor successfully.");
                        $("#popunBtn").trigger("click");
                    } else if (result.isSecondPaperCompleted == false) {
                        $("#modalP").text("Upload Success. Please complete your second paper to assign this paper to a tutor.");
                        $("#popunBtn").trigger("click");
                    }
                }
            }
        })
    }
});

$("#modalOkBtn").click(function () {
    window.location.href = "/Dashboard/Exams";
})

function GetFirstPaperTime() {
    $.ajax({
        type: "Post",
        url: "/Exam/GetFirstPaperTime",
        data: { "ExamId": $("#examId").val()},
        success: function (result) {
            if (result.success) {
                $('#cd_seconds').val(result.time);
            } else {
                
            }
        }
    })
}