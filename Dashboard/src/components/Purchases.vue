<template>
  <v-container mt-2 grid-list-xs>
    <v-dialog 
      max-width="1000px"
      v-model="supplierDialog">
      <v-card>
        <v-card-title>
          <span class="headline"> Products </span>
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
    <v-layout row wrap>
      <v-flex d-flex xs10 offset-xs1>
        <v-card>
          <v-card-title>
            <div class="headline"> Purchases </div>
            <div class="ml-3"><b>Total : {{(totalAmount + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ")}}€</b> between {{dateBegin}} and {{dateEnd}}</div>
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
    <div class="mt-2"></div>
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
                  <td> {{(props.item.TotalMerc + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") }}€</td>
                  <td> {{props.item.Serie }} </td>
                </tr>
              </template>
              <template slot="expand" scope="props">
                <v-card color="grey lighten-3">
                  <v-card-text>
                    <table>
                      <tr>
                      <th>Artigo</th><th>Descr</th><th>Quantidade</th><th>PreçoUn</th><th>Armazem</th>
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
      <v-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Top Suppliers</div>
            <p>between {{dateBegin}} and {{dateEnd}} </p>
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
                <tr class="pointer" @click="() => {displaySupplierModal(props.item.CodFornecedor)}">
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
    </v-layout>
  </v-container>
</template>

<script>

import LineChart from '@/components/charts/LineChart'
import PurchasesService from '@/services/Purchases'
import Products from '@/services/Products'
import ChartOptions from '@/components/charts/config'

export default {
  components: {
    LineChart
  },
  methods: {
    async displaySupplierModal (id) {
      try {
        this.supplierProducts = null
        this.supplierDialog = true
        const res = await Products.getProductsBySupplier(id)
        const totalSupplier = await PurchasesService.getTotalAmountBySupplier(this.dateBegin, this.dateEnd, id)
        console.log(totalSupplier)
        this.supplierProducts = res.data
      } catch (err) {
      }
    }
  },
  data () {
    return {
      search_1: '',
      search_2: '',
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
        {text: 'Entidade', value: 'Entidade', align: 'left'},
        {text: 'Tipo Doc', value: 'TipoDoc', align: 'center'},
        {text: 'Data', value: 'Data', align: 'center'},
        {text: 'Total Merc', value: 'TotalMerc', align: 'center'},
        {text: 'Serie', value: 'Serie', align: 'center'}
      ],
      supplierProductsHeader: [
        {text: 'Produto', value: 'CodArtigo', align: 'left'},
        {text: 'Descrição', value: 'DescArtigo', align: 'center'},
        {text: 'Stock Atual', value: 'STKAtual', align: 'center'}
      ],
      suppliersHeader: [
        {text: 'Fornecedor', value: 'CodFornecedor', align: 'left'},
        {text: 'Nome Fiscal', value: 'NomeFiscal', align: 'center'},
        {text: 'Telefone', value: 'Telefone', align: 'center'},
        {text: 'NumContribuinte', value: 'NumContrib', align: 'center'},
        {text: 'Total', value: 'Total', align: 'center'}
      ],
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
      this.currentDataSet = res.data
      this.totalAmount = total.data
      this.items = sups.data
    },
    dateEnd: async function (val) {
      const res = await PurchasesService.request(this.dateBegin, val)
      const total = await PurchasesService.getTotalAmount(this.dateBegin, val)
      const sups = await PurchasesService.getSuppliers(this.dateBegin, val)
      this.currentDataSet = res.data
      this.totalAmount = total.data
      this.items = sups.data
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
</style>

