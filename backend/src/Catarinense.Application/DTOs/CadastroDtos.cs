namespace Catarinense.Application.DTOs;

public record CadastrarUsuarioRequest(string Matricula, string Nome, string Senha, string Perfil, string? Email);
public record UsuarioResumoDto(Guid Id, string Matricula, string Nome, string? Email, string Perfil, bool Ativo);

public record CadastrarOnibusRequest(string Prefixo, string? Placa);
public record OnibusResumoDto(Guid Id, string Prefixo, string? Placa, bool Ativo);





public record CadastrarEtapaPadraoRequest(string Nome, int Ordem, string? Descricao, string? LinkVideo);
public record EtapaPadraoResumoDto(Guid Id, string Nome, string? Descricao, string? LinkVideo, int Ordem, bool Ativo);
public record EditarEtapaPadraoRequest(Guid Id, string Nome, int Ordem, string? Descricao, string? LinkVideo);
