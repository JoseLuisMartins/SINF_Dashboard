import Api from '@/services/Api'

export default {
  request (begin, end) {
    return Api().get(`/api/DocCompra/?begin=${begin}&end=${end}`)
  },
  getSuppliersInfo (ids) {
    let url = 'api/Fornecedor/?'
    for (let i = 0; i < ids.length; i++) {
      url += `fIds=${ids[i]}`

      if (i !== ids.length - 1) {
        url += '&'
      }
    }

    return Api().get(url)
  }
}
