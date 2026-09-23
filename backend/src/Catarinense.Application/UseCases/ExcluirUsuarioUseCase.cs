using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ExcluirUsuarioUseCase : IExcluirUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;

    public ExcluirUsuarioUseCase(IUsuarioRepository usuarioRepository, ILimpezaFinaRepository limpezaFinaRepository)
    {
        _usuarioRepository = usuarioRepository;
        _limpezaFinaRepository = limpezaFinaRepository;
    }

    public async Task ExecutarAsync(Guid usuarioId)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new NotFoundException("Usuário não encontrado.");

        if (usuario.EhAdministrador())
        {
            var todos = await _usuarioRepository.ListarAsync();
            var outrosAdminsAtivos = todos.Count(u => u.Id != usuarioId && u.EhAdministrador() && u.Ativo);
            if (outrosAdminsAtivos == 0)
                throw new DomainException("Não é possível excluir: este é o único Administrador ativo do sistema.");
        }

        var limpezasComoOperador = await _limpezaFinaRepository.ListarPorOperadorAsync(usuarioId);
        if (limpezasComoOperador.Count > 0)
            throw new DomainException(
                $"Não é possível excluir: existem {limpezasComoOperador.Count} registro(s) de limpeza fina vinculados a este usuário como operador. Exclua-os primeiro.");

        _usuarioRepository.Remover(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();
    }
}
