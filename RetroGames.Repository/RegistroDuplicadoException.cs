namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException()
        : base("Já existe um registro com esse valor.")
    {
    }
}