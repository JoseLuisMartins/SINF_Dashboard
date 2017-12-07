<template>
  <v-layout justify-center>
  <v-flex :class="color" class="loading a">L</v-flex>
  <v-flex :class="color" class="loading b">O</v-flex>
  <v-flex :class="color" class="loading c">A</v-flex> 
  <v-flex :class="color" class="loading d">D</v-flex> 
  <v-flex :class="color" class="loading e">I</v-flex> 
  <v-flex :class="color" class="loading f">N</v-flex> 
  <v-flex :class="color" class="loading g">G</v-flex>
</v-layout>
</template>

<script>
export default {
  props: ['color']
}
</script>

