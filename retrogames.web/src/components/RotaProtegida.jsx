import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { Carregando } from './MensagensEstado'
import useAutenticacao from '../hooks/useAutenticacao'

function RotaProtegida({ somenteAdministrador = false }) {
  const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao()
  const localizacao = useLocation()

  if (carregando) {
    return <Carregando texto="Verificando sessão..." />
  }

  if (!estaAutenticado) {
    const destino = `${localizacao.pathname}${localizacao.search}`

    return (
      <Navigate
        to={`/login?voltarPara=${encodeURIComponent(destino)}`}
        replace
      />
    )
  }

  if (somenteAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}

export default RotaProtegida