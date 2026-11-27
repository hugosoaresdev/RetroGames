import { useEffect, useState } from 'react'
import {
  obterErrosDosCampos,
  obterMensagemErro,
} from '../../api/erros'
import { plataformaApi } from '../../api/servicos'
import {
  Carregando,
  EstadoVazio,
  MensagemErro,
} from '../../components/MensagensEstado'

function Plataforma() {
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erroLista, setErroLista] = useState('')
  const [tentativa, setTentativa] = useState(0)

  const [plataformaEmEdicao, setPlataformaEmEdicao] = useState(null)
  const [nome, setNome] = useState('')
  const [fabricante, setFabricante] = useState('')
  const [anoLancamento, setAnoLancamento] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [erroFormulario, setErroFormulario] = useState('')
  const [salvando, setSalvando] = useState(false)

  useEffect(() => {
    let cancelado = false

    setCarregando(true)
    setErroLista('')

    plataformaApi
      .listar()
      .then((resultado) => {
        if (!cancelado) {
          setPlataformas(resultado)
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
  }, [tentativa])

  function limparFormulario() {
    setPlataformaEmEdicao(null)
    setNome('')
    setFabricante('')
    setAnoLancamento('')
    setErrosCampos({})
    setErroFormulario('')
  }

  function iniciarEdicao(plataforma) {
    setPlataformaEmEdicao(plataforma)
    setNome(plataforma.nome)
    setFabricante(plataforma.fabricante)
    setAnoLancamento(String(plataforma.anoLancamento))
    setErrosCampos({})
    setErroFormulario('')
  }

  async function aoSalvar(evento) {
    evento.preventDefault()
    setErrosCampos({})
    setErroFormulario('')
    setSalvando(true)

    const dadosPlataforma = {
      nome: nome.trim(),
      fabricante: fabricante.trim(),
      anoLancamento: Number(anoLancamento),
    }

    try {
      if (plataformaEmEdicao) {
        await plataformaApi.alterar(
          plataformaEmEdicao.id,
          dadosPlataforma,
        )
      } else {
        await plataformaApi.inserir(dadosPlataforma)
      }

      limparFormulario()
      setTentativa((valor) => valor + 1)
    } catch (erro) {
      setErrosCampos(obterErrosDosCampos(erro))
      setErroFormulario(obterMensagemErro(erro))
    } finally {
      setSalvando(false)
    }
  }

  async function excluirPlataforma(plataforma) {
    const confirmou = window.confirm(
      `Deseja excluir a plataforma "${plataforma.nome}"?`,
    )

    if (!confirmou) {
      return
    }

    setErroFormulario('')

    try {
      await plataformaApi.excluir(plataforma.id)

      if (plataformaEmEdicao?.id === plataforma.id) {
        limparFormulario()
      }

      setTentativa((valor) => valor + 1)
    } catch (erroExclusao) {
      setErroFormulario(obterMensagemErro(erroExclusao))
    }
  }

  if (carregando) {
    return <Carregando texto="Carregando plataformas..." />
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
      <h1>Plataformas</h1>

      <h2>{plataformaEmEdicao ? 'Editar plataforma' : 'Nova plataforma'}</h2>

      <form onSubmit={aoSalvar} className="mb-4">
        <div className="row g-3">
          <div className="col-12 col-md-4">
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

          <div className="col-12 col-md-4">
            <label className="form-label" htmlFor="fabricante">
              Fabricante
            </label>
            <input
              className={`form-control${errosCampos.fabricante ? ' is-invalid' : ''}`}
              id="fabricante"
              value={fabricante}
              onChange={(evento) => setFabricante(evento.target.value)}
            />
            {errosCampos.fabricante && (
              <div className="invalid-feedback">
                {errosCampos.fabricante}
              </div>
            )}
          </div>

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
        </div>

        {erroFormulario && (
          <div className="alert alert-danger mt-3" role="alert">
            {erroFormulario}
          </div>
        )}

        <div className="mt-3">
          <button
            className="btn btn-primary me-2"
            type="submit"
            disabled={salvando}
          >
            {salvando ? 'Salvando...' : 'Salvar'}
          </button>

          {plataformaEmEdicao && (
            <button
              className="btn btn-outline-secondary"
              type="button"
              onClick={limparFormulario}
            >
              Cancelar edição
            </button>
          )}
        </div>
      </form>

      {plataformas.length === 0 ? (
        <EstadoVazio titulo="Nenhuma plataforma encontrada">
          Ainda não há plataformas cadastradas.
        </EstadoVazio>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped">
            <thead>
              <tr>
                <th scope="col">Nome</th>
                <th scope="col">Fabricante</th>
                <th scope="col">Ano de lançamento</th>
                <th scope="col">Ações</th>
              </tr>
            </thead>
            <tbody>
              {plataformas.map((plataforma) => (
                <tr key={plataforma.id}>
                  <td>{plataforma.nome}</td>
                  <td>{plataforma.fabricante}</td>
                  <td>{plataforma.anoLancamento}</td>
                  <td>
                    <button
                      className="btn btn-outline-primary btn-sm"
                      type="button"
                      onClick={() => iniciarEdicao(plataforma)}
                    >
                      Editar
                    </button>
                    <button
                      className="btn btn-outline-danger btn-sm ms-2"
                      type="button"
                      onClick={() => excluirPlataforma(plataforma)}
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default Plataforma