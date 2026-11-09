using System;
using System.Threading.Tasks;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class EditarUsuarioUseCase : IEditarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public EditarUsuarioUseCase(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioResumoDto> ExecutarAsync(EditarUsuarioRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(request.Id)
            ?? throw new NotFoundException("Usuário não encontrado.");

        if (!string.IsNullOrWhiteSpace(request.Matricula) &&
            !string.Equals(request.Matricula.Trim(), usuario.Matricula, StringComparison.OrdinalIgnoreCase))
        {
            var existente = await _usuarioRepository.ObterPorMatriculaAsync(request.Matricula.Trim());
            if (existente != null && existente.Id != usuario.Id)
            {
                throw new DomainException($"Já existe outro usuário cadastrado com o login/matrícula '{request.Matricula.Trim()}'.");
            }
        }

        string? hash = null;
        if (!string.IsNullOrWhiteSpace(request.Senha))
        {
            hash = _passwordHasher.Hash(request.Senha);
        }

        usuario.Atualizar(request.Nome, request.Email, hash, request.Matricula, request.LoginAcesso);

        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return new UsuarioResumoDto(
            usuario.Id,
            usuario.Matricula,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil.ToString(),
            usuario.Ativo,
            usuario.EhMaster(),
            usuario.LoginAcesso
        );
    }
}
