using Catarinense.Application.DTOs;

namespace Catarinense.Application.Interfaces.UseCases;

public interface IAutenticarUsuarioUseCase
{
    Task<LoginResponse> ExecutarAsync(LoginRequest request);
}

public interface IIniciarLimpezaFinaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(IniciarLimpezaFinaRequest request);
}

public interface IDefinirNumeroOSUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(DefinirNumeroOSRequest request);
}

public interface IRegistrarFotoEtapaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(RegistrarFotoEtapaRequest request);
}

public interface IRemoverFotoEtapaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(RemoverFotoEtapaRequest request);
}

public interface IFinalizarLimpezaFinaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(FinalizarLimpezaFinaRequest request);
}

public interface IAprovarLimpezaFinaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(AprovarLimpezaFinaRequest request);
}

public interface IReprovarLimpezaFinaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(ReprovarLimpezaFinaRequest request);
}

public interface IEnviarNotificacaoLimpezaUseCase
{
    Task ExecutarAsync(EnviarNotificacaoLimpezaRequest request);
}

public interface IConsultarLimpezaFinaUseCase
{
    Task<LimpezaFinaDetalhesDto> ObterDetalhesAsync(Guid limpezaFinaId);
    Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarPorOnibusAsync(string prefixo);
    Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarPorOperadorAsync(Guid operadorId);
    Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarPorStatusAsync(string status);
    Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarParaRelatorioAsync(DateTime? dataInicio, DateTime? dataFim, string? status, Guid? operadorId);
}

public interface IExcluirLimpezaFinaUseCase
{
    /// <summary>Exclui o registro, suas etapas e as fotos salvas em disco. Uso: limpar dados de teste.</summary>
    Task ExecutarAsync(Guid limpezaFinaId);
}
