using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ConsultarDadosMestresUseCase : IConsultarDadosMestresUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public ConsultarDadosMestresUseCase(
        IUsuarioRepository usuarioRepository,
        IOnibusRepository onibusRepository,
        IEtapaPadraoRepository etapaPadraoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _onibusRepository = onibusRepository;
        _etapaPadraoRepository = etapaPadraoRepository;
    }

    public async Task<IReadOnlyList<UsuarioResumoDto>> ListarUsuariosAsync()
    {
        var usuarios = await _usuarioRepository.ListarAsync();
        return usuarios.Select(u => new UsuarioResumoDto(u.Id, u.Matricula, u.Nome, u.Email, u.Perfil.ToString(), u.Ativo)).ToList();
    }

    public async Task<IReadOnlyList<OnibusResumoDto>> ListarOnibusAsync()
    {
        var onibus = await _onibusRepository.ListarAsync();
        return onibus.Select(o => new OnibusResumoDto(o.Id, o.Prefixo, o.Placa, o.Ativo)).ToList();
    }

    public async Task<IReadOnlyList<EtapaPadraoResumoDto>> ListarEtapasPadraoAsync()
    {
        var etapas = await _etapaPadraoRepository.ListarAsync();
        return etapas.OrderBy(e => e.Ordem).Select(e => new EtapaPadraoResumoDto(e.Id, e.Nome, e.Descricao, e.LinkVideo, e.Ordem, e.Ativo)).ToList();
    }
}
