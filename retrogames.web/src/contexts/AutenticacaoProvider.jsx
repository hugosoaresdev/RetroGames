import { useEffect, useState } from 'react'
import { autenticacaoApi } from '../api/servicos'
import AutenticacaoContext from './autenticacaoContext'

function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    let cancelado = false

    async function carregarUsuario() {
      try {
        const usuarioAtual = await autenticacaoApi.usuarioAtual()

        if (!cancelado) {
          setUsuario(usuarioAtual)
        }
      } catch (erro) {
        if (!cancelado) {
          setUsuario(null)

          if (erro.response?.status !== 401) {
            console.error('Não foi possível verificar a sessão.', erro)
          }
        }
      } finally {
        if (!cancelado) {
          setCarregando(false)
        }
      }
    }

    function aoExpirarSessao() {
      setUsuario(null)
    }

    window.addEventListener('sessao:expirada', aoExpirarSessao)
    carregarUsuario()

    return () => {
      cancelado = true
      window.removeEventListener('sessao:expirada', aoExpirarSessao)
    }
  }, [])

  async function entrar(email, senha) {
    const usuarioAutenticado = await autenticacaoApi.entrar(email, senha)
    setUsuario(usuarioAutenticado)
    return usuarioAutenticado
  }

  async function sair() {
    await autenticacaoApi.sair()
    setUsuario(null)
  }

  const valor = {
    usuario,
    carregando,
    estaAutenticado: usuario !== null,
    ehAdministrador: usuario?.perfil === 'Administrador',
    entrar,
    sair,
  }

  return (
    <AutenticacaoContext.Provider value={valor}>
      {children}
    </AutenticacaoContext.Provider>
  )
}

export default AutenticacaoProvider