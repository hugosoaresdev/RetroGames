import { useContext } from 'react'
import AutenticacaoContext from '../contexts/autenticacaoContext'

function useAutenticacao() {
  const contexto = useContext(AutenticacaoContext)

  if (contexto === null) {
    throw new Error(
      'useAutenticacao deve ser usado dentro de AutenticacaoProvider.',
    )
  }

  return contexto
}

export default useAutenticacao