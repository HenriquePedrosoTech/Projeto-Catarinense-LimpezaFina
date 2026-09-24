using System.Threading.Channels;
using Catarinense.Application.Interfaces;

namespace Catarinense.Infrastructure.Services;

public interface IEmailQueueService
{
    void EnfileirarEmail(EmailMensagem mensagem);
    IAsyncEnumerable<EmailMensagem> ObterEmailsAsync(CancellationToken cancellationToken);
}

public class EmailQueueService : IEmailQueueService
{
    private readonly Channel<EmailMensagem> _queue;

    public EmailQueueService()
    {
        _queue = Channel.CreateUnbounded<EmailMensagem>();
    }

    public void EnfileirarEmail(EmailMensagem mensagem)
    {
        _queue.Writer.TryWrite(mensagem);
    }

    public IAsyncEnumerable<EmailMensagem> ObterEmailsAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAllAsync(cancellationToken);
    }
}