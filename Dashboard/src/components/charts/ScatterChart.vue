<script>

import { Scatter, mixins } from 'vue-chartjs'
const { reactiveProp } = mixins

export default {
  extends: Scatter,
  mixins: [reactiveProp],
  props: ['options'],
  mounted () {
    this.renderChart(this.chartData, {
      responsive: true,
      height: 100,
      width: 300,
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
        }],
        xAxes: [{
          ticks: {
          }
        }]
      },
      legend: {
        display: false,
        labels: {
          fontColor: '#FFF'
        }
      }
    })
  }
}
</script>

<style scoped>

</style>


