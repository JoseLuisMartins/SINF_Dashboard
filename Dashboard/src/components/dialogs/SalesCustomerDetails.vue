<template>
        <v-card class="grey lighten-2">
          <v-card-title>
            <div class="title">
              {{Item.CompanyName}} details
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
                          Total money spent by the customer:                        
                        </v-flex>
                        <v-flex d-flex xs4>
                          {{(totalSpentValue.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €"}}                        
                        </v-flex>
                      </v-layout>

                       <v-layout row wrap>
                        <v-flex d-flex xs4 elevation-2 pt-3>
                             <v-layout column>
                               <div>
                                 <v-layout row wrap>
                                  <p class="title"> Customer Id: </p>
                                  <p class="pl-3">{{Item.CustomerID}}</p>
                                 </v-layout>
                               </div>
                               <div>
                                 <v-layout row wrap>
                                  <p class="title"> Account Id: </p>
                                  <p class="pl-3">{{Item.AccountID}}</p>
                                 </v-layout>
                               </div>
                               <div>
                                 <v-layout row wrap>
                                  <p class="title"> Company Name: </p>
                                  <p class="pl-3">{{Item.CompanyName}}</p>
                                 </v-layout>
                               </div>
                                                 
                               <div>
                                 <p class="title text-xs-left" > Billing Adrress </p>
                                 <div class="ml-4">                                   
                                    <div>
                                      <v-layout row wrap>
                                        <p class="title"> Address Detail: </p>
                                        <p class="pl-3">{{Item.BillingAddress.AddressDetail}}</p>
                                      </v-layout>
                                    </div> 
                                    <div>
                                      <v-layout row wrap>
                                        <p class="title text-xs-left"> City: </p>
                                        <p class="pl-3">{{Item.BillingAddress.City}}</p>
                                      </v-layout>
                                    </div>  
                                    <div>
                                      <v-layout row wrap>
                                        <p class="title"> Postal Code: </p>
                                        <p class="pl-3">{{Item.BillingAddress.PostalCode}}</p>
                                      </v-layout>
                                    </div>    
                                    <div>
                                      <v-layout row wrap>
                                        <p class="title"> Country: </p>
                                        <p class="pl-3">{{Item.BillingAddress.Country}}</p>
                                      </v-layout>
                                    </div>                                                          
                                 </div>
                                 
                               </div>
                             </v-layout>               
                        </v-flex>

                        <v-flex d-flex xs8  elevation-2 pt-1>
                             <v-card >
                                <v-card-title>
                                  <div class="headline"> Bought Products </div>
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
                                    class="elevation-2"                                  
                                    :loading="productsDataSet.length == 0"
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
      totalSpentValue: 0,
      productsDataSet: [],
      productsHeader: [
        {text: 'Category', value: 'ProductGroup', align: 'left'},
        {text: 'Description', value: 'ProductDescription', align: 'left'},
        {text: 'Number code', value: 'ProductNumberCode', align: 'left'}
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
      this.totalSpentValue = null
      this.productsDataSet = []
      this.totalSpentValue = (await SalesService.getCustomerSpentValue(this.Item.CustomerID, this.Begin, this.End)).data[0].total_spent
      this.productsDataSet = (await SalesService.getCustomersBoughtProducts(this.Item.CustomerID)).data
    }
  },
  props: [
    'Item',
    'Begin',
    'End'
  ]
}
</script>

<style scoped>

.title {
  font-weight: bold;
}

</style>