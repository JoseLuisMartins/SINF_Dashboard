<template>
  <v-toolbar
    fixed
    color="blue darken-4"
    dark
  >
    <v-toolbar-side-icon></v-toolbar-side-icon>
    <v-icon class="pointer" @click="loadPage" dark large> track_changes </v-icon>
    <v-toolbar-title class="pointer" @click="loadPage"> {{title}}</v-toolbar-title>
    <v-spacer></v-spacer>
  </v-toolbar>  
</template>

<script>
export default {
  data () {
    return {
    }
  },
  props: [ 'title' ],
  methods: {
    loadPage: function () {
      this.$router.push({name: 'root'})
    }
  }
}
</script>

<style scoped>
.pointer{
  cursor: pointer;
  color: white;
}
</style>


