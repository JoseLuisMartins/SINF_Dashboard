<template>
  <v-container grid-list-md text-xs-center>
    <v-layout row justify-center>
      <v-flex xs12>
      <v-btn dark @click.stop="dialogStatus=true">See Invoice Details</v-btn>
      <v-dialog v-model="dialogStatus" max-width="1000" >
        <v-card class="grey lighten-2">
          <v-card-title>
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
            
                <v-card >
                
                  <v-card-text>
                    <v-layout row justify-space-between>
                      <v-flex d-flex xs4 >
                        <v-card class="elevation-0">
                          <v-card-text class="pa-1">{{ShipFromAddressDetail}}</v-card-text>
                          <v-card-text class="pa-1">{{ShipFromCity}}</v-card-text>
                          <v-card-text class="pa-1">{{ShipFromPostalCode}}</v-card-text>
                        </v-card>
                      </v-flex>            

                      <v-flex d-flex xs4 >
                        <v-card >
                          <v-card-text class="pa-1">{{InvoiceDate}} </v-card-text>
                          <v-card-text class="pa-1">{{InvoiceNo}} </v-card-text>
                          <v-card-text class="pa-1">{{CustomerID}} </v-card-text>
                        </v-card>
                      </v-flex>
                    </v-layout>

                    <v-layout row justify-space-between>
                      <v-flex d-flex xs4 >
                        <v-card class="elevation-0 mb-4 ">
                          <v-card-title>
                            Bill to
                          </v-card-title>
                          <v-card-text class="pa-1">{{ShipToAddressDetail}}</v-card-text>
                          <v-card-text class="pa-1">{{ShipToCity}}</v-card-text>
                          <v-card-text class="pa-1">{{ShipToPostalCode}}</v-card-text>
                        </v-card>
                      </v-flex>
                    </v-layout>


                    <v-layout row wrap>
                      <v-flex d-flex xs12 >
                        <v-data-table
                          v-bind:headers="invoiceLineHeader"
                          :items="Lines"
                          hide-actions
                          class="elevation-3"
                        >
                        <template slot="items" slot-scope="props">
                          <td>{{ props.item.ProductDescription }}</td>
                          <td>{{ props.item.Quantity }}</td>
                          <td>{{ props.item.UnitPrice }}</td>
                         
                        </template>
                      </v-data-table>
                      </v-flex>
                    </v-layout>

                     <v-layout column wrap>
                      <v-flex d-flex offset-xs8 >
                        <v-card class="elevation-1 mt-2 ">                         
                          <v-card-text class="pa-1"> Net Total: {{NetTotal}}</v-card-text>
                          <v-card-text class="pa-1" > Gross Total: {{GrossTotal}}</v-card-text>                      
                        </v-card>
                      </v-flex>
                    </v-layout>



                  </v-card-text>
                </v-card>
          
              </v-flex>
            </v-layout>
        </v-card-text>

        <v-layout row wrap>
          <v-flex d-flex offset-xs10 >
            <v-card-actions>
              <v-btn color="primary" flat @click.stop="dialogStatus=false">Close</v-btn>
            </v-card-actions>
          </v-flex>  
        </v-layout>
          

        </v-card>
      </v-dialog>
      </v-flex>
    </v-layout>
  </v-container>
</template>


<script>

export default {
  data () {
    return {
      dialogStatus: false,
      invoiceLineHeader: [
        {text: 'Description', value: 'ProductDescription', align: 'left'},
        {text: 'Quantity', value: 'Quantity'},
        {text: 'Price', value: 'UnitPrice'}
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
  mounted: function () {
    console.log(this.ShipToAddressDetail)
  },
  props: [
    'ShipFromAddressDetail',
    'ShipFromCity',
    'ShipFromPostalCode',
    'ShipToAddressDetail',
    'ShipToCity',
    'ShipToPostalCode',
    'InvoiceDate',
    'InvoiceNo',
    'CustomerID',
    'Lines',
    'NetTotal',
    'GrossTotal'
  ]
}
</script>

<style scoped>


</style>

