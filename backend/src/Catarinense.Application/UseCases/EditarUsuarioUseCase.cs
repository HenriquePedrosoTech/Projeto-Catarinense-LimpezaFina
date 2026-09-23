using System.Threading.Tasks;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
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

        string? hash = null;
        if (!string.IsNullOrWhiteSpace(request.Senha))
        {
            hash = _passwordHasher.Hash(request.Senha);
        }

        usuario.Atualizar(request.Nome, request.Email, hash);

        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return new UsuarioResumoDto(
            usuario.Id,
            usuario.Matricula,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil.ToString(),
            usuario.Ativo
        );
    }
}