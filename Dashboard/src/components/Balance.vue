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
      <v-flex class="elevation-1 white" md12 offset-lg2 lg8>
        <v-layout row wrap>
          <v-flex d-flex md6 sm12 xs12 v-if="balanceSheetData!==null">
            <statements  class="elevation-1" :data="balanceSheetData.Assets" title="Assets"> </statements>
          </v-flex>
          <v-flex d-flex md6 sm12 xs12>
            <v-layout collumn wrap>
              <v-flex xs12 d-flex>
                <v-card class="elevation-1"> 
                  <v-card-title class="headline" v-if="balanceSheetData!==null"> 
                    <span> Equity </span> 
                    <v-spacer> </v-spacer>  
                    <span>  {{((balanceSheetData.Assets.total - balanceSheetData.Liabilities.total).toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }}</span> 
                  </v-card-title>
                </v-card>
              </v-flex xs12>
              <v-flex d-flex xs12 v-if="balanceSheetData!==null">
                <statements class="elevation-1" :data="balanceSheetData.Liabilities" title="Liabilities"> </statements>
              </v-flex>
            </v-layout>
          </v-flex>
        </v-layout> 
      </v-flex>
    </v-layout>

  </v-container>
</template>

<script>
import LineChart from '@/components/charts/LineChart'
import PieChart from '@/components/charts/PieChart'
import Loading from '@/components/loadings/Loading'
import Statements from '@/components/balance/Statements'
import SalesService from '@/services/Sales'

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
      error: null
    }
  },
  methods: {
  },
  watch: {
    dateBegin: function (val) {
    },
    dateEnd: function (val) {
    }
  },
  mounted: async function () {
    let currentYear = new Date().getFullYear()
    this.dateEnd = `${currentYear}-01-01`
    currentYear -= 1
    this.dateBegin = `${currentYear}-01-01`

    this.balanceSheetData = (await SalesService.getBalanceSheet()).data
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


