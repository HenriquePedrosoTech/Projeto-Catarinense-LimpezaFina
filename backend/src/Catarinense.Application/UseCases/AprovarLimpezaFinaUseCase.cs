using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class AprovarLimpezaFinaUseCase : IAprovarLimpezaFinaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public AprovarLimpezaFinaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IUsuarioRepository usuarioRepository,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _usuarioRepository = usuarioRepository;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(AprovarLimpezaFinaRequest request)
    {
        var avaliador = await _usuarioRepository.ObterPorIdAsync(request.AvaliadorId)
            ?? throw new NotFoundException("Avaliador não encontrado.");

        if (!avaliador.EhAdministrador())
            throw new NaoAutorizadoException("Somente um administrador pode aprovar uma limpeza fina.");

        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        limpeza.Aprovar(avaliador.Id);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}
