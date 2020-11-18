$(document).ready(function () {
    var oTable = $('#paperTable').DataTable({
        "ajax": {
            "url": '/AdminPanel/GetPastPapers',
            "type": "get",
            "datatype": "json"
        },
        "columns": [
            { "data": "PaperName", "autoWidth": true },
            { "data": "ExamType", "autoWidth": true },
            { "data": "Medium", "autoWidth": true },
            { "data": "Subject", "autoWidth": true },
            { "data": "Year", "autoWidth": true },
            {
                "data": "PastPaerID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-modal-lg" onclick="DispalyPreviewImage(' + data + ')">View</button>';
                }
            },
            {
                "data": "PastPaerID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-modal-lg" onclick="DispalyPaperPdf(' + data + ')">View</button>';
                }
            },
            {
                "data": "PastPaerID", "Width": "50px", "render": function (data) {
                    return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-modal-lg" onclick="DispalyMarkingScheme(' + data + ')">View</button>';
                }
            },
            {
                "data": "PastPaerID", "Width": "50px", "render": function (data, type, row, meta) {
                    if (row.HasMap === true) {
                        return '<button type="button" class="btn btn-outline-primary waves-effect waves-light" data-toggle="modal" data-target=".profile-modal-lg" onclick="DispalyMapImage(' + data + ')">View</button>';
                    } else {
                        return '<span>N/A</span>';
                    }
                }
            },
            { "data": "FirstPaperTime", "autoWidth": true },
            { "data": "SecondPaperTime", "autoWidth": true },
            { "data": "ThirdPaperTime", "autoWidth": true },
            {
                "data": "HasMcq", "Width": "50px", "render": function (data) {
                    if (data === true) {
                        return '<input type="checkbox" class="editor-active" onclick="return false;" checked>';
                    } else {
                        return '<input type="checkbox" onclick="return false;" class="editor-active">';
                    }
                    return data;
                }
            },
            {
                "data": "HasMap", "Width": "50px", "render": function (data) {
                    if (data === true) {
                        return '<input type="checkbox" class="editor-active" onclick="return false;" checked>';
                    } else {
                        return '<input type="checkbox" onclick="return false;" class="editor-active">';
                    }
                    return data;
                }
            },
            {
                "data": "HasThirdPaper", "Width": "50px", "render": function (data) {
                    if (data === true) {
                        return '<input type="checkbox" class="editor-active" onclick="return false;" checked>';
                    } else {
                        return '<input type="checkbox" onclick="return false;" class="editor-active">';
                    }
                    return data;
                }
            },
            {
                "data": "IsOnlineExam", "Width": "50px", "render": function (data) {
                    if (data === true) {
                        return '<input type="checkbox" class="editor-active" onclick="return false;" checked>';
                    } else {
                        return '<input type="checkbox" onclick="return false;" class="editor-active">';
                    }
                    return data;
                }
            },
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
                "data": "PastPaerID", "Width": "50px", "render": function (data) {
                    //return '<button type="button" rel="tooltip" class="btn btn-info btn-icon btn-sm" data-original-title="" title="" onclick="ModifySubject(' + data + ')"><i class="fa fa-pencil-square-o"></i></button>';
                    return '<button type="button" rel="tooltip" class="btn btn-success btn-icon btn-sm mr-1" data-original-title="" title="" onclick="ModifyPaper(' + data + ')"><i class="fa fa-pencil-square-o"></i></button>'
                }
            }
        ]
    });
});


$("#saveBtn").click(function () {
    if (window.FormData !== undefined) {
        // Create FormData object
        var fileData = new FormData();

        var isValidate = true;

        fileData.append("PreviewImage", $("#previewImage").get(0).files[0]);
        fileData.append("PaperPdf", $("#paperPdf").get(0).files[0]);
        fileData.append("MarkingScheme", $("#markingScheme").get(0).files[0]);
        fileData.append("MapImage", $("#mapImage").get(0).files[0]);
        fileData.append("ExamTypeId", $("#cmbExamType").val());
        fileData.append("MediumId", $("#cmbMedium").val());
        fileData.append("SubjectId", $("#cmbSubject").val());
        fileData.append("PaperName", $("#txtPaperName").val());
        fileData.append("Year", $("#txtYear").val());
        fileData.append("FirstPaperTime", $("#txtFirstPaperTime").val());
        fileData.append("SecondPaperTime", $("#txtSecondPaperTime").val());
        fileData.append("ThirdPaperTime", $("#txtThirdPaperTime").val());
        fileData.append("HaveMcq", $('#cbHaveMcq').is(":checked"));
        fileData.append("HasMap", $('#cbHasMap').is(":checked"));
        fileData.append("IsOnlineExam", $('#cbIsOnlineExam').is(":checked"));
        fileData.append("IsActive", $('#cbIsActive').is(":checked"));
        fileData.append("HasThirdPaper", $('#cbHasThirdPaper').is(":checked"));
        fileData.append("PaperId", $('#hiddenPaperId').val());

        if ($("#saveBtn").text() === "Save") {
            
            if ($("#cbHasMap").prop('checked') == true && $("#mapImage").val() === "") {
                Notification("Error", "Map Image Required.", "danger");
                isValidate = false;
            } else if ($("#cbIsOnlineExam").prop('checked') == true && $("#markingScheme").val() === "") {
                Notification("Error", "Marking Scheme Required.", "danger");
                isValidate = false;
            } else if ($("#txtPaperName").val() === "") {
                Notification("Error", "Paper Name Required.", "danger");
                isValidate = false;
            } else if ($("#txtYear").val() === "") {
                Notification("Error", "Year Required.", "danger");
                isValidate = false;
            } else if ($("#cbIsOnlineExam").prop('checked') == true && $("#txtFirstPaperTime").val() === "") {
                Notification("Error", "Time for First Paper Required.", "danger");
                isValidate = false;
            } else if ($("#cbIsOnlineExam").prop('checked') == true && $("#txtSecondPaperTime").val() === "") {
                Notification("Error", "Time for Second Paper Required.", "danger");
                isValidate = false;
            } else if ($("#cbIsOnlineExam").prop('checked') == true && $("#txtThirdPaperTime").val() === "" && $("#cbHasThirdPaper").prop('checked') == true) {
                Notification("Error", "Time for Third Paper Required.", "danger");
                isValidate = false;
            } else if ($("#previewImage").val() === "") {
                Notification("Error", "Preview Image Required.", "danger");
                isValidate = false;
            } else if ($("#paperPdf").val() === "") {
                Notification("Error", "PDF File for Paper Required.", "danger");
                isValidate = false;
            }

            if (isValidate) {
                $.ajax({
                    type: "Post",
                    url: "/AdminPanel/SavePastPaper",
                    contentType: false, // Not to set any content header
                    processData: false,
                    data: fileData,
                    success: function (result) {
                        if (result.success) {
                            Notification("Success", "Submitted Successfully", "success");
                            $('#paperTable').DataTable().ajax.reload();
                            ClearControls();
                        } else {
                            Notification("Error", result.message, "danger");
                        }
                    }
                })
            }
            
        } else {

            if ($("#txtPaperName").val() === "") {
                Notification("Error", "Paper Name Required.", "danger");
                isValidate = false;
            } else if ($("#txtYear").val() === "") {
                Notification("Error", "Year Required.", "danger");
                isValidate = false;
            } else if ($("#cbIsOnlineExam").prop('checked') == true && $("#txtFirstPaperTime").val() === "") {
                Notification("Error", "Time for First Paper Required.", "danger");
                isValidate = false;
            } else if ($("#cbIsOnlineExam").prop('checked') == true && $("#txtSecondPaperTime").val() === "") {
                Notification("Error", "Time for Second Paper Required.", "danger");
                isValidate = false;
            } else if ($("#cbIsOnlineExam").prop('checked') == true && $("#txtThirdPaperTime").val() === "" && $("#cbHasThirdPaper").prop('checked') == true) {
                Notification("Error", "Time for Third Paper Required.", "danger");
                isValidate = false;
            }

            if (isValidate) {
                $.ajax({
                    type: "Post",
                    url: "/AdminPanel/UpdatePastPaper",
                    contentType: false, // Not to set any content header
                    processData: false,
                    data: fileData,
                    success: function (result) {
                        if (result.success) {
                            Notification("Success", "Submitted Successfully", "success");
                            $('#paperTable').DataTable().ajax.reload();
                            ClearControls();
                        } else {
                            Notification("Error", result.message, "danger");
                        }
                    }
                })
            }
        }
        
    }
});

function ModifyPaper(PaperId) {
    $.ajax({
        type: "Get",
        url: "/AdminPanel/GetPastPapersToEdit?PaperId=" + PaperId,
        success: function (result) {
            if (result.success) {
                $("#cmbExamType").val(result.dataList[0].ExamTypeID);
                $("#cmbMedium").val(result.dataList[0].MediumID);
                $("#cmbSubject").val(result.dataList[0].SubjectID);
                $("#txtPaperName").val(result.dataList[0].PaperName);
                $("#txtYear").val(result.dataList[0].Year);
                $("#txtFirstPaperTime").val(result.dataList[0].FirstPaperTime);
                $("#txtSecondPaperTime").val(result.dataList[0].SecondPaperTime);
                $("#txtThirdPaperTime").val(result.dataList[0].ThirdPaperTime);
                $("#cbIsActive").prop('checked', result.dataList[0].IsActive);
                $("#cbHaveMcq").prop('checked', result.dataList[0].HasMcq);
                $("#cbHasMap").prop('checked', result.dataList[0].HasMap);
                $("#cbIsOnlineExam").prop('checked', result.dataList[0].IsOnlineExam);
                $("#cbHasThirdPaper").prop('checked', result.dataList[0].HasThirdPaper);
                $("#previewImage").val('');
                $("#paperPdf").val('');
                $("#markingScheme").val('');
                $("#mapImage").val('');
                $("#saveBtn").html('Update');
                $('#hiddenPaperId').val(PaperId)
            }
        }
    })
}

$("#cancelBtn").click(function () {
    ClearControls();
})

function ClearControls() {
    $("#previewImage").val('');
    $("#paperPdf").val('');
    $("#markingScheme").val('');
    $("#mapImage").val('');
    $("#cmbExamType").val($("#cmbExamType option:first").val());
    $("#cmbMedium").val($("#cmbMedium option:first").val());
    $("#cmbSubject").val($("#cmbSubject option:first").val());
    $("#txtPaperName").val('');
    $("#txtYear").val('');
    $("#cbHaveMcq").prop("checked", false);
    $("#cbHasThirdPaper").prop("checked", false);
    $("#cbHasMap").prop("checked", false);
    $("#cbIsOnlineExam").prop("checked", false);
    $("#cbIsActive").prop("checked", true);
    $("#txtFirstPaperTime").val('');
    $("#txtSecondPaperTime").val('');
    $("#txtThirdPaperTime").val('');
    $("#saveBtn").html('Save');
}

function DispalyPaperPdf(PaperId) {
    window.open('/AdminPanel/DispalyPaperPdf?PaperId=' + PaperId, "_blank");
}

function DispalyMarkingScheme(PaperId) {
    window.open('/AdminPanel/DispalyMarkingScheme?PaperId=' + PaperId, "_blank");
}

function DispalyPreviewImage(PaperId) {
    var image = new Image();
    image.src = '/AdminPanel/DispalyPreviewImage?PaperId=' + PaperId;

    var w = window.open("");
    w.document.write(image.outerHTML);
}

function DispalyMapImage(PaperId) {
    var image = new Image();
    image.src = '/AdminPanel/DispalyMapImage?PaperId=' + PaperId;

    var w = window.open("");
    w.document.write(image.outerHTML);
}

//notification
function Notification(title, message, type) {
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