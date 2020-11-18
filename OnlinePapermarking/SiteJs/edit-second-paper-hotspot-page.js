$(document).ready(function () {
    //GetImages();
    //$('.HotspotPlugin_Save').hide();

    //setTimeout(function () {
        $("#theElement-a").hotspot({
            mode: "admin",
            // uncomment
            /*ajax: true,
            ajaxOptions: {
                'url': 'links.to.server'
            },*/
            interactivity: "click",
            LS_Variable: "HotspotPlugin-a",

            afterSave: function (err, data) {
                if (err) {
                    console.log('Error occurred', err);
                    return;
                }

                var imageId = $('.active').find('img').attr('id');

                

                // `data` in json format can be stored
                //  & passed in `display` mode for the image
                console.log(data);
            },
            afterRemove: function (err, message) {
                if (err) {
                    console.log('Error occurred', err);
                    return;
                }

                $.ajax({
                    type: "Post",
                    url: "/TutorDashboard/DeleteHotspotDetailsForSecondPaper",
                    data: { "ExamId": $('#examId').val(), "ImageId": $('#imageId').val()},
                    success: function (result) {
                        if (result.success) {
                            $("#theElement-a .HotspotPlugin_Hotspot").remove();
                            GetHotSpots();
                            Notification("Success", "Removed Successfully", "success");
                        } else {
                            Notification("Success", "Some Error Occured", "danger");
                        }
                    }
                })
            },
            afterSend: function (err, message) {
                if (err) {
                    console.log('Error occurred', err);
                    return;
                }
                alert(message);
            }
        });
    //}, 1000);

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


    setTimeout(function () {
        GetHotSpots();
    }, 650);

});

function GetHotSpots() {
    $("#theElement-a .HotspotPlugin_Hotspot").remove();
    $.ajax({
        url: "/TutorDashboard/GetHotspotDetailsForSecondPaper",
        data: { "ExamId": $('#examId').val(), "ImageId": $('#imageId').val() },
        success: function (result) {
            $("#theElement-a").hotspot({
                data: result.dataList,
                interactivity: "click",
            });
        }
    });
}

function Notification(title, message, type) {
    //Right Top Notification
    $.notify({
        title: '<strong>' + title + '</strong>',
        message: "<br>" + message + "",
        icon: 'fa fa-check',
        //url: '/Dashboard/Cart',
        target: ''
    }, {
        // settings
        element: 'body',
        //position: null,
        type: "" + type + "",
        allow_dismiss: false,
        //newest_on_top: false,
        showProgressbar: false,
        placement: {
            from: "top",
            align: "right"
        },
        offset: 20,
        spacing: 10,
        z_index: 1031,
        delay: 1000,
        timer: 4000,
        url_target: '',
        mouse_over: null,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutRight'
        },
        onShow: null,
        onShown: null,
        onClose: null,
        onClosed: null,
        icon_type: 'class',
    });
}



