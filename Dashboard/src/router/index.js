import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import Products from '@/components/Products'
import Sales from '@/components/Sales'
import Purchases from '@/components/Purchases'
import Invoice from '@/components/dialogs/Invoice'
import Balance from '@/components/Balance'

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
    },
    {
      path: '/purchases',
      name: 'purchases',
      component: Purchases
    },
    {
      path: '/invoice',
      name: 'invoice',
      component: Invoice
    },
    {
      path: '/accounting',
      name: 'accounting',
      component: Balance
    }
  ]
})
