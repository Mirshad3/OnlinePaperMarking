$(window).on('load', function () {
    GetUserName();
})

function GetUserName() {
    $.ajax({
        type: "Get",
        url: "/AdminPanel/GetAdminUserName",
        success: function (result) {
            if (result.success) {
                $('#txtUserName').val(result.UserName);
            }
        }
    })
}

$('#saveBtn').click(function () {
    if (window.FormData !== undefined) {
        // Create FormData object
        var fileData = new FormData();

        if ($('#txtUserName').val() === '') {
            Notification("Error", "User name required.", "danger");
        } else if ($('#txtOldPassword').val() === '') {
            Notification("Error", "Old password required.", "danger");
        } else if ($('#txtNewPassword').val() === '') {
            Notification("Error", "New password required.", "danger");
        } else if ($('#txtConfirmPassword').val() !== $('#txtNewPassword').val()) {
            Notification("Error", "Confirm password mismatched." , "danger");
        } else {
            fileData.append("UserName", $('#txtUserName').val());
            fileData.append("OldPassword", $('#txtOldPassword').val());
            fileData.append("ConfirmPassword", $('#txtConfirmPassword').val());
            
            $.ajax({
                type: "Post",
                url: "/AdminPanel/UpdatePassword",
                contentType: false, // Not to set any content header
                processData: false,
                data: fileData,
                success: function (result) {
                    if (result.success) {
                        $('#txtOldPassword').val('');
                        $('#txtConfirmPassword').val('');
                        $('#txtNewPassword').val('');
                        Notification("Success!", "Submitted Successfully.", "success");
                    } else {
                        Notification("Error!", result.message, "danger");
                    }
                }
            })
        }
    }
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