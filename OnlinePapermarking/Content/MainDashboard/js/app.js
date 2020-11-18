$(document).ready(function () {

    $("#vertical-menu-btn").click(function () {
        $("body").toggleClass("sidebar-enable");
    });

    $('#examTypes').multiselect();
    $('#mediums').multiselect();
    $('#subjects').multiselect();
   
    $(document).on('change', '.btn-file :file', function () {
        var input = $(this),
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger('fileselect', [label]);
    });

    $('.btn-file :file').on('fileselect', function (event, label) {

        var input = $(this).parents('.input-group').find(':text'),
            log = label;

        if (input.length) {
            input.val(log);
        } else {
            if (log) alert(log);
        }

    });
    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#img-upload').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $("#imgInp").change(function () {
        readURL(this);
    }); 
});








//Science Chart
var options = {
    series: [{
        name: 'Attempt 1',
        data: [75, 55, 75, 35, 79, 78]
    }, {
        name: 'Attempt 2',
        data: [85, 85, 55, 45, 56, 58]
    }, {
        name: 'Attempt 3',
        data: [88, 75, 90, 82, 79, 48]
    }, {
        name: 'Attempt 4',
        data: [45, 85, 75, 68, 56, 88]
    }],
    chart: {
        type: 'bar',
        height: 350
    },
    plotOptions: {
        bar: {
            horizontal: false,
            columnWidth: '55%',
            endingShape: 'rounded'
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
    xaxis: {
        categories: ['2014', '2015', '2016', '2017', '2018', '2019'],
    },
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
var chart2 = new ApexCharts(document.querySelector("#chart2"), options);
chart2.render();


//Sinhala Chart
var options = {
    series: [{
        name: 'Attempt 1',
        data: [75, 55, 75, 35, 79, 78, 25]
    }, {
        name: 'Attempt 2',
        data: [85, 85, 55, 45, 56, 58, 65]
    }, {
        name: 'Attempt 3',
        data: [88, 75, 90, 82, 79, 88 , 70  ]
    }],
    chart: {
        type: 'bar',
        height: 350
    },
    plotOptions: {
        bar: {
            horizontal: false,
            columnWidth: '55%',
            endingShape: 'rounded'
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
    xaxis: {
        categories: ['2012', '2014', '2015', '2016', '2017', '2018', '2019'],
    },
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
var chart3 = new ApexCharts(document.querySelector("#chart3"), options);
chart3.render();



