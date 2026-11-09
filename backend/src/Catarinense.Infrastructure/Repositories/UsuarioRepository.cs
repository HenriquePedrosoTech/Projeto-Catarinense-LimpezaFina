using System.Globalization;
using System.Text;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Repositories;

public class UsuarioRepository : RepositorioBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext contexto) : base(contexto) { }

    public async Task<Usuario?> ObterPorMatriculaAsync(string matricula) =>
        await DbSet.FirstOrDefaultAsync(u => u.Matricula == matricula.Trim());

    public async Task<IReadOnlyList<Usuario>> BuscarPorMatriculaOuNomeAsync(string identificador)
    {
        if (string.IsNullOrWhiteSpace(identificador))
            return Array.Empty<Usuario>();

        var termo = identificador.Trim();
        var termoNorm = Normalizar(termo);

        // Primeiro tenta busca direta por matrícula exata no banco
        var porMatricula = await DbSet.FirstOrDefaultAsync(u => u.Matricula == termo);
        if (porMatricula != null)
            return new[] { porMatricula };

        // Carrega os usuários para comparar nome ignorando maiúsculas/minúsculas e acentos (ex: "joao" == "João")
        var todos = await DbSet.ToListAsync();

        var candidatos = todos
            .Select(u =>
            {
                var nomeNorm = Normalizar(u.Nome);
                var primeiroNomeNorm = nomeNorm.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";

                int prioridade = 99;
                if (string.Equals(Normalizar(u.Matricula), termoNorm, StringComparison.Ordinal))
                    prioridade = 1;
                else if (string.Equals(nomeNorm, termoNorm, StringComparison.Ordinal))
                    prioridade = 2;
                else if (string.Equals(primeiroNomeNorm, termoNorm, StringComparison.Ordinal))
                    prioridade = 3;
                else if (nomeNorm.StartsWith(termoNorm, StringComparison.Ordinal))
                    prioridade = 4;
                else if (nomeNorm.Contains(termoNorm, StringComparison.Ordinal))
                    prioridade = 5;

                return (Usuario: u, Prioridade: prioridade);
            })
            .Where(x => x.Prioridade < 99)
            .OrderBy(x => x.Prioridade)
            .Select(x => x.Usuario)
            .ToList();

        return candidatos;
    }

    private static string Normalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        var decomposto = texto.Trim().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposto.Length);
        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
