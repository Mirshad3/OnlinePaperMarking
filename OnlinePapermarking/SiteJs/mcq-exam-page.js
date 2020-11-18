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


$('#radioBtnList .btnSpan').on('click', function (event) {
    //console.log($(this).find('input').val());
    var val = $(this).find('input').val();
    //alert(val);

    $.ajax({
        type: "Post",
        url: "/Exam/UpdateMcqAnswer",
        data: { "ExamId": $('#examId').val(), "LoginId": $('#loginId').val(), "Answer": val },
        success: function (result) {
            if (result.success) {
                
            }
        }
    })
    //$('#output').html(val);
});

$("#submitBtn1").click(function () {
    SubmitAnswers();
})

$("#submitBtn2").click(function () {
    SubmitAnswers();
})

function SubmitAnswers() {
    $.ajax({
        type: "Post",
        url: "/Exam/SubmitMcqAnswerSheet",
        data: { "ExamId": $('#examId').val(), "LoginId": $('#loginId').val() },
        success: function (result) {
            if (result.success) {
                $("#btnCloseTimeUp").trigger("click");
                $("#modalP").text(result.message);
                $("#popunBtn").trigger("click");
            }
        }
    })
}

$("#modalOkBtn").click(function () {
    window.location.href = url;
})

function LoadMcqAnswersList() {
    $.ajax({
        type: "Post",
        url: "/Exam/LoadMcqAnswersList",
        data: {"PastPaperId" : 22},
        success: function (result) {
            if (result.success) {
                $("#olList").empty();
                for (i = 0; i < result.questionCount; i++) {
                    var data = '<li>' +
                        '<span class="btnSpan">' +
                        '<input type="radio" value="' + i +'.1" name="31" id="31-1" />' +
                        '<label for="31-1"></label>' +
                        '</span>' +
                        '<span class="btnSpan">' +
                        '<input type="radio" value="' + i +'.2" name="31" id="31-1" />' +
                        '<label for="31-1"></label>' +
                        '</span>' +
                        '<span class="btnSpan">' +
                        '<input type="radio" value="' + i +'.3" name="31" id="31-1" />' +
                        '<label for="31-1"></label>' +
                        '</span>' +
                        '<span class="btnSpan">' +
                        '<input type="radio" value="' + i +'.4" name="31" id="31-1" />' +
                        '<label for="31-1"></label>' +
                        '</span>' +
                        '</li>';

                    $("#olList").append(data);
                } 
            }
        }
    })
}

function GetFirstPaperTime() {
    $.ajax({
        type: "Post",
        url: "/Exam/GetFirstPaperTime",
        data: { "ExamId": $("#examId").val() },
        success: function (result) {
            if (result.success) {
                $('#cd_seconds').val(result.time);
            } else {

            }
        }
    })
}