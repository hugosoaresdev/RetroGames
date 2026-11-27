import api from './api'

export const jogoApi = {
  async listar(parametros = {}) {
    const resposta = await api.get('/Jogo', {
      params: parametros,
    })
    return resposta.data.data
  },

  async buscarPorId(id) {
    const resposta = await api.get(`/Jogo/${id}`)
    return resposta.data.data
  },

  async inserir(jogo) {
    const resposta = await api.post('/Jogo', jogo)
    return resposta.data.data
  },

  async alterar(id, jogo) {
    await api.put(`/Jogo/${id}`, jogo)
  },

  async desativar(id) {
    await api.delete(`/Jogo/${id}`)
  },
}

export const plataformaApi = {
  async listar() {
    const resposta = await api.get('/Plataforma')
    return resposta.data.data
  },

  async inserir(plataforma) {
    const resposta = await api.post('/Plataforma', plataforma)
    return resposta.data.data
  },

  async alterar(id, plataforma) {
    await api.put(`/Plataforma/${id}`, plataforma)
  },

  async excluir(id) {
    await api.delete(`/Plataforma/${id}`)
  },
}

export const generoApi = {
  async listar() {
    const resposta = await api.get('/Genero')
    return resposta.data.data
  },
}

export const condicaoJogoApi = {
  async listar() {
    const resposta = await api.get('/CondicaoJogo')
    return resposta.data.data
  },
}

export const autenticacaoApi = {
  async entrar(email, senha) {
    const resposta = await api.post('/Autenticacao/login', {
      email,
      senha,
    })
    return resposta.data.data
  },

  async sair() {
    await api.post('/Autenticacao/logout')
  },

  async usuarioAtual() {
    const resposta = await api.get('/Autenticacao/eu')
    return resposta.data.data
  },
}