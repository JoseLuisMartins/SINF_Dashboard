<template>
  <v-card> 
    <v-card-title class="headline"> {{title}} </v-card-title>
    <v-card-text> 
      <v-layout row wrap>
        <template  v-for="(data, i) in data.fields">
          <v-flex class="text-xs-left mb-3" xs12 :key="i"> 
            <expansion-block>
              <b slot="header" class="title text-xs-left"> {{data.name}}       </b> 
              <b slot="header" class="title text-xs-right"> {{(data.total.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €"}} </b> 

              <v-layout slot="content" row wrap class="ml-5">
                <template  v-for="(value, k) in data.values">
                  <v-flex xs6 class="text-xs-left" :class="[{'grey lighten-4': k%2==0},{'grey lighten-2':k%2==1}]" :key="k">
                    <b> {{value.name}}</b>
                  </v-flex>
                  <v-flex xs6 class="text-xs-center" :class="[{'grey lighten-4': k%2==0},{'grey lighten-2':k%2==1}]" :key="k">
                    {{(value.value.toFixed(2) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1 ") + " €" }}
                  </v-flex>
                </template>
              </v-layout>
            </expansion-block>
          </v-flex>
        </template>
      </v-layout>
    </v-card-text>
  </v-card>
</template>

<script>
import ExpansionBlock from '@/components/balance/ExpansionBlock'
export default {
  components: {
    ExpansionBlock
  },
  props: ['title', 'data']

}
</script>

<style>


</style>
