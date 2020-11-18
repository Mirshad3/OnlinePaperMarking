$(document).ready(function () {
    LoadMainSubjects(1, 1);
})

//User login
$("#loginBtn").click(function () {
    var data = $("#SubmitForm").serialize();
    $.ajax({
        type: "Post",
        url: "/Home/CheckLogin",
        data: data,
        success: function (result) {
            if (result.success) {
                $('#invalidAlert').hide();
                window.location.href = "/" + result.controllerName + "/" + result.actionName;
            }
            else {
                if (result.success == false && result.isOldUser == true) {
                    window.location.href = "/Home/Register";
                } else if (result.success == false && result.isVerified == false) {
                    $('#email').val(result.email);
                    $('#invalidAlert').html('<p>Please<a href="#" onclick="VerifyAccount()">Verify</a> your email.</p>');
                    $('#invalidAlert').show();
                } else {
                    $('#invalidAlert').text(result.message);
                    $('#invalidAlert').show();
                }
            }
        }
    })
});

$("#sinhalaTab").click(function () {
    var mediumId = 1;
    var examTypeId = 0;
    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }
    LoadMainSubjects(mediumId, examTypeId);
    $('.home-language a').removeClass("active");
    $(this).addClass("active");
});

$("#englishTab").click(function () {
    var mediumId = 2;
    var examTypeId = 0;
    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }
    LoadMainSubjects(mediumId, examTypeId);
    $('.home-language a').removeClass("active");
    $(this).addClass("active");
});

$("#tamilTab").click(function () {
    var mediumId = 3;
    var examTypeId = 0;
    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }
    LoadMainSubjects(mediumId, examTypeId);
    $('.home-language a').removeClass("active");
    $(this).addClass("active");
});

$("#olTab, #alTab").click(function () {
    $('.home-Examination-type a').removeClass("active");
    $(this).addClass("active");
});

function LoadMainSubjects(MediumId, ExamTypeId) {
    $.ajax({
        type: "Post",
        url: "/Home/LoadMainSubjects?MediumId=" + MediumId + "&ExamTypeId=" + ExamTypeId,
        success: function (result) {
            $('#sinhalaPapers').empty();
            $('#englishPapers').empty();
            $('#tamilPapers').empty();
            $.each(result.dataList, function (key, value) {

                var SetData = $("#id1");

                if (MediumId == 1) {
                    SetData = $("#sinhalaPapers");
                } else if (MediumId == 2) {
                    SetData = $("#englishPapers");
                } else if (MediumId == 3) {
                    SetData = $("#tamilPapers");
                }

                var Data = "<div class='col-md-2 col-6'>" +
                    "<a href='javascript:void(0)' onclick='LoadPastPapers(" + ExamTypeId + "," + MediumId + "," + value.SubjectId + ")' class='download-item'><img src='/Home/LoadSubjectImage?ImageId=" + value.ImageId + "' alt=''><p></p></a>" +
                    "</div>";
                SetData.append(Data);
            });
            viewalpopup();
        }
    })
}

//function LoadPastPapers(MediumId, ExamTypeId, SubjectId) {
//    alert(1);
//    $.ajax({
//        type: "Post",
//        url: "/Home/LoadPastPapers?MediumId=" + MediumId + "&ExamTypeId=" + ExamTypeId + "&SubjectId=" + SubjectId,
//        success: function (result) {
//            $('#papersList').empty();
//            $.each(result.dataList, function (key, value) {

//                var SetData = $("#papersList");

//                var Data = "<div class='pastpaper-list'>" +
//                    "<p>" + value.PaperName + "</p>" +
//                    "<a href='#' class='btn btn-secondary mb-2'>Preview</a>" +
//                    "<a onclick='DownloadFiles(" + value.PastPaerID + ")' class='btn btn-secondary mb-2' style='color: whitesmoke'>Download</a>" +
//                    "</div>";

//                SetData.append(Data);
//            });

//        }
//    })
//}

//Download attach files in approved exam
function DownloadFiles(PaperId) {
    if ($('#loginId').val() === '') {
        window.location.href = '/Home/Login';
    } else {
        debugger
        var ajax = new XMLHttpRequest();
        ajax.open("Post", "/Home/DownloadPapers?PaperId=" + PaperId + "&LoginId=" + $('#loginId').val(), true);
        ajax.responseType = "blob";
        ajax.onreadystatechange = function () {
            if (this.readyState == 4) {
                debugger;
                var blob = new Blob([this.response], { type: "application/octet-stream" });
                var fileName = "PastPaper.zip";
                saveAs(blob, fileName);

            }
        };
        ajax.send(null);
    }

}


function viewalpopup() {
    //Landing past papers
    $('.download-subjects-list .download-item').click(function () {
        var papername = $(this).text();
        $('span.pop-subject').html(papername);
        setTimeout(function () {
            $('body').addClass('pop-anim-active');
            $('.mfp-hide').addClass('popactive')
        }, 100);

    });

    //Landing past papers
    $('.mfp-hide .mfp-close').click(function () {
        $("body").find(".mfp-hide").removeClass("popactive");
        $("body").removeClass("pop-anim-active");
    });

    $('#selall').click(function () {
        $('.pop-exam-list input:checkbox').prop('checked', this.checked);
    });
}