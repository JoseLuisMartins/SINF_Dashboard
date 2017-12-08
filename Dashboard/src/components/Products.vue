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

    <v-layout row wrap class="mb-4">
      <v-flex d-flex xs12 sm12 offset-lg2 lg8 >
        <v-expansion-panel >
          <v-expansion-panel-content>
            <div slot="header" class="headline">
            Total = {{(this.totalInventoryValue + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ")+ "€"}}
            </div>
            <v-card>
              <v-card-title class="pb-0">
                <div class="headline"> Inventory </div>
                <v-spacer></v-spacer>
                <v-text-field
                  append-icon="search"
                  label="Search"
                  single-line
                  hide-details
                  v-model="search_3"
                ></v-text-field>
              </v-card-title>
              <v-card-text>
                <v-data-table 
                  :search="search_3"
                  v-bind:headers="stockHeader"
                  :items="inventory"
                  class="elevation-1"
                  :loading="inventory.length == 0"
                >
                  <template slot="items" scope="props">
                    <td class="text-xs-right">{{ props.item.ProductID }}</td>
                    <td class="text-xs-right">{{ props.item.ProductDesc }}</td> 
                    <td class="text-xs-right">{{ props.item.ActualSTK }}</td>
                    <td class="text-xs-right">{{ (props.item.PCM.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€" }}</td>
                    <td class="text-xs-right">{{ (props.item.TotalValue.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€" }}</td>
                  </template>

                </v-data-table>
              </v-card-text>
            </v-card>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-flex>

      <v-flex d-flex xs12 sm12 offset-lg2 lg8 >

        <v-card>
          <v-card-title class="headline"> Total Inventory Chart (In €)</v-card-title>
          <v-card-text>

            <div style="min-height: 400px"> 
              <transition name="fade">
                <loading color="teal" v-if="inventoryChartData.datasets == null"> </loading>
              </transition>
              <transition name="fade">
                <pie-chart class="chartHolder" style="min-height: 400px" 
                  v-if="inventoryChartData.datasets != null" 
                  :chartData="inventoryChartData.datasets" :options="pieChartOptions">
                </pie-chart>
              </transition>
            </div>
          </v-card-text>
        </v-card>

      </v-flex>
    </v-layout>
   
    <v-layout row wrap class="mb-1">
      <v-flex xs12 sm12 offset-lg2 lg4 md6 class="elevation-1">
        <v-layout collumn wrap>
          <v-flex xs12>
            <v-expansion-panel>
              <v-expansion-panel-content>
                <div slot="header" class="headline">
                    IN   Total = {{(this.totalIn + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ")+ "€"}}
                </div>
                <v-card>
                  <v-card-title class="pb-0">
                    <v-spacer></v-spacer>
                    <v-text-field
                      append-icon="search"
                      label="Search"
                      single-line
                      hide-details
                      v-model="search_1"
                    ></v-text-field>
                  </v-card-title>
                  <v-card-text>
                    <v-data-table 
                      :search="search_1"
                      v-bind:headers="headers"
                      :items="productsIn"
                      class="elevation-1"
                      :loading="productsIn == null"
                    >
                      <template slot="items" scope="props">
                        <td class="text-xs-right">{{ props.item.CodArtigo }}</td>
                        <td class="text-xs-right">{{ props.item.Description }}</td> 
                        <td class="text-xs-right">{{ props.item.Stock }}</td>
                        <td class="text-xs-right">{{ (props.item.Value.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€" }}</td>
                      </template>

                    </v-data-table>
                  </v-card-text>
                </v-card>
              </v-expansion-panel-content>
            </v-expansion-panel>
          </v-flex>
          <v-flex xs12>
            <v-card>
              <v-card-title class="headline"> By Families </v-card-title>
              <div style="min-height: 400px"> 
                <transition name="fade">
                  <loading color="teal" v-if="inventoryChartData.familiesIN == null"> </loading>
                </transition>
                <transition name="fade">
                  <pie-chart class="chartHolder" style="min-height: 400px" 
                    v-if="inventoryChartData.familiesIN != null" 
                    :chartData="inventoryChartData.familiesIN" :options="pieChartOptions">
                  </pie-chart>
                </transition>
              </div>
            </v-card>
          </v-flex>
        </v-layout>
      </v-flex>

      <v-flex xs12 sm12 lg4 md6 class="elevation-1">
        <v-layout collumn wrap>
          <v-flex xs12> 
            <v-expansion-panel >
              <v-expansion-panel-content>
                <div slot="header" class="headline">
                    Out   Total = {{ (this.totalOut + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€" }} 
                </div>
                <v-card>
                  <v-card-title class="pb-0">
                    <v-spacer></v-spacer>
                    <v-text-field
                      append-icon="search"
                      label="Search"
                      single-line
                      hide-details
                      v-model="search_2"
                    ></v-text-field>
                  </v-card-title>
                  <v-card-text>
                    <v-data-table
                      :search="search_2"
                      v-bind:headers="headers"
                      :items="productsOut"
                      class="elevation-1"
                      :loading="productsOut == null"
                    >
                      <template slot="items" scope="props">
                        <td class="text-xs-right">{{ props.item.CodArtigo }}</td> 
                        <td class="text-xs-right">{{ props.item.Description }}</td>
                        <td class="text-xs-right">{{ props.item.Stock }}</td>
                        <td class="text-xs-right">{{ (props.item.Value.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€" }}</td>
                      </template>

                    </v-data-table>
                  </v-card-text>
                </v-card>
              </v-expansion-panel-content>
            </v-expansion-panel>
          </v-flex>
          <v-flex xs12>
            <v-card>
              <v-card-title class="headline"> By Families </v-card-title>
              <div style="min-height: 400px"> 
                <transition name="fade">
                  <loading color="teal" v-if="inventoryChartData.familiesOut == null"> </loading>
                </transition>
                <transition name="fade">
                  <pie-chart class="chartHolder" style="min-height: 400px" 
                    v-if="inventoryChartData.familiesOut != null" 
                    :chartData="inventoryChartData.familiesOut" :options="pieChartOptions">
                  </pie-chart>
                </transition>
              </div>
            </v-card>
          </v-flex>
        </v-layout>
      </v-flex>

    </v-layout>

    <v-layout row wrap>
      <v-flex d-flex xs12 offset-lg2 lg8>
        <v-card>
          <v-card-title>
            <div class="headline"> Inventory Value </div>
          </v-card-title>
          <div class="relative limitHeight chartHolder pa-0" style="min-height:400px">
            <transition name="fade">
              <loading class="transition" color="teal" v-if="movementsGraph == null"> 
              </loading>
            </transition>
            <transition name="fade">
              <line-chart class="transition chartHolder" style="min-height:400px"
              v-if="movementsGraph != null"
              :chartData="movementsGraph" :options="chartOptions"> 
              </line-chart>
            </transition>
          </div>
        </v-card>
      </v-flex>
    </v-layout>

  </v-container>
</template>

<script>
import Products from '@/services/Products'
import ChartOptions from '@/components/charts/config'
import LineChart from '@/components/charts/LineChart'
import PieChart from '@/components/charts/PieChart'
import Loading from '@/components/loadings/Loading'

export default {
  components: {
    LineChart,
    PieChart,
    Loading
  },
  data () {
    return {
      search_1: '',
      search_2: '',
      search_3: '',
      totalIn: 0,
      totalOut: 0,
      inventoryValue: 200,
      pagination: {
        sortBy: 'Codigo'
      },
      selected: [],
      headers: [
        { text: 'Codigo', value: 'Code', align: 'left' },
        { text: 'Descrição', value: 'DescArtigo', allign: 'left' },
        { text: 'Stock', value: 'Stock', allign: 'left' },
        { text: 'Value', value: 'Value', allign: 'left' }
      ],
      stockHeader: [
        { text: 'Artigo', value: 'ProductID', allign: 'left' },
        { text: 'Descrição', value: 'ProductDesc', allign: 'left' },
        { text: 'Quantidade', value: 'ActualSTK', allign: 'left' },
        { text: 'Preço medio', value: 'PCM', allign: 'left' },
        { text: 'Total', value: 'TotalValue', allign: 'left' }
      ],
      inventoryChartData: {
        datasets: null,
        familiesIN: null,
        familiesOut: null
      },
      productsIn: [],
      productsOut: [],
      chartOptions: ChartOptions.options2,
      pieChartOptions: ChartOptions.pieOptions,

      movementsGraph: null,
      inventoryChart: null,

      inventory: [],
      totalInventoryValue: 0,
      dateBegin: null,
      menu: false,
      dateEnd: null,

      error: null
    }
  },
  methods: {
    toggleAll () {
      if (this.selected.length) this.selected = []
      else this.selected = this.items.slice()
    },
    changeSort (column) {
      if (this.pagination.sortBy === column) {
        this.pagination.descending = !this.pagination.descending
      } else {
        this.pagination.sortBy = column
        this.pagination.descending = false
      }
    },
    async getSTKIn () {
      try {
        this.productsIn = await Products.getMovements(this.dateBegin, this.dateEnd, 'IN')
        this.productsIn = this.productsIn.data
        this.totalIn = 0

        for (let product of this.productsIn) {
          this.totalIn += product.Value
        }

        this.totalIn = this.totalIn.toFixed(2)
      } catch (error) {
        this.error = error
      }
    },
    async getSTKOut () {
      try {
        this.productsOut = await Products.getMovements(this.dateBegin, this.dateEnd, 'OUT')
        this.productsOut = this.productsOut.data
        this.totalOut = 0

        for (let product of this.productsOut) {
          this.totalOut += product.Value
        }

        this.totalOut = this.totalOut.toFixed(2)
      } catch (error) {
        this.error = error
      }
    },
    prepareFamilyChart (contents) {
      let labels = []
      let data = []
      let backgroundColor = []
      for (let element of contents) {
        if (element.value <= 0) continue
        labels.push(element.description)
        data.push(element.value.toFixed(0))
        let color = `#${((1 << 24) * Math.random() | 0).toString(16)}`

        backgroundColor.push(color)
      }
      return {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: backgroundColor
        }]
      }
    },

    async getSTKInChart () {
      try {
        let response = await Products.getMovements(this.dateBegin, this.dateEnd, 'INC')
        this.inventoryChartData.familiesIN = this.prepareFamilyChart(response.data)
      } catch (error) {
        this.error = error
      }
    },

    async getSTKOutChart () {
      try {
        console.log('HEREA')
        let response = await Products.getMovements(this.dateBegin, this.dateEnd, 'OUTC')
        console.log('HERE')
        this.inventoryChartData.familiesOut = this.prepareFamilyChart(response.data)
        console.log('HEREC')
      } catch (error) {
        console.log(error)
        this.error = error
      }
    },

    getSTKMovements () {
      this.getSTKIn()
      this.getSTKOut()
      this.getSTKInChart()
      this.getSTKOutChart()
    },

    sortData (movements) {
      let contents = []

      for (var movement of movements) {
        contents.push({
          x: new Date(movement.year, movement.month),
          y: movement.value
        })
      }
      contents.sort((x, y) => (x.x > y.x) ? 1 : -1)
      return contents
    },

    async getMovementGraphData () {
      try {
        let tempData = await Products.getMovementsGraph(this.dateBegin, this.dateEnd)
        tempData = tempData.data

        this.movementsGraph = {
          datasets: [
            {
              pointRadius: 3,
              pointHoverRadius: 6,
              pointBackgroundColor: '#FF5522',
              backgroundColor: '#FF5522',
              borderColor: '#FF5522',
              borderWidth: 3,
              showLine: true,
              label: 'Movements In',
              snapGaps: false,
              data: this.sortData(tempData.movementsIn)
            },
            {
              pointRadius: 3,
              pointHoverRadius: 6,
              pointBackgroundColor: '#2255FF',
              backgroundColor: '#2255FF',
              borderColor: '#2255FF',
              borderWidth: 3,
              showLine: true,
              label: 'Movements Out',
              snapGaps: false,
              data: this.sortData(tempData.movementsOut)
            }
          ]
        }
      } catch (error) {
        this.error = error
      }
    },
    async getInventoryByFamilies () {
      try {
        let response = await Products.getTotalInventoryByFamilies(this.dateEnd)
        let contents = response.data
        this.inventoryChartData.datasets = this.prepareFamilyChart(contents)
      } catch (error) {
        this.error = error
      }
    },
    async getInventory () {
      try {
        let response = await Products.getInventory(this.dateEnd)
        this.inventory = response.data

        this.totalInventoryValue = 0

        for (let article of this.inventory) {
          this.totalInventoryValue += article.TotalValue
        }

        this.totalInventoryValue = this.totalInventoryValue.toFixed(2)
      } catch (error) {
        this.error = error
      }
    }
  },
  watch: {
    dateBegin: function (val) {
      this.getSTKMovements()
      this.getMovementGraphData()
    },
    dateEnd: function (val) {
      this.getSTKMovements()
      this.getMovementGraphData()
      this.getInventory()
      this.getInventoryByFamilies()
    }
  },
  mounted: function () {
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

