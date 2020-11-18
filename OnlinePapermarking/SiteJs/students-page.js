$(document).ready(function () {
    var oTable = $('#recPendingTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetStudents',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "IsBlocked", "autoWidth": true, "visible": false },
            { "data": "Name", "autoWidth": true },
            { "data": "Email", "autoWidth": true },
            { "data": "ContactNo1", "autoWidth": true },
            { "data": "JoinDate", "autoWidth": true },
            { "data": "PurchasedPaperCount", "autoWidth": true },
            { "data": "DownloadedPaperCount", "autoWidth": true },
            {
                "data": "LoginID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-modal-lg" onclick="GetStudentProfileDetails(' + data + ')"><i class="fa fa-eye mr-2"></i>View</button>';
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
})

function GetStudentProfileDetails(LoginId) {
    $.ajax({
        type: "Get",
        url: "/Dashboard/GetStudentProfileDetails?LoginId=" + LoginId,
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
                $('#recPendingTable').DataTable().ajax.reload();
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