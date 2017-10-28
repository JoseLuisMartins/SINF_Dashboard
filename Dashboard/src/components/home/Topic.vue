<template>
  <a class="pointer" @click="loadPage">
  <v-card class="hoverAnim" :color="color" dark @click="loadPage">
    <v-card-title primary-title class="pb-0">
      <div class="headline" ma0>  {{title}} <v-icon class="hiddenIcon" :color="color2" x-large> {{icon}} </v-icon> </div>
    </v-card-title>
    <v-card-text pa0>
      <v-layout row wrap justify-center pa1>
        <v-flex d-flex > 
          <span class="text-xs-left"> {{description}} </span>
        </v-flex>
        <v-flex d-flex> 
          <span class="text-xs-right"> {{value}} </span>
        </v-flex>
      </v-layout>
    </v-card-text>
  </v-card>
  </a>
</template>

<script>
export default {
  data () {
    return {
    }
  },
  props: [
    'title',
    'value',
    'icon',
    'description',
    'color',
    'color2',
    'dest'
  ],
  methods: {
    loadPage: function () {
      this.$router.push(this.dest)
    }
  }
}
</script>

<style scoped>
.hiddenIcon{
  position: absolute;
  top: 10px;
  right: 10px;
}
.pointer{
  cursor:pointer;
}

.hoverAnim:hover{
  transition: background-color 0.5s ease 0.1s;
  background-color: #333 !important;
}

</style>


