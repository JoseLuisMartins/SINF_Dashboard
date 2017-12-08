import Api from '@/services/Api'

export default{
  all () {
    return Api().get('api/artigos')
  },
  getFornecedor (idF) {
    let url = `api/artigos/?fornecedor=${idF}`
    return Api().get(url)
  },
  getMovements (begin, end, inout) {
    return Api().get(`api/Inventory/?begin=${begin}&end=${end}&inout=${inout}`)
  },
  getInventory (date) {
    return Api().get(`api/Inventory/?date=${date}&k=2`)
  },
  getProductsBySupplier (id) {
    return Api().get(`api/Artigos/?fornecedor=${id}`)
  },
  getMovementsGraph (begin, end) {
    return Api().get(`api/Inventory/?begin=${begin}&end=${end}`)
  },
  getTotalValueInventory (date) {
    return Api().get(`api/Inventory/?date=${date}&k=1`)
  },
  getTotalInventoryByFamilies (date) {
    return Api().get(`api/Inventory/?date=${date}&k=3`)
  }

}
