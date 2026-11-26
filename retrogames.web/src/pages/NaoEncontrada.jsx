import { Link } from 'react-router-dom'

function NaoEncontrada() {
  return (
    <div className="text-center">
      <h1>Página não encontrada</h1>
      <p>O endereço acessado não existe.</p>
      <Link className="btn btn-primary" to="/">
        Voltar ao catálogo
      </Link>
    </div>
  )
}

export default NaoEncontrada