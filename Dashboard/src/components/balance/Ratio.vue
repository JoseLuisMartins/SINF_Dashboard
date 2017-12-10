<template>
  <v-card> 
    <v-card-title class="headline"> {{title}} </v-card-title>
    <v-card-text> 
      <v-layout row wrap>
       
      </v-layout>
    </v-card-text>
  </v-card>
</template>

<script>

export default {
  props: [
    'title',
    'Explanation',
    'value'
  ]
}
</script>

<style>


</style>
