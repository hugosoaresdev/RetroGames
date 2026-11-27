import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const url = erro.config?.url ?? ''
    const ehRotaEsperada =
      url.endsWith('/Autenticacao/eu') ||
      url.endsWith('/Autenticacao/login')

    if (erro.response?.status === 401 && !ehRotaEsperada) {
      window.dispatchEvent(new Event('sessao:expirada'))
    }

    return Promise.reject(erro)
  },
)

export default api