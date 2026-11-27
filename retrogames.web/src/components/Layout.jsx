import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'

function Layout() {
  const { usuario, carregando, ehAdministrador, sair } = useAutenticacao()
  const navegar = useNavigate()

  async function aoSair() {
    await sair()
    navegar('/')
  }

  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <NavLink className="navbar-brand" to="/">
            RetroGames
          </NavLink>

          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuNavegacao"
            aria-controls="menuNavegacao"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="menuNavegacao">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <NavLink
                  className={({ isActive }) =>
                    `nav-link${isActive ? ' active' : ''}`
                  }
                  end
                  to="/"
                >
                  Catálogo
                </NavLink>
              </li>
            </ul>

            <ul className="navbar-nav ms-auto">
              {!carregando &&
                (usuario ? (
                  <>
                    <li className="nav-item">
                      <span className="nav-link">
                        Olá, {usuario.nome}
                      </span>
                    </li>

                    {ehAdministrador && (
                      <li className="nav-item dropdown">
                        <button
                          className="nav-link dropdown-toggle"
                          type="button"
                          data-bs-toggle="dropdown"
                          aria-expanded="false"
                        >
                          Administração
                        </button>
                        <ul className="dropdown-menu dropdown-menu-end">
                          <li>
                            <NavLink
                              className="dropdown-item"
                              to="/admin/jogos"
                            >
                              Jogos
                            </NavLink>
                          </li>
                          <li>
                            <NavLink
                              className="dropdown-item"
                              to="/admin/plataformas"
                            >
                              Plataformas
                            </NavLink>
                          </li>
                        </ul>
                      </li>
                    )}

                    <li className="nav-item">
                      <button
                        className="nav-link"
                        type="button"
                        onClick={aoSair}
                      >
                        Sair
                      </button>
                    </li>
                  </>
                ) : (
                  <li className="nav-item">
                    <NavLink
                      className={({ isActive }) =>
                        `nav-link${isActive ? ' active' : ''}`
                      }
                      to="/login"
                    >
                      Entrar
                    </NavLink>
                  </li>
                ))}
            </ul>
          </div>
        </div>
      </nav>

      <main className="container py-4">
        <Outlet />
      </main>

      <footer className="border-top py-3 mt-4">
        <div className="container text-center text-muted">RetroGames</div>
      </footer>
    </>
  )
}

export default Layout