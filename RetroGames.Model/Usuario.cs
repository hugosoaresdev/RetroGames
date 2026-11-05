
namespace RetroGames.Model.Model;

public class Usuario
{
    /// <summary>
    /// Coluna: co_seq_usuario
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Coluna: no_usuario
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Coluna: ds_email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Coluna: ds_senha_hash
    /// </summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>
    /// Coluna: st_ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Coluna: dt_cadastro
    /// </summary>
    public DateTime DataCadastro { get; set; }

    /// <summary>
    /// Coluna: co_perfil
    /// </summary>
    public int IdPerfil { get; set; }
}
