$(document).ready(function () {
    ////Email Reset Password
    //$('.email-reset').click(function () { //Remove this later
    //    emailReset();
    //});
    //function emailReset() {
    //    $('.recover-form').hide();
    //    $('.email-reset-code-form').slideDown();
    //}

    ////Mobile Reset Password
    //$('.mobile-reset').click(function () { //Remove this later
    //    mobileReset();
    //});
    //function mobileReset() {
    //    $('.recover-form').hide();
    //    $('.mobile-reset-code-form').slideDown();
    //}

    ////Cancel Reset Password
    //$('#resetScreen').click(function () { //Remove this later
    //    cancelReset();
    //});
    //function cancelReset() {
    //    $('.recover-form').slideDown();
    //    $('.email-reset-code-form').hide();
    //    $('#frmResetPassword input').val("");
    //}
});

$("#loginBtn").click(function () {
    var data = $("#SubmitForm").serialize();
    $.ajax({
        type: "Post",
        url: "/Home/CheckLogin",
        data: data,
        success: function (result) {
            if (result.success) {
                //$.notify(result.message, "success");
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

$("#resetPasswordBtn1").click(function () {
    $('#email').val($("#resetPasswordEmail").val());

    $.ajax({
        type: "Post",
        url: "/Home/SendEmail",
        data: { "Email": $("#resetPasswordEmail").val()},
        success: function (result) {
            if (result.success) {
                $('.recover-form').hide();
                $('.email-reset-code-form').slideDown();
            }else {
                Notification("Error!", "" + result.message + "", "danger");
            }
        }
    })
});

$("#resetPasswordBtn2").click(function () {
    $.ajax({
        type: "Post",
        url: "/Home/CheckPasswordResetCode",
        data: { "Email": $('#email').val(), "VerificationCode": $("#txtverificationCode").val() },
        success: function (result) {
            if (result.success) {
                $('.email-reset-code-form').hide();
                $('.password-reset-form').slideDown();
            } else {
                Notification("Error!", "" + result.message + "", "danger");
            }
        }
    })
});

$("#resetPasswordBtn3").click(function () {
    var isValid = true;
    if ($("#txtConfirmPassword").val() === '') {
        isValid = false;
        Notification("Error!", "Confirm password required.", "danger");
    } else if ($("#txtNewPassword").val() === '') {
        isValid = false;
        Notification("Error!", "New password required.", "danger");
    } else if ($("#txtNewPassword").val().length < 8) {
        isValid = false;
        Notification("Error!", "Minimum need 8 characters to password.", "danger");
    } else if ($("#txtConfirmPassword").val() != $("#txtNewPassword").val()) {
        isValid = false;
        Notification("Error!", "Passwords are not matching.", "danger");
    } 
    

    if (isValid) {
        $.ajax({
            type: "Post",
            url: "/Home/ChangePassword",
            data: { "Email": $('#email').val(), "ConfirmPassword": $("#txtConfirmPassword").val() },
            success: function (result) {
                if (result.success) {
                    Notification("Success!", "Your paswword change successfully.", "success");
                    $('.password-reset-form').hide();
                    $('.wr-login-form').slideDown();
                } else {
                    Notification("Error!", "" + result.message + "", "danger");
                }
            }
        })
    }
});

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
        type: ""+ type +"",
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