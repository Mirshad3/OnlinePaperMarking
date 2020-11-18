$(window).on('load', function () {
    //Random image selector
    var bgColorArray = ['/Content/img/slides/multi-purpose/bg1.jpg', '/Content/img/slides/multi-purpose/bg2.jpg', '/Content/img/slides/multi-purpose/bg3.jpg', '/Content/img/slides/multi-purpose/bg4.jpg'],
        selectBG = bgColorArray[Math.floor(Math.random() * bgColorArray.length)];
    $('.tp-bgimg.defaultimg ').css('background-image', 'url(' + selectBG + ')');


    $('.owl-carousel').empty();
    setTimeout(function () {
        if (typeof $('#mediumId').val() === "undefined") {
            LoadMainSubjects(1, 1);
        }
    }, 500);

 })

$("#sinhalaTab").click(function () {
    var examTypeId = 0;
    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }
    LoadMainSubjects(1, examTypeId);
});

$("#englishTab").click(function () {
    var examTypeId = 0;
    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }
    LoadMainSubjects(2, examTypeId);
});

$("#tamilTab").click(function () {
    var examTypeId = 0;
    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }
    LoadMainSubjects(3, examTypeId);
});

$("#olTab, #alTab").click(function () {
    $('.home-Examination-type a').removeClass("active");
    $(this).addClass("active");

    var examTypeId = 0;
    var mediumId = 0;
    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }

    if ($("#sinhalaTab").hasClass("active")) {
        mediumId = 1;
    } else if ($("#englishTab").hasClass("active")) {
        mediumId = 2;
    } else if ($("#tamilTab").hasClass("active")) {
        mediumId = 3;
    }

    LoadMainSubjects(mediumId, examTypeId);
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


                //$.notify(result.message, "error");
            }

        }
    })
});

function VerifyAccount() {
    $('.signin-form').hide();
    $('.reset-code-new').slideDown();
    $('#spanVerifyEmail').text($('#email').val());

    $.ajax({
        type: "Post",
        url: "/Home/SendEmailToVerify?Email=" + $('#email').val(),
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

function ResendVerifyCode() {
    $.ajax({
        type: "Post",
        url: "/Home/SendEmailToVerify?Email=" + $('#email').val(),
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

$('#submitVerifyCodeBtn').click(function () {
    $.ajax({
        type: "Post",
        url: "/Home/SubmitVerification",
        data: { "Email": $("#email").val(), "VerificationCode": $("#txtverificationCodeNew").val() },
        success: function (result) {
            if (result.success) {
                $('#invalidAlert').hide();
                //Notification("Success.", "Verification success. .", "success");
                //$('.reset-code-new').hide();
                //$('.signin-form').slideDown();
                LogIn($('#email').val(), $("#txtUserPassword").val());
            }
            else {
                $('#invalidAlert').text(result.message);
                $('#invalidAlert').show();
            }

        }
    })
})

$("#resetPasswordBtn1").click(function () {
    $('#invalidAlert').hide();
    $('.recover-form').hide();
    $('.change-password-reset-code').slideDown();
    $('#email').val($("#resetPasswordEmail").val());
    $('#spanChangePasswordResetEmail').text($("#resetPasswordEmail").val());

    $.ajax({
        type: "Post",
        url: "/Home/SendEmail?Email=" + $("#resetPasswordEmail").val(),
        success: function (result) {
            if (result.success) {


            }
            else {
                $('#invalidAlert').text(result.message);
                $('#invalidAlert').show();
            }

        }
    })
});

$("#resetPasswordBtn2").click(function () {
    $('#invalidAlert').hide();
    $.ajax({
        type: "Post",
        url: "/Home/CheckPasswordResetCode",
        data: { "Email": $('#email').val(), "VerificationCode": $("#txtverificationCode").val() },
        success: function (result) {
            if (result.success) {
                $('.email-reset-code-form').hide();
                $('.password-reset-form').slideDown();
            } else {
                $('#invalidAlert').text(result.message);
                $('#invalidAlert').show();
            }
        }
    })
});

$("#resetPasswordBtn3").click(function () {
    $('#invalidAlert').hide();
    $('#confirmPasswordError').hide();
    $('#newPasswordError').hide();
    var isValid = true;
    if ($("#txtConfirmPassword").val() === '') {
        isValid = false;
        $('#confirmPasswordError').text("Confirm password required.");
        $('#confirmPasswordError').show();
    } else if ($("#txtNewPassword").val() === '') {
        isValid = false;
        $('#newPasswordError').text("New password required.");
        $('#newPasswordError').show();
    } else if ($("#txtNewPassword").val().length < 8) {
        isValid = false;
        $('#invalidAlert').text("Minimum need 8 characters to password.");
        $('#invalidAlert').show();
    } else if ($("#txtConfirmPassword").val() != $("#txtNewPassword").val()) {
        isValid = false;
        $('#invalidAlert').text("Passwords are not matching.");
        $('#invalidAlert').show();
    }

    if (isValid) {
        $.ajax({
            type: "Post",
            url: "/Home/ChangePassword",
            data: { "Email": $('#email').val(), "ConfirmPassword": $("#txtConfirmPassword").val() },
            success: function (result) {
                if (result.success) {
                    //Notification("Success!", "Your paswword change successfully.", "success");
                    //$('.password-reset-form').hide();
                    //$('.signin-form').slideDown();
                    LogIn($('#email').val(), $("#txtConfirmPassword").val());
                } else {
                    $('#invalidAlert').text(result.message);
                    $('#invalidAlert').show();
                }
            }
        })
    }
});

function ResendVerifyCodeForResetPassword() {
    $.ajax({
        type: "Post",
        url: "/Home/SendEmail?Email=" + $("#resetPasswordEmail").val(),
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

function LogIn(UserName, Password) {
    var dataRow = {
        'Email': UserName,
        'Password': Password,
        'IsRemember': true
    }
    $.ajax({
        type: "Post",
        url: "/Home/CheckLogin",
        data: dataRow,
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


                //$.notify(result.message, "error");
            }

        }
    })
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

function UpdatePaperPartialView() {
    $.ajax({
        type: "Post",
        url: "/Home/UpdatePaperPartialView",
        success: function (result) {
            $("#id1").html(result);
        }
    })
}

function LoadMainSubjects(MediumId,ExamTypeId) {
    $.ajax({
        type: "Post",
        url: "/Home/LoadMainSubjects?MediumId=" + MediumId + "&ExamTypeId=" + ExamTypeId,
        success: function (result) {
            $('.owl-carousel').empty();
            $.each(result.dataList, function (key, value) {
                //alert(value[0].PaperName);
                var SetData = $("#id1");
                //$("#cmbUserTypes").append($("<option></option>").val(value.UserTypeid).html(value.UserTypeName));
                var Data = "<div>" +
                    "<a href='javascript: void(0)' onclick='LoadPastPapers(" + ExamTypeId + "," + MediumId + "," + value.SubjectId + ")'><img src='/Home/LoadSubjectImage?ImageId=" + value.ImageId + "' alt=''><p></p></a>" +
                    "</div>";

                //alert(Data);


                SetData.append(Data);



            });
            //Owl carousel destroy and refresh
            $('.owl-carousel').owlCarousel('destroy');
            $(".owl-carousel").owlCarousel({
                loop: false,
                nav: true,
                dots: false,
                responsiveClass: true,
                responsive: {
                    320: {
                        items: 1,
                        nav: false,
                        loop: true
                    },
                    479: {
                        items: 1,
                        nav: false,
                        loop: true
                    },
                    768: {
                        items: 3
                    },
                    979: {
                        items: 4
                    },
                    1199: {
                        items: 6
                    }
                }
            });
            $('.owl-carousel').owlCarousel('refresh');

            //Landing past papers
            $('.owl-carousel .owl-item div').click(function () {
                var papername = $(this).text();
                $('span.pop-subject').html(papername );
                setTimeout(function () {
                    $('body').addClass('pop-anim-active');
                    $('.mfp-hide').addClass('popactive')
                }, 100);

            });
            $('#selall').click(function () {
                $('.pop-exam-list input:checkbox').prop('checked', this.checked);
            });
        }
    })
}

function LoadPastPapers(ExamTypeId,MediumId,SubjectId) {
    $.ajax({
        type: "Post",
        url: "/Home/LoadPastPapersToPopup",
        data: { "SubjectId": SubjectId, "ExamTypeId": ExamTypeId, "MediumId": MediumId},
        success: function (result) {
            $('#popupPaperList').empty();
            $('#paperPreviewImages').empty();
            $.each(result.dataList, function (key, value) {
                
                var data1 = '<div class="pop-exam-list-item">' +
                    '<div> <input type="checkbox" name="cehck1" value="' + value.PastPaerID + '"> </div>' +
                    '<p>' + value.Year + '</p>' +
                    '<button class="btn-icon-effect-1 btn btn-outline btn-2 btn-tertiary mb-2" data-toggle="modal" data-target="#popup-image-gallery" tabindex="0">' +
                    '<span class="wrap">' +
                    '<span>Preview</span>' +
                    '<i class="fas fa-angle-right"></i>' +
                    '</span>' +
                    '</button>' +
                    '</div>';

                var data2 = '<div class="main-image">' +
                    '<img src="/Home/LoadPaperPreviewImage?PaperId=' + value.PastPaerID + '" onclick="GetName(this.src)" alt="">' +
                    '</div>';

                $('#popupPaperList').append(data1);
                $('#paperPreviewImages').append(data2);
            });
            $('#paperPreviewImages').slick('unslick');
        }
    })
}

function GetName(src) {
    //alert(src.replace(/^.*[\\\/]/, ''));
}

$('#downloadAllBtn').click(function () {
    if ($('#loginId').val() === '') {
        window.location.href = '/Home/Login';
    } else {
        $('input[name="cehck1"]:checked').each(function () {
            //alert(this.value);
            debugger
            var ajax = new XMLHttpRequest();
            ajax.open("Post", "/Home/DownloadPapers?PaperId=" + this.value + "&LoginId=" + $('#loginId').val(), true);
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
        });
        
    }
})

function GoToViewAll() {
    var examTypeId = 0;
    var mediumId = 0;

    if ($("#olTab").hasClass("active")) {
        examTypeId = 1;
    } else if ($("#alTab").hasClass("active")) {
        examTypeId = 2;
    }

    if ($("#sinhalaTab").hasClass("active")) {
        mediumId = 1;
    } else if ($("#englishTab").hasClass("active")) {
        mediumId = 2;
    } else if ($("#tamilTab").hasClass("active")) {
        mediumId = 3;
    }

    var data = "?ExamTypeId=" + examTypeId + "&MediumId=" + mediumId;

    window.location.href = "/Home/ViewAll" + data;
}

$('#contactUsBtn').click(function () {
    if (window.FormData !== undefined) {
        // Create FormData object
        var fileData = new FormData();

        if ($('#contactUsName').val() === '') {
            $('#contactUsNameError').show();
        } else if ($('#contactUsEmail').val() === '') {
            $('#contactUsEmailError').show();
        } else if ($('#contactUsSubject').val() === '') {
            $('#contactUsSubjectError').show();
        } else if ($('#contactUsMessage').val() === '') {
            $('#contactUsMessageError').show();
        } else {
            fileData.append("SenderName", $('#contactUsName').val());
            fileData.append("SenderEmail", $('#contactUsEmail').val());
            fileData.append("Subject", $('#contactUsSubject').val());
            fileData.append("Message", $('#contactUsMessage').val());

            $.ajax({
                type: "Post",
                url: "/Home/SendEmailToContacUs",
                contentType: false, // Not to set any content header
                processData: false,
                data: fileData,
                success: function (result) {
                    if (result.success) {
                        Notification("Success!", "Your message has been sent to us.", "success");
                        $('#contactUsName').val('');
                        $('#contactUsEmail').val('');
                        $('#contactUsSubject').val('');
                        $('#contactUsMessage').val('');
                    } else {
                        Notification("Error!", "Some error occured. Please try again later.", "success");
                    }
                }
            })
        }  
    }
})