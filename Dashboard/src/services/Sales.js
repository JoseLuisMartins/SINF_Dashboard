import Api from '@/services/Api'

export default {
  getTotalNetSales (begin, end) {
    return Api().get(`/api/saft/TotalNetSales/?arg1=${begin}&arg2=${end}`)
  },
  getInvoices (begin, end) {
    return Api().get(`/api/saft/SalesInvoices/?arg1=${begin}&arg2=${end}`)
  },
  getStockMovements (begin, end) {
    return Api().get(`/api/saft/StockMovements/?arg1=${begin}&arg2=${end}`)
  },
  getAccountByID (accountId) {
    return Api().get(`/api/saft/Accounts/?vid=${accountId}`)
  },
  getCustomerByID (customerID) {
    return Api().get(`/api/saft/Customers/?vid=${customerID}`)
  },
  getProductByID (productID) {
    return Api().get(`/api/saft/Products/?vid=${productID}`)
  },
  getProductCustomers (productID) {
    return Api().get(`/api/saft/ProductCustomers/?vid=${productID}`)
  },
  getCustomersBoughtProducts (customerID) {
    return Api().get(`/api/saft/CustomersBoughtProducts/?vid=${customerID}`)
  },
  getProductSales (productID, begin, end) {
    return Api().get(`/api/saft/ProductSales/?vid=${productID}&begin=${begin}&end=${end}`)
  },
  getCustomerSpentValue (customerID, begin, end) {
    return Api().get(`/api/saft/CustomerSpentValue/?vid=${customerID}&begin=${begin}&end=${end}`)
  },
  getCustomers () {
    return Api().get(`/api/saft/Customers`)
  },
  getProducts () {
    return Api().get(`/api/saft/Products`)
  },
  getBacklog (begin, end) {
    return Api().get(`/api/DocVenda/?begin=${begin}&end=${end}`)
  },
  getTop10Products (begin, end) {
    return Api().get(`/api/saft/Top10Products/?arg1=${begin}&arg2=${end}`)
  },
  getTop10Customers (begin, end) {
    return Api().get(`/api/saft/Top10Customers/?arg1=${begin}&arg2=${end}`)
  },
  getBalanceSheet () {
    return Api().get(`/api/saft/BalanceSheet`)
  },
  getIncomeStatement () {
    return Api().get('/api/saft/IncomeStatement')
  },
  getFinancialRatios () {
    return Api().get('/api/saft/FinancialRatios')
  }
}
