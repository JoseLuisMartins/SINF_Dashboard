<template>
        <v-card class="grey lighten-3">
          <v-card-title>
            <div class="title">
              {{Item.ProductDescription}} details
            </div>  
            <v-spacer></v-spacer>
            <v-menu bottom left>
              <v-btn icon slot="activator">
                <v-icon>more_vert</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile v-for="(item, i) in items" :key="i">
                  <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                </v-list-tile>
              </v-list>
            </v-menu>
          </v-card-title>
          <v-card-text>
            <v-layout row wrap>
              <v-flex xs12 >
                <v-card>
                  <v-card-text>
                    
                      <v-layout row justify-space-between elevation-3 pb-2 pt-2>
                        <v-flex class="title" d-flex xs4 offset-xs2>
                          Product Sales:                        
                        </v-flex>
                        <v-flex d-flex xs4 v-if="totalProductSales!==null">
                          {{(totalProductSales.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €"}}                        
                        </v-flex>
                      </v-layout>

                       <v-layout row wrap>
                        <v-flex d-flex xs4 elevation-2 pt-3>
                             <v-layout column>
                               <div>
                                 <v-layout row wrap>
                                  <p class="title"> Product Type: </p>
                                  <p class="pl-3">{{Item.ProductType}}</p>
                                 </v-layout>
                               </div>
                               <div>
                                 <v-layout row wrap>
                                  <p class="title"> Product Code: </p>
                                  <p class="pl-3">{{Item.ProductCode}}</p>
                                 </v-layout>
                               </div>
                               <div>
                                 <v-layout row wrap>
                                  <p class="title"> Product Group: </p>
                                  <p class="pl-3">{{Item.ProductGroup}}</p>
                                 </v-layout>
                               </div>
                              <div>
                                 <v-layout row wrap>
                                  <p class="title"> Product Description: </p>
                                  <p class="pl-3">{{Item.ProductDescription}}</p>
                                 </v-layout>
                               </div>
                              <div>
                                 <v-layout row wrap>
                                  <p class="title"> Product Number Code: </p>
                                  <p class="pl-3">{{Item.ProductNumberCode}}</p>
                                 </v-layout>
                               </div>
                                                 
                             
                                 
                               </div>
                             </v-layout>               
                        </v-flex>

                        <v-flex d-flex xs8  elevation-2 pt-1>
                             <v-card >
                                <v-card-title>
                                  <div class="headline"> Customers who bought </div>
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
                                    v-bind:headers="customersHeader"
                                    :items="customersDataSet"
                                    :search="search_1"
                                    class="elevation-2"                                  
                                    :loading="customersDataSet.length == 0"
                                    >

                                    <template slot="items" scope="props">
                                        <td> {{props.item.CustomerID }} </td>
                                        <td> {{props.item.CompanyName }} </td>
                                        <td> {{props.item.City }} </td>
                                    </template>

                                  </v-data-table>

                                </v-card-text>
                              </v-card>               
                        </v-flex>
                      </v-layout>

                  </v-card-text>
                </v-card>           
          
              </v-flex>
            </v-layout>
        </v-card-text>
        </v-card>
</template>


<script>
import SalesService from '@/services/Sales'

export default {
  data () {
    return {
      search_1: '',
      totalProductSales: 0,
      customersDataSet: [],
      customersHeader: [
        {text: 'Id', value: 'CustomerID', align: 'left'},
        {text: 'Company name', value: 'CompanyName', align: 'left'}
      ],
      items: [
        {
          title: 'Export to PDF'
        },
        {
          title: 'Print'
        }
      ]
    }
  },
  watch: {
    Item: async function () {
      this.getData()
    }
  },
  mounted: async function () {
    this.getData()
  },
  methods: {
    getData: async function () {
      this.totalProductSales = null
      this.customersDataSet = []
      this.totalProductSales = (await SalesService.getProductSales(this.Item.ProductCode, this.Begin, this.End)).data[0].total_sold
      this.customersDataSet = (await SalesService.getProductCustomers(this.Item.ProductCode)).data
    }
  },
  props: [
    'Item',
    'Begin',
    'End'
  ]
}
</script>

