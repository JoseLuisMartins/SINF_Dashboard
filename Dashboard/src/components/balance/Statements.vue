<template>
  <v-card> 
    <v-card-title class="headline"> {{title}} </v-card-title>
    <v-card-text> 
      <v-layout row wrap>
        <template  v-for="(data, i) in data">
          <v-flex class="text-xs-left mb-3" xs12 :key="i"> 
            <b class="biggerText"> {{data.name}} </b> 
            <v-layout row wrap class="ml-5">
              <template  v-for="(value, k) in data.values">
              <v-flex xs6 class="text-xs-left" :class="[{'grey lighten-4': k%2==0},{'grey lighten-2':k%2==1}]" :key="k">
                <b> {{value.name}}</b>
              </v-flex>
              <v-flex xs6 class="text-xs-center" :class="[{'grey lighten-4': k%2==0},{'grey lighten-2':k%2==1}]" :key="k">
                {{value.value}}
              </v-flex>
              </template>
            </v-layout>
          </v-flex>
        </template>
      </v-layout>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  props: ['title', 'data']

}
</script>

<style>
.biggerText{
  font-size: 18px
}

</style>
