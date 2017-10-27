<template>
  <v-container mt-4 grid-list-md>
    <v-layout row wrap mb-4>
      <v-flex d-flex offset-md4 md4 xs12>
        <v-card>
          <v-menu
            white
            lazy
            :close-on-content-click="false"
            v-model="menu"
            transition="scale-transition"
            :nudge-top="-20"
            max-width="290px"
            min-width="290px"
          >
            <v-text-field
              slot="activator"
              label="Date"
              v-model="date"
              prepend-icon="event"
              readonly
            ></v-text-field>
            <v-date-picker type="month" v-model="date" no-title scrollable actions :allowed-dates="december">
              <template scope="{ save, cancel }">
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn flat color="primary" @click="cancel">Cancel</v-btn>
                  <v-btn flat color="primary" @click="save">OK</v-btn>
                </v-card-actions>
              </template>
            </v-date-picker>
          </v-menu>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex d-inline xs12 sm6 md3 v-for="topic in topics" :key="topic.title">
        <topic :title="topic.title" :icon="topic.icon" :value="topic.value" :description="topic.description" :color="topic.color" :color2="topic.color2" :dest="topic.dest"/> 
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import Topic from '@/components/home/Topic'

export default {
  name: 'HelloWorld',
  data () {
    return {
      date: null,
      menu: false,
      modal: false,
      december: function (date) {
        return date.getMonth() === 11
      },
      topics: [
         {color: 'teal darken-1', color2: 'teal lighten-3', icon: 'attach_money', title: 'Sales', value: '130€', description: 'Total Sales', dest: 'sales'},
         {color: 'deep-orange darken-1', color2: 'deep-orange lighten-3', icon: 'shopping_cart', title: 'Purchases', value: '130€', description: 'Total Purchases', dest: 'purchases'},
         {color: 'light-blue darken-1', color2: 'light-blue lighten-3', icon: 'view_quilt', title: 'Inventory', value: '130€', description: 'Value in Inventory', dest: 'inventory'},
         {color: 'purple darken-1', color2: 'purple lighten-3', icon: 'account_balance_wallet', title: 'Accounting', value: '130€', description: 'Cashflow', dest: 'accounting'}
      ]
    }
  },
  components: {
    Topic
  },
  mounted: function () {
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h1, h2 {
  font-weight: normal;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}

a {
  color: #42b983;
}
</style>
