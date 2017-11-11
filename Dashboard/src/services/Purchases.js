import Api from '@/services/Api'

export default {
  request (begin, end) {
    return Api().get(`/api/DocCompra/?begin=${begin}&end=${end}`)
  },
  getSupplierInfo (id) {
    let url = `api/Fornecedor/?id=${id}`

    return Api().get(url)
  },
  getSuppliers () {
    return Api().get('api/Fornecedor/')
  }
}
