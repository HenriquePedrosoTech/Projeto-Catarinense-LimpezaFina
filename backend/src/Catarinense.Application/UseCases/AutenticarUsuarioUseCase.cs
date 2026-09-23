using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

/// <summary>
/// Valida matrícula + senha e retorna um token de acesso.
/// Depende só de abstrações (repositório, hasher, gerador de token) — nunca de EF Core ou JWT diretamente.
/// </summary>
public class AutenticarUsuarioUseCase : IAutenticarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AutenticarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> ExecutarAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorMatriculaAsync(request.Matricula)
            ?? throw new NaoAutorizadoException("Matrícula ou senha inválida.");

        if (usuario.EstaBloqueado())
            throw new NaoAutorizadoException("Conta temporariamente bloqueada por muitas tentativas incorretas. Tente novamente em alguns minutos.");

        if (!usuario.Ativo || !_passwordHasher.Verificar(request.Senha, usuario.SenhaHash))
        {
            // Mesma mensagem genérica em ambos os casos (matrícula errada ou senha errada),
            // pra não dar dica pra quem está tentando adivinhar matrículas válidas.
            usuario.RegistrarTentativaFalha();
            _usuarioRepository.Atualizar(usuario);
            await _usuarioRepository.SalvarAlteracoesAsync();
            throw new NaoAutorizadoException("Matrícula ou senha inválida.");
        }

        usuario.RegistrarLoginComSucesso();
        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        var token = _jwtTokenGenerator.GerarToken(usuario);

        return new LoginResponse(usuario.Id, usuario.Nome, usuario.Matricula, usuario.Perfil.ToString(), token);
    }
}
