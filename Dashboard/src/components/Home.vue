<template>
  <v-container mt-4 grid-list-md>
    <v-layout row wrap mb-4>
      <v-flex mb-4 d-flex sm6 offset-sm3 xs12>
        <v-card>
          <v-card-text>
            <v-layout row wrap>
              <v-flex xs12 sm6>
                <v-menu
                  lazy
                  :close-on-content-click="false"
                  transition="scale-transition"
                  full-width
                  v-model="menu"
                  offset-y
                  :nudge-right="40"
                  max-width="290px"
                  max-height="600px"
                >

                  <v-text-field
                    slot="activator"
                    v-model="dateBegin"
                    prepend-icon="event"
                    readonly
                    label="Begin Date"
                  ></v-text-field>
                  
                  <v-date-picker v-model="dateBegin" no-title scrollable actions>
                    <template  slot-scope="{save, cancel}">
                      <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn flat color="primary" @click="cancel"> Cancel </v-btn>
                        <v-btn flat color="primary" @click="save"> Ok </v-btn>
                      </v-card-actions>
                    </template>
                  </v-date-picker>
                </v-menu>
              </v-flex>
              <v-flex xs12 sm6>
                <v-menu
                  lazy
                  :clonse-on-content-click="false"
                  transition="scale-transition"
                  full-width
                  :nudge-right="40"
                  max-width="290px"
                  max-height="600px"
                >

                  <v-text-field
                    slot="activator"
                    v-model="dateEnd"
                    prepend-icon="event"
                    readonly
                    label="End Date"
                  ></v-text-field>
                  
                  <v-date-picker v-model="dateEnd" no-title scrollable actions>
                    <template slot-scope="{save, cancel}">
                      <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn flat color="primary" @click="cancel"> Cancel </v-btn>
                        <v-btn flat color="primary" @click="save"> Ok </v-btn>
                      </v-card-actions>
                    </template>
                  </v-date-picker>
                </v-menu>
              </v-flex>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex d-inline xs12 sm6 md3 v-for="topic in topics" :key="topic.title">
        <topic class="hoverAnim" :title="topic.title" :icon="topic.icon" :value="topic.value" :description="topic.description" :color="topic.color" :color2="topic.color2" :dest="topic.dest"/> 
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex class="chartHolder" sm12 md6 v-for="data in chartData" :key="data.name">
        <v-card class="chartHeight white darken-3"> 
          <v-card-title primary-title pb-1 > <div class="headline">{{data.title}}</div> </v-card-title>
          <line-chart class="limitHeight"
            :data="data"
            :options="chartOptions"
            ></line-chart>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import Topic from '@/components/home/Topic'
import LineChart from '@/components/charts/LineChart'
import PurchasesService from '@/services/Purchases'
import SalesService from '@/services/Sales'

export default {
  name: 'HelloWorld',
  data () {
    return {
      dateBegin: null,
      dateEnd: null,
      menu: false,
      modal: false,
      chartOptions: {
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
      },
      chartData: [
        {
          title: 'Turnover',
          labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
          datasets: [
            {
              pointBackgroundColor: '#FF5522',
              label: 'Turnover',
              data: [20, 30, 20, 23, 21, 12, 23, 23, 32, 52, 50, 25]
            }
          ]
        },
        {
          title: 'Costs',
          labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
          datasets: [
            {
              pointBackgroundColor: '#FF5522',
              label: 'Costs',
              data: [20, 50, 20, 23, 21, 12, 23, 23, 32, 52, 50, 25]
            }
          ]
        }
      ],
      december: function (date) {
        return date.getMonth() === 11
      },
      topics: [
         {color: 'teal darken-1', color2: 'teal lighten-3', icon: 'attach_money', title: 'Sales', value: '', description: 'Total Sales', dest: 'sales'},
         {color: 'deep-orange darken-1', color2: 'deep-orange lighten-3', icon: 'shopping_cart', title: 'Purchases', value: '', description: 'Total Purchases', dest: 'purchases'},
         {color: 'light-blue darken-1', color2: 'light-blue lighten-3', icon: 'view_quilt', title: 'Inventory', value: '130€', description: 'Value in Inventory', dest: 'inventory'},
         {color: 'purple darken-1', color2: 'purple lighten-3', icon: 'account_balance_wallet', title: 'Accounting', value: '130€', description: 'Cashflow', dest: 'accounting'}
      ]
    }
  },
  components: {
    Topic, LineChart
  },
  mounted: function () {
    let currentYear = new Date().getFullYear()
    this.dateEnd = `${currentYear}-01-01`
    currentYear -= 1
    this.dateBegin = `${currentYear}-01-01`
  },
  watch: {
    dateBegin: async function (val) {
      let totalPurchaseValue = await PurchasesService.getTotalAmount(this.dateBegin, this.dateEnd)
      this.topics[1].value = `${totalPurchaseValue.data}€`

      let totalSalesValue = await SalesService.getTotalNetSales(this.dateBegin, this.dateEnd)
      this.topics[0].value = `${parseFloat(totalSalesValue.data.TotalNetSales).toFixed(2)}€`

      console.log('sales: ' + totalSalesValue.data.TotalNetSales)
    },
    dateEnd: async function (val) {
      let totalPurchaseValue = await PurchasesService.getTotalAmount(this.dateBegin, this.dateEnd)
      this.topics[1].value = `${totalPurchaseValue.data}€`

      let totalSalesValue = await SalesService.getTotalNetSales(this.dateBegin, this.dateEnd)
      this.topics[0].value = `${totalSalesValue.data.TotalNetSales}€`
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.chartHolder{
  width: 100%;
}

.limitHeight{
  max-height: 200px;
}

.hoverAnim:hover{
  color: black;
}

.hoverAnim{
  transition: color 1s linear 1s;
}



</style>
