using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class TrocarSenhaUsuarioUseCase : ITrocarSenhaUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public TrocarSenhaUsuarioUseCase(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task ExecutarAsync(Guid usuarioId, TrocarSenhaRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new NotFoundException("Usuario nao encontrado.");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
        usuario.TrocarSenha(senhaHash);

        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();
    }
}
