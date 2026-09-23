using Catarinense.Application.DTOs;

namespace Catarinense.Application.Interfaces.UseCases;

public interface ICadastrarUsuarioUseCase
{
    Task<UsuarioResumoDto> ExecutarAsync(CadastrarUsuarioRequest request);
}

public interface ICadastrarOnibusUseCase
{
    Task<OnibusResumoDto> ExecutarAsync(CadastrarOnibusRequest request);
}

public interface IImportarOnibusEmLoteUseCase
{
    /// <summary>
    /// Cadastra vários ônibus de uma vez (ex.: a partir de um CSV enviado pelo admin).
    /// Prefixos já existentes são apenas relatados (não é erro); prefixos inválidos
    /// entram na lista de erros. Nada é abortado por causa de uma linha ruim — o
    /// resto do lote continua sendo processado.
    /// </summary>
    Task<ResultadoImportacaoOnibus> ExecutarAsync(IReadOnlyList<ItemImportacaoOnibus> itens);
}

public interface ICadastrarEtapaPadraoUseCase
{
    Task<EtapaPadraoResumoDto> ExecutarAsync(CadastrarEtapaPadraoRequest request);
}

public interface IConsultarDadosMestresUseCase
{
    Task<IReadOnlyList<UsuarioResumoDto>> ListarUsuariosAsync();
    Task<IReadOnlyList<OnibusResumoDto>> ListarOnibusAsync();
    Task<IReadOnlyList<EtapaPadraoResumoDto>> ListarEtapasPadraoAsync();
}

public interface IExcluirOnibusUseCase
{
    /// <summary>Uso: limpar dado de teste. Bloqueia se existir limpeza fina vinculada a este ônibus.</summary>
    Task ExecutarAsync(Guid onibusId);
}

public interface IExcluirEtapaPadraoUseCase
{
    Task ExecutarAsync(Guid etapaPadraoId);
}

public interface IExcluirUsuarioUseCase
{
    /// <summary>Bloqueia se for o único Administrador ativo, ou se existir limpeza fina vinculada como operador.</summary>
    Task ExecutarAsync(Guid usuarioId);
}
