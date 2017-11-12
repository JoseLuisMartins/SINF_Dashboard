import Api from '@/services/Api'

export default {
  getInvoices (begin, end) {
    return Api().get(`/api/sales/?begin=${begin}&end=${end}`)
  }
}
