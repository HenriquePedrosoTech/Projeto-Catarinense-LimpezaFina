using System;

namespace Catarinense.Domain.Entities;

public class FotoHash : EntidadeBase
{
    public string Hash { get; private set; }
    public string Url { get; private set; }

    protected FotoHash() { }

    public FotoHash(string hash, string url)
    {
        Hash = hash;
        Url = url;
    }
}
