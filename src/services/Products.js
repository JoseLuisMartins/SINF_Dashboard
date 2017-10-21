import Api from '@/services/Api'

export default{
  all () {
    return Api().get('api/artigos')
  }

}
