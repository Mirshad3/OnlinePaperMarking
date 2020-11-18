$(document).ready(function () {

    $.ajax({
        type: "Post",
        url: "/AdminPanel/GetDashboardBoxDetails",
        success: function (result) {
            if (result.success) {
                $('#headerActiveStudentCount').text(result.activeStudentCount);
                $('#headerNewStudentCount').text(result.newStudentCount);
                $('#headerNewOlStudentCount').text('O/L - ' + result.newOlStudentCount);
                $('#headerNewAlStudentCount').text('A/L - ' + result.newAlStudentCount);
                $('#headerOnlineExamCount').text(result.purchasedExamsCount);
                $('#headerDownloadPaperCount').text(result.downloadedPaperCount);
            }
        }
    })

    //function GetJSON_Simple() {
    //    var resp = [];
    //    $.ajax({
    //        type: "GET",
    //        url: '/Adminpanel/GetDashboardChartDetails',
    //        contentType: "application/json",
    //        success: function (data) {
    //            resp.push(data);
    //        },
    //        error: function (req, status, error) {
    //            // do something with error
    //            alert("error");
    //        }
    //    });
    //    return resp;
    //}

    //var ctx = document.getElementById('bigDashboardChart').getContext("2d");

    //var gradientStroke = ctx.createLinearGradient(500, 0, 100, 0);
    //gradientStroke.addColorStop(0, '#80b6f4');
    //gradientStroke.addColorStop(1, chartColor);

    //var gradientFill = ctx.createLinearGradient(0, 200, 0, 50);
    //gradientFill.addColorStop(0, "rgba(128, 182, 244, 0)");
    //gradientFill.addColorStop(1, "rgba(255, 255, 255, 0.24)");

    //var simpleData = GetJSON_Simple();

    //var myChart = new Chart(ctx, {
    //    type: 'line',
    //    data: {
    //        labels: simpleData[0].month,
    //        datasets: [{
    //            label: "Users for Month",
    //            borderColor: chartColor,
    //            pointBorderColor: chartColor,
    //            pointBackgroundColor: "#1e3d60",
    //            pointHoverBackgroundColor: "#1e3d60",
    //            pointHoverBorderColor: chartColor,
    //            pointBorderWidth: 1,
    //            pointHoverRadius: 7,
    //            pointHoverBorderWidth: 2,
    //            pointRadius: 5,
    //            fill: true,
    //            backgroundColor: gradientFill,
    //            borderWidth: 2,
    //            data: simpleData[0].values
    //        }]
    //    },
    //    options: {
    //        layout: {
    //            padding: {
    //                left: 20,
    //                right: 20,
    //                top: 0,
    //                bottom: 0
    //            }
    //        },
    //        maintainAspectRatio: false,
    //        tooltips: {
    //            backgroundColor: '#fff',
    //            titleFontColor: '#333',
    //            bodyFontColor: '#666',
    //            bodySpacing: 4,
    //            xPadding: 12,
    //            mode: "nearest",
    //            intersect: 0,
    //            position: "nearest"
    //        },
    //        legend: {
    //            position: "bottom",
    //            fillStyle: "#FFF",
    //            display: false
    //        },
    //        scales: {
    //            yAxes: [{
    //                ticks: {
    //                    fontColor: "rgba(255,255,255,0.4)",
    //                    fontStyle: "bold",
    //                    beginAtZero: true,
    //                    maxTicksLimit: 5,
    //                    padding: 10
    //                },
    //                gridLines: {
    //                    drawTicks: true,
    //                    drawBorder: false,
    //                    display: true,
    //                    color: "rgba(255,255,255,0.1)",
    //                    zeroLineColor: "transparent"
    //                }

    //            }],
    //            xAxes: [{
    //                gridLines: {
    //                    zeroLineColor: "transparent",
    //                    display: false,

    //                },
    //                ticks: {
    //                    padding: 10,
    //                    fontColor: "rgba(255,255,255,0.4)",
    //                    fontStyle: "bold"
    //                }
    //            }]
    //        }
    //    }
    //});

    

})

