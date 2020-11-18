$(document).ready(function () {
    var oTable = $('#recPendingTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetPendingLectures?AdminApproval=Pending',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "Name", "autoWidth": true },
            { "data": "TutorEmail", "autoWidth": true },
            { "data": "ContactNo1", "autoWidth": true },
            { "data": "JoinDate", "autoWidth": true },
            { "data": "DownloadedPaperCount", "autoWidth": true },
            {
                "data": "LoginID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-modal-lg" onclick="GetTutorProfileDetails(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                }
            },
            {
                "data": "TutorID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" onclick="MoveToSecondExam(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                }
            },
            {
                "data": "TutorID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-success waves-effect waves-light mr-1" onclick="ApproveTutor(' + data + ')"><i class="fa fa-check"></i></button>' +
                        '<button type="button" class="btn btn-danger waves-effect waves-light" onclick="RejectTutor(' + data + ')"><i class="fa fa-times"></i></button>';
                }
            }
        ]
    });

    var oTable = $('#recApprovedTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetPendingLectures?AdminApproval=Approved',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "IsBlocked", "autoWidth": true, "visible": false },
            { "data": "Name", "autoWidth": true },
            { "data": "TutorEmail", "autoWidth": true },
            { "data": "ContactNo1", "autoWidth": true },
            { "data": "JoinDate", "autoWidth": true },
            { "data": "CompletedExamsCount", "autoWidth": true },
            { "data": "DownloadedPaperCount", "autoWidth": true },
            {
                "data": "LoginID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-modal-lg" onclick="GetTutorProfileDetails(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                }
            },
            {
                "data": "LoginID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-qualifications" onclick="GetImages(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
                }
            },
            {
                "data": "LoginID", "Width": "50px", "render": function (data, type, row, meta) {

                    if (row.IsBlocked == false) {
                        return '<button type="button" class="btn btn-outline-danger waves-effect waves-light" onclick="BlockUnblockUsers(' + data + ')"><i class="fa fa-lock mr-2"></i>Block</button>';
                    } else if (row.IsBlocked == true) {
                        return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" onclick="BlockUnblockUsers(' + data + ')"><i class="fa fa-unlock mr-2"></i>UnBlock</button>';
                    } else {
                        
                    }
                }
            }
        ]
    });

    $('#examTypes').multiselect();
    $('#mediums').multiselect();
    $('#subjects').multiselect();
})

//get tutor's profile details
function GetTutorProfileDetails(LoginId) {
    $.ajax({
        type: "Get",
        url: "/TutorDashboard/GetTutorProfileDetails?LoginId=" + LoginId,
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
                }, 50);
            }
        }
    })
}

function GetImages(TutorLoginId) {
    $.ajax({
        url: "/AdminPanel/GetQualificationImages?LoginId=" + TutorLoginId,
        success: function (result) {
            if (result.success) {
                var i = 0;
                $("#carouselInner").empty();
                $.each(result.dataList, function (key, value) {
                    i = i + 1;
                    //alert()
                    var SetData = $("#carouselInner");
                    var Data = '';
                    //alert(Data);

                    if (i == 1) {
                        Data = '<div class="carousel-item active">' +
                            '<div style="text-align:center" id="element' + i + '">' +
                            '<img class="d-block w-100" src="' + value.Src + '" alt="First slide" id="' + value.FileName + '">' +
                            '</div>' +
                            '</div>';
                    } else {
                        Data = '<div class="carousel-item">' +
                            '<div style="text-align:center" id="element' + i + '">' +
                            '<img class="d-block w-100" src="' + value.Src + '" alt="First slide" id="' + value.FileName + '">' +
                            '</div>' +
                            '</div>';
                    }

                    SetData.append(Data);
                });
            }
        }
    });
}

function ApproveTutor(TutorId) {
    $.ajax({
        type: "Post",
        url: "/AdminPanel/ApproveTutor?TutorId=" + TutorId,
        success: function (result) {
            if (result.success) {
                Notification("Success", "Submitted Successfully", "success");
                $('#recPendingTable').DataTable().ajax.reload();
                $('#recApprovedTable').DataTable().ajax.reload();
            } else {
                Notification("Error", result.message, "danger");
            }
        }
    })
}

function RejectTutor(TutorId) {
    $.ajax({
        type: "Post",
        url: "/AdminPanel/RejectTutor?TutorId=" + TutorId,
        success: function (result) {
            if (result.success) {
                Notification("Success", "Submitted Successfully", "success");
                $('#recPendingTable').DataTable().ajax.reload();
            } else {
                Notification("Error", result.message, "danger");
            }
        }
    })
}

function BlockUnblockUsers(LoginId) {
    $.ajax({
        type: "Post",
        url: "/AdminPanel/BlockUnblockUsers?LoginId=" + LoginId,
        success: function (result) {
            if (result.success) {
                Notification("Success", "Submitted Successfully", "success");
                $('#recApprovedTable').DataTable().ajax.reload();
            } else {
                Notification("Error", result.message, "danger");
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