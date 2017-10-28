import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import Products from '@/components/Products'
import Sales from '@/components/Sales'

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
    },
    {
      path: '/sales',
      name: 'sales',
      component: Sales
    }
  ]
})
