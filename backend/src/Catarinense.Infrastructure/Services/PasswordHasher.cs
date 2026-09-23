using Catarinense.Application.Interfaces;

namespace Catarinense.Infrastructure.Services;

/// <summary>
/// Implementação com BCrypt (pacote BCrypt.Net-Next). O hash já embute o salt,
/// então não precisamos armazenar salt separadamente.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string senhaPura) => BCrypt.Net.BCrypt.HashPassword(senhaPura);

    public bool Verificar(string senhaPura, string senhaHash) =>
        BCrypt.Net.BCrypt.Verify(senhaPura, senhaHash);
}
