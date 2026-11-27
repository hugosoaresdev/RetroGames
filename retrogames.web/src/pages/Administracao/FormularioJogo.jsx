import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  obterErrosDosCampos,
  obterMensagemErro,
} from '../../api/erros'
import {
  condicaoJogoApi,
  generoApi,
  jogoApi,
  plataformaApi,
} from '../../api/servicos'
import { Carregando } from '../../components/MensagensEstado'

function FormularioJogo() {
  const { id } = useParams()
  const emEdicao = Boolean(id)
  const navegar = useNavigate()

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [condicoes, setCondicoes] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erroCarregamento, setErroCarregamento] = useState('')

  const [nome, setNome] = useState('')
  const [descricao, setDescricao] = useState('')
  const [anoLancamento, setAnoLancamento] = useState('')
  const [preco, setPreco] = useState('')
  const [quantidadeEstoque, setQuantidadeEstoque] = useState('')
  const [idPlataforma, setIdPlataforma] = useState('')
  const [idCondicaoJogo, setIdCondicaoJogo] = useState('')
  const [idsGeneros, setIdsGeneros] = useState([])
  const [ativo, setAtivo] = useState(true)

  const [errosCampos, setErrosCampos] = useState({})
  const [erroFormulario, setErroFormulario] = useState('')
  const [salvando, setSalvando] = useState(false)

  useEffect(() => {
    let cancelado = false

    async function carregarDados() {
      setCarregando(true)
      setErroCarregamento('')

      try {
        const [
          listaPlataformas,
          listaGeneros,
          listaCondicoes,
          jogo,
        ] = await Promise.all([
          plataformaApi.listar(),
          generoApi.listar(),
          condicaoJogoApi.listar(),
          emEdicao ? jogoApi.buscarPorId(id) : Promise.resolve(null),
        ])

        if (cancelado) {
          return
        }

        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
        setCondicoes(listaCondicoes)

        if (jogo) {
          setNome(jogo.nome)
          setDescricao(jogo.descricao ?? '')
          setAnoLancamento(String(jogo.anoLancamento))
          setPreco(String(jogo.preco))
          setQuantidadeEstoque(String(jogo.quantidadeEstoque))
          setIdPlataforma(String(jogo.idPlataforma))
          setIdCondicaoJogo(String(jogo.idCondicaoJogo))
          setIdsGeneros(jogo.generos.map((genero) => String(genero.id)))
          setAtivo(jogo.ativo)
        }
      } catch (erro) {
        if (!cancelado) {
          setErroCarregamento(obterMensagemErro(erro))
        }
      } finally {
        if (!cancelado) {
          setCarregando(false)
        }
      }
    }

    carregarDados()

    return () => {
      cancelado = true
    }
  }, [emEdicao, id])

  function alterarGenero(idGenero, marcado) {
    setIdsGeneros((atuais) =>
      marcado
        ? [...atuais, idGenero]
        : atuais.filter((idAtual) => idAtual !== idGenero),
    )
  }

  async function aoSalvar(evento) {
    evento.preventDefault()
    setErrosCampos({})
    setErroFormulario('')
    setSalvando(true)

    const dadosJogo = {
      nome: nome.trim(),
      descricao: descricao.trim() || null,
      anoLancamento: Number(anoLancamento),
      preco: Number(preco),
      quantidadeEstoque: Number(quantidadeEstoque),
      idPlataforma: Number(idPlataforma),
      idCondicaoJogo: Number(idCondicaoJogo),
      idsGeneros: idsGeneros.map(Number),
    }

    try {
      if (emEdicao) {
        await jogoApi.alterar(id, {
          ...dadosJogo,
          ativo,
        })
      } else {
        await jogoApi.inserir(dadosJogo)
      }

      navegar('/admin/jogos', { replace: true })
    } catch (erro) {
      setErrosCampos(obterErrosDosCampos(erro))
      setErroFormulario(obterMensagemErro(erro))
    } finally {
      setSalvando(false)
    }
  }

  if (carregando) {
    return <Carregando texto="Carregando dados do jogo..." />
  }

  if (erroCarregamento) {
    return (
      <div className="alert alert-danger" role="alert">
        {erroCarregamento}
      </div>
    )
  }

  return (
    <section>
      <h1>{emEdicao ? 'Editar jogo' : 'Novo jogo'}</h1>

      <form onSubmit={aoSalvar}>
        <div className="mb-3">
          <label className="form-label" htmlFor="nome">
            Nome
          </label>
          <input
            className={`form-control${errosCampos.nome ? ' is-invalid' : ''}`}
            id="nome"
            value={nome}
            onChange={(evento) => setNome(evento.target.value)}
          />
          {errosCampos.nome && (
            <div className="invalid-feedback">{errosCampos.nome}</div>
          )}
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="descricao">
            Descrição
          </label>
          <textarea
            className={`form-control${errosCampos.descricao ? ' is-invalid' : ''}`}
            id="descricao"
            rows="3"
            value={descricao}
            onChange={(evento) => setDescricao(evento.target.value)}
          />
          {errosCampos.descricao && (
            <div className="invalid-feedback">{errosCampos.descricao}</div>
          )}
        </div>

        <div className="row g-3 mb-3">
          <div className="col-12 col-md-4">
            <label className="form-label" htmlFor="anoLancamento">
              Ano de lançamento
            </label>
            <input
              className={`form-control${errosCampos.anoLancamento ? ' is-invalid' : ''}`}
              id="anoLancamento"
              type="number"
              value={anoLancamento}
              onChange={(evento) => setAnoLancamento(evento.target.value)}
            />
            {errosCampos.anoLancamento && (
              <div className="invalid-feedback">
                {errosCampos.anoLancamento}
              </div>
            )}
          </div>

          <div className="col-12 col-md-4">
            <label className="form-label" htmlFor="preco">
              Preço
            </label>
            <input
              className={`form-control${errosCampos.preco ? ' is-invalid' : ''}`}
              id="preco"
              type="number"
              step="any"
              value={preco}
              onChange={(evento) => setPreco(evento.target.value)}
            />
            {errosCampos.preco && (
              <div className="invalid-feedback">{errosCampos.preco}</div>
            )}
          </div>

          <div className="col-12 col-md-4">
            <label className="form-label" htmlFor="quantidadeEstoque">
              Estoque
            </label>
            <input
              className={`form-control${errosCampos.quantidadeEstoque ? ' is-invalid' : ''}`}
              id="quantidadeEstoque"
              type="number"
              value={quantidadeEstoque}
              onChange={(evento) =>
                setQuantidadeEstoque(evento.target.value)
              }
            />
            {errosCampos.quantidadeEstoque && (
              <div className="invalid-feedback">
                {errosCampos.quantidadeEstoque}
              </div>
            )}
          </div>
        </div>

        <div className="row g-3 mb-3">
          <div className="col-12 col-md-6">
            <label className="form-label" htmlFor="idPlataforma">
              Plataforma
            </label>
            <select
              className={`form-select${errosCampos.idPlataforma ? ' is-invalid' : ''}`}
              id="idPlataforma"
              value={idPlataforma}
              onChange={(evento) => setIdPlataforma(evento.target.value)}
            >
              <option value="">Selecione uma plataforma</option>
              {plataformas.map((plataforma) => (
                <option key={plataforma.id} value={plataforma.id}>
                  {plataforma.nome}
                </option>
              ))}
            </select>
            {errosCampos.idPlataforma && (
              <div className="invalid-feedback">
                {errosCampos.idPlataforma}
              </div>
            )}
          </div>

          <div className="col-12 col-md-6">
            <label className="form-label" htmlFor="idCondicaoJogo">
              Condição
            </label>
            <select
              className={`form-select${errosCampos.idCondicaoJogo ? ' is-invalid' : ''}`}
              id="idCondicaoJogo"
              value={idCondicaoJogo}
              onChange={(evento) => setIdCondicaoJogo(evento.target.value)}
            >
              <option value="">Selecione uma condição</option>
              {condicoes.map((condicao) => (
                <option key={condicao.id} value={condicao.id}>
                  {condicao.descricao}
                </option>
              ))}
            </select>
            {errosCampos.idCondicaoJogo && (
              <div className="invalid-feedback">
                {errosCampos.idCondicaoJogo}
              </div>
            )}
          </div>
        </div>

        <fieldset className="mb-3">
          <legend className="form-label">Gêneros</legend>
          {generos.map((genero) => {
            const idGenero = String(genero.id)

            return (
              <div className="form-check" key={genero.id}>
                <input
                  className="form-check-input"
                  id={`genero-${genero.id}`}
                  type="checkbox"
                  checked={idsGeneros.includes(idGenero)}
                  onChange={(evento) =>
                    alterarGenero(idGenero, evento.target.checked)
                  }
                />
                <label
                  className="form-check-label"
                  htmlFor={`genero-${genero.id}`}
                >
                  {genero.nome}
                </label>
              </div>
            )
          })}
          {errosCampos.idsGeneros && (
            <div className="text-danger small mt-1">
              {errosCampos.idsGeneros}
            </div>
          )}
        </fieldset>

        {erroFormulario && (
          <div className="alert alert-danger" role="alert">
            {erroFormulario}
          </div>
        )}

        <button
          className="btn btn-primary me-2"
          type="submit"
          disabled={salvando}
        >
          {salvando ? 'Salvando...' : 'Salvar'}
        </button>

        <Link className="btn btn-outline-secondary" to="/admin/jogos">
          Cancelar
        </Link>
      </form>
    </section>
  )
}

export default FormularioJogo