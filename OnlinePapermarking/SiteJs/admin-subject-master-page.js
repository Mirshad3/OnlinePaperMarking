$(document).ready(function () {
   var oTable = $('#subjectTable').DataTable({
        "ajax": {
           "url": '/AdminPanel/GetSubjects',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "ExamType", "autoWidth": true },
            { "data": "SubjectCode", "autoWidth": true },
            { "data": "SubjectName", "autoWidth": true },
            { "data": "Price", "autoWidth": true },
            { "data": "PriceForTutor", "autoWidth": true },
            {
                "data": "IsActive", "Width": "50px", "render": function (data) {
                    if (data === true) {
                        return '<input type="checkbox" class="editor-active" onclick="return false;" checked>';
                    } else {
                        return '<input type="checkbox" onclick="return false;" class="editor-active">';
                    }
                    return data;
                }
            },
            {
                "data": "SubjectID", "Width": "50px", "render": function (data) {
                    //return '<button type="button" rel="tooltip" class="btn btn-info btn-icon btn-sm" data-original-title="" title="" onclick="ModifySubject(' + data + ')"><i class="fa fa-pencil-square-o"></i></button>';
                    return '<button type="button" rel="tooltip" class="btn btn-success btn-icon btn-sm mr-1" data-original-title="" title="" onclick="ModifySubject(' + data + ')"><i class="fa fa-pencil-square-o"></i></button>'
                }
            }
        ]
    });
});

$("#saveBtn").click(function () {
    if (window.FormData !== undefined) {
        //var fileUpload = $("#imgUpload").get(0);
        //var files = fileUpload.files;

        // Create FormData object
        var fileData = new FormData();

        // Looping over all files and add it to FormData object
        //for (var i = 0; i < files.length; i++) {
        //    fileData.append(files[i].name, files[i]);
        //}

        fileData.append("LoginId", $("#loginId").val());
        fileData.append("ExamTypeId", $("#cmbExamType").val());
        fileData.append("SubjectCode", $("#txtSubjectCode").val());
        fileData.append("SubjectName", $("#txtSubjectName").val());
        fileData.append("IsActive", $('#cbIsActive').is(":checked"));
        fileData.append("SubjectId", $('#SubjectId').val());
        fileData.append("PriceForPaper", $('#txtPrice').val());
        fileData.append("PriceForTutor", $('#txtPriceForTutor').val());

        if ($("#saveBtn").text() === 'Save') {
            $.ajax({
                type: "Post",
                url: "/AdminPanel/SaveSubjects",
                contentType: false, // Not to set any content header
                processData: false,
                data: fileData,
                success: function (result) {
                    if (result.success) {
                        Notification("Success", "Submitted Successfully", "success");
                        $('#subjectTable').DataTable().ajax.reload();
                        ClearControls();
                    } else {
                        Notification("Error", result.message, "danger");
                    }
                }
            })
        } else {
            $.ajax({
                type: "Post",
                url: "/AdminPanel/UpdateSubjects",
                contentType: false, // Not to set any content header
                processData: false,
                data: fileData,
                success: function (result) {
                    if (result.success) {
                        Notification("Success", "Submitted Successfully", "success");
                        $('#subjectTable').DataTable().ajax.reload();
                        ClearControls();
                    } else {
                        Notification("Error", result.message, "danger");
                    }
                }
            })
        }
        
    }
});

function ModifySubject(subjectId) {
    $.ajax({
        type: "Get",
        url: "/AdminPanel/GetSubjectsForEdit?SubjectId=" + subjectId,
        success: function (result) {
            if (result.success) {
                $("#txtSubjectCode").val(result.dataList[0].SubjectCode);
                $("#txtSubjectName").val(result.dataList[0].SubjectName);
                $("#cmbExamType").val(result.dataList[0].ExamTypeId);
                $("#cbIsActive").prop('checked', result.dataList[0].IsActive);
                $("#SubjectId").val(result.dataList[0].SubjectID);
                $("#txtPrice").val(result.dataList[0].Price);
                $("#txtPriceForTutor").val(result.dataList[0].PriceForTutor);
                $("#saveBtn").html('Update');
            }
        }
    })
}

function ClearControls() {
    $("#txtSubjectCode").val('');
    $("#txtSubjectName").val('');
    $("#txtPrice").val('');
    $("#txtPriceForTutor").val('');
    $("#imgUpload").val('');
    $("#cmbExamType").val($("#cmbExamType option:first").val());
    $("#saveBtn").html('Save');
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