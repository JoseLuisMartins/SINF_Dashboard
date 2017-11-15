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
          </v-card-title>
          <v-card-text >
            <line-chart class="chartHolder" :chartData="salesChartData" :options="chartOptions"> </line-chart>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

    <v-layout row wrap>
      <v-flex d-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Products </div>
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
              v-bind:headers="productsHeader"
              :items="productsDataSet"
              :search="search_1"
              class="elevation-1"
              >

              <template slot="items" scope="props">
                
                <td> {{props.item.ProductGroup }} </td>
                <td> {{props.item.ProductDescription }} </td>
                <td> {{props.item.ProductNumberCode }} </td>
               
              </template>

            </v-data-table>

          </v-card-text>
        </v-card>
      </v-flex>

      <v-flex d-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Customers </div>
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
              v-bind:headers="customersHeader"
              :items="customersDataSet"
              class="elevation-1"
              >

              <template slot="items" scope="props">
                
                <td> {{props.item.CustomerID }} </td>
                <td> {{props.item.CompanyName }} </td>

              </template>

            </v-data-table>

          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

     <v-layout row wrap>
      <v-flex sm12 >
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Sales Backlog </div>
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
              v-bind:headers="backlogHeader"
              :items="backlogDataSet"
              class="elevation-1"
              >

              <template slot="items" scope="props">
                
                <td> {{props.item.Entidade }} </td>
                <td> {{props.item.Data }} </td>
                <td> {{props.item.TotalMerc }} </td>

              </template>

            </v-data-table>

          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

    <v-layout row wrap>
      <v-flex sm12 >
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Sales Invoices </div>
            <v-spacer></v-spacer>
            <v-text-field
              append-icon="search"
              label="Search"
              single-line
              hide-details
              v-model="search_4"
            ></v-text-field>
          </v-card-title>
          <v-card-text>

            <v-data-table
              :search="search_4"
              v-bind:headers="invoiceHeader"
              :items="invoicesDataSet"
              class="elevation-1"
              item-key="Hash"
              >
              <template slot="items" scope="props">
                <tr  @click="props.expanded = !props.expanded">
                  <td> {{props.item.InvoiceNo }} </td>
                  <td> {{props.item.InvoiceDate }} </td>
                  <td> {{props.item.InvoiceType }} </td>
                </tr>
              </template>
              <template slot="expand" scope="props">
                <v-card flat>
                  <v-card-text>
                  
                <invoice
                  :ShipFromAddressDetail="props.item.ShipFrom.Address.AddressDetail"
                  :ShipFromCity="props.item.ShipFrom.Address.City"
                  :ShipFromPostalCode="props.item.ShipFrom.Address.PostalCode"
                  :ShipToAddressDetail="props.item.ShipTo.Address.AddressDetail"
                  :ShipToCity="props.item.ShipTo.Address.City"
                  :ShipToPostalCode="props.item.ShipTo.Address.PostalCode"
                  :InvoiceDate="props.item.InvoiceDate"
                  :InvoiceNo="props.item.InvoiceNo"
                  :CustomerID="props.item.CustomerID"
                  :Lines="props.item.Line"
                  :NetTotal="props.item.DocumentTotals.NetTotal"
                  :GrossTotal="props.item.DocumentTotals.GrossTotal"
                >
                </invoice>
                   

                  </v-card-text>
                </v-card>
              </template>

            </v-data-table>

          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

  </v-container>
</template>


<script>
import Invoice from '@/components/dialogs/Invoice'
import LineChart from '@/components/charts/LineChart'
import SalesService from '@/services/Sales'
import ChartOptions from '@/components/charts/config'

export default {
  data () {
    return {
      search_1: '',
      search_2: '',
      search_3: '',
      search_4: '',
      menu: false,
      productDetail: false,
      invoiceHeader: [
        {text: 'Invoice Number', value: 'InvoiceNo', align: 'left'},
        {text: 'Invoice Date', value: 'InvoiceDate', align: 'left'},
        {text: 'Invoice Type', value: 'InvoiceType', align: 'left'}
      ],
      productsHeader: [
        {text: 'Category', value: 'ProductGroup', align: 'left'},
        {text: 'Description', value: 'ProductDescription', align: 'left'},
        {text: 'Number code', value: 'ProductNumberCode', align: 'left'}
      ],
      customersHeader: [
        {text: 'Id', value: 'CustomerID', align: 'left'},
        {text: 'Company name', value: 'CompanyName', align: 'left'}
      ],
      backlogHeader: [
        {text: 'Entity', value: 'Entidade', align: 'left'},
        {text: 'Data', value: 'Data', align: 'left'},
        {text: 'Total value', value: 'TotalMerc', align: 'left'}
      ],
      salesChartData: {
        datasets: []
      },
      chartOptions: ChartOptions.options,
      invoicesDataSet: [],
      customersDataSet: [],
      productsDataSet: [],
      backlogDataSet: [],
      dateBegin: null,
      dateEnd: null
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
    },
    dateEnd: async function (val) {
      const invoices = await SalesService.getInvoices(this.dateBegin, this.dateEnd)
      const backlog = await SalesService.getBacklog(this.dateBegin, this.dateEnd)

      this.backlogDataSet = backlog.data
      this.invoicesDataSet = invoices.data
    },
    invoicesDataSet: function (val) {
      let data = []

      for (var i = 0; i < val.length; i++) {
        val[i].NetTotal = val[i].DocumentTotals.NetTotal
        data.push({
          x: new Date(val[i].InvoiceDate),
          y: val[i].NetTotal
        })
      }

      data.sort((a, b) => {
        return a.x > b.x ? 1 : a.x < b.x ? -1 : 0
      })

      this.salesChartData = {
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
  },
  components: {
    LineChart,
    Invoice
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

