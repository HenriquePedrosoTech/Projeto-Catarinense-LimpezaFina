using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.Common;

/// <summary>
/// Monta o LimpezaFinaDetalhesDto completo (com nome do ônibus, nome do operador
/// e nomes das etapas) a partir da entidade. Centraliza esse fluxo, usado por
/// vários casos de uso (Iniciar, RegistrarFoto, Finalizar, Aprovar, Reprovar, Consultar).
/// </summary>
public class DetalhesDtoBuilder
{
    private readonly IOnibusRepository _onibusRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public DetalhesDtoBuilder(
        IOnibusRepository onibusRepository,
        IUsuarioRepository usuarioRepository,
        IEtapaPadraoRepository etapaPadraoRepository)
    {
        _onibusRepository = onibusRepository;
        _usuarioRepository = usuarioRepository;
        _etapaPadraoRepository = etapaPadraoRepository;
    }

    public async Task<LimpezaFinaDetalhesDto> ConstruirAsync(LimpezaFina limpeza)
    {
        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId)
            ?? throw new NotFoundException("Ônibus não encontrado.");

        var operador = await _usuarioRepository.ObterPorIdAsync(limpeza.OperadorId)
            ?? throw new NotFoundException("Operador não encontrado.");

        var etapasPadrao = await LimpezaFinaMapper.CarregarEtapasPadraoAsync(_etapaPadraoRepository, limpeza);

        return LimpezaFinaMapper.ParaDetalhesDto(limpeza, onibus, operador, etapasPadrao);
    }

    public async Task<LimpezaFinaResumoDto> ConstruirResumoAsync(LimpezaFina limpeza)
    {
        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId)
            ?? throw new NotFoundException("Ônibus não encontrado.");

        var operador = await _usuarioRepository.ObterPorIdAsync(limpeza.OperadorId)
            ?? throw new NotFoundException("Operador não encontrado.");

        return LimpezaFinaMapper.ParaResumoDto(limpeza, onibus, operador);
    }
}
