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
      <v-flex d-flex sm12 md12>
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
              :search="search_1"
              v-bind:headers="headers"
              :items="items"
              hide-actions
              class="elevation-1"
              :loading="0 == 0"
            >
              <template slot="items" scope="props">
                <td>{{ props.item.CodArtigo }}</td>
                <td class="text-xs-right">{{ props.item.DescArtigo }}</td>
                <td class="text-xs-right">{{ props.item.STKAtual }}</td>
              </template>

            </v-data-table>
            <span>TOTAL = {{inventoryValue}} €</span>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

    <v-layout row wrap>
      <v-flex d-flex xs12 md12>
        <v-card>
          <v-card-title>
            <div class="headline"> Inventory Value </div>
          </v-card-title>
          <v-card-text >
            <div class="limitHeight chartHolder" v-if="1 != 0"> 
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
            <line-chart class="chartHolder"
             v-if="1 == 1"
             :chartData="inventoryChartData" :options="chartOptions"> 
            </line-chart>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

  </v-container>
</template>

<script>
import Products from '@/services/Products'
import ChartOptions from '@/components/charts/config'

export default {
  data () {
    return {
      search_1: '',
      inventoryValue: 200,
      pagination: {
        sortBy: 'Codigo'
      },
      selected: [],
      headers: [
        { text: 'Codigo', value: 'CodArtigo', align: 'left' },
        { text: 'Descrição', value: 'DescArtigo' },
        { text: 'STKAtual', value: 'STKAtual' }
      ],
      inventoryChartData: {
        datasets: []
      },
      items: [],
      chartOptions: ChartOptions.options,
      dateBegin: null,
      dateEnd: null
    }
  },
  methods: {
    toggleAll () {
      if (this.selected.length) this.selected = []
      else this.selected = this.items.slice()
    },
    changeSort (column) {
      if (this.pagination.sortBy === column) {
        this.pagination.descending = !this.pagination.descending
      } else {
        this.pagination.sortBy = column
        this.pagination.descending = false
      }
    }
  },
  mounted: function () {
    let currentYear = new Date().getFullYear()
    this.dateEnd = `${currentYear}-01-01`
    currentYear -= 1
    this.dateBegin = `${currentYear}-01-01`

    this.$nextTick(async () => {
      const res = await Products.all()
      this.items = res.data
    })
  }
}
</script>

<style scoped>
/* @import './assets/css/global.css'*/

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

.loading{
  width: 20px;
  height: 20px;
  min-width: 20px;
  min-height: 20px;
  max-height: 20px;
  max-width: 20px;
  border-radius: 10px;
  color: white;
  animation: loadinga 1s infinite 0.0s;
  animation-timing-function: infinite;
  animation-direction: alternate-reverse;
}

.loading.a {
  animation-delay: 0.0s;
}

.loading.b {
  animation-delay: 0.1s;
}

.loading.c {
  animation-delay: 0.2s;
}

.loading.d {
  animation-delay: 0.3s;
}

.loading.e {
  animation-delay: 0.4s;
}

.loading.f {
  animation-delay: 0.5s;
}

.loading.g {
  animation-delay: 0.5s;
}


@keyframes loadinga {
  100%{
    transform: translateY(0px)
  } 
  0%{
    transform: translateY(40px)
  }
}

</style>

