export default {
  options: {
    responsive: true,
    maintainAspectRatio: false,
    fontColor: '#FFF',
    elements: {
      line: {
        backgroundColor: '#F00',
        pointBackgroundColor: '#FF00FF',
        borderColor: '#CC3311',
        fill: false,
        tension: 0.5
      }
    },
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: false
        }
      },
      {
        stacked: true
      }],
      xAxes: [{
        type: 'time',
        distribution: 'linear',
        time: {
          displayFormats: {
            quarter: 'MMM YYYY'
          },
          unit: 'month'
        }
      }]
    },
    legend: {
      display: false,
      labels: {
        fontColor: '#FFF'
      }
    }
  }
}
