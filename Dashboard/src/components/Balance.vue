<template>
  <v-container mt-5 fluid grid-list-xs>
    <v-layout>
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
                  v-model="menu2"
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
                  v-model="menu1"
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
   
    <v-layout row wrap class="mb-4">
      <v-flex d-flex xs12 sm12 md12 offset-lg2 lg8 >

        <v-expansion-panel expanded>
          <v-expansion-panel-content class="deep-purple darken-3">
            <span class="display-2" slot="header" style="color:white"> Ratios </span>
            <v-divider> </v-divider>
            <v-layout row wrap class="white">
              <v-flex xs12 sm12 md6 lg6 v-for="item in ratios" :key="item.name">
                <v-layout row wrap class="white">
                  <v-flex xs12 md12>
                    <v-expansion-panel class="min-width:0px;">
                      <v-expansion-panel-content class="">
                        <b class="subheading bold" slot="header"> {{item.name}} </b>
                        <v-divider> </v-divider>
                        <v-card class="" v-for="(itemzinho,i) in item.values" v-bind:key="itemzinho.name" :class="[{'grey lighten-3': i%2==1}]">

                          <v-card-title primary-title class="pa-1">
                            <div class="ml-5">
                              <v-tooltip top>
                                <div dark color="primary" slot="activator">{{itemzinho.name}}</div>
                                <span>{{itemzinho.explanation}}</span>
                              </v-tooltip>
                            </div>
                            <v-spacer></v-spacer>
                            <div class="mr-5 mr-5"> {{ (itemzinho.value.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") }} </div>
                          </v-card-title>
                        </v-card>
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                  </v-flex>
                  <v-flex xs12>
                    <v-card class="relative" style="max-height:200px" v-if="item.chartData != null">
                      <radar-chart class="chartHolder" style="max-height:200px; width:99%;" :chartData="item.chartData" :options="pieOptions"> </radar-chart>
                    </v-card>
                  </v-flex>
                </v-layout>
              </v-flex>
            </v-layout>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-flex>
    </v-layout>
    

    <v-layout>
      
      <v-flex class="elevation-1 white" md12 offset-lg2 lg8 v-if="balanceSheetData!==null">       
        <v-expansion-panel expand >
          <v-expansion-panel-content class="blue darken-4">
            <span class="display-2" slot="header" style="color:white"> Balance Sheet </span>
            <v-layout row wrap>
                <v-flex d-flex md6 sm12 xs12 >
                  <statements  class="elevation-1" :data="balanceSheetData.Assets" title="Assets"> </statements>            
                </v-flex>
                <v-flex d-flex md6 sm12 xs12>
                  <v-layout collumn wrap>
                    <v-flex xs12 d-flex>
                      <v-card class="elevation-1" style=""> 
                        <v-card-title class="headline"> 
                          <span> Equity </span> 
                          <v-spacer> </v-spacer>  
                          <span>  {{((balanceSheetData.Assets.total - balanceSheetData.Liabilities.total).toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }}</span> 
                        </v-card-title>
                      </v-card>
                    </v-flex>
                    <v-flex d-flex xs12 >
                      <statements class="elevation-1" :data="balanceSheetData.Liabilities" title="Liabilities"> </statements>              
                    </v-flex>  
                    <v-flex d-flex xs12 >
                          <v-card class="elevation-1 " > 
                              <v-card-title class="headline"> 
                                <span> Total Liabilities </span> 
                                <v-spacer> </v-spacer>  
                                <span>  {{(balanceSheetData.Liabilities.total.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }}</span> 
                              </v-card-title>
                            </v-card>
                      </v-flex>              
                  </v-layout>
                </v-flex>
            </v-layout> 
            <v-layout row wrap>
              <v-flex d-flex md6 sm12 xs12 >
                <v-card class="elevation-1 blue darken-1" style="color:white"> 
                    <v-card-title class="headline"> 
                      <span> Total Assets </span> 
                      <v-spacer> </v-spacer>  
                      <span>  {{(balanceSheetData.Assets.total.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }}</span> 
                    </v-card-title>
                  </v-card>
              </v-flex>
              <v-flex d-flex md6 sm12 xs12 >
                  <v-card class="elevation-1 blue darken-1" style="color:white"> 
                      <v-card-title class="headline"> 
                        <span> Equity + Liabilities </span> 
                        <v-spacer> </v-spacer>  
                        <span>  {{(balanceSheetData.Assets.total.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }}</span> 
                      </v-card-title>
                    </v-card>
              </v-flex>
            </v-layout>             
          </v-expansion-panel-content>
        </v-expansion-panel >
      </v-flex>
    </v-layout>

    <v-layout row class="mt-4 mb-5">
      <v-flex d-flex md12 offset-lg2 lg8 class="elevation-4">
        <v-expansion-panel expand >
          <v-expansion-panel-content class="indigo darken-3">
            <span class="display-2" slot="header" style="color:white"> Income Statement </span>
            <v-layout column wrap>
              <div v-for='(item, index) in incomeStatementData' :key='index'>
                <div v-if="!item.result">
                  <v-card class=" " :class="[{'grey lighten-2':index%2==1}]">            
                      <v-card-title primary-title class="pa-1">
                        <b class="" > {{ item.name }} </b>  
                        <v-spacer ></v-spacer>                
                        <span > {{ (item.value.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }} </span>          
                      </v-card-title>
                  </v-card>            
                </div>
                <div v-if="item.result">
                  <v-card  class="indigo darken-2" style="color:white">            
                    <v-card-title primary-title class="pa-2">
                        <b class="title" > {{ item.name }} </b>  
                        <v-spacer ></v-spacer>                    
                        <b class="title"> {{ (item.value.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }} </b>
                    </v-card-title>
                  </v-card>            
                </div>
              </div>
            </v-layout>
          </v-expansion-panel-content> 
        </v-expansion-panel> 
      </v-flex>
    </v-layout>

    
  </v-container>
</template>

<script>
import LineChart from '@/components/charts/LineChart'
import PieChart from '@/components/charts/PieChart'
import ChartOptions from '@/components/charts/config'
import RadarChart from '@/components/charts/BarChart'
import Loading from '@/components/loadings/Loading'
import Statements from '@/components/balance/Statements'
import AccountingService from '@/services/Accounting'

export default {
  components: {
    LineChart,
    PieChart,
    Loading,
    Statements,
    RadarChart
  },
  data () {
    return {

      dateBegin: null,
      menu1: false,
      menu2: false,
      dateEnd: null,
      balanceSheetData: null,
      error: null,
      incomeStatementData: [],
      ratios: [],
      pieOptions: ChartOptions.barOptions
    }
  },
  methods: {
    transformRatiosToCharts () {
      for (var ratio of this.ratios) {
        let labels = []
        let data = []

        for (var value of ratio.values) {
          if (ratio.name !== 'Liquidity' || value.name.includes('ratio')) {
            labels.push(value.name)
            data.push(value.value.toFixed(2))
          }
        }

        ratio.chartData = {
          labels: labels,
          datasets: [{
            data: data,
            borderColor: '#4527a0',
            borderWidth: 1,
            backgroundColor: '#998ac4'
          }]
        }
      }
    },
    async getData (begin, end) {
      try {
        this.balanceSheetData = (await AccountingService.getBalanceSheet(begin, end)).data
        this.incomeStatementData = (await AccountingService.getIncomeStatement(begin, end)).data
        this.ratios = (await AccountingService.getFinancialRatios(begin, end)).data
        this.transformRatiosToCharts()
      } catch (e) {
        console.error(e)
      }
    }
  },
  watch: {
    dateBegin: function (val) {
      this.getData(this.dateBegin, this.dateEnd)
    },
    dateEnd: function (val) {
      this.getData(this.dateBegin, this.dateEnd)
    }
  },
  mounted: async function () {
    let currentYear = new Date().getFullYear()
    this.dateEnd = `${currentYear}-01-01`
    currentYear -= 1
    this.dateBegin = `${currentYear}-01-01`
  }
}
</script>

<style scoped> 
.fade-enter-active, .fade-leave-active {
  transition: opacity .5s
}

.fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0
}

.relative {
  position: relative;
}

.transition {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
}

.bold {
  font-weight: bold;
}
</style>
