import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import Products from '@/components/Products'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'root',
      component: Home
    },
    {
      path: '/inventory',
      name: 'inventory',
      component: Products
    }
  ]
})
