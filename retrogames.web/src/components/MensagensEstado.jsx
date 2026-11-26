export function Carregando({ texto = 'Carregando...' }) {
  return (
    <p className="text-muted" role="status">
      {texto}
    </p>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger" role="alert">
      <p className="mb-2">{mensagem}</p>
      <button
        className="btn btn-outline-danger"
        type="button"
        onClick={aoTentarNovamente}
      >
        Tentar novamente
      </button>
    </div>
  )
}

export function EstadoVazio({ titulo, children }) {
  return (
    <div className="text-center py-5">
      <h2>{titulo}</h2>
      {children && <p>{children}</p>}
    </div>
  )
}