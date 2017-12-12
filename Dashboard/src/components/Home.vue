<template>
  <v-container mt-4 grid-list-md mb-4>
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
    <v-layout row wrap class="">
      <v-flex d-flex class="chartHolder" sm12 md6>
        <v-card class="chartHeight white darken-3 pb-2"> 
          <v-card-title primary-title pb-1 > <div class="headline">Net Income</div> </v-card-title>
          <div class="ma-4 limitHeight relative" style="min-height: 200px">
            <loading color="teal" v-if="chartData == null"> </loading>
            <line-chart class="chartHolder" 
              style="min-height: 200px; max-height: 200px"
              v-if="chartData != null"
              :chartData="chartData"
              :options="chartOptions1"
              ></line-chart>
          </div>
        </v-card>
      </v-flex>
      <v-flex d-flex class="chartHolder" sm12 md6>
        <v-card class="chartHeight white darken-3 pb-2"> 
          <v-card-title primary-title pb-1 > <div class="headline">Account Receivables VS Account Payables</div> </v-card-title>
          <div class="ma-4 limitHeight relative" style="min-height: 200px">
            <loading color="teal" v-if="chartDataIvsR == null"> </loading>
            <line-chart class="chartHolder" 
              style="min-height: 200px; max-height: 200px"
              v-if="chartDataIvsR != null"
              :chartData="chartDataIvsR"
              :options="chartOptions"
              ></line-chart>
          </div>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import Topic from '@/components/home/Topic'
import LineChart from '@/components/charts/LineChart'
import ChartOptions from '@/components/charts/config'
import PurchasesService from '@/services/Purchases'
import SalesService from '@/services/Sales'
import ProductService from '@/services/Products'
import AccountingService from '@/services/Accounting'
import Loading from '@/components/loadings/loading'

export default {
  name: 'HelloWorld',
  data () {
    return {
      dateBegin: null,
      dateEnd: null,
      menu: false,
      modal: false,
      enableCharts: false,
      chartOptions: ChartOptions.options2,
      chartOptions1: ChartOptions.options,
      chartData: null,
      chartDataIvsR: null,
      december: function (date) {
        return date.getMonth() === 11
      },
      topics: [
         {color: 'teal darken-1', color2: 'teal lighten-3', icon: 'euro_symbol', title: 'Sales', value: '', description: 'Total Sales', dest: 'sales'},
         {color: 'deep-orange darken-1', color2: 'deep-orange lighten-3', icon: 'shopping_cart', title: 'Purchases', value: '', description: 'Total Purchases', dest: 'purchases'},
         {color: 'light-blue darken-1', color2: 'light-blue lighten-3', icon: 'view_quilt', title: 'Inventory', value: '', description: 'Value in Inventory', dest: 'inventory'},
         {color: 'purple darken-1', color2: 'purple lighten-3', icon: 'account_balance_wallet', title: 'Accounting', value: '', description: 'NetIncome', dest: 'accounting'}
      ]
    }
  },
  components: {
    Topic, LineChart, Loading
  },
  mounted: function () {
    let currentYear = new Date().getFullYear()
    this.dateEnd = `${currentYear}-01-01`
    currentYear -= 1
    this.dateBegin = `${currentYear}-01-01`
  },
  watch: {
    dateBegin: async function (val) {
      this.getData()
    },
    dateEnd: async function (val) {
      this.getData()
    }
  },
  methods: {
    async getData () {
      this.LoadIvsR()
      this.LoadChartData()
      let totalSalesValue = await SalesService.getTotalNetSales(this.dateBegin, this.dateEnd)
      this.topics[0].value = `${parseFloat(totalSalesValue.data[0].total).toFixed(0)}€`

      let totalPurchaseValue = await PurchasesService.getTotalAmount(this.dateBegin, this.dateEnd)
      this.topics[1].value = `${totalPurchaseValue.data}€`

      let totalInventoryValue = await ProductService.getTotalValueInventory(this.dateEnd)
      this.topics[2].value = `${parseFloat(totalInventoryValue.data.TotalValue).toFixed(0)}€`

      let NetIncomeValue = await AccountingService.getNetIncome(this.dateBegin, this.dateEnd)
      this.topics[3].value = `${parseFloat(NetIncomeValue.data.value).toFixed(0)}€`
    },
    async LoadIvsR () {
      try {
        this.chartDataIvsR = null
        let res = await AccountingService.getReceivableVSPayable(this.dateBegin, this.dateEnd)

        for (var value of res.data.payables) {
          value.y = Math.abs(value.y).toFixed(2)
        }

        for (value of res.data.receivables) {
          value.y = value.y.toFixed(2)
        }

        this.chartDataIvsR = {
          datasets: [{
            data: res.data.receivables,
            pointRadius: 3,
            fill: true,
            pointHoverRadius: 6,
            pointBackgroundColor: '#FF5522',
            backgroundColor: 'rgba(255,85,34,0.3)',
            borderColor: '#FF5522',
            borderWidth: 3,
            showLine: true,
            snapGaps: false,
            label: 'Account Receivables'
          },
          {
            pointRadius: 3,
            pointHoverRadius: 6,
            fill: true,
            backgroundColor: 'rgba(34,85,255,0.3)',
            pointBackgroundColor: '#2255FF',
            borderColor: '#2255FF',
            borderWidth: 3,
            showLine: true,
            snapGaps: false,
            data: res.data.payables,
            label: 'Account Payables'
          }]
        }
      } catch (error) {
        console.error(error)
      }
    },
    async LoadChartData () {
      try {
        this.chartData = null
        let res = await AccountingService.getNetIncomeChart(this.dateBegin, this.dateEnd)
        this.chartData = {
          datasets: [{
            data: res.data.turnover,
            pointRadius: 3,
            fill: true,
            pointHoverRadius: 6,
            pointBackgroundColor: '#FF5522',
            backgroundColor: 'rgba(255,85,34,0.3)',
            borderColor: '#FF5522',
            borderWidth: 3,
            showLine: true,
            snapGaps: false
          }]
        }
      } catch (error) {
        console.error(error)
      }
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
