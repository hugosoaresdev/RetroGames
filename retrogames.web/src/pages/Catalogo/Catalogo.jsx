import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import {
  generoApi,
  jogoApi,
  plataformaApi,
} from '../../api/servicos'
import CardJogo from '../../components/CardJogo'
import {
  Carregando,
  EstadoVazio,
  MensagemErro,
} from '../../components/MensagensEstado'
import Paginacao from '../../components/Paginacao'

function Catalogo() {
  const [parametrosBusca, setParametrosBusca] = useSearchParams()
  const [busca, setBusca] = useState(parametrosBusca.get('busca') ?? '')
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [paginaJogos, setPaginaJogos] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erroJogos, setErroJogos] = useState('')
  const [erroFiltros, setErroFiltros] = useState('')
  const [tentativaJogos, setTentativaJogos] = useState(0)
  const [tentativaFiltros, setTentativaFiltros] = useState(0)

  const pagina = Number(parametrosBusca.get('pagina') || 1)

  useEffect(() => {
    setBusca(parametrosBusca.get('busca') ?? '')
  }, [parametrosBusca])

  useEffect(() => {
    let cancelado = false

    setErroFiltros('')

    Promise.all([plataformaApi.listar(), generoApi.listar()])
      .then(([listaPlataformas, listaGeneros]) => {
        if (cancelado) return

        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
      })
      .catch((erro) => {
        if (!cancelado) {
          setErroFiltros(obterMensagemErro(erro))
        }
      })

    return () => {
      cancelado = true
    }
  }, [tentativaFiltros])

  useEffect(() => {
    let cancelado = false

    setCarregando(true)
    setErroJogos('')

    jogoApi
      .listar({
        busca: parametrosBusca.get('busca') || undefined,
        idPlataforma:
          parametrosBusca.get('idPlataforma') || undefined,
        idGenero: parametrosBusca.get('idGenero') || undefined,
        pagina,
        tamanhoPagina: 12,
      })
      .then((resultado) => {
        if (!cancelado) {
          setPaginaJogos(resultado)
        }
      })
      .catch((erro) => {
        if (!cancelado) {
          setErroJogos(obterMensagemErro(erro))
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
  }, [parametrosBusca, pagina, tentativaJogos])

  function atualizarFiltro(nome, valor) {
    const novosParametros = new URLSearchParams(parametrosBusca)

    if (valor) {
      novosParametros.set(nome, valor)
    } else {
      novosParametros.delete(nome)
    }

    novosParametros.set('pagina', '1')
    setParametrosBusca(novosParametros)
  }

  function aoBuscar(evento) {
    evento.preventDefault()
    atualizarFiltro('busca', busca.trim())
  }

  function aoMudarPagina(novaPagina) {
    const novosParametros = new URLSearchParams(parametrosBusca)
    novosParametros.set('pagina', String(novaPagina))
    setParametrosBusca(novosParametros)
  }

  return (
    <section>
      <h1>Catálogo</h1>

      {erroFiltros && (
        <MensagemErro
          mensagem={erroFiltros}
          aoTentarNovamente={() =>
            setTentativaFiltros((tentativa) => tentativa + 1)
          }
        />
      )}

      <form onSubmit={aoBuscar}>
        <div className="row g-3 align-items-end mb-4">
          <div className="col-12 col-md-5">
            <label className="form-label" htmlFor="busca">
              Buscar por nome
            </label>
            <div className="input-group">
              <input
                className="form-control"
                id="busca"
                value={busca}
                onChange={(evento) => setBusca(evento.target.value)}
              />
              <button className="btn btn-primary" type="submit">
                Buscar
              </button>
            </div>
          </div>

          <div className="col-12 col-md-4">
            <label className="form-label" htmlFor="idPlataforma">
              Plataforma
            </label>
            <select
              className="form-select"
              id="idPlataforma"
              value={parametrosBusca.get('idPlataforma') ?? ''}
              onChange={(evento) =>
                atualizarFiltro('idPlataforma', evento.target.value)
              }
            >
              <option value="">Todas as plataformas</option>
              {plataformas.map((plataforma) => (
                <option key={plataforma.id} value={plataforma.id}>
                  {plataforma.nome}
                </option>
              ))}
            </select>
          </div>

          <div className="col-12 col-md-3">
            <label className="form-label" htmlFor="idGenero">
              Gênero
            </label>
            <select
              className="form-select"
              id="idGenero"
              value={parametrosBusca.get('idGenero') ?? ''}
              onChange={(evento) =>
                atualizarFiltro('idGenero', evento.target.value)
              }
            >
              <option value="">Todos os gêneros</option>
              {generos.map((genero) => (
                <option key={genero.id} value={genero.id}>
                  {genero.nome}
                </option>
              ))}
            </select>
          </div>
        </div>
      </form>

      {carregando ? (
        <Carregando texto="Carregando jogos..." />
      ) : erroJogos ? (
        <MensagemErro
          mensagem={erroJogos}
          aoTentarNovamente={() =>
            setTentativaJogos((tentativa) => tentativa + 1)
          }
        />
      ) : !paginaJogos || paginaJogos.itens.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          Ajuste a busca ou os filtros e tente novamente.
        </EstadoVazio>
      ) : (
        <>
          <div className="row g-3">
            {paginaJogos.itens.map((jogo) => (
              <div className="col-12 col-md-6 col-xl-4" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>

          <div className="mt-4">
            <Paginacao
              pagina={paginaJogos.pagina}
              totalPaginas={paginaJogos.totalPaginas}
              aoMudar={aoMudarPagina}
            />
          </div>
        </>
      )}
    </section>
  )
}

export default Catalogo