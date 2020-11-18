$(window).on('load', function () {
    GetStudentProfileDetailsForSideBar();
    GetCartDetailsToNotificationList();
})

$(document).ajaxError(function () {
    window.location.href = "/";
});

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

//Main Chart
var options = {
    series: [],
    chart: {
        type: 'bar',
        height: 350,
        stacked: true,
        toolbar: {
            show: true
        },
        zoom: {
            enabled: false
        }
    },
    responsive: [{
        breakpoint: 480,
        options: {
            legend: {
                position: 'bottom',
                offsetX: -10,
                offsetY: 0
            }
        }
    }],
    plotOptions: {
        bar: {
            horizontal: false,
        },
    },
    //xaxis: {
    //    type: '',
    //    categories: ['Maths', 'Sinhala', 'Commerce', 'English', 'Science', 'Buddhism', 'Art'
    //    ],
    //},
    legend: {
        position: 'bottom',
        offsetY: 5
    },
    fill: {
        opacity: 1
    },
    dataLabels: {
        enabled: false
    }
};
var chartmain = new ApexCharts(document.querySelector("#chart-main"), options);
$.getJSON("/Dashboard/GetMainChartDetails?StudentLoginId=" + $('#loginId').val(), function (response) {
    chartmain.updateSeries([{
        name: 'Paper 1',
        data: response.fpMarks
    }, {
            name: 'Paper 2',
            data: response.spMarks
        }
    ])
});
chartmain.render();

//maths Chart
var options = {
    series: [],//{
    //    name: 'Attempt 1',
    //    data: [75, 55, 75, 35, 79, 78]
    //}, {
    //    name: 'Attempt 2',
    //    data: [85, 85, 55, 45, 56, 58]
    //}, {
    //    name: 'Attempt 3',
    //    data: [88, 75, 90, 82, 79, 88]
    //}],
    chart: {
        type: 'bar',
        height: 350
    },
    plotOptions: {
        bar: {
            horizontal: false,
            columnWidth: '55%'
            //endingShape: 'rounded'
        },
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        show: true,
        width: 2,
        colors: ['transparent']
    },
    //xaxis: {
    //    categories: ['2014', '2015', '2016', '2017', '2018', '2019'],
    //},
    yaxis: [{
        min: 0,
        max: 100,
        title: {
            text: 'Marks',
        },
    }],
    fill: {
        opacity: 1
    },
    tooltip: {
        y: {
            formatter: function (val) {
                return + val + " / 100"
            }
        },
        x: {
            formatter: function (val) {
                return + val + " Past Paper"
            }
        }
    }
};
var chart1 = new ApexCharts(document.querySelector("#chart1"), options);
$.getJSON("/Dashboard/GetSubChartDetails?StudentLoginId=" + $('#loginId').val() + "&SubjectId=" + $('#subjectsToChart').val(), function (response) {
    chart1.updateSeries([{
        name: "Marks",
        data: response.dataList
    }])
});
chart1.render();

$("#subjectsToChart").change(function () {
    $.ajax({
        type: "Post",
        url: "/Dashboard/GetSubChartDetails",
        data: { "StudentLoginId": $('#loginId').val(), "SubjectId": $('#subjectsToChart').val()},
        success: function (result) {
            chart1.updateSeries([{
                name: "Marks",
                data: result.dataList
            }])
        }
    })
});