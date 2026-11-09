using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

/// <summary>
/// Valida matrícula (ou nome de usuário) + senha e retorna um token de acesso.
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
        var candidatos = await _usuarioRepository.BuscarPorMatriculaOuNomeAsync(request.Matricula);
        if (candidatos.Count == 0)
            throw new NaoAutorizadoException("Nome/matrícula ou senha inválida.");

        // Se houver mais de um candidato (ex: dois operadores com o mesmo primeiro nome),
        // prioriza o que está ativo e cuja senha confere.
        var usuarioAutenticado = candidatos.FirstOrDefault(u =>
            u.Ativo && !u.EstaBloqueado() && _passwordHasher.Verificar(request.Senha, u.SenhaHash));

        if (usuarioAutenticado is null)
        {
            var primeiro = candidatos[0];
            if (primeiro.EstaBloqueado())
                throw new NaoAutorizadoException("Conta temporariamente bloqueada por muitas tentativas incorretas. Tente novamente em alguns minutos.");

            primeiro.RegistrarTentativaFalha();
            _usuarioRepository.Atualizar(primeiro);
            await _usuarioRepository.SalvarAlteracoesAsync();
            throw new NaoAutorizadoException("Nome/matrícula ou senha inválida.");
        }

        usuarioAutenticado.RegistrarLoginComSucesso();
        _usuarioRepository.Atualizar(usuarioAutenticado);
        await _usuarioRepository.SalvarAlteracoesAsync();

        var token = _jwtTokenGenerator.GerarToken(usuarioAutenticado);

        return new LoginResponse(
            usuarioAutenticado.Id,
            usuarioAutenticado.Nome,
            usuarioAutenticado.Matricula,
            usuarioAutenticado.Perfil.ToString(),
            token,
            usuarioAutenticado.PrecisaTrocarSenha);
    }
}
