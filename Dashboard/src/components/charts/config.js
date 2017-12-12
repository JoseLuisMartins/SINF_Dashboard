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
        tension: 0
      }
    },
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: false
        }
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
  },
  options2: {
    responsive: true,
    maintainAspectRatio: false,
    fontColor: '#FFF',
    elements: {
      line: {
        backgroundColor: '#F00',
        pointBackgroundColor: '#FF00FF',
        borderColor: '#CC3311',
        fill: false,
        tension: 0
      }
    },
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: false
        }
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
    }
  },
  pieOptions: {
    responsive: true,
    maintainAspectRatio: false,
    fontColor: '#FFF'
  },
  radarOptions: {
    responsive: true,
    maintainAspectRatio: false,
    fontColor: '#FFF',
    legend: {
      display: false,
      labels: {
        fontColor: '#FFF'
      }
    }
  },
  barOptions: {
    responsive: true,
    maintainAspectRatio: false,
    fontColor: '#FFF',
    legend: {
      display: false,
      labels: {
        fontColor: '#FFF'
      }
    },
    scales: {
      xAxes: [{
        barPercentage: 0.8,
        categoryPercentage: 0.8
      }]
    }
  }
}
