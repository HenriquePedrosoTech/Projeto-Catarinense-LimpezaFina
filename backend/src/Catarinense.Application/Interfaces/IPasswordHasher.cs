namespace Catarinense.Application.Interfaces;

/// <summary>
/// Abstrai o algoritmo de hash de senha. A Infrastructure implementa (ex.: BCrypt).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string senhaPura);
    bool Verificar(string senhaPura, string senhaHash);
}
