<template>
  <v-container mt-5 grid-list-xs>
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
      <v-flex d-flex xs12 md6>
        <v-card>
          <v-card-title>
            <div class="headline"> Sort by </div> 
          </v-card-title>
          <v-card-text>
            <v-layout row wrap>
              <v-flex xs12 md8>
                <v-layout column>
                  <v-flex>
                    <v-layout row>
                      <v-flex xs4 class="relative">
                      <v-btn class="allSize" block round outline> Region </v-btn>
                      </v-flex>
                      <v-flex xs8>
                      <v-text-field
                        label="Search for a Region"
                      > </v-text-field>
                      </v-flex>
                    </v-layout>
                  </v-flex>
                  <v-flex d-flex>
                    <v-layout row>
                      <v-flex xs4 class="relative">
                      <v-btn class="allSize" block round outline> Product </v-btn>
                      </v-flex>
                      <v-flex xs8>
                      <v-text-field
                        label="Search for a Product"
                      > </v-text-field>
                      </v-flex>
                    </v-layout>
                  </v-flex>
                  <v-flex>
                    <v-layout row>
                      <v-flex xs4 class="relative">
                      <v-btn class="allSize" block round outline> Category </v-btn>
                      </v-flex>
                      <v-flex xs8>
                      <v-text-field
                        label="Search for a Category"
                      > </v-text-field>
                      </v-flex>
                    </v-layout>
                  </v-flex>
                </v-layout>

              </v-flex>
              <v-flex d-flex xs12 md4>
                <v-layout row wrap align-center>
                  <v-flex sm6 md12>
                    <v-btn block outline large>Add</v-btn>
                  </v-flex>
                  <v-flex sm6 md12>
                    <v-btn block outline large>Cancel</v-btn>
                  </v-flex>
                </v-layout>
              </v-flex>

            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex d-flex xs12 md6>
        <v-card>
          <v-card-title>
            <div class="headline"> Sales </div>
            <v-spacer></v-spacer>
            <v-btn class="ma-0" outline> Day </v-btn>
            <v-btn class="ma-0" outline> Month </v-btn>
            <v-btn color="blue" class="ma-0" dark> Year </v-btn>
            <v-btn class="ma-0" outline> Custom </v-btn>
          </v-card-title>
          <v-card-text >
            <line-chart class="chartHolder" :chartData="salesChartData"> </line-chart>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Sales Invoices </div>
            <v-spacer></v-spacer>
            <v-text-field 
              label="Search"
              > </v-text-field> <v-icon> search </v-icon>
          </v-card-title>
          <v-card-text>

            <v-data-table
              v-bind:headers="salesHeader"
              :items="currentDataSet"
              class="elevation-1"
              >

              <template slot="items" scope="props">
                
                <td> {{props.item.InvoiceNo }} </td>
                <td> {{props.item.InvoiceDate }} </td>
                <td> {{props.item.InvoiceType }} </td>

              </template>

            </v-data-table>

          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Customers </div>
            <v-spacer></v-spacer>
            <v-text-field 
              label="Search"
              > </v-text-field> <v-icon> search </v-icon>
          </v-card-title>
          <v-card-text>

            <v-data-table
              v-bind:headers="salesHeader"
              :items="currentDataSet"
              hide-actions
              class="elevation-1"
              >

              <template slot="items" scope="props">
                
                <td> {{props.item.name }} </td>
               
              </template>

            </v-data-table>

          </v-card-text>
        </v-card>
      </v-flex>
      </v-flex>
    </v-layout>


  </v-container>
</template>


<script>

import LineChart from '@/components/charts/LineChart'
import SalesService from '@/services/Sales'

export default {
  data () {
    return {
      menu: false,
      productDetail: false,
      salesHeader: [
        {text: 'Invoice Number', value: 'InvoiceNo', align: 'left'},
        {text: 'Invoice Date', value: 'InvoiceDate', align: 'left'},
        {text: 'Invoice Type', value: 'InvoiceType', align: 'left'}
      ],
      salesChartData: {
        title: 'Turnover',
        labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        datasets: [
          {
            pointRadius: 5,
            pointHoverRadius: 10,
            pointBackgroundColor: '#FF5522',
            label: 'tmp',
            data: [20, 30, 20, 23, 21, 12, 23, 23, 32, 52, 50, 25]
          },
          {
            pointRadius: 5,
            pointHoverRadius: 10,
            pointBackgroundColor: '#2255FF',
            borderColor: '#1144AA',
            label: 'tmp2',
            data: [30, 20, 23, 21, 12, 23, 23, 32, 52, 50, 25, 12]
          }
        ]
      },
      currentDataSet: [],
      dateBegin: null,
      dateEnd: null
    }
  },
  mounted: function () {
    let currentYear = new Date().getFullYear()
    this.dateEnd = `${currentYear}-01-01`
    currentYear -= 1
    this.dateBegin = `${currentYear}-01-01`
  },
  watch: {
    dateBegin: async function (val) {
      const res = await SalesService.getInvoices(this.dateBegin, this.dateEnd)
      this.currentDataSet = res.data
    },
    dateEnd: async function (val) {
      const res = await SalesService.getInvoices(this.dateBegin, this.dateEnd)
      this.currentDataSet = res.data
    }
  },
  components: {
    LineChart
  }
}
</script>

<style scoped>

.allSize{
  top: 0px;
  bottom: 0px;

  padding:0px;
  margin:0px;
  min-height: 0px;
  position: absolute;
  min-width: 0px;
}

.vertical-center{
  vertical-align: center;
}

.relative{
  position: relative;
}

.chartHolder{
  width: 100%;
}

.limitHeight{
  max-height: 200px;
}

.chartHolder{
  position: relative;
  height: 300px;
  width: 100%;
  min-width: 0;
  min-height: 0;
}

</style>

