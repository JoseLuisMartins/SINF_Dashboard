<template>
  <v-container mt-5 grid-list-xs>
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
            <v-spacer></v-spacer>
            <v-btn class="ma-0" outline> Day </v-btn>
            <v-btn class="ma-0" outline> Month </v-btn>
            <v-btn color="blue" class="ma-0" dark> Year </v-btn>
            <v-btn color="blue" class="ma-0" dark> Custom </v-btn>
          </v-card-title>
          <v-card-text class="chartHolder">
            <line-chart class="limitHeight" :data="salesChartData"> </line-chart>
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
              v-bind:headers="headers"
              :items="items"
              hide-actions
              class="elevation-1"
              >

              <template slot="items" scope="props">
                
                <td> {{props.item.name }} </td>
                <td> {{props.item.company }} </td>

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
              v-bind:headers="headers"
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

import LineChart from '@/components/charts/LineChart'

export default {
  data () {
    return {
      headers: [
        {text: 'Name', value: 'name', align: 'left'}
      ],
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
        title: 'Turnover',
        labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        datasets: [
          {
            pointRadius: 5,
            pointHoverRadius: 10,
            pointBackgroundColor: '#FF5522',
            label: 'tmp',
            data: [20, 30, 20, 23, 21, 12, 23, 23, 32, 52, 50, 25]
          },
          {
            pointRadius: 5,
            pointHoverRadius: 10,
            pointBackgroundColor: '#2255FF',
            borderColor: '#1144AA',
            label: 'tmp2',
            data: [30, 20, 23, 21, 12, 23, 23, 32, 52, 50, 25, 12]
          }
        ]
      }
    }
  },
  components: {
    LineChart
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
  width: 100%;
}

.limitHeight{
  max-height: 200px;
}

</style>

