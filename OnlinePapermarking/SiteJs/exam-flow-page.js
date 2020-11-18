$(document).ready(function () {

    var oTable = $('#recPendingTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetAllNotAssignedExamsForTutor',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamNo", "autoWidth": true },
            { "data": "CompleteDate", "autoWidth": true },
            { "data": "StudentName", "autoWidth": true },
            { "data": "ExamType", "autoWidth": true },
            { "data": "Subject", "autoWidth": true },
            { "data": "Medium", "autoWidth": true },
            { "data": "Year", "autoWidth": true },
            {
                "data": "ExamID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-success" data-toggle="modal" data-target=".upload-modal-lg" onclick="LoadPopupToAssign(' + data + ')">Assign</button>';
                }
            }
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            var curentDate = new Date();
            var completeDate = new Date(aData.CompleteDate);
            var checkDate = new Date(completeDate.setDate(completeDate.getDate() + 2));
            if (checkDate <= curentDate) {
                $('td', nRow).css('background-color', '#ed8c7b');
            }
        }
    });

    var bTable = $('#recApprovedTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetAllApprovedExamsByTutors',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamNo", "autoWidth": true },
            { "data": "CompleteDate", "autoWidth": true },
            { "data": "StudentName", "autoWidth": true },
            { "data": "TutorName", "autoWidth": true },
            { "data": "ExamType", "autoWidth": true },
            { "data": "Subject", "autoWidth": true },
            { "data": "Medium", "autoWidth": true },
            { "data": "Year", "autoWidth": true },
            { "data": "ApprovedDate", "autoWidth": true },
            { "data": "Deadline", "autoWidth": true }
        ]
    });

    var cTable = $('#recCompletedTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetAllCompletedExamsByTutors',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamNo", "autoWidth": true },
            { "data": "StudentCompleteDate", "autoWidth": true },
            { "data": "StudentName", "autoWidth": true },
            { "data": "TutorName", "autoWidth": true },
            { "data": "ExamType", "autoWidth": true },
            { "data": "Subject", "autoWidth": true },
            { "data": "Medium", "autoWidth": true },
            { "data": "Year", "autoWidth": true },
            { "data": "ApprovedDate", "autoWidth": true },
            { "data": "Deadline", "autoWidth": true },
            { "data": "TutorCompleteDate", "autoWidth": true },
        ]
    });

})

function LoadPopupToAssign(ExamId) {
    $('#tbComments').val('');
    $('#examId').val(ExamId);
    LoadBankBranchesCombo(ExamId);
}

function LoadBankBranchesCombo(ExamId) {
    $.ajax({
        type: "Get",
        url: "/AdminPanel/GetTutorsForPopup",
        data: {
            "ExamId": ExamId,
        },
        success: function (result) {
            if (result.success) {
                $("#cmbTutors").empty();
                //$("#cmbTutors").append($("<option></option>").val("").html("None Selected"));
                $.each(result.dataList, function (key, value) {
                    $("#cmbTutors").append($("<option></option>").val(value.LoginID).html(value.TuterName));
                });
            }
        }
    })
}

$('#popupSaveBtn').click(function () {
    $.ajax({
        type: "Post",
        url: "/AdminPanel/AssignTutor",
        data: {
            "ExamId": $('#examId').val(), "TutorLoginId": $('#cmbTutors').val(), "LoginId": $('#loginId').val(), "Reason": $('#tbComments').val()
        },
        success: function (result) {
            if (result.success) {
                Notification("Success", "Assigned Successfully", "success");
                $('#recPendingTable').DataTable().ajax.reload();
                $('#recApprovedTable').DataTable().ajax.reload();
                $(".upload-modal-lg .modal-footer .btn-secondary").trigger("click");
            } else {
                Notification("Error", result.message, "danger");
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