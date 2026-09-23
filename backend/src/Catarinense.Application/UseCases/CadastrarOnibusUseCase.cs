using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class CadastrarOnibusUseCase : ICadastrarOnibusUseCase
{
    private readonly IOnibusRepository _onibusRepository;

    public CadastrarOnibusUseCase(IOnibusRepository onibusRepository)
    {
        _onibusRepository = onibusRepository;
    }

    public async Task<OnibusResumoDto> ExecutarAsync(CadastrarOnibusRequest request)
    {
        if (await _onibusRepository.ObterPorPrefixoAsync(request.Prefixo) is not null)
            throw new DomainException($"Já existe um ônibus cadastrado com o prefixo '{request.Prefixo}'.");

        var onibus = new Onibus(request.Prefixo, request.Placa);

        await _onibusRepository.AdicionarAsync(onibus);
        await _onibusRepository.SalvarAlteracoesAsync();

        return new OnibusResumoDto(onibus.Id, onibus.Prefixo, onibus.Placa, onibus.Ativo);
    }
}
