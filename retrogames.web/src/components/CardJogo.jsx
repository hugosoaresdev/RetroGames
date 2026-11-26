import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  const esgotado = jogo.quantidadeEstoque === 0

  return (
    <article className="card h-100">
      <div className="card-body d-flex flex-column">
        <p className="card-subtitle mb-2 text-body-secondary">
          {jogo.nomePlataforma}
        </p>

        <h2 className="card-title h5">{jogo.nome}</h2>

        <p className="card-text">
          {jogo.anoLancamento} · {jogo.descricaoCondicaoJogo}
        </p>

        <p className="card-text">Gêneros: {jogo.generos.join(', ')}</p>

        <p className="fw-bold">{formatarPreco(jogo.preco)}</p>

        {esgotado && <p className="text-danger">Esgotado</p>}

        <Link
          className="btn btn-outline-primary mt-auto"
          to={`/jogos/${jogo.id}`}
        >
          Ver detalhes
        </Link>
      </div>
    </article>
  )
}

export default CardJogo