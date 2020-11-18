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

    GetImages();

    setTimeout(function () {
        GetHotSpots();
    }, 650);

});

function GetHotSpots() {
    var imageId = $('.active').find('img').attr('id');
    var elementId = $('.active').find('div').attr('id');
    $.ajax({
        url: "/TutorDashboard/GetHotspotDetailsForSecondPaper",
        data: { "ExamId": $('#examId').val(), "ImageId": imageId},
        success: function (result) {
            $("#" + elementId).hotspot({
                data: result.dataList,
                interactivity: "click",
            });
        }
    });
}

$('#carouselExampleControls').bind('slide.bs.carousel', function (e) {
    $("#theElement-a .HotspotPlugin_Hotspot").remove();
    setTimeout(function () {
        GetHotSpots();
    }, 650);
    //test();
});

function GetImages() {
    $.ajax({
        url: "/TutorDashboard/GetImagesForSecondPaperCorrections?ExamId=" + $('#examId').val(),
        success: function (result) {
            if (result.success) {
                var i = 0;
                $.each(result.dataList, function (key, value) {
                    i = i + 1;
                    //alert()
                    var SetData = $("#carouselInner");
                    var Data = '';
                    //alert(Data);

                    if (i == 1) {
                        Data = '<div class="carousel-item active">' +
                            '<div style="text-align:center" id="element' + i + '">' +
                            '<img class="d-block w-100" src="' + value.Src + '" alt="First slide" id="' + value.FileName + '">' +
                            '</div>' +
                        '</div>';
                    } else {
                        Data = '<div class="carousel-item">' +
                            '<div style="text-align:center" id="element' + i + '">' +
                            '<img class="d-block w-100" src="' + value.Src + '" alt="First slide" id="' + value.FileName + '">' +
                            '</div>' +
                        '</div>';
                    }

                    SetData.append(Data);
                });
            }
        }
    });
}

function EditSecondPaperHotSpot() {
    var imageId = $('.active').find('img').attr('id');
    window.open('/TutorDashboard/EditSecondPaperHotspots?ExamId=' + $('#examId').val() + '&ImageId=' + imageId);
}