<template>
  <v-container mt-2 grid-list-xs>
    <v-dialog 
      max-width="1000px"
      v-model="supplierDialog">
      <v-card>
        <v-card-title>
          <span class="headline"> Products from {{currentSupplier}} </span>
        </v-card-title>
        <v-card-text>
          <div v-if="supplierProducts == null"> Loading... </div>
          <v-data-table v-if="supplierProducts != null"
            :loading="supplierProducts == null"
            v-bind:headers="supplierProductsHeader"
            :items="supplierProducts"
            >
            <template slot="items" scope="props">
              <td> {{props.item.CodArtigo }} </td>
              <td> {{props.item.DescArtigo }} </td>
              <td> {{props.item.STKAtual }} </td>
            </template>
          </v-data-table>

        </v-card-text>
      </v-card>
    </v-dialog>
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

    <v-layout>
        <v-flex mb-4 d-flex sm6 offset-sm3 xs12>             
          <v-card style="color:#f2f2f2">
            <v-card-title class="headline green darken-2" >
              <b > Total Purchases </b>
              <v-spacer> </v-spacer>
              <b > {{(totalAmount + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ")}} <v-icon large>euro_symbol</v-icon> </b>
            </v-card-title>
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
                <v-flex class="loading b blue">O</v-flex> 
                <v-flex class="loading c blue">A</v-flex> 
                <v-flex class="loading d blue">D</v-flex> 
                <v-flex class="loading e blue">I</v-flex> 
                <v-flex class="loading f blue">N</v-flex> 
                <v-flex class="loading g blue">G</v-flex> 
              
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
      <v-flex d-flex sm12 md8>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline">Suppliers</div>
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
                <tr class="cursor-pointer" @click="() => {displaySupplierModal(props.item.CodFornecedor, props.item.NomeFiscal)}">
                <td> {{props.item.CodFornecedor }} </td>
                <td> {{props.item.NomeFiscal }} </td>
                <td> {{props.item.Telefone }} </td>
                <td> {{props.item.NumContribuinte }} </td>
                <td> {{ props.item.Total.toFixed(2 + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") }} €</td>
                </tr>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-flex>

      <v-flex d-flex sm12 md4>
        <v-card>
          <v-card-title class="headline"> Top Suppliers </v-card-title>
          <div style="min-height: 400px">
            <transition name="fade">
              <loading color="teal" v-if="topChartData == null"> </loading>
            </transition>
            <transition name="fade">
              <pie-chart class="chartHolder" style="min-height: 400px" v-if="topChartData != null" :chartData="topChartData"
                :options="pieChartOptions">
              </pie-chart>
            </transition>
          </div>
        </v-card>
      </v-flex>
    </v-layout>

    <v-layout row wrap>
      <v-flex sm12>
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
                  <tr class="cursor-pointer" @click="props.expanded = !props.expanded"> 
                    <td> {{props.item.Entidade}} </td>
                    <td> {{props.item.TipoDoc }} </td>
                    <td> {{props.item.Data }} </td>
                    <td> {{(props.item.TotalMerc + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") }}€</td>
                    <td> {{props.item.Serie }} </td>
                  </tr>
                </template>
                <template slot="expand" scope="props">
                  <v-card color="grey lighten-3">
                    <v-card-text>
                      <table>
                        <tr>
                        <th>Product</th><th>Description</th><th>Quantity</th><th>Unit Price</th><th>Warehouse</th>
                        </tr>
                        <tr flat v-for="line in props.item.LinhasDoc" :key="line.id">
                          <td>{{line.CodArtigo}}</td>
                          <td>{{line.DescArtigo}}</td>
                          <td>{{Math.abs(line.Quantidade)}}</td>
                          <td>{{Math.abs(line.PrecoUnitario)}}</td>
                          <td>{{line.Armazem}}</td>
                        </tr>
                      </table>
                    </v-card-text>
                  </v-card>
                </template>

              </v-data-table>
            </v-card-text>
          </v-card>
        </v-flex>
      </v-layout>

    <v-layout row wrap>
        <v-flex sm12>
          <v-card>
            <v-card-title class="pb-0">
              <div class="headline"> Purchases Backlog </div>
              <v-spacer></v-spacer>
              <v-text-field append-icon="search" label="Search" single-line hide-details v-model="search_3"></v-text-field>
            </v-card-title>
            <v-card-text>

              <v-data-table :search="search_3" v-bind:headers="backlogHeader" :items="backlogData" class="elevation-1" :loading="backlogData.length == 0">

                <template slot="items" scope="props">

                  <td> {{props.item.Entidade }} </td>
                  <td> {{props.item.Artigo }} </td>
                  <td> {{props.item.DataEntrega }} </td>
                  <td> {{props.item.Quantidade.toFixed(0) + ""}} </td>
                  <td> {{(props.item.Total.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€" }} </td>

                </template>

              </v-data-table>

            </v-card-text>
          </v-card>
        </v-flex>
    </v-layout>
  </v-container>
</template>

<script>

import LineChart from '@/components/charts/LineChart'
import PurchasesService from '@/services/Purchases'
import Products from '@/services/Products'
import ChartOptions from '@/components/charts/config'
import PieChart from '@/components/charts/PieChart'
import Loading from '@/components/loadings/Loading'

export default {
  components: {
    LineChart,
    PieChart,
    Loading
  },
  methods: {
    async displaySupplierModal (id, name) {
      try {
        this.supplierProducts = null
        this.currentSupplier = name
        this.supplierDialog = true
        const res = await Products.getProductsBySupplier(id)
        this.supplierProducts = res.data
      } catch (err) {
      }
    },
    prepareFamilyChart (contents, key, value) {
      let labels = []
      let data = []
      let backgroundColor = []
      for (let element of contents) {
        if (element[value] <= 0) continue
        labels.push(element[key])
        data.push(element[value].toFixed(0))
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
    }
  },
  data () {
    return {
      search_1: '',
      search_2: '',
      search_3: '',
      pieChartOptions: ChartOptions.pieOptions,
      topChartData: null,
      currentSupplier: '',
      totalAmount: 0,
      linhasDoc: [
        {text: 'Code', value: 'CodArtigo', align: 'left'},
        {text: 'Description', value: 'DescArtigo'},
        {text: 'Quantity', value: 'Quantidade'},
        {text: 'Unity', value: 'Unidade'},
        {text: 'Discount', value: 'Desconto'},
        {text: 'Unit Price', value: 'PrecoUnitario'},
        {text: 'Warehouse', value: 'Armazem'}
      ],
      purchasesHeader: [
        {text: 'Supplier', value: 'Entidade', align: 'left'},
        {text: 'Document Type', value: 'TipoDoc', align: 'center'},
        {text: 'Date', value: 'Data', align: 'center'},
        {text: 'Total Merc', value: 'TotalMerc', align: 'center'},
        {text: 'Serie', value: 'Serie', align: 'center'}
      ],
      supplierProductsHeader: [
        {text: 'Product', value: 'CodArtigo', align: 'left'},
        {text: 'Description', value: 'DescArtigo', align: 'center'},
        {text: 'Current Stock', value: 'STKAtual', align: 'center'}
      ],
      suppliersHeader: [
        {text: 'Supplier', value: 'CodFornecedor', align: 'left'},
        {text: 'Fiscal Name', value: 'NomeFiscal', align: 'center'},
        {text: 'Phone Number', value: 'Telefone', align: 'center'},
        {text: 'VAT Number', value: 'NumContrib', align: 'center'},
        {text: 'Total', value: 'Total', align: 'center'}
      ],
      backlogHeader: [
        {text: 'Supplier', value: 'Entidade', align: 'left'},
        {text: 'Product', value: 'Artigo', align: 'center'},
        {text: 'Date', value: 'DataEntrega', align: 'center'},
        {text: 'Quantity', value: 'Quantidade', align: 'center'},
        {text: 'Total', value: 'Total', align: 'center'}
      ],
      backlogData: [],
      menu: false,
      dateBegin: null,
      dateEnd: null,
      currentDataSet: [],
      items: [],
      purchasesChartData: {
        datasets: []
      },
      chartOptions: ChartOptions.options,
      supplierDialog: false,
      supplierProducts: [
        {CodArtigo: '13123', DescArtigo: 'Descr', STKAtual: 'Stk'}
      ]
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
      const res = await PurchasesService.request(val, this.dateEnd)
      const total = await PurchasesService.getTotalAmount(val, this.dateEnd)
      const sups = await PurchasesService.getSuppliers(val, this.dateEnd)
      const data = await PurchasesService.getPurchasesBacklog(val, this.dateEnd)
      this.currentDataSet = res.data
      this.totalAmount = total.data
      this.items = sups.data
      this.backlogData = data.data
      this.topChartData = this.prepareFamilyChart(sups.data, 'NomeFiscal', 'Total')
    },
    dateEnd: async function (val) {
      const res = await PurchasesService.request(this.dateBegin, val)
      const total = await PurchasesService.getTotalAmount(this.dateBegin, val)
      const sups = await PurchasesService.getSuppliers(this.dateBegin, val)
      const data = await PurchasesService.getPurchasesBacklog(this.dateBegin, val)
      this.currentDataSet = res.data
      this.totalAmount = total.data
      this.items = sups.data
      this.backlogData = data.data
      this.topChartData = this.prepareFamilyChart(sups.data, 'NomeFiscal', 'Total')
    },
    currentDataSet: function (val) {
      let data = []
      let dict = {}

      for (var i = 0; i < val.length; i++) {
        val[i].TotalMerc = Math.abs(val[i].TotalMerc)

        const regex = /(\d{4}-\d{2})/
        const date = val[i].Data.match(regex)[1]
        dict[date] = Number(val[i].TotalMerc) + (dict[date] || 0)
      }

      for (let key in dict) {
        const dataString = key.split('-')
        data.push({
          x: new Date(Number(dataString[0]), Number(dataString[1])),
          y: dict[key]
        })
      }

      data.sort((a, b) => {
        return a.x > b.x ? 1 : (a.x < b.x ? -1 : 0)
      })

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

.cursor-pointer {
    cursor: pointer;
}

</style>

