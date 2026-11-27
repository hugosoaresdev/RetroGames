import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import useAutenticacao from '../../hooks/useAutenticacao'

function Login() {
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [carregando, setCarregando] = useState(false)
  const [erro, setErro] = useState('')

  const [parametrosBusca] = useSearchParams()
  const navegar = useNavigate()
  const { entrar } = useAutenticacao()

  async function aoEnviar(evento) {
    evento.preventDefault()
    setErro('')
    setCarregando(true)

    try {
      await entrar(email, senha)

      const voltarPara = parametrosBusca.get('voltarPara')
      const destino =
        voltarPara?.startsWith('/') && !voltarPara.startsWith('//')
          ? voltarPara
          : '/'

      navegar(destino, { replace: true })
    } catch (erroLogin) {
      setErro(obterMensagemErro(erroLogin))
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <section className="col-12 col-md-6 col-lg-4">
        <h1>Entrar</h1>

        <form onSubmit={aoEnviar}>
          <div className="mb-3">
            <label className="form-label" htmlFor="email">
              E-mail
            </label>
            <input
              className="form-control"
              id="email"
              type="email"
              autoComplete="username"
              value={email}
              onChange={(evento) => setEmail(evento.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label className="form-label" htmlFor="senha">
              Senha
            </label>
            <input
              className="form-control"
              id="senha"
              type="password"
              autoComplete="current-password"
              value={senha}
              onChange={(evento) => setSenha(evento.target.value)}
              required
            />
          </div>

          {erro && (
            <div className="alert alert-danger" role="alert">
              {erro}
            </div>
          )}

          <button
            className="btn btn-primary"
            type="submit"
            disabled={carregando}
          >
            {carregando ? 'Entrando...' : 'Entrar'}
          </button>
        </form>
      </section>
    </div>
  )
}

export default Login