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
                      <v-btn class="allSize" block round outline> Supplier </v-btn>
                      </v-flex>
                      <v-flex xs8>
                      <v-text-field
                        label="Search for a Supplier"
                      > </v-text-field>
                      </v-flex>
                    </v-layout>
                  </v-flex>
                  <v-flex d-flex>
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
                  <v-flex>
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
            <div class="headline"> Purchases </div>
          </v-card-title>
          <v-card-text>
            <line-chart class="limitHeight chartHolder" :chart-data="salesChartData"> </line-chart>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex sm12 md6>
        <v-card>
          <v-card-title class="pb-0">
            <div class="headline"> Purchased Products </div>
            <v-spacer></v-spacer>
            <v-text-field 
              label="Search"
              > </v-text-field> <v-icon> search </v-icon>
          </v-card-title>
          <v-card-text>
            <v-data-table
              v-bind:headers="purchasesHeader"
              :items="currentDataSet"
              hide-actions
              class="elevation-1"
              item-key="id"
              >

              <template slot="items" scope="props">
                <tr @click="props.expanded = !props.expanded"> 
                  <td> {{props.item.Entidade}} </td>
                  <td> {{props.item.Data }} </td>
                  <td> {{props.item.TotalMerc }} </td>
                  <td> {{props.item.Serie }} </td>
                </tr>
              </template>
              <template slot="expand" scope="props">
                <v-card color="grey lighten-3">
                  <v-card-text>
                    <v-table>
                      <tr>
                      <th> Art </th> <th>Descr</th><th>Quantity</th><th> UnitPrice </th><th> - LiquidPrice</th><th>  - Warehouse</th><th>  - Lot</th>
                      </tr>
                      <tr flat v-for="line in props.item.LinhasDoc" :key="line.id">
                        <td>{{line.CodArtigo}}</td>
                        <td>{{line.DescArtigo}}</td>
                        <td>{{line.Quantidade}}</td>
                        <td>{{line.PrecoUnitario}}</td>
                        <td>{{line.TotalLiquido}}</td>
                        <td>{{line.Armazem}}</td>
                        <td>{{line.Lote}}</td>
                      </tr>
                    </v-table>
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
              label="Search"
              > </v-text-field> <v-icon> search </v-icon>
          </v-card-title>
          <v-card-text>

            <v-data-table
              v-bind:headers="linhasDoc"
              :items="items"
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

import LineChart from '@/components/charts/ScatterChart'
import PurchasesService from '@/services/Purchases'

export default {
  components: {
    LineChart
  },
  data () {
    return {
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
        {text: 'Date', value: 'Data', align: 'center'},
        {text: 'Total Merc', value: 'TotalMerc', align: 'center'},
        {text: 'Serie', value: 'Serie', align: 'center'}
      ],
      menu: false,
      dateBegin: null,
      dateEnd: null,
      currentDataSet: [],
      items: [
        {name: 'Pistacho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Amarelo', company: 'Felisberto Inc.'},
        {name: 'Pistacho Vermelho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Azul', company: 'Felisberto Inc.'},
        {name: 'Pistacho Laranja', company: 'Felisberto Inc.'},
        {name: 'Pistacho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Amarelo', company: 'Felisberto Inc.'},
        {name: 'Pistacho Vermelho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Azul', company: 'Felisberto Inc.'},
        {name: 'Pistacho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Amarelo', company: 'Felisberto Inc.'},
        {name: 'Pistacho Vermelho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Azul', company: 'Felisberto Inc.'},
        {name: 'Pistacho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Amarelo', company: 'Felisberto Inc.'},
        {name: 'Pistacho Vermelho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Azul', company: 'Felisberto Inc.'},
        {name: 'Pistacho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Amarelo', company: 'Felisberto Inc.'},
        {name: 'Pistacho Vermelho', company: 'Felisberto Inc.'},
        {name: 'Pistacho Azul', company: 'Felisberto Inc.'}
      ],
      salesChartData: {
        datasets: []
      }
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
      let dict = {}

      for (var i = 0; i < val.length; i++) {
        let x = new Date(val[i].Data).getMonth()
        dict[x] = val[i].TotalMerc + (dict[x] || 0)
      }

      let data = []

      for (var key in dict) {
        data.push({
          x: Number(key) + 1,
          y: dict[key]
        })
      }

      console.log(data)

      this.salesChartData = {
        data: data,
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

.relative{
  position: relative;
}

.chartHolder{
  position: relative;
  height: 300px;
  width: 100%;
  min-width: 0;
  min-height: 0;
}

</style>

