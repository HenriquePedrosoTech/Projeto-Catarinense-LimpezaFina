using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

/// <summary>
/// Representa um colaborador que pode operar (executar limpeza) ou administrar (validar/notificar) o sistema.
/// </summary>
public class Usuario : EntidadeBase
{
    public string Matricula { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string SenhaHash { get; private set; } = string.Empty;
    public PerfilUsuario Perfil { get; private set; }
    public bool Ativo { get; private set; } = true;
    public int TentativasFalhasLogin { get; private set; }
    public DateTime? BloqueadoAte { get; private set; }

    private const int MaximoTentativasFalhas = 5;
    private static readonly TimeSpan DuracaoBloqueio = TimeSpan.FromMinutes(15);

    protected Usuario() { } // uso por ORM

    public Usuario(string matricula, string nome, string senhaHash, PerfilUsuario perfil, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(matricula))
            throw new DomainException("A matrícula é obrigatória.");

        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DomainException("A senha é obrigatória.");

        Matricula = matricula.Trim();
        Nome = nome.Trim();
        SenhaHash = senhaHash;
        Perfil = perfil;
        Email = email?.Trim();
    }

    
    public void Atualizar(string nome, string? email, string? senhaHash = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome e obrigatorio.");
        Nome = nome.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        if (!string.IsNullOrWhiteSpace(senhaHash))
            SenhaHash = senhaHash;
    }
    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;

    public bool EhAdministrador() => Perfil == PerfilUsuario.Administrador;

    /// <summary>Verdadeiro enquanto o bloqueio temporário por tentativas erradas ainda estiver valendo.</summary>
    public bool EstaBloqueado() => BloqueadoAte is not null && BloqueadoAte > DateTime.UtcNow;

    /// <summary>Chamado quando a senha informada no login está errada. Bloqueia temporariamente após 5 erros seguidos.</summary>
    public void RegistrarTentativaFalha()
    {
        TentativasFalhasLogin++;
        if (TentativasFalhasLogin >= MaximoTentativasFalhas)
        {
            BloqueadoAte = DateTime.UtcNow.Add(DuracaoBloqueio);
            TentativasFalhasLogin = 0;
        }
    }

    /// <summary>Chamado em todo login bem-sucedido, para zerar o contador de tentativas erradas.</summary>
    public void RegistrarLoginComSucesso()
    {
        TentativasFalhasLogin = 0;
        BloqueadoAte = null;
    }
}

