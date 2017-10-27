<template>
  <div id="app">
    <v-app>
      <main>
        <toolbar title="360º Dashboard"/>
        <router-view/>
      </main>
    </v-app>
  </div>
</template>

<script>
import Toolbar from '@/components/Toolbar'

export default {
  name: 'app',
  components: {
    Toolbar
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
