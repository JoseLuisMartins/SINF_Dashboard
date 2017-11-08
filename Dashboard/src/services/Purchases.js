import Api from '@/services/Api'

export default {
  request (begin, end) {
    return Api().get(`/api/DocCompra/?begin=${begin}&end=${end}`)
  }
}
