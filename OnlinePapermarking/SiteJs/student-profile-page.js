$(document).ready(function () {
    GetStudentProfileDetails();
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
                $('#headerProfileCompletion').text('Profile Completion ' + result.percentage + '%');
                $('#divProgressBar').empty();

                var data = '<div class="progress-bar" role="progressbar" style="width: '+ result.percentage +'%" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100"></div>';
                $('#divProgressBar').append(data);
            }
        }
    })
}

function GetStudentProfileDetails() {
    $.ajax({
        type: "Get",
        url: "/Dashboard/GetStudentProfileDetails?LoginId=" + $("#loginId").val(),
        success: function (result) {
           if (result.success) {
               //Personal Info
               $("#firstName").val(result.dataList[0].StudentFirstName);
               $("#lastName").val(result.dataList[0].StudentLastName);
               $('#birthDay').val(moment(result.dataList[0].DateOfBirth).format("YYYY-MM-DD"));
               $("#address").val(result.dataList[0].AddressLine1);
               $("#mobileNo").val(result.dataList[0].ContactNo1);
               $("#email").val(result.dataList[0].Email);

               //School Info
               $("#school").val(result.dataList[0].School);
               $("#medium").val(result.dataList[0].MediumId);
               $("#district").val(result.dataList[0].DistrictId);
               $("#province").val(result.dataList[0].ProvinceId);

               //Exam types and exam year
               $("#examYear").val(result.dataList[0].AcademicYear);
               $("#examType").val(result.dataList[0].ExamTypeId);

               //Exams count
               $("#headerPendingExams").text(result.pendingExams);
               $("#headerCompletedExams").text(result.completedExam);

               $('#pName').text(result.dataList[0].StudentFirstName + " " + result.dataList[0].StudentLastName);
               $('#pSchool').text(result.dataList[0].School);
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

$("#btnPersonalInfo").click(function () {
    var dataRow = {
        'StudentFirstName': $('#firstName').val(),
        'StudentLastName': $('#lastName').val(),
        'DateOfBirth': $('#birthDay').val(),
        'AddressLine1': $('#address').val(),
        'ContactNo1': $('#mobileNo').val(),
        'LoginID': $('#loginId').val(),
    }

    $.ajax({
        type: "Post",
        url: "/Dashboard/SaveStudentPersonalInfo",
        data: dataRow,
        success: function (result) {
            if (result.success) {
                GetStudentProfileDetailsForSideBar();
                Notification("Success", "Submitted Successfully", "success");
            } else {
                Notification("Error", "Some Error Occured", "danger");
            }
        }
    })
})

$("#btnSchoolInfo").click(function () {
    var dataRow = {
        'School': $('#school').val(),
        'ProvinceId': $('#province').find(":selected").val(), 
        'DistrictId': $('#district').find(":selected").val(),
        'MediumId': $('#medium').find(":selected").val(),
        'LoginID': $('#loginId').val(),
    }

    $.ajax({
        type: "Post",
        url: "/Dashboard/SaveStudentSchoolInfo",
        data: dataRow,
        success: function (result) {
            if (result.success) {
                GetStudentProfileDetailsForSideBar();
                Notification("Success", "Submitted Successfully", "success");
            } else {
                Notification("Error", "Some Error Occured", "danger");
            }
        }
    })
})

$("#btnExamType").click(function () {
    var dataRow = {
        'ExamTypeId': $('#examType').val(),
        'AcademicYear': $('#examYear').val(),
        'LoginID': $('#loginId').val(),
    }

    $.ajax({
        type: "Post",
        url: "/Dashboard/SaveStudentExamType",
        data: dataRow,
        success: function (result) {
            if (result.success) {
                GetStudentProfileDetailsForSideBar();
                Notification("Success", "Submitted Successfully", "success");
            } else {
                Notification("Error", "Some Error Occured", "danger");
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