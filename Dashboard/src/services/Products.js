import Api from '@/services/Api'

export default{
  all () {
    return Api().get('api/artigos')
  },
  getFornecedor (idF) {
    let url = `api/artigos/?fornecedor=${idF}`
    return Api().get(url)
  }
}
