using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;
using Catarinense.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Catarinense.Application.UseCases;

public class FinalizarLimpezaFinaUseCase : IFinalizarLimpezaFinaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly IServiceScopeFactory _scopeFactory;

    public FinalizarLimpezaFinaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository, 
        IEtapaPadraoRepository etapaPadraoRepository,
        DetalhesDtoBuilder detalhesDtoBuilder,
        IUsuarioRepository usuarioRepository,
        IOnibusRepository onibusRepository,
        IServiceScopeFactory scopeFactory)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _etapaPadraoRepository = etapaPadraoRepository;
        _detalhesDtoBuilder = detalhesDtoBuilder;
        _usuarioRepository = usuarioRepository;
        _onibusRepository = onibusRepository;
        _scopeFactory = scopeFactory;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(FinalizarLimpezaFinaRequest request)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        var etapas = await _etapaPadraoRepository.ListarAsync();
        var etapaCortina = etapas.FirstOrDefault(e => e.Nome.ToUpper().Contains("CORTINA"));

        limpeza.Finalizar(etapaCortina?.Id);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        // 1. Busca os dados de forma sincrona (para evitar conflito no DbContext)
        var usuarios = await _usuarioRepository.ListarAsync();
        var emailsAdmins = usuarios
            .Where(u => u.Perfil == PerfilUsuario.Administrador && u.Ativo && !string.IsNullOrWhiteSpace(u.Email))
            .Select(u => u.Email!)
            .ToList();

        if (emailsAdmins.Any())
        {
            var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId);
            var operador = await _usuarioRepository.ObterPorIdAsync(limpeza.OperadorId);
            var prefixo = onibus?.Prefixo ?? "Desconhecido";
            var nomeOperador = operador?.Nome ?? "Desconhecido";
            
            var link = string.IsNullOrWhiteSpace(request.UrlBaseDetalhes) 
                ? "" 
                : $"{request.UrlBaseDetalhes.TrimEnd('/')}/{limpeza.Id}";

            var corpoHtml = $@"
                <h2>Limpeza Fina Concluída</h2>
                <p>O operador <strong>{nomeOperador}</strong> finalizou a limpeza fina do ônibus <strong>{prefixo}</strong>.</p>
                <p>Acesse o painel para revisar as fotos e aprovar/reprovar a limpeza.</p>
                {(string.IsNullOrEmpty(link) ? "" : $"<p><a href='{link}'>Clique aqui para ver os detalhes</a></p>")}
            ";

            var msg = new EmailMensagem(
                emailsAdmins,
                $"Aviso: Limpeza Fina Concluída - Ônibus {prefixo}",
                corpoHtml
            );

            // 2. Cria um novo escopo para rodar o serviço em background com segurança
            _ = Task.Run(async () => {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                
                try {
                    await emailService.EnviarAsync(msg);
                    Console.WriteLine("EMAIL ENVIADO COM SUCESSO BACKGROUND!");
                } catch(Exception ex) {
                    Console.WriteLine("ERRO AO ENVIAR EMAIL NO BACKGROUND: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}