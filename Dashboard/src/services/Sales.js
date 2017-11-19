import Api from '@/services/Api'

export default {
  getTotalNetSales (begin, end) {
    return Api().get(`/api/saft/TotalNetSales?begin=${begin}&end=${end}`)
  },
  getInvoices (begin, end) {
    return Api().get(`/api/sales/?begin=${begin}&end=${end}`)
  },
  getCustomers () {
    return Api().get(`/api/saft/Customers`)
  },
  getProducts () {
    return Api().get(`/api/saft/Products`)
  },
  getBacklog (begin, end) {
    return Api().get(`/api/DocVenda/?begin=${begin}&end=${end}`)
  }
}
