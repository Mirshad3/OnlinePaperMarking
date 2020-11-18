$(window).on('load', function () {
    LoadClasses();
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

function LoadClasses() {
    $.ajax({
        type: "Post",
        url: "/Dashboard/LoadClasses",
        success: function (result) {
            $('#papersList').empty();
            $.each(result.dataList, function (key, value) {

                var SetData = $("#papersList");

                var Data = '<div class="col-lg-6 col-sm-6">' +
                    '<div class="card border border-secondary">' +
                    '<div class="card-header bg-transparent border-primary wr-subject-items">' +
                    '<h5 class="my-0 text-primary">' + value.ClassName+'</h5>' +
                    '<h5 class="card-title mt-2 mb-2 pb-1 font-size-13 font-weight-medium">' + value.Master + '</h5>' +
                    '<h6 class="card-title mt-2 mb-2 pb-1 font-size-13 font-weight-medium">' + value.Date +' - '+ value.Time + '</h6>' +
                    '<a onclick="LoadClassInfo(' + value.ClassID + ')" data-toggle="modal"  data-target="#myModal"  style="color: darkblue;font-weight: 600;">More Info >></a>' +
                    '<div class="row border-top pt-2">' +
                    //'<div class="col-12">' +
                    //'<div class="wr-rating">' +
                    //'<span class="fa fa-star checked"></span>' +
                    //'<span class="fa fa-star checked"></span>' +
                    //'<span class="fa fa-star checked"></span>' +
                    //'<span class="fa fa-star"></span>' +
                    //'<span class="fa fa-star mr-1"></span>(120)' +
                    //'</div>' +
                    //'</div>' +
                    '<div class="col-12 pt-2">' +
                    '<h5 class="my-0 font-size-17 " style="font-weight: 900 !important;color: green;">Rs. ' + value.Price + '</h5>' +
                    '</div>' +
                    '</div>' +
                    //'<a href="#" class="wr-subject-items-wish">' +
                    //'<span class="fa fa-heart"></span>' +
                    //'</a>' +
                    '<div class="wr-subject-items-btns mobile-onlineclass-button" style="opacity:0;bottom: 8px;left: 40%;">' +
                    '<a onclick="BuyNowClasses(' + value.ClassID + ')" class="btn btn-primary  waves-effect waves-light radius-4 font-weight-medium">' +
                    '<i class="fa fa-shopping-bag mr-1"></i>' +
                    'Buy Now' +
                    '</a>' +
                    '</div>' +
                    '<div class="wr-subject-items-btns" style="opacity:0">' +
                    '<a onclick="AddToCartClasses(' + value.ClassID + ')" class="btn btn-warning  waves-effect waves-light radius-4 font-weight-medium">' +
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

function AddToCartClasses(ClassId) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/AddToCartClasses",
        data: { "LoginId": $("#loginId").val(), "ClassId": ClassId },
        success: function (result) {
            if (result.success) {
                GetCartDetailsToNotificationList();
            }
        }
    });

    //Right Top Notification
    $.notify({
        title: '<strong>Item Added To Cart</strong>',
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

function BuyNowClasses(ClassId) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/AddToCartClasses",
        data: { "LoginId": $("#loginId").val(), "ClassId": ClassId },
        success: function (result) {
            if (result.success) {
                GetCartDetailsToNotificationList();
                window.location.href = '/Dashboard/Cart';
            }
        }
    });

}


function LoadClassInfo(classID) {

    $('#popupPaperList').empty();

    var data2 = '<div class="main-image">' +
        '<img src="/Dashboard/LoadOnlineClasssImage?ClassID=' + classID + '" onclick="GetName(this.src)" alt="" class="w-100">' +
        '</div>';

    $('#popupPaperList').append(data2);

}

 