$(document).ready(function () {
    //alert(1);
    //if ($("#loginId").val() === '' || typeof $("#loginId").val() === "undefined") {
    //    CheckCookies();
    //} else {
        GetTutorProfileDetailsForSideBar();
        GetTutorNotifications();

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
                { "data": "Subject", "autoWidth": true, "className": "text-left"},
                { "data": "Medium", "autoWidth": true },
                { "data": "Year", "autoWidth": true },
                {
                    "data": "ExamID", "Width": "50px", "render": function (data) {
                        return '<button type="button" class="btn btn-outline-success" onclick="Approve(' + data + ')"><i class="fa fa-check mr-2"></i>Approve</button>';
                    }
                }
            ]
        });

        var bTable = $('#recApprovedTable').DataTable({
            "ajax": {
                "url": '/TutorDashboard/GetAssignedExamsForTutor?TutorLoginId=' + $("#loginId").val(),
                "type": "get",
                "datatype": "json"
            },
            "columns": [
                { "data": "ExamNo", "autoWidth": true },
                { "data": "StudentName", "autoWidth": true },
                { "data": "ExamType", "autoWidth": true },
                { "data": "Subject", "autoWidth": true },
                { "data": "Medium", "autoWidth": true },
                { "data": "Year", "autoWidth": true },
                { "data": "ApprovedDate", "autoWidth": true },
                { "data": "Deadline", "autoWidth": true },
                {
                    "data": "Paper1ExamId", "Width": "50px", "render": function (data) {
                        if (data === '') {
                            return '<span>N/A</span>';
                        } else {
                            return '<button type="button" class="btn btn-outline-primary" onclick="ViewFirstPaperAnswers(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                        }
                    }
                },
                {
                    "data": "ExamID", "Width": "50px", "render": function (data) {
                        return '<button type="button" class="btn btn-outline-primary" onclick="ViewSecondPaperAnswers(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                    }
                },
                {
                    "data": "Paper2ExamId", "Width": "50px", "render": function (data) {
                        if (data === '') {
                            return '<span>N/A</span>';
                        } else {
                            return '<button type="button" class="btn btn-outline-primary" onclick="DispalyMapPage(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                        }
                    }
                },
                {
                    "data": "ExamID", "Width": "50px", "render": function (data) {
                        return '<button type="button" class="btn btn-outline-primary" onclick="DispalyMarkingScheme(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                    }
                },
                {
                    "data": "ExamID", "Width": "50px", "render": function (data) {
                        return '<button type="button" class="btn btn-outline-info" data-toggle="modal" data-target=".upload-modal-lg" onclick="OnUpload(' + data + ')"><i class="fa fa-upload mr-2"></i>Upload</button>';
                    }
                },
                {
                    "data": "ExamID", "Width": "50px", "render": function (data) {
                        return '<button type="button" class="btn btn-outline-success" data-toggle="modal" data-target="#completeConfirmationModal" onclick="CompleteExam(' + data + ')"><i class="fa fa-check-circle mr-2"></i>Complete</button>';
                    }
                }
            ]
        });

        var cTable = $('#recCompletedTable').DataTable({
            "ajax": {
                "url": '/TutorDashboard/GetCompletedExamsForTutor?TutorLoginId=' + $("#loginId").val(),
                "type": "get",
                "datatype": "json"
            },
            "columns": [
                { "data": "ExamNo", "autoWidth": true },
                { "data": "StudentName", "autoWidth": true },
                { "data": "ExamType", "autoWidth": true },
                { "data": "Subject", "autoWidth": true },
                { "data": "Medium", "autoWidth": true },
                { "data": "Year", "autoWidth": true },
                { "data": "ApprovedDate", "autoWidth": true },
                { "data": "DeadlineDate", "autoWidth": true },
                { "data": "CompletedDate", "autoWidth": true },
                {
                    "data": "Paper1ExamId", "Width": "50px", "render": function (data) {
                        if (data === '') {
                            return '<span>N/A</span>';
                        } else {
                            return '<button type="button" class="btn btn-outline-primary" data-toggle="modal" data-target=".view-pape1" onclick="ViewFirstPaper(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                        }
                    }
                },
                {
                    "data": "ExamID", "Width": "50px", "render": function (data) {
                        return '<button type="button" class="btn btn-outline-primary" data-toggle="modal" data-target=".view-paper2" onclick="ViewSecondPaper(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                    }
                }
            ]
        });
    //}

    //if (window.File && window.FileList && window.FileReader) {
    //    $("#imgInp").on("change", function (e) {
    //        alert(1);
    //        var files = e.target.files,
    //            filesLength = files.length;
    //        for (var i = 0; i < filesLength; i++) {
    //            var f = files[i]
    //            var fileReader = new FileReader();
    //            fileReader.onload = (function (e) {
    //                var file = e.target;
    //                $("<span class=\"pip\">" +
    //                    "<img class=\"imageThumb\" src=\"" + e.target.result + "\" title=\"" + file.name + "\"/>" +
    //                    "<br/><span class=\"remove\">Remove image</span>" +
    //                    "</span>").insertAfter("#files");
    //                $(".remove").click(function () {
    //                    $(this).parent(".pip").remove();
    //                });

    //                // Old code here
    //                /*$("<img></img>", {
    //                  class: "imageThumb",
    //                  src: e.target.result,
    //                  title: file.name + " | Click to remove"
    //                }).insertAfter("#files").click(function(){$(this).remove();});*/

    //            });
    //            fileReader.readAsDataURL(f);
    //        }
    //    });
    //} else {
    //    alert("Your browser doesn't support to File API")
    //}
});

//Check cookies
function CheckCookies() {
    $.ajax({
        type: "Post",
        url: "/Home/CheckCookiesNew",
        success: function (result) {
            if (result.success) {
                //window.location.href = "/" + result.controllerName + "/" + result.actionName;
            } else {
                window.location.href = "/";
            }
        }
    })
}

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

//To redirect when notification click
function SelectNotification(AssignId) {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SelectNotification",
        data: { "AssignId": AssignId},
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

//Download attach files in student completed first paper
function DownloadFirstPaperAnswers(ExamId) {
    debugger
    var ajax = new XMLHttpRequest();
    ajax.open("Post", "/TutorDashboard/DownloadFirstPaperAnswers?ExamId=" + ExamId, true);
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

//Download attach files in student completed first paper
function DownloadSecondPaperAnswers(ExamId) {
    debugger
    var ajax = new XMLHttpRequest();
    ajax.open("Post", "/TutorDashboard/DownloadSecondPaperAnswers?ExamId=" + ExamId, true);
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

//Download attach files in tutor completed first paper
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

function OnUpload(ExamId) {
    $('#examId').val(ExamId);

    $.ajax({
        type: "Post",
        url: "/TutorDashboard/CheckPaperHasMcq",
        data: { "ExamId": ExamId},
        success: function (result) {
            if (result.success) {
                if (result.hasMcq) {
                    $("#radio1stPaper").prop("disabled", true);
                    $("#radio2ndPaper").prop("disabled", false);
                    $("#radio2ndPaper").prop("checked", true);
                    $("#radio1stPaper").prop("checked", false);
                } else {
                    $("#radio1stPaper").prop("disabled", false);
                    $("#radio2ndPaper").prop("disabled", false);
                    $("#radio2ndPaper").prop("checked", false);
                    $("#radio1stPaper").prop("checked", true);
                }

                if ($("#radio1stPaper").is(':checked')) {
                    $("#lblMarks").text('Please enter the marks for first paper');
                }
                else {
                    $("#lblMarks").text('Please enter the marks for second paper');
                }
            }
        }
    })
}

$('#radio1stPaper').click(function () {
    if ($(this).is(':checked')) {
        $("#lblMarks").text('Please enter the marks for first paper');
    }
    else {
        $("#lblMarks").text('Please enter the marks for second paper');
    }
});

$('#radio2ndPaper').click(function () {
    if ($(this).is(':checked')) {
        $("#lblMarks").text('Please enter the marks for second paper');
    }
    else {
        $("#lblMarks").text('Please enter the marks for first paper');
    }
});

//$(function () {
//    // Multiple images preview in browser
//    var imagesPreview = function (input, placeToInsertImagePreview) {

//        if (input.files) {
//            var filesAmount = input.files.length;

//            for (i = 0; i < filesAmount; i++) {
//                var reader = new FileReader();

//                reader.onload = function (event) {
//                    $($.parseHTML('<img>')).attr('src', event.target.result).appendTo(placeToInsertImagePreview);
//                }

//                reader.readAsDataURL(input.files[i]);
//            }
//        }

//    };

//    $('#imgInp').on('change', function () {
//        imagesPreview(this, 'div.gallery');
//    });
//});

$("#popupSaveBtn").click(function () {
    if (window.FormData !== undefined) {
        var fileUpload = $("#imgInp").get(0);
        var files = fileUpload.files;

        // Create FormData object
        var fileData = new FormData();
        
        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            ///alert(files[i].name);
            fileData.append(files[i].name, files[i]);
        }

        fileData.append("TutorLoginId", $("#loginId").val());
        fileData.append("ExamId", $("#examId").val());
        fileData.append("RadioBtnValue", $('input[name="inlineRadioOptions"]:checked').val());
        fileData.append("Marks", $("#numMarks").val());
        fileData.append("Comment", $("#tbComments").val());

        $.ajax({
            type: "Post",
            url: "/TutorDashboard/UploadMarkedSheet",
            contentType: false, // Not to set any content header
            processData: false,
            data: fileData,
            success: function (result) {
                if (result.success) {
                    //$('#uploadModal').modal('hide');
                    //$('#recApprovedTable').DataTable().ajax.reload();
                    //$('#recCompletedTable').DataTable().ajax.reload();
                    $(".upload-modal-lg .modal-footer .btn-secondary").trigger("click");
                }
            }
        })
    }
});

function CompleteExam(ExamId) {
    $('#examId').val(ExamId);
}

$("#popupConfirmBtn").click(function () {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/CompleteExam",
        data: { "ExamId": $('#examId').val(), "TutorLoginId": $("#loginId").val() },
        success: function (result) {
            if (result.success) {
                $('#recApprovedTable').DataTable().ajax.reload();
                $('#recCompletedTable').DataTable().ajax.reload();
                $(".complete-confirmation-modal .modal-footer .btn-secondary").trigger("click");
            }
        }
    })
});

function DispalyMarkingScheme(ExamId) {
    window.open('/TutorDashboard/DispalyMarkingScheme?ExamId=' + ExamId, "_blank");
}

function DispalyMapPage(ExamId) {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SetSessionForMapImage",
        data: { "ExamId": ExamId },
        success: function (result) {
            if (result.success) {
                window.open('/TutorDashboard/MapPageForTutor?ExamId=' + ExamId, "_blank");
            }
        }
    })
    
}

function ViewSecondPaperAnswers(ExamId) {
   $.ajax({
        type: "Post",
        url: "/TutorDashboard/SetSessionForSecondPaperCorrections",
        data: { "ExamId": ExamId },
        success: function (result) {
            if (result.success) {
                window.open(url);
            }
        }
    })
}

function ViewFirstPaperAnswers(ExamId) {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SetSessionForFirstPaperCorrections",
        data: { "ExamId": ExamId },
        success: function (result) {
            if (result.success) {
                window.open(url2);
            }
        }
    })
}

function ViewFirstPaper(ExamId) {
    $('#examId').val(ExamId);
    $.ajax({
        type: "Post",
        url: "/Dashboard/GetFirstPaperTutorComment",
        data: { "ExamId": ExamId },
        success: function (result) {
            $('#txtAreaPaper1Comment').val(result.comment);
            $('#headerPaper1Name').text(result.paperName);
        }
    })
}

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

function ViewMyAnswers() {
    window.open('/Dashboard/ViewTutorUploads?ExamId=' + $('#examId').val());
}

function ViewStudentUploads() {
    window.open('/Dashboard/ViewMyAnswers?ExamId=' + $('#examId').val());
}

function ViewMyAnswersForFirstPaper() {
    window.open('/Dashboard/ViewTutorUploadsForFirstPaper?ExamId=' + $('#examId').val());
}

function ViewStudentUploadsForFirstPaper() {
    window.open('/Dashboard/ViewAnswerImagesForFirstPaper?ExamId=' + $('#examId').val());
}