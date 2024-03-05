$(document).ready(function () {
    loadCustomersAndBookingsLineChart();
});
function loadCustomersAndBookingsLineChart() {
    $(".chart-spinner").show();
    
    $.ajax({
        url: "/Dashboard/GetMemberAndBookingLineChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            
            loadLineChart("customersAndBookingsLineChart",data);

            $(".chart-spinner").hide();
        }
    });
}

function loadLineChart(id, data) {

    var chartColors = getChartColorsArray(id);

    options = {
        series: data.series,
        
        colors: chartColors,

        chart: {
            height: 350,
            type: 'line',
        },
        stroke: {
            curve: "smooth",
            width:2
        },
        markers: {
            size: 3,
            strokeWidth: 0,
            hover: {
                size: 9
            }
        },
        xaxis: {
            categories: data.categories,
            labels: {
                style: {
                    colors: "#ddd",
                },
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: "#fff",
                },
            }
        },
        legend: {
            labels: {
                colors: "#fff",
            },
        },
        tooltip: {
            theme : "dark",
        }
    }

    var chart = new ApexCharts(document.querySelector("#" + id), options);
        chart.render();


}


