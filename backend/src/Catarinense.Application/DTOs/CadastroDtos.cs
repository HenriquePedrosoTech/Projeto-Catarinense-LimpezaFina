namespace Catarinense.Application.DTOs;

public record CadastrarUsuarioRequest(string Matricula, string Nome, string Senha, string Perfil, string? Email);
public record UsuarioResumoDto(Guid Id, string Matricula, string Nome, string? Email, string Perfil, bool Ativo);

public record CadastrarOnibusRequest(string Prefixo, string? Placa);
public record OnibusResumoDto(Guid Id, string Prefixo, string? Placa, bool Ativo);





public record CadastrarEtapaPadraoRequest(string Nome, int Ordem, string? Descricao, string? LinkVideo, bool Obrigatoria, IReadOnlyList<EtapaItemPadraoDto>? Itens);
public record EtapaItemPadraoDto(Guid? Id, string Texto, int Ordem, string? Descricao, string? EnquadramentoFoto, bool Obrigatorio);
public record EtapaPadraoResumoDto(Guid Id, string Nome, string? Descricao, string? LinkVideo, int Ordem, bool Ativo, bool Obrigatoria, IReadOnlyList<EtapaItemPadraoDto>? Itens);
public record EditarEtapaPadraoRequest(Guid Id, string Nome, int Ordem, string? Descricao, string? LinkVideo, bool Obrigatoria, IReadOnlyList<EtapaItemPadraoDto>? Itens);
