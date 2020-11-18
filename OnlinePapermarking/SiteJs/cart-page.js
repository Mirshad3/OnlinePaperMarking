$(window).on('load', function () {
    LoadCartDataToTable();
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
                        '<p class="mb-1">' + value.ItemName + ' </p>' +
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

function LoadCartDataToTable() {
    $.ajax({
        type: "Post",
        url: "/Dashboard/GetCartDetails?LoginId=" + $("#loginId").val(),
        success: function (result) {
            var total = 0.00;
            $('#countHeader').text(result.itemCount + ' Papers');
            $('#divCount').text(result.itemCount);
            
            $('#cartTable').empty();

            var data1 = '<tr>' +
                '<th class="text-left" width="350px">ITEM</th>' +
                '<th>PAPER PRICE</th>' +
                '<th>QUANTITY</th>' +
                '<th>TOTAL AMOUNT</th>' +
                '<th>REMOVE</th>' +
                '</tr>';

            $("#cartTable").append(data1);

            $.each(result.dataList, function (key, value) {
                //alert(value[0].PaperName);
                var SetData = $("#cartTable");

                var Data = '<tr>' +
                    '<td class="text-left font-size-15">' + value.ItemName + '</td>' +
                    '<td>Rs.' + value.UnitPrice + '</td>' +
                    '<td>' +
                    '<div class="num-block skin-1">' +
                    '<div class="num-in">' +
                    '<span class="minus dis"></span>' +
                    '<input type="text" class="in-num" value="1" readonly="">' +
                    '<span class="plus"></span>' +
                    '</div>' +
                    '</div>' +
                    '</td>' +
                    '<td>Rs.' + value.UnitPrice + '</td>' +
                    '<td><a href="#" onclick="DeleteFromCart(' + value.ItemID + ')"><i class="fa fa-times"></i></a></td>' +
                    '</tr>';
                                
                SetData.append(Data);

                total = total + value.UnitPrice;
            });

            if (result.itemCount > 0) {
                $('#divTotal').text(result.dataList[0].Total);
                $('#divSubTotal').text(result.dataList[0].SubTotal);
            } else {
                $('#divTotal').text("0");
                $('#divSubTotal').text("0");
            }
            
        }
    })
}

function DeleteFromCart(ItemId) {
    $.ajax({
        type: "Post",
        url: "/Dashboard/DeleteFromCart",
        data: { "ItemId": ItemId },
        success: function (result) {
            if (result.success) {
                GetCartDetailsToNotificationList();
                LoadCartDataToTable();
            }
        }
    })
}

$("#promoCodeBtn").click(function () {
    $.ajax({
        type: "Post",
        url: "/Dashboard/ApplyPromoCode",
        data: { "LoginId": $("#loginId").val(), "PromoCode": $("#txtPromoCode").val() },
        success: function (result) {
            if (result.success) {
                $('#divSubTotal').text(result.SubTotal);
                Notification("Success", result.message, "success");
            } else {
                Notification("Error", result.message, "danger");
            }
        }
    })
})

$("#checkOutBtn").click(function () {
    $.ajax({
        type: "Post",
        url: "/Dashboard/CheckOut?LoginId=" + $("#loginId").val(),
        //data: { "LoginId": $("#loginId").val() },
        success: function (result) {
            if (result.success && result.moveToGateway) {
                window.location.href = url1;
            } else if (result.success && result.moveToGateway == false) {
                GetCartDetailsToNotificationList();
                LoadCartDataToTable();
            }
        }
    })
})

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