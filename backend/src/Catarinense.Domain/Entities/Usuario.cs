using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class Usuario : EntidadeBase
{
    public string Matricula { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string SenhaHash { get; private set; } = string.Empty;
    public PerfilUsuario Perfil { get; private set; }
    public bool PrecisaTrocarSenha { get; private set; } = true;
    public bool Ativo { get; private set; } = true;
    public int TentativasFalhasLogin { get; private set; }
    public DateTime? BloqueadoAte { get; private set; }

    protected Usuario() { }

    public Usuario(string matricula, string nome, string senhaHash, PerfilUsuario perfil, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(matricula))
            throw new DomainException("A matrcula Ǹ obrigatria.");

        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome Ǹ obrigatrio.");

        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DomainException("A senha Ǹ obrigatria.");

        Matricula = matricula.Trim();
        Nome = nome.Trim();
        SenhaHash = senhaHash;
        Perfil = perfil;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }

    public void TrocarSenha(string novaSenha)
    {
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        PrecisaTrocarSenha = false;
    }

    public void Renomear(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome Ǹ obrigatrio.");

        Nome = nome.Trim();
    }

    public void AtualizarEmail(string? email)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }

    public void AlterarPerfil(PerfilUsuario novoPerfil)
    {
        Perfil = novoPerfil;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;

    public void RegistrarFalhaLogin(int maxTentativas, int minutosBloqueio)
    {
        TentativasFalhasLogin++;
        if (TentativasFalhasLogin >= maxTentativas)
        {
            BloqueadoAte = DateTime.UtcNow.AddMinutes(minutosBloqueio);
        }
    }

    public void ResetarFalhasLogin()
    {
        TentativasFalhasLogin = 0;
        BloqueadoAte = null;
    }

    public bool EstaBloqueado()
    {
        return BloqueadoAte.HasValue && BloqueadoAte.Value > DateTime.UtcNow;
    }
}
