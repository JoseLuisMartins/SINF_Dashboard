<template>
  <v-container mt-2 grid-list-xs>
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
      <v-flex d-flex xs12>
        <v-card>
          <v-card-title>
            <div class="headline"> Purchases </div>
          </v-card-title>
          <v-card-text>
            <div class="limitHeight chartHolder" v-if="purchasesChartData.datasets.length == 0"> 
              <v-layout justify-center>
                <v-flex class="loading a blue ">L</v-flex> 
                <v-flex class="loading b blue">o</v-flex> 
                <v-flex class="loading c blue">a</v-flex> 
                <v-flex class="loading d blue">d</v-flex> 
                <v-flex class="loading e blue">i</v-flex> 
                <v-flex class="loading f blue">n</v-flex> 
                <v-flex class="loading g blue">g</v-flex> 
              
              </v-layout>
              </div>
            <line-chart class="limitHeight chartHolder"
              v-if="purchasesChartData.datasets.length != 0"
              :chart-data="purchasesChartData"
              :options="chartOptions"> </line-chart>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Purchases Documents </div>
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
              :headers="purchasesHeader"
              :items="currentDataSet"
              class="elevation-1"
              item-key="id"
              :loading="currentDataSet.length == 0"
              >
              <template slot="items" scope="props">
                <tr @click="props.expanded = !props.expanded"> 
                  <td> {{props.item.Entidade}} </td>
                  <td> {{props.item.TipoDoc }} </td>
                  <td> {{props.item.Data }} </td>
                  <td> {{props.item.TotalMerc }} </td>
                  <td> {{props.item.Serie }} </td>
                </tr>
              </template>
              <template slot="expand" scope="props">
                <v-card color="grey lighten-3">
                  <v-card-text>
                    <table>
                      <tr>
                      <th>Art</th><th>Descr</th><th>Quantity</th><th>UnitPrice</th><th>LiquidPrice</th><th>Warehouse</th><th>Lot</th>
                      </tr>
                      <tr flat v-for="line in props.item.LinhasDoc" :key="line.id">
                        <td>{{line.CodArtigo}}</td>
                        <td>{{line.DescArtigo}}</td>
                        <td>{{Math.abs(line.Quantidade)}}</td>
                        <td>{{Math.abs(line.PrecoUnitario)}}</td>
                        <td>{{Math.abs(line.TotalLiquido)}}</td>
                        <td>{{line.Armazem}}</td>
                        <td>{{line.Lote}}</td>
                      </tr>
                    </table>
                  </v-card-text>
                </v-card>
              </template>

            </v-data-table>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Suppliers </div>
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
              v-bind:headers="suppliersHeader"
              :items="items"
              :loading="currentDataSet.length == 0"
              class="elevation-1"
              >
              <template slot="items" scope="props">
                <td> {{props.item.CodFornecedor }} </td>
                <td> {{props.item.NomeFornecedor }} </td>
                <td> {{props.item.Telefone }} </td>
                <td> {{props.item.NumContribuinte }} </td>
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
import PurchasesService from '@/services/Purchases'
import ChartOptions from '@/components/Charts/config'

export default {
  components: {
    LineChart
  },
  methods: {
    getSup: async function () {
      const res = await PurchasesService.getSuppliers()
      this.items = res.data
    }
  },
  beforeMount () {
    this.getSup()
  },
  data () {
    return {
      search_1: '',
      search_2: '',
      linhasDoc: [
        {text: 'Code', value: 'CodArtigo', align: 'left'},
        {text: 'Description', value: 'DescArtigo'},
        {text: 'Quantity', value: 'Quantidade'},
        {text: 'Unity', value: 'Unidade'},
        {text: 'Discount', value: 'Desconto'},
        {text: 'Unit Price', value: 'PrecoUnitario'},
        {text: 'Total Liquid', value: 'TotalLiquido'},
        {text: 'Warehouse', value: 'Armazem'},
        {text: 'Lot', value: 'Lote'}
      ],
      purchasesHeader: [
        {text: 'Entity', value: 'Entidade', align: 'left'},
        {text: 'TypeDoc', value: 'TipoDoc', align: 'center'},
        {text: 'Date', value: 'Data', align: 'center'},
        {text: 'Total Merc', value: 'TotalMerc', align: 'center'},
        {text: 'Serie', value: 'Serie', align: 'center'}
      ],
      suppliersHeader: [
        {text: 'Fornecedor', value: 'CodFornecedor', align: 'left'},
        {text: 'Nome', value: 'Nome', align: 'center'},
        {text: 'Telefone', value: 'Telefone', align: 'center'},
        {text: 'NumContribuinte', value: 'NumContrib', align: 'center'}
      ],
      menu: false,
      dateBegin: null,
      dateEnd: null,
      currentDataSet: [],
      items: [],
      purchasesChartData: {
        datasets: []
      },
      chartOptions: ChartOptions.options
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
      const res = await PurchasesService.request(this.dateBegin, this.dateEnd)
      this.currentDataSet = res.data
    },
    dateEnd: async function (val) {
      const res = await PurchasesService.request(this.dateBegin, this.dateEnd)
      this.currentDataSet = res.data
    },
    currentDataSet: function (val) {
      let data = []
      let dict = {}

      for (var i = 0; i < val.length; i++) {
        val[i].TotalMerc = Math.abs(val[i].TotalMerc)
        const regex = /(\d{4}-\d{2}-\d{2})/
        const date = val[i].Data.match(regex)[1]
        dict[date] = Number(val[i].TotalMerc) + (dict[date] || 0)
      }

      for (let key in dict) {
        const dataString = key.split('-')
        data.push({
          x: new Date(Number(dataString[0]), Number(dataString[1]), Number(dataString[2])),
          y: dict[key]
        })
      }

      data.sort((a, b) => {
        return a.x > b.x ? 1 : (a.x < b.x ? -1 : 0)
      })

      console.log(data)

      this.purchasesChartData = {
        datasets: [
          {
            pointRadius: 3,
            pointHoverRadius: 6,
            pointBackgroundColor: '#FF5522',
            borderWidth: 3,
            showLine: true,
            label: 'Sales',
            snapGaps: false,
            data: data
          }
        ]
      }
    }
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
</style>

