import Api from '@/services/Api'

export default {
  getBalanceSheet () {
    return Api().get(`/api/saft/BalanceSheet`)
  },
  getIncomeStatement () {
    return Api().get('/api/saft/IncomeStatement')
  },
  getFinancialRatios () {
    return Api().get('/api/saft/FinancialRatios')
  },
  getNetIncome () {
    return Api().get('/api/saft/NetIncome')
  }
}
