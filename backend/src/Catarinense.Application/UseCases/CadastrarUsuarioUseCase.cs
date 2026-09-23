using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class CadastrarUsuarioUseCase : ICadastrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CadastrarUsuarioUseCase(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioResumoDto> ExecutarAsync(CadastrarUsuarioRequest request)
    {
        if (await _usuarioRepository.ObterPorMatriculaAsync(request.Matricula) is not null)
            throw new DomainException($"Já existe um usuário cadastrado com a matrícula '{request.Matricula}'.");

        if (!Enum.TryParse<PerfilUsuario>(request.Perfil, ignoreCase: true, out var perfil))
            throw new DomainException($"Perfil '{request.Perfil}' inválido. Use 'Operador' ou 'Administrador'.");

        if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 8)
            throw new DomainException("A senha precisa ter no mínimo 8 caracteres.");

        var senhaHash = _passwordHasher.Hash(request.Senha);
        var usuario = new Usuario(request.Matricula, request.Nome, senhaHash, perfil, request.Email);

        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return new UsuarioResumoDto(usuario.Id, usuario.Matricula, usuario.Nome, usuario.Email, usuario.Perfil.ToString(), usuario.Ativo);
    }
}
