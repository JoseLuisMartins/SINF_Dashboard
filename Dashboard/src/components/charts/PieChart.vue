<script>

import { Pie, mixins } from 'vue-chartjs'
const { reactiveProp } = mixins

export default {
  extends: Pie,
  mixins: [reactiveProp],
  props: ['options'],
  mounted () {
    this.renderChart(this.chartData, {
      responsive: true,
      maintainAspectRatio: false,
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
