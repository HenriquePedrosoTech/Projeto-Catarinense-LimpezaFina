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
    public bool PrecisaTrocarSenha { get; private set; } = true;
    public bool Ativo { get; private set; } = true;
    public bool IsMaster { get; private set; }
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
        IsMaster = string.Equals(Matricula, MatriculaMaster, StringComparison.OrdinalIgnoreCase);
    }

    
    public void TrocarSenha(string novaSenhaHash)
    {
        SenhaHash = novaSenhaHash;
        PrecisaTrocarSenha = false;
    }

    public void Atualizar(string nome, string? email, string? senhaHash = null, string? novaMatricula = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome e obrigatorio.");
        if (!string.IsNullOrWhiteSpace(novaMatricula))
        {
            if (string.Equals(Matricula, MatriculaMaster, StringComparison.OrdinalIgnoreCase))
                IsMaster = true;
            Matricula = novaMatricula.Trim();
        }
        Nome = nome.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        if (!string.IsNullOrWhiteSpace(senhaHash))
            SenhaHash = senhaHash;
    }
    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;

    public const string MatriculaMaster = "347134";

    public bool EhAdministrador() => Perfil == PerfilUsuario.Administrador;

    public bool EhMaster() => IsMaster || string.Equals(Matricula?.Trim(), MatriculaMaster, StringComparison.OrdinalIgnoreCase);

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


