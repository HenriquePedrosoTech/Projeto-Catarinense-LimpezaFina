using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Catarinense.Application.Interfaces;
using Catarinense.Domain.Entities;
using Catarinense.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Catarinense.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _opcoes;

    public JwtTokenGenerator(IOptions<JwtOptions> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public string GerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim("matricula", usuario.Matricula),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.Chave));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opcoes.Emissor,
            audience: _opcoes.Audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_opcoes.ExpiracaoHoras),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
