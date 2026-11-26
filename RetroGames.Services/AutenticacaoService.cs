using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet repositorio;
    private readonly IPasswordHasher<Usuario> passwordHasher;

    public AutenticacaoService(
        IRepositorySet repositorio,
        IPasswordHasher<Usuario> passwordHasher)
    {
        this.repositorio = repositorio;
        this.passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await repositorio.Usuario.BuscarPorEmailAsync(email.Trim());

        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        var resultado = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha);

        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return usuario;
    }

    public async Task GarantirAdministradorInicialAsync(
        string email,
        string senha)
    {
        if (await repositorio.Usuario.ExisteAdministradorAsync())
        {
            return;
        }

        var usuario = new Usuario
        {
            Nome = "Administrador",
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.Now,
            IdPerfil = PerfilIds.Administrador
        };

        usuario.SenhaHash = passwordHasher.HashPassword(usuario, senha);
        usuario.Id = await repositorio.Usuario.InserirAsync(usuario);
    }
}