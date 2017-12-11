<template>
  <v-container mt-5 grid-list-xs>

    <v-dialog v-model="showInvoiceDialog" scrollable max-width="1000">
      <invoice v-if="invoiceItem!==null" :ShipFromAddressDetail="invoiceItem.ShipFrom.Address.AddressDetail" :ShipFromCity="invoiceItem.ShipFrom.Address.City"
        :ShipFromPostalCode="invoiceItem.ShipFrom.Address.PostalCode" :ShipToAddressDetail="invoiceItem.ShipTo.Address.AddressDetail"
        :ShipToCity="invoiceItem.ShipTo.Address.City" :ShipToPostalCode="invoiceItem.ShipTo.Address.PostalCode" :InvoiceDate="invoiceItem.InvoiceDate"
        :InvoiceNo="invoiceItem.InvoiceNo" :CustomerID="invoiceItem.CustomerID" :Lines="invoiceItem.Line" :NetTotal="invoiceItem.DocumentTotals.NetTotal"
        :GrossTotal="invoiceItem.DocumentTotals.GrossTotal">
      </invoice>
    </v-dialog>


    <v-dialog v-model="showProductDetailsDialog" scrollable max-width="1000">
      <sales-product-details v-if="productitem!==null" :Item="productitem" :Begin="dateBegin" :End="dateEnd">
      </sales-product-details>
    </v-dialog>


    <v-dialog v-model="showCustomerDetailsDialog" scrollable max-width="1000">
      <sales-customer-details v-if="customerItem!==null" :Item="customerItem" :Begin="dateBegin" :End="dateEnd">
      </sales-customer-details>
    </v-dialog>


    <v-layout>
      <v-flex mb-4 d-flex sm6 offset-sm3 xs12>
        <v-card>
          <v-card-text>
            <v-layout row wrap>
              <v-flex xs12 sm6>
                <v-menu lazy :close-on-content-click="false" transition="scale-transition" full-width v-model="menuDateBegin" offset-y :nudge-right="40"
                  max-width="290px" max-height="600px">

                  <v-text-field slot="activator" v-model="dateBegin" prepend-icon="event" readonly label="Begin Date"></v-text-field>

                  <v-date-picker v-model="dateBegin" no-title scrollable actions>
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
              <v-flex xs12 sm6>
                <v-menu lazy :clonse-on-content-click="false" transition="scale-transition" full-width v-model="menuDateEnd" :nudge-right="40"
                  max-width="290px" max-height="600px">

                  <v-text-field slot="activator" v-model="dateEnd" prepend-icon="event" readonly label="End Date"></v-text-field>

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
            <b > Total Sales </b>
            <v-spacer> </v-spacer>
            <b > {{totalSales}}  <v-icon large>euro_symbol</v-icon> </b>
          </v-card-title>
        </v-card>
    </v-flex>
   </v-layout>


    <v-layout row wrap>
      <v-flex d-flex xs12 md12>
        <v-card>
          <v-card-title>
            <div class="headline"> Sales </div>
          </v-card-title>
          <v-card-text>
            <div class="limitHeight chartHolder" v-if="salesChartData.datasets.length == 0">
              <v-layout justify-center>
                <v-flex class="loading a blue">L</v-flex>
                <v-flex class="loading b blue">o</v-flex>
                <v-flex class="loading c blue">a</v-flex>
                <v-flex class="loading d blue">d</v-flex>
                <v-flex class="loading e blue">i</v-flex>
                <v-flex class="loading f blue">n</v-flex>
                <v-flex class="loading g blue">g</v-flex>

              </v-layout>
            </div>
            <div v-if="salesChartData.datasets.length !== 0">
              <line-chart class="chartHolder" :chartData="salesChartData" :options="chartOptions"> </line-chart>
            </div>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

    <v-layout row wrap >
      <v-expansion-panel>
        <v-expansion-panel-content class="cyan darken-3" style="color:white">
          <div slot="header" class="headline" >
            All Products and Customer details
          </div>
          <v-layout row wrap>
            <v-flex d-flex sm12 md6 pa-1>
              <v-card>
                <v-card-title class="pb-0">
                  <div class="headline"> Products </div>
                  <v-spacer></v-spacer>
                  <v-text-field append-icon="search" label="Search" single-line hide-details v-model="search_1"></v-text-field>
                </v-card-title>
                <v-card-text>

                  <v-data-table v-bind:headers="productsHeader" :items="productsDataSet" :search="search_1" class="elevation-1" item-key="ProductCode"
                    :loading="productsDataSet.length == 0">

                    <template slot="items" scope="props">
                      <tr class="cursor-pointer" @click="() => { showProductDetailsDialog=true, productitem=props.item }">
                        <td> {{props.item.ProductGroup }} </td>
                        <td> {{props.item.ProductDescription }} </td>
                        <td> {{props.item.ProductNumberCode }} </td>
                      </tr>
                    </template>

                  </v-data-table>

                </v-card-text>
              </v-card>
            </v-flex>

            <v-flex d-flex sm12 md6 pa-1>
              <v-card>
                <v-card-title class="pb-0">
                  <div class="headline"> Customers </div>
                  <v-spacer></v-spacer>
                  <v-text-field append-icon="search" label="Search" single-line hide-details v-model="search_2"></v-text-field>
                </v-card-title>
                <v-card-text>

                  <v-data-table 
                    :search="search_2" 
                    v-bind:headers="customersHeader" 
                    :items="customersDataSet" 
                    class="elevation-1"
                    item-key="CustomerID"
                    :loading="customersDataSet.length == 0">

                    <template slot="items" scope="props">
                      <tr class="cursor-pointer" @click="() => { showCustomerDetailsDialog=true, customerItem=props.item }">
                        <td> {{props.item.CustomerID }} </td>
                        <td> {{props.item.CompanyName }} </td>
                      </tr>
                    </template>

                  </v-data-table>

                </v-card-text>
              </v-card>
            </v-flex>
          </v-layout>

        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-layout>

    <v-layout row wrap>
      <v-flex xs12 lg6>
        <v-card>
          <v-card-title class="headline"> Top Products </v-card-title>
          <div style="min-height: 400px">
            <transition name="fade">
              <loading color="teal" v-if="topsChartData.topProducts == null"> </loading>
            </transition>
            <transition name="fade">
              <pie-chart class="chartHolder" style="min-height: 400px" v-if="topsChartData.topProducts != null" :chartData="topsChartData.topProducts"
                :options="pieChartOptions">
              </pie-chart>
            </transition>
          </div>
        </v-card>
      </v-flex>

      <v-flex xs12 lg6>
        <v-card>
          <v-card-title class="headline"> Top Customers </v-card-title>
          <div style="min-height: 400px">
            <transition name="fade">
              <loading color="teal" v-if="topsChartData.topCustomers == null"> </loading>
            </transition>
            <transition name="fade">
              <pie-chart class="chartHolder" style="min-height: 400px" v-if="topsChartData.topCustomers != null" :chartData="topsChartData.topCustomers"
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
            <div class="headline"> Sales Invoices </div>
            <v-spacer></v-spacer>
            <v-text-field append-icon="search" label="Search" single-line hide-details v-model="search_4"></v-text-field>
          </v-card-title>
          <v-card-text>

            <v-data-table 
              :search="search_4" 
              v-bind:headers="invoiceHeader" 
              :items="invoicesDataSet" 
              class="elevation-1" 
              item-key="Hash"
              :loading="invoicesDataSet.length == 0">

              <template slot="items" scope="props">
                <tr class="cursor-pointer" @click="() => { showInvoiceDialog=true, invoiceItem=props.item }">
                  <td> {{props.item.InvoiceNo }} </td>
                  <td> {{props.item.InvoiceDate }} </td>
                  <td> {{props.item.InvoiceType }} </td>
                </tr>
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
            <div class="headline"> Sales Backlog </div>
            <v-spacer></v-spacer>
            <v-text-field append-icon="search" label="Search" single-line hide-details v-model="search_3"></v-text-field>
          </v-card-title>
          <v-card-text>

            <v-data-table 
              :search="search_3" 
              v-bind:headers="backlogHeader" 
              :items="backlogDataSet" 
              class="elevation-1" 
              item-key="id"
              :loading="backlogDataSet.length == 0">

              <template slot="items" scope="props">

                <tr class="cursor-pointer" @click="() => {openBacklogDialog(props.item)}">
                  <td> {{props.item.Entidade }} </td>
                  <td> {{props.item.Data }} </td>
                  <td> {{(props.item.TotalMerc.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€" }} </td>
                </tr>
              </template>

            </v-data-table>

          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>


     <v-dialog v-model="showSalesBacklogDialog"  max-width="1000px">
      <v-card v-if="salesBacklogItem!== null">
        <v-card-title>
          <span class="headline">  {{salesBacklogItem.Entidade}} </span>
          <v-spacer> </v-spacer>
           <span class="headline">  {{salesBacklogItem.Data}} </span>
        </v-card-title>
        <v-card-text>          
          <v-data-table
            :loading="salesBacklogItem == null"
            v-bind:headers="backlogProductsHeader"
            :items="salesBacklogItem.LinhasDoc"           
            >
            <template slot="items" scope="props" v-if="props.item.Quantidade != 0">
              <td> {{props.item.CodArtigo }} </td>
              <td> {{props.item.DescArtigo }} </td>
              <td> {{props.item.Quantidade }} </td>
              <td> {{(props.item.TotalLiquido.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + "€"  }} </td>
            </template>
          </v-data-table>

        </v-card-text>
      </v-card>
    </v-dialog>

  </v-container>
</template>


<script>
  import Invoice from '@/components/dialogs/Invoice'
  import SalesCustomerDetails from '@/components/dialogs/SalesCustomerDetails'
  import SalesProductDetails from '@/components/dialogs/SalesProductDetails'
  import LineChart from '@/components/charts/LineChart'
  import PieChart from '@/components/charts/PieChart'
  import SalesService from '@/services/Sales'
  import ChartOptions from '@/components/charts/config'
  import Loading from '@/components/loadings/Loading'

  export default {
    data () {
      return {
        search_1: '',
        search_2: '',
        search_3: '',
        search_4: '',
        menuDateBegin: false,
        menuDateEnd: false,
        productDetail: false,
        invoiceHeader: [{
          text: 'Invoice Number',
          value: 'InvoiceNo',
          align: 'left'
        },
        {
          text: 'Invoice Date',
          value: 'InvoiceDate',
          align: 'left'
        },
        {
          text: 'Invoice Type',
          value: 'InvoiceType',
          align: 'left'
        }
        ],
        productsHeader: [{
          text: 'Category',
          value: 'ProductGroup',
          align: 'left'
        },
        {
          text: 'Description',
          value: 'ProductDescription',
          align: 'left'
        },
        {
          text: 'Number code',
          value: 'ProductNumberCode',
          align: 'left'
        }
        ],
        customersHeader: [{
          text: 'Id',
          value: 'CustomerID',
          align: 'left'
        },
        {
          text: 'Company name',
          value: 'CompanyName',
          align: 'left'
        }
        ],
        backlogHeader: [{
          text: 'Entity',
          value: 'Entidade',
          align: 'left'
        },
        {
          text: 'Data',
          value: 'Data',
          align: 'left'
        },
        {
          text: 'Total value',
          value: 'TotalMerc',
          align: 'left'
        }
        ],
        backlogProductsHeader: [{
          text: 'Item code',
          value: 'CodArtigo',
          align: 'left'
        },
        {
          text: 'Item description',
          value: 'DescArtigo',
          align: 'left'
        },
        {
          text: 'Quantity',
          value: 'Quantidade',
          align: 'left'
        },
        {
          text: 'Total (without taxes)',
          value: 'TotalLiquido',
          align: 'left'
        }
        ],
        salesChartData: {
          datasets: []
        },
        topsChartData: {
          topProducts: null,
          topCustomers: null
        },
        chartOptions: ChartOptions.options,
        pieChartOptions: ChartOptions.pieOptions,
        invoicesDataSet: [],
        customersDataSet: [],
        productsDataSet: [],
        backlogDataSet: [],
        showInvoiceDialog: false,
        showCustomerDetailsDialog: false,
        showProductDetailsDialog: false,
        showSalesBacklogDialog: false,
        salesBacklogItem: null,
        invoiceItem: null,
        productitem: null,
        customerItem: null,
        dateBegin: null,
        dateEnd: null,
        totalSales: 0
      }
    },
    methods: {
      openBacklogDialog (item) {
        this.showSalesBacklogDialog = true
        this.salesBacklogItem = item
      },
      async getTops (begin, end) {
        let top10Products = (await SalesService.getTop10Products(begin, end)).data
        this.topsChartData.topProducts = this.prepareFamilyChart(top10Products, 'product_description', 'total_sold')

        let top10Customers = (await SalesService.getTop10Customers(begin, end)).data
        this.topsChartData.topCustomers = this.prepareFamilyChart(top10Customers, '_id', 'total_spent')
      },
      prepareFamilyChart (contents, key, value) {
        let labels = []
        let data = []
        let backgroundColor = []
        for (let element of contents) {
          if (element[value] <= 0) continue
          labels.push(element[key])
          data.push((element[value].toFixed(2) + '').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1'))
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
    mounted: async function () {
      let currentYear = new Date().getFullYear()
      this.dateEnd = `${currentYear}-01-01`
      currentYear -= 1
      this.dateBegin = `${currentYear}-01-01`

      this.customersDataSet = (await SalesService.getCustomers()).data
      this.productsDataSet = (await SalesService.getProducts()).data
    },
    watch: {
      dateBegin: async function (val) {
        const invoices = await SalesService.getInvoices(this.dateBegin, this.dateEnd)
        const backlog = await SalesService.getBacklog(this.dateBegin, this.dateEnd)

        this.backlogDataSet = backlog.data
        this.invoicesDataSet = invoices.data
        this.getTops(this.dateBegin, this.dateEnd)

        this.totalSales = ((await SalesService.getTotalNetSales(this.dateBegin, this.dateEnd)).data[0].total.toFixed(2) + '').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1 ')
      },
      dateEnd: async function (val) {
        const invoices = await SalesService.getInvoices(this.dateBegin, this.dateEnd)
        const backlog = await SalesService.getBacklog(this.dateBegin, this.dateEnd)

        this.backlogDataSet = backlog.data
        this.invoicesDataSet = invoices.data
        this.getTops(this.dateBegin, this.dateEnd)

        this.totalSales = ((await SalesService.getTotalNetSales(this.dateBegin, this.dateEnd)).data[0].total.toFixed(2) + '').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1 ')
      },
      invoicesDataSet: function (val) {
        let data = []
        let dict = {}

        for (var i = 0; i < val.length; i++) {
          val[i].NetTotal = val[i].DocumentTotals.NetTotal
          const mult = val[i].InvoiceType === 'NC' ? -1 : 1

          const regex = /(\d{4}-\d{2})/
          const date = val[i].InvoiceDate.match(regex)[1]
          dict[date] = Number(val[i].NetTotal) * mult + (dict[date] || 0)
        }

        for (let key in dict) {
          const dataString = key.split('-')
          data.push({
            x: new Date(Number(dataString[0]), Number(dataString[1])),
            y: dict[key]
          })
        }

        data.sort((a, b) => {
          return a.x > b.x ? 1 : a.x < b.x ? -1 : 0
        })

        this.salesChartData = {
          datasets: [{
            pointRadius: 3,
            pointHoverRadius: 6,
            pointBackgroundColor: '#FF5522',
            borderWidth: 3,
            showLine: true,
            label: 'Sales',
            snapGaps: false,
            data: data
          }]
        }
      }
    },
    components: {
      LineChart,
      Invoice,
      SalesCustomerDetails,
      SalesProductDetails,
      PieChart,
      Loading
    }
  }
</script>

<style scoped>
  .allSize {
    top: 0px;
    bottom: 0px;

    padding: 0px;
    margin: 0px;
    min-height: 0px;
    position: absolute;
    min-width: 0px;
  }

  .vertical-center {
    vertical-align: center;
  }

  .cursor-pointer {
    cursor: pointer;
  }

</style>
