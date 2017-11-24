import Api from '@/services/Api'

export default{
  all () {
    return Api().get('api/artigos')
  },
  getFornecedor (idF) {
    let url = `api/artigos/?fornecedor=${idF}`
    return Api().get(url)
  },
  getInventory (begin, end) {
    let urlInventory = `api/Inventory/?begin=${begin}&end=${end}`
    return Api().get(urlInventory)
  },
  getProductsBySupplier (id) {
    return Api().get(`api/Artigos/?fornecedor=${id}`)
  }

}
