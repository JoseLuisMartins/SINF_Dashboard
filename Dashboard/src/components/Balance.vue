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
   

    

    <v-layout>
      
      <v-flex class="elevation-1 white" md12 offset-lg2 lg8 v-if="balanceSheetData!==null">       
        <v-expansion-panel expand >
          <v-expansion-panel-content class="light-blue darken-3">
            <span class="display-2" slot="header" style="color:white"> Balance Sheet </span>
            <v-layout row wrap>
                <v-flex d-flex md6 sm12 xs12 >
                  <statements  class="elevation-1" :data="balanceSheetData.Assets" title="Assets"> </statements>            
                </v-flex>
                <v-flex d-flex md6 sm12 xs12>
                  <v-layout collumn wrap>
                    <v-flex xs12 d-flex>
                      <v-card class="elevation-1 blue darken-1" style="color:white"> 
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
                          <v-card class="elevation-1 blue darken-1" style="color:white"> 
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

    <v-layout row>
      <v-flex d-flex md12 offset-lg2 lg8>
        <v-expansion-panel expand >
          <v-expansion-panel-content class="teal darken-3">
            <span class="display-2" slot="header" style="color:white"> Income Statement </span>
            <v-layout column wrap class="elevation-2">
              <div v-for='(item, index) in incomeStatementData' :key='index'>
                <div v-if="!item.result">
                  <v-card class="elevation-3">            
                      <v-card-title primary-title>
                        <b class="title" > {{ item.name }} </b>  
                        <v-spacer ></v-spacer>                
                        <p > {{ (item.value.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }} </p>          
                      </v-card-title>
                  </v-card>            
                </div>
                <div v-if="item.result">
                  <v-card  class="cyan darken-2 elevation-10" style="color:white">            
                    <v-card-title primary-title>
                        <v-spacer ></v-spacer>
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

    <span class="display-2 mb-5 mt-5 " > Ratios </span>
    <v-layout row >
      <v-flex d-flex xs6 offset-xs3>
        <v-expansion-panel expand >
          <v-expansion-panel-content class="light-blue lighten-3" v-for="item in ratios" v-bind:key="item.name">
            <div class="headline" slot="header"> {{item.name}} </div>
            <v-card class="light-blue lighten-4" v-for="itemzinho in item.values" v-bind:key="itemzinho.name">
              <v-card-title primary-title>
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
    </v-layout>
  </v-container>
</template>

<script>
import LineChart from '@/components/charts/LineChart'
import PieChart from '@/components/charts/PieChart'
import Loading from '@/components/loadings/Loading'
import Statements from '@/components/balance/Statements'
import AccountingService from '@/services/Accounting'

export default {
  components: {
    LineChart,
    PieChart,
    Loading,
    Statements
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
      ratios: []
    }
  },
  methods: {
    async getData (begin, end) {
      try {
        this.balanceSheetData = (await AccountingService.getBalanceSheet(begin, end)).data
        this.incomeStatementData = (await AccountingService.getIncomeStatement(begin, end)).data
        this.ratios = (await AccountingService.getFinancialRatios(begin, end)).data
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
</style>
