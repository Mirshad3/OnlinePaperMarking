$(document).ready(function () {
    LoadMainSubjects($('#mediumId').val(), $('#examTypeId').val());
})

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

