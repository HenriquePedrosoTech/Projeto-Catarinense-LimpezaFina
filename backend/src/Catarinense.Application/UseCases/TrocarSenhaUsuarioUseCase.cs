using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;
using Catarinense.Application.Interfaces;

namespace Catarinense.Application.UseCases;

public class TrocarSenhaUsuarioUseCase : ITrocarSenhaUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public TrocarSenhaUsuarioUseCase(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task ExecutarAsync(Guid usuarioId, TrocarSenhaRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new NotFoundException("Usuario nao encontrado.");

        var senhaHash = _passwordHasher.Hash(request.NovaSenha);
        usuario.TrocarSenha(senhaHash);

        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();
    }
}
