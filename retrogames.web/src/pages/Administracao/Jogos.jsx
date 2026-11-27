import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { jogoApi } from '../../api/servicos'
import {
  Carregando,
  EstadoVazio,
  MensagemErro,
} from '../../components/MensagensEstado'
import Paginacao from '../../components/Paginacao'

function Jogos() {
  const [paginaJogos, setPaginaJogos] = useState(null)
  const [busca, setBusca] = useState('')
  const [buscaAplicada, setBuscaAplicada] = useState('')
  const [pagina, setPagina] = useState(1)
  const [carregando, setCarregando] = useState(true)
  const [erroLista, setErroLista] = useState('')
  const [erroAcao, setErroAcao] = useState('')
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let cancelado = false

    setCarregando(true)
    setErroLista('')

    jogoApi
      .listar({
        busca: buscaAplicada || undefined,
        incluirInativos: true,
        pagina,
        tamanhoPagina: 12,
      })
      .then((resultado) => {
        if (!cancelado) {
          setPaginaJogos(resultado)
        }
      })
      .catch((erroRequisicao) => {
        if (!cancelado) {
          setErroLista(obterMensagemErro(erroRequisicao))
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
  }, [buscaAplicada, pagina, tentativa])

  function aoBuscar(evento) {
    evento.preventDefault()
    setPagina(1)
    setBuscaAplicada(busca.trim())
  }

  async function desativarJogo(jogo) {
    const confirmou = window.confirm(
      `Deseja desativar o jogo "${jogo.nome}"?`,
    )

    if (!confirmou) {
      return
    }

    setErroAcao('')

    try {
      await jogoApi.desativar(jogo.id)
      setTentativa((valor) => valor + 1)
    } catch (erro) {
      setErroAcao(obterMensagemErro(erro))
    }
  }

  if (carregando) {
    return <Carregando texto="Carregando jogos..." />
  }

  if (erroLista) {
    return (
      <MensagemErro
        mensagem={erroLista}
        aoTentarNovamente={() => setTentativa((valor) => valor + 1)}
      />
    )
  }

  return (
    <section>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1>Jogos</h1>
        <Link className="btn btn-primary" to="/admin/jogos/novo">
          Novo jogo
        </Link>
      </div>

      {erroAcao && (
        <div className="alert alert-danger" role="alert">
          {erroAcao}
        </div>
      )}

      <form onSubmit={aoBuscar} className="mb-4">
        <label className="form-label" htmlFor="buscaJogos">
          Buscar por nome
        </label>
        <div className="input-group">
          <input
            className="form-control"
            id="buscaJogos"
            value={busca}
            onChange={(evento) => setBusca(evento.target.value)}
          />
          <button className="btn btn-outline-primary" type="submit">
            Buscar
          </button>
        </div>
      </form>

      {!paginaJogos || paginaJogos.itens.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          Ajuste a busca ou cadastre um jogo.
        </EstadoVazio>
      ) : (
        <>
          <div className="table-responsive">
            <table className="table table-striped">
              <thead>
                <tr>
                  <th scope="col">Nome</th>
                  <th scope="col">Plataforma</th>
                  <th scope="col">Ano</th>
                  <th scope="col">Situação</th>
                  <th scope="col">Ações</th>
                </tr>
              </thead>
              <tbody>
                {paginaJogos.itens.map((jogo) => (
                  <tr key={jogo.id}>
                    <td>{jogo.nome}</td>
                    <td>{jogo.nomePlataforma}</td>
                    <td>{jogo.anoLancamento}</td>
                    <td>{jogo.ativo ? 'Ativo' : 'Inativo'}</td>
                    <td>
                      <Link
                        className="btn btn-outline-primary btn-sm me-2"
                        to={`/admin/jogos/${jogo.id}/editar`}
                      >
                        Editar
                      </Link>

                      {jogo.ativo && (
                        <button
                          className="btn btn-outline-danger btn-sm"
                          type="button"
                          onClick={() => desativarJogo(jogo)}
                        >
                          Desativar
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <Paginacao
            pagina={paginaJogos.pagina}
            totalPaginas={paginaJogos.totalPaginas}
            aoMudar={setPagina}
          />
        </>
      )}
    </section>
  )
}

export default Jogos