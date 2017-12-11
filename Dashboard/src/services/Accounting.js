import Api from '@/services/Api'

export default {
  getBalanceSheet (begin, end) {
    return Api().get(`/api/saft/BalanceSheet?arg1=${begin}&arg2=${end}`)
  },
  getIncomeStatement (begin, end) {
    return Api().get(`/api/saft/IncomeStatement?arg1=${begin}&arg2=${end}`)
  },
  getFinancialRatios (begin, end) {
    return Api().get(`/api/saft/FinancialRatios?arg1=${begin}&arg2=${end}`)
  },
  getNetIncome (begin, end) {
    return Api().get(`/api/saft/NetIncome?arg1=${begin}&arg2=${end}`)
  }
}
