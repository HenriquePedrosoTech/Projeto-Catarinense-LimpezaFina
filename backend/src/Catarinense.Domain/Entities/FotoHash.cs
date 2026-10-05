using System;

namespace Catarinense.Domain.Entities;

public class FotoHash : EntidadeBase
{
    public string Hash { get; private set; }
    public string Url { get; private set; }
    public string? PrefixoOrigem { get; private set; }

    protected FotoHash() { }

    public FotoHash(string hash, string url, string? prefixoOrigem = null)
    {
        Hash = hash;
        Url = url;
        PrefixoOrigem = prefixoOrigem;
    }
}
