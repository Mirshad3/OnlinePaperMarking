$(document).ready(function () {
    GetTutorProfileDetailsForSideBar();
    GetTutorNotifications();
    GetTutorProfileDetails();
    GetQualificationDetails();
});

//Side bar tutor details load
function GetTutorProfileDetailsForSideBar() {
    $.ajax({
        type: "Get",
        url: "/TutorDashboard/GetTutorProfileDetailsForSideBar?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $("#sideBarWorkingDetails").empty();
                $("#sideBarWorkingDetails").append(result.dataList[0].Occupation + ", " + result.dataList[0].School_University + "</br>Colombo District");
                $("#sideBarName").text(result.dataList[0].TutorFirstName + " " + result.dataList[0].TutorLastName);
                $("#upFirstName").text(result.dataList[0].TutorFirstName);
                $('#headerProfileCompletion').text('Profile Completion ' + result.percentage + '%');
                $('#divProgressBar').empty();

                var data = '<div class="progress-bar" role="progressbar" style="width: ' + result.percentage + '%" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100"></div>';
                $('#divProgressBar').append(data);
            }
        }
    })
}

//Get tutor's notifications
function GetTutorNotifications() {
    $.ajax({
        type: "Get",
        url: "/TutorDashboard/GetTutorNotifications?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $("#notificationCount").text(result.dataCount);
                $("#notificationList").empty();
                $.each(result.dataList, function (key, value) {
                    var Data = "<a onclick='SelectNotification(" + value.AssignID + ")' class='text-reset notification-item'>" +
                        "<div class='media'>" +
                        //"<div class='avatar-xs mr-3'>" +
                        //"<span class='avatar-title bg - primary rounded - circle font - size - 16'><i class='bx bx-cart'></i></span>" +
                        //"</div>" +
                        "<div class='media-body'>" +
                        "<h6 class='mt-0 mb-1'>" + value.NotificationHeader + "</h6><div class='font-size-12 text-muted'><p class='mb-1'>" + value.NotificationDetail + "</p></div>" +
                        "</div>" +
                        "</div>" +
                        "</a>";

                    $("#notificationList").append(Data);
                })
            }
        }
    })
}

//To redirect when notification click
function SelectNotification(AssignId) {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SelectNotification",
        data: { "AssignId": AssignId },
        success: function (result) {
            if (result.success) {
                window.location.href = "/" + result.controller + "/" + result.method;
            }
        }
    })
}

//get tutor's profile details
function GetTutorProfileDetails() {
    $.ajax({
        type: "Get",
        url: "/TutorDashboard/GetTutorProfileDetails?LoginId=" + $("#loginId").val(),
        success: function (result) {
            if (result.success) {
                $("#pSchool").empty();
                $("#pSchool").append(result.dataList[0].Occupation + ", " + result.dataList[0].School_University + "</br>Colombo District");
                $("#pName").text(result.dataList[0].TutorFirstName + " " + result.dataList[0].TutorLastName);

                //Personal Info
                $("#firstName").val(result.dataList[0].TutorFirstName);
                $("#lastName").val(result.dataList[0].TutorLastName);
                $("#nicNo").val(result.dataList[0].TutorNIC);
                $("#address").val(result.dataList[0].AddressLine1);
                $("#mobileNo").val(result.dataList[0].ContactNo1);
                $("#email").val(result.dataList[0].TutorEmail);

                //Job related Info
                $("#university").val(result.dataList[0].School_University);
                $("#district").val(result.dataList[0].DistrictId);
                $("#occupation").val(result.dataList[0].Occupation);

                var subjectSelected = [];
                $.each(result.subjectDet, function (key, value) {
                    subjectSelected.push(value.SubjectId);
                })
                $("#subjects").val(subjectSelected);
                $("#subjects").multiselect("refresh");

                var mediumSelected = [];
                $.each(result.mediumDet, function (key, value) {
                    mediumSelected.push(value.MediumId);
                })
                $("#mediums").val(mediumSelected);
                $("#mediums").multiselect("refresh");

                var exmTypSelected = [];
                $.each(result.exmTypDet, function (key, value) {
                    exmTypSelected.push(value.ExamTypeId);
                })
                $("#examTypes").val(exmTypSelected);
                $("#examTypes").multiselect("refresh");

                //Bank Info
                $("#banks").val(result.dataList[0].BankId);
                $("#accountNo").val(result.dataList[0].AccountNo);
                LoadBankBranchesCombo();
                setTimeout(function () {
                    $("#branches").val(result.dataList[0].BranchId);
                }, 100); 

                //Exam count
                $("#headerMarkedExam").text(result.completedExam);
                $("#headerPendingExam").text(result.pendingExam);
            }
        }
    })
}

//Load banks branches select list according to the selected bankid
function LoadBankBranchesCombo() {
    $.ajax({
        type: "Get",
        url: "/TutorDashboard/LoadBankBranchesCombo",
        data: {
            "BankId": $("#banks").val(),
        },
        success: function (result) {
            if (result.success) {
                $("#branches").empty();
                $("#branches").append($("<option></option>").val("").html("None Selected"));
                $.each(result.dataList, function (key, value) {
                    $("#branches").append($("<option></option>").val(value.BranchID).html(value.BranchName));
                });
            }
        }
    })
}

//Tutor's personal info update
$("#btnPersonalInfo").click(function () {
    var dataRow = {
        'TutorFirstName': $('#firstName').val(),
        'TutorLastName': $('#lastName').val(),
        'TutorNIC': $('#nicNo').val(),
        'AddressLine1': $('#address').val(),
        'ContactNo1': $('#mobileNo').val(),
        'LoginID': $('#loginId').val(),
    }

    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SaveTutorPersonalInfo",
        data: dataRow,
        success: function (result) {
            if (result.success) {
                Notification("Success", "Submitted Successfully", "success");
                GetTutorProfileDetailsForSideBar();
                GetTutorProfileDetails();
            } else {
                Notification("Error", "Some Error Occured", "danger");
            }
        }
    })
})

//Tutors bank info update
$("#btnTutorBankInfo").click(function () {
    var dataRow = {
        'BankId': $('#banks').val(),
        'BranchId': $('#branches').val(),
        'AccountNo': $('#accountNo').val(),
        'LoginID': $('#loginId').val(),
    }

    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SaveTutorBankInfo",
        data: dataRow,
        success: function (result) {
            if (result.success) {
                Notification("Success", "Submitted Successfully", "success");
                GetTutorProfileDetailsForSideBar();
            } else {
                Notification("Error", "Some Error Occured", "danger");
            }
        }
    })
})

//Tutor's working info update
$("#btnTutorInfo").click(function () {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/SaveTutorInfo",
        data: {
            "Subjects": $("#subjects").val(), "Mediums": $("#mediums").val(),
            "ExamTypes": $("#examTypes").val(), "Occupation": $("#occupation").val(),
            "University": $("#university").val(), "DistrictId": $("#district").val(),
            "LoginId": $("#loginId").val()
        },
        success: function (result) {
            if (result.success) {
                Notification("Success", "Submitted Successfully", "success");
                GetTutorProfileDetailsForSideBar();
                GetTutorProfileDetails();
            } else {
                Notification("Error", "Some Error Occured", "danger");
            }
        }
    })
})

//Tutor's qualification details upload
$("#submitQualificationBtn").click(function () {
    if (window.FormData !== undefined) {
        var fileUpload = $("#imgInp").get(0);
        var files = fileUpload.files;

        // Create FormData object
        var fileData = new FormData();

        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            ///alert(files[i].name);
            fileData.append(files[i].name, files[i]);
        }

        fileData.append("TutorLoginId", $("#loginId").val());
        fileData.append("CertificateType", $('#cmbCertificate').val());

        if ($("#imgInp").val() === '') {
            Notification("Error", "Please choose a image for the qualification", "danger");
        } else {
            $.ajax({
                type: "Post",
                url: "/TutorDashboard/AddTutorQualifications",
                contentType: false, // Not to set any content header
                processData: false,
                data: fileData,
                success: function (result) {
                    if (result.success) {
                        GetTutorProfileDetailsForSideBar();
                        GetQualificationDetails();
                        Notification("Success", "Submitted Successfully", "success");
                        $('#imgInp').val('');
                        $('#ImgTxt').val('');
                    } else {
                        Notification("Error", result.message, "danger");
                    }
                }
            })
        }
    }
})

//Get Qualification Details
function GetQualificationDetails() {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/GetQualificationDetails",
        data: { "LoginId": $("#loginId").val() },
        success: function (result) {
            $('#QualificationList').empty();
            $.each(result.dataList, function (key, value) {
                var SetData = $("#QualificationList");
                var Data = '';
                if (value.AdminApproval === 'Approved') {
                    Data = '<div class="col-md-4 mb-3">' +
                        '<table class="table table-nowrap table-centered border">' +
                        '<tbody>' +
                        '<tr>' +
                        '<td class="border-0" width="80%">' +
                        '<h5 class="text-truncate font-size-14 m-0">' + value.CertificateType + '</h5>' +
                        '<div class="text-left mt-2 border-bottom mb-2 pb-2">' +
                        '<span class="badge badge-pill badge-soft-success font-size-11">Approved</span>' +
                        '</div>' +
                        '<button type="button" class="btn btn-danger btn-sm waves-effect waves-light" onclick="DeleteQualification(' + value.QualificationID +')">Delete</button>' +
                        '</td>' +
                        '<td class="border-0 text-right" width="40%">' +
                        '<img src="/TutorDashboard/LoadQualificationImage?QualificationId=' + value.QualificationID + '" class="rounded avatar-lg" alt="">' +
                        '</td>' +
                        '</tr>' +
                        '</tbody>' +
                        '</table>' +
                        '</div>';
                } else {
                    Data = '<div class="col-md-4 mb-3">' +
                        '<table class="table table-nowrap table-centered border">' +
                        '<tbody>' +
                        '<tr>' +
                        '<td class="border-0" width="80%">' +
                        '<h5 class="text-truncate font-size-14 m-0">' + value.CertificateType + '</h5>' +
                        '<div class="text-left mt-2 border-bottom mb-2 pb-2">' +
                        '<span class="badge badge-pill badge-soft-warning font-size-11">Pending Approval</span>' +
                        '</div>' +
                        '<button type="button" class="btn btn-danger btn-sm waves-effect waves-light">Delete</button>' +
                        '</td>' +
                        '<td class="border-0 text-right" width="40%">' +
                        '<img src="~/Content/MainDashboard/img/avatar-2.jpg" class="rounded avatar-lg" alt="">' +
                        '</td>' +
                        '</tr>' +
                        '</tbody>' +
                        '</table>' +
                        '</div>';
                }
                
            SetData.append(Data);
            });
        }
    })
}

//Delete qualification
function DeleteQualification(QualificationId) {
    $.ajax({
        type: "Post",
        url: "/TutorDashboard/DeleteQualification",
        data: { "QualificationId": QualificationId},
        success: function (result) {
            if (result.success) {
                Notification("Success", "Deleted Successfully", "success");
                GetQualificationDetails();
                GetTutorProfileDetailsForSideBar();
            } else {
                Notification("Error", "Some Error Occured", "danger");
            }
        }
    })
}

//load branches when selected bank changing
$("#banks").change(function () {
    LoadBankBranchesCombo();
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

