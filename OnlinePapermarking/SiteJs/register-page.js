$(window).on('load', function () {
    if ($('#oldUser').val() === 'True') {
        $('#exampleModalCenter').modal('show');
    }
})
//---------------------------------------------------------Start text change validations-----------------------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------Start student register text change validations-------------------------------------------------------------------
$('#txtEmail').on('input', function (e) {
    var email = $("#txtEmail").val();
    if (email === "") {
        $('#stdEmailError').text("Email is required");
        $('#stdEmailError').show();
    } else if (IsEmail(email) == false) {
        $('#stdEmailError').text("Not a valid email");
        $('#stdEmailError').show();
    } else {
        $.ajax({
            type: "Get",
            url: "/Home/CheckEmail?StudentEmail=" + $('#txtEmail').val(),
            success: function (result) {
                if (result.success) {
                    $('#stdEmailError').hide();
                }
                else {
                    $('#stdEmailError').text("Email already exists");
                    $('#stdEmailError').show();
                }
            }
        })
    }
    
});

function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}

$('#txtFirstName').on('input', function (e) {
    if ($('#txtFirstName').val() === "") {
        $('#stdFirstNameError').show();
    } else {
        $('#stdFirstNameError').hide();
    }
})

$('#txtLastName').on('input', function (e) {
    if ($('#txtLastName').val() === "") {
        $('#stdLastNameError').show();
    } else {
        $('#stdLastNameError').hide();
    }
})

$('#txtAddress').on('input', function (e) {
    if ($('#txtAddress').val() === "") {
        $('#stdAddressError').show();
    } else {
        $('#stdAddressError').hide();
    }
})

$('#txtDistrict').on('input', function (e) {
    if ($('#txtDistrict').val() === "") {
        $('#stdDistrictError').show();
    } else {
        $('#stdDistrictError').hide();
    }
})

$('#txtMobileNo').on('input', function (e) {
    if ($('#txtMobileNo').val() === "") {
        $('#stdMobileNoError').show();
    } else {
        $('#stdMobileNoError').hide();
    }
})

$('#txtPassword').on('input', function (e) {
    if ($('#txtPassword').val() === "") {
        $('#stdPasswordError').text("Password is required");
        $('#stdPasswordError').show();
    } else if ($('#txtPassword').val().length < 8) {
        $('#stdPasswordError').text("Minimum 8 characters");
        $('#stdPasswordError').show();
    } else {
        $('#stdPasswordError').hide();
    }
})
    //----------------------------------------------------Start student register text change validations----------------------------------------------------------------------------

    //----------------------------------------------------Start tutor register text change validations-------------------------------------------------------------------------------------------
$('#txtTutEmail').on('input', function (e) {
    var email = $("#txtTutEmail").val();
    if (email === "") {
        $('#tutEmailError').text("Email is required");
        $('#tutEmailError').show();
    } else if (IsEmail(email) == false) {
        $('#tutEmailError').text("Not a valid email");
        $('#tutEmailError').show();
    } else {
        $.ajax({
            type: "Get",
            url: "/Home/CheckEmail?StudentEmail=" + $('#txtTutEmail').val(),
            success: function (result) {
                if (result.success) {
                    $('#tutEmailError').hide();
                }
                else {
                    $('#tutEmailError').text("Email already exists");
                    $('#tutEmailError').show();
                }
            }
        })
    }
});

$('#txtTutFirstName').on('input', function (e) {
    if ($('#txtTutFirstName').val() === "") {
        $('#tutFistNameError').show();
    } else {
        $('#tutFistNameError').hide();
    }
})

$('#txtTutLastName').on('input', function (e) {
    if ($('#txtTutLastName').val() === "") {
        $('#tutLastNameError').show();
    } else {
        $('#tutLastNameError').hide();
    }
})

$('#txtTutAddress').on('input', function (e) {
    if ($('#txtTutAddress').val() === "") {
        $('#tutAddressError').show();
    } else {
        $('#tutAddressError').hide();
    }
})

$('#txtTutNic').on('input', function (e) {
    if ($('#txtTutNic').val() === "") {
        $('#tutNicError').show();
    } else {
        $('#tutNicError').hide();
    }
})

$('#txtTutMobileNo').on('input', function (e) {
    if ($('#txtTutMobileNo').val() === "") {
        $('#tutMobileNoError').show();
    } else {
        $('#tutMobileNoError').hide();
    }
})

$('#txtTutUniversity').on('input', function (e) {
    if ($('#txtTutUniversity').val() === "") {
        $('#tutUniversityError').show();
    } else {
        $('#tutUniversityError').hide();
    }
})

$('#txtTutPassword').on('input', function (e) {
    if ($('#txtTutPassword').val() === "") {
        $('#tutPasswordError').text("Password is required");
        $('#tutPasswordError').show();
    } else if ($('#txtTutPassword').val().length < 8) {
        $('#tutPasswordError').text("Minimum 8 characters");
        $('#tutPasswordError').show();
    } else {
        $('#tutPasswordError').hide();
    }
})
    //----------------------------------------------------End tutor register text change validations-------------------------------------------------------------------------------------------
//---------------------------------------------------------------------End text change validations--------------------------------------------------------------------------------------------------------------------------------------------------

//---Register a student------
$("#BtnStdReg").click(function () {
    var data = $("#SubmitForm").serialize();
    var IsValidate = true;

    if ($("#txtEmail").val() === "") {
        IsValidate = false;
        $('#stdEmailError').text("Email is required");
        $('#stdEmailError').show();
    } else if (IsEmail($("#txtEmail").val()) == false) {
        IsValidate = false;
        $('#stdEmailError').text("Not a valid email");
        $('#stdEmailError').show();
    }
    if ($("#txtPassword").val() === "") {
        IsValidate = false;
        $('#stdPasswordError').text("Password is required");
        $('#stdPasswordError').show();
    } else if ($("#txtPassword").val().length<8) {
        IsValidate = false;
        $('#stdPasswordError').text("Minimum 8 characters");
        $('#stdPasswordError').show();
    }
    if ($("#txtFirstName").val() === "") {
        IsValidate = false;
        $('#stdFirstNameError').show();
    }
    if ($("#txtLastName").val() === "") {
        IsValidate = false;
        $('#stdLastNameError').show();
    }
    if ($("#txtAddress").val() === "") {
        IsValidate = false;
        $('#stdAddressError').show();
    }
    if ($("#txtDistrict").val() === "") {
        IsValidate = false;
        $('#stdDistrictError').show();
    }
    if ($("#txtMobileNo").val() === "") {
        IsValidate = false;
        $('#stdMobileNoError').show();
    }

    if (IsValidate) {
        $.ajax({
            type: "Post",
            url: "/Home/StudentRegister",
            data: data,
            success: function (result) {
                if (result.success) {
                    $("#loginId").val(result.loginId);
                    $("#verifiedEmail").text(result.email)
                    $('.main-registerContainer').hide();
                    $('.register-Code-Container').slideDown();
                }
                else if (result.emailExists) {
                    $('#stdEmailError').text("Email already exists");
                    $('#stdEmailError').show();
                }
                //else {
                //    alert('error');
                //    Notification("Error", result.message, "danger");
                //}

            }
        })
    }
});

//---Register a tutor------------
$("#BtnTutReg").click(function () {
    var data = $("#SubmitForm2").serialize();
    var IsValidate = true;

    if ($("#txtTutEmail").val() === "") {
        IsValidate = false;
        $('#tutEmailError').text("Email is required");
        $('#tutEmailError').show();
    } else if (IsEmail($("#txtTutEmail").val()) == false) {
        IsValidate = false;
        $('#tutEmailError').text("Not a valid email");
        $('#tutEmailError').show();
    }
    if ($("#txtTutPassword").val() === "") {
        IsValidate = false;
        $('#tutPasswordError').text("Password is required");
        $('#tutPasswordError').show();
    } else if ($("#txtTutPassword").val().length < 8) {
        IsValidate = false;
        $('#tutPasswordError').text("Minimum 8 characters");
        $('#tutPasswordError').show();
    }
    if ($("#txtTutFirstName").val() === "") {
        IsValidate = false;
        $('#tutFistNameError').show();
    }
    if ($("#txtTutLastName").val() === "") {
        IsValidate = false;
        $('#tutLastNameError').show();
    }
    if ($("#txtTutAddress").val() === "") {
        IsValidate = false;
        $('#tutAddressError').show();
    }
    if ($("#txtTutNic").val() === "") {
        IsValidate = false;
        $('#tutNicError').show();
    }
    if ($("#txtTutMobileNo").val() === "") {
        IsValidate = false;
        $('#tutMobileNoError').show();
    }
    if ($("#txtTutUniversity").val() === "") {
        IsValidate = false;
        $('#tutUniversityError').show();
    }

    if (IsValidate) {
        $.ajax({
            type: "Post",
            url: "/Home/TutorRegister",
            data: data,
            success: function (result) {
                if (result.success) {
                    $("#loginId").val(result.loginId);
                    $("#verifiedEmail").text(result.email)
                    $('.main-registerContainer').hide();
                    $('.register-Code-Container').slideDown();
                }
                else if (result.emailExists) {
                    $('#tutEmailError').text("Email already exists");
                    $('#tutEmailError').show();
                }

            }
        })
    }
});

function ResendVerifyCode() {
    alert(1);
    $.ajax({
        type: "Post",
        url: "/Home/SendEmailToVerify?Email=" + $('#verifiedEmail').text(),
        success: function (result) {
            if (result.success) {

            }
            else {
                $('#invalidAlert').text(result.message);
                $('#invalidAlert').show();
            }

        }
    })
}

$("#btnVerifyCode").click(function () {
    $.ajax({
        type: "Post",
        url: "/Home/CheckVerificationCode",
        data: { "LoginId": $("#loginId").val(), "VerificationCode": $("#verificationCode").val()},
        success: function (result) {
            if (result.success) {
                //window.location.href = "/Home/Login";	

                $.ajax({
                    type: "Post",
                    url: "/Home/CheckLogin",
                    data: { Email: result.Email, Password: result.Password },
                    success: function (result) {
                        if (result.success) {
                            //$.notify(result.message, "success");
                            Swal.fire({
                                position: 'center',
                                title: 'WelCome to Disapamock',
                                showConfirmButton: false,
                                timer: 2000,
                                onOpen: () => {
                                    swal.showLoading();
                                }
                            });
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
                });
            }
            else {
                $('#invalidAlert').text(result.message);
                $('#invalidAlert').show();
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

