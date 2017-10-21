<template>
  <v-toolbar
    fixed
    color="blue darken-4"
    dark
  >
    <v-toolbar-side-icon></v-toolbar-side-icon>
    <v-toolbar-title>{{title}}</v-toolbar-title>
    <v-spacer></v-spacer>
    <v-btn icon>
      <v-icon>search</v-icon>
    </v-btn>
    <v-btn icon>
      <v-icon>favorite</v-icon>
    </v-btn>
    <v-btn icon>
      <v-icon>more_vert</v-icon>
    </v-btn>
  </v-toolbar>  
</template>

<script>
export default {
  data () {
    return {
    }
  },
  props: [ 'title' ]
}
</script>

<style scoped>

</style>


