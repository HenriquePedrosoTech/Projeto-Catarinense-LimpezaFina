using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

/// <summary>
/// Dispara o e-mail avisando que a limpeza fina de um ônibus foi executada (e aprovada),
/// com prefixo, O.S. e link para a página que mostra cada etapa com as fotos.
/// Só é permitido notificar limpezas já aprovadas (regra garantida na entidade).
/// </summary>
public class EnviarNotificacaoLimpezaUseCase : IEnviarNotificacaoLimpezaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;

    public EnviarNotificacaoLimpezaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IOnibusRepository onibusRepository,
        IUsuarioRepository usuarioRepository,
        IEmailService emailService)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _onibusRepository = onibusRepository;
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
    }

    public async Task ExecutarAsync(EnviarNotificacaoLimpezaRequest request)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId)
            ?? throw new NotFoundException("Ônibus não encontrado.");

        var operador = await _usuarioRepository.ObterPorIdAsync(limpeza.OperadorId)
            ?? throw new NotFoundException("Operador não encontrado.");

        if (request.DestinatariosEmail.Count == 0)
            throw new Exceptions.NaoAutorizadoException("Informe ao menos um destinatário para a notificação.");

        var link = $"{request.UrlBaseDetalhes.TrimEnd('/')}/{limpeza.Id}";

        var corpo = MontarCorpoHtml(onibus.Prefixo, limpeza.NumeroOS, operador.Nome, link);

        await _emailService.EnviarAsync(new EmailMensagem(
            request.DestinatariosEmail,
            $"Limpeza fina concluída — Ônibus {onibus.Prefixo}",
            corpo
        ));

        // Regra "só pode notificar se estiver Aprovada" é garantida dentro da entidade.
        limpeza.MarcarNotificacaoEnviada();

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();
    }

    private static string MontarCorpoHtml(string prefixo, string numeroOS, string nomeOperador, string link) => $"""
        <p>A limpeza fina do ônibus <strong>{prefixo}</strong> (O.S. {numeroOS}) foi executada e aprovada.</p>
        <p><strong>Responsável:</strong> {nomeOperador}</p>
        <p>Veja as fotos de cada etapa concluída no link abaixo:</p>
        <p><a href="{link}">{link}</a></p>
        """;
}
