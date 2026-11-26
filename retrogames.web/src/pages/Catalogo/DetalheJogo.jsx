import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { jogoApi } from '../../api/servicos'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'
import { formatarPreco } from '../../lib/formato'

function DetalheJogo() {
  const { id } = useParams()
  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let cancelado = false

    setCarregando(true)
    setErro('')

    jogoApi
      .buscarPorId(id)
      .then((resultado) => {
        if (!cancelado) {
          setJogo(resultado)
        }
      })
      .catch((erroRequisicao) => {
        if (!cancelado) {
          setErro(obterMensagemErro(erroRequisicao))
        }
      })
      .finally(() => {
        if (!cancelado) {
          setCarregando(false)
        }
      })

    return () => {
      cancelado = true
    }
  }, [id, tentativa])

  if (carregando) {
    return <Carregando texto="Carregando jogo..." />
  }

  if (erro) {
    return (
      <>
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={() =>
            setTentativa((valorAtual) => valorAtual + 1)
          }
        />
        <Link to="/">Voltar ao catálogo</Link>
      </>
    )
  }

  return (
    <article>
      <Link className="btn btn-link px-0" to="/">
        Voltar ao catálogo
      </Link>

      <h1>{jogo.nome}</h1>

      <dl className="row">
        <dt className="col-sm-3">Plataforma</dt>
        <dd className="col-sm-9">{jogo.nomePlataforma}</dd>

        <dt className="col-sm-3">Condição</dt>
        <dd className="col-sm-9">{jogo.descricaoCondicaoJogo}</dd>

        <dt className="col-sm-3">Ano de lançamento</dt>
        <dd className="col-sm-9">{jogo.anoLancamento}</dd>

        <dt className="col-sm-3">Gêneros</dt>
        <dd className="col-sm-9">
          {jogo.generos.map((genero) => genero.nome).join(', ')}
        </dd>

        <dt className="col-sm-3">Descrição</dt>
        <dd className="col-sm-9">
          {jogo.descricao || 'Sem descrição disponível.'}
        </dd>

        <dt className="col-sm-3">Preço</dt>
        <dd className="col-sm-9">{formatarPreco(jogo.preco)}</dd>

        <dt className="col-sm-3">Estoque</dt>
        <dd className="col-sm-9">
          {jogo.quantidadeEstoque === 0
            ? 'Esgotado'
            : `${jogo.quantidadeEstoque} em estoque`}
        </dd>
      </dl>
    </article>
  )
}

export default DetalheJogo