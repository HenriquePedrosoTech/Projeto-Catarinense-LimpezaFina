using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public interface IAlternarConclusaoEtapaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, bool concluida, Guid usuarioLogadoId, bool isAdministrador);
}

public class AlternarConclusaoEtapaUseCase : IAlternarConclusaoEtapaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public AlternarConclusaoEtapaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IEtapaPadraoRepository etapaPadraoRepository,
        IUsuarioRepository usuarioRepository,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _etapaPadraoRepository = etapaPadraoRepository;
        _usuarioRepository = usuarioRepository;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, bool concluida, Guid usuarioLogadoId, bool isAdministrador)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(limpezaFinaId)
            ?? throw new NotFoundException("Limpeza fina nao encontrada.");

        var etapaExecucao = limpeza.Etapas.FirstOrDefault(e => e.EtapaPadraoId == etapaPadraoId)
            ?? throw new NotFoundException("Etapa nao encontrada na limpeza atual.");

        var etapaPadrao = await _etapaPadraoRepository.ObterPorIdAsync(etapaPadraoId);
        bool exigeFoto = etapaPadrao?.ExigeFoto ?? false;

        if (!isAdministrador && usuarioLogadoId != limpeza.OperadorId)
        {
            var colaborador = await _usuarioRepository.ObterPorIdAsync(usuarioLogadoId);
            if (colaborador != null)
            {
                limpeza.AdicionarColaborador(colaborador.Id, colaborador.Nome);
            }
        }

        etapaExecucao.AlternarConclusaoManual(concluida, exigeFoto);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}

