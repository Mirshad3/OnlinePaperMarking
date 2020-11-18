$(window).on('load', function () {
    LoadMainSubjects(1, 1);
    LoadPastPapers(1, 1, 0, 0);
    GetStudentProfileDetailsForSideBar();
    GetCartDetailsToNotificationList();
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

$('#customRadio1').click(function () {
    $('#lblOrdinaryLevel').text('අ.පො.ස. සාමාන්‍ය පෙළ');
    $('#lblAdvancedLevel').text('අ.පො.ස. උසස් පෙළ');
    LoadMainSubjects(1, 1);
    LoadPastPapers(1, 1, 0, 0)
});

$('#customRadio2').click(function () {
    $('#lblOrdinaryLevel').text('Ordinary Level');
    $('#lblAdvancedLevel').text('Advanced Level');
    LoadMainSubjects(2, 1);
    LoadPastPapers(2, 1, 0, 0)
});

$('#customRadio3').click(function () {
    $('#lblOrdinaryLevel').text('சாதாரண நிலை');
    $('#lblAdvancedLevel').text('மேம்பட்ட நிலை');
    LoadMainSubjects(3, 1);
    LoadPastPapers(3, 1, 0, 0)
});


function LoadMainSubjects(MediumId, ExamTypeId) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/LoadMainSubjects?MediumId=" + MediumId + "&ExamTypeId=" + ExamTypeId,
        success: function (result) {
            $('#subjectList').empty();
            $.each(result.dataList, function (key, value) {
                //alert(value[0].PaperName);
                var SetData = $("#subjectList");
                
                var Data = "<div class='custom-control custom-radio mb-2'>" +
                    "<input type='radio' class='custom-control-input' onclick='ClickSubject(" + value.MediumId + "," + value.ExamTypeId + "," + value.SubjectId + ")' name='subjectCheck' id=" + value.PaperName + ">" +
                    "<label class='custom-control-label' for=" + value.PaperName + ">" + value.PaperName +"</label>" +
                    "</div>";

                SetData.append(Data);
            });
        }
    })
}

function ClickSubject(MediumId, ExamTypeId, SubjectId) {
    LoadPastPapers(MediumId, ExamTypeId, SubjectId, 0);
}

function LoadPastPapers(MediumId, ExamTypeId, SubjectId, Year) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/LoadPastPapers?MediumId=" + MediumId + "&ExamTypeId=" + ExamTypeId + "&SubjectId=" + SubjectId + "&Year=" + Year,
        success: function (result) {
            $('#papersList').empty();
            $.each(result.dataList, function (key, value) {

                var SetData = $("#papersList");

                var Data = '<div class="col-lg-6 col-sm-6">' +
                    '<div class="card border border-secondary">' +
                    '<div class="card-header bg-transparent border-primary wr-subject-items">' +
                    '<h5 class="my-0 text-primary">' + value.PaperName + '-' + value.Year+'</h5>' +
                    '<h5 class="card-title mt-2 mb-2 pb-1 font-size-13 font-weight-medium">' + value.ExamType +'</h5>' +
                    '<div class="row border-top pt-2">' +
                    //'<div class="col-12">' +
                    //'<div class="wr-rating">' +
                    //'<span class="fa fa-star checked"></span>' +
                    //'<span class="fa fa-star checked"></span>' +
                    //'<span class="fa fa-star checked"></span>' +
                    //'<span class="fa fa-star"></span>' +
                    //'<span class="fa fa-star mr-1"></span>(120)'+
                    //'</div>' +
                    //'</div>' +
                    '<div class="col-12 pt-2 pb-1">' +
                    '<h5 class="my-0 font-size-15">Rs. ' + value.Price + '</h5>' +
                    '</div>' +
                    '</div>' +
                    //'<a href="#" class="wr-subject-items-wish">' +
                    //'<span class="fa fa-heart"></span>' +
                    //'</a>' +
                    '<div class="wr-subject-items-btns" style="opacity:0">' +
                    '<a onclick="AddToCart(' + value.PastPaerID +')" class="btn btn-primary  waves-effect waves-light radius-4 font-weight-medium">' +
                    '<i class="fa fa-shopping-cart mr-1"></i>' +
                    'Add to Cart' +
                    '</a>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';

                SetData.append(Data);
            });

        }
    })
}

function AddToCart(PastPaperId) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/AddToCart",
        data: { "LoginId": $("#loginId").val(), "PastPaperId": PastPaperId },
        success: function (result) {
            if (result.success) {
                GetCartDetailsToNotificationList();
            }
        }
    });

    //Right Top Notification
    $.notify({
        title: '<strong>Subject Added To Cart</strong>',
        message: "<br>Subject successfully added to your cart <em><strong>Click to view Cart</strong></em>",
        icon: 'fa fa-check',
        url: '/Dashboard/Cart',
        target: ''
    }, {
            // settings
            element: 'body',
            //position: null,
            type: "success",
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