using Catarinense.Application.Interfaces;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Enums;
using Catarinense.Domain.Interfaces;

namespace Catarinense.API.Startup;

/// <summary>
/// Resolve o problema do "primeiro usuário": como criar um usuário exige estar
/// logado como Administrador (POST /api/usuarios), e na primeira execução não
/// existe nenhum, este seed cria um Administrador inicial direto no banco,
/// lendo os dados da seção "SeedAdmin" do appsettings.
///
/// É seguro deixar habilitado: se a matrícula já existir, o seed não faz nada.
/// Ainda assim, a recomendação é colocar "SeedAdmin:Habilitado": false depois
/// de criar o primeiro administrador, pra não deixar uma senha em texto puro
/// esquecida no appsettings.
/// </summary>
public static class SeedAdministradorInicial
{
    public static async Task ExecutarAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        var secao = configuration.GetSection("SeedAdmin");
        if (!secao.GetValue<bool>("Habilitado"))
            return;

        var matricula = secao["Matricula"];
        var senha = secao["Senha"];
        var nome = secao["Nome"] ?? "Administrador";

        if (string.IsNullOrWhiteSpace(matricula) || string.IsNullOrWhiteSpace(senha))
        {
            logger.LogWarning("SeedAdmin está habilitado, mas 'Matricula' ou 'Senha' não foram configurados. Seed ignorado.");
            return;
        }

        using var scope = services.CreateScope();
        var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (await usuarioRepository.ObterPorMatriculaAsync(matricula) is not null)
        {
            logger.LogInformation("SeedAdmin: já existe um usuário com a matrícula '{Matricula}'. Nada a fazer.", matricula);
            return;
        }

        var senhaHash = passwordHasher.Hash(senha);
        var administrador = new Usuario(matricula, nome, senhaHash, PerfilUsuario.Administrador);

        await usuarioRepository.AdicionarAsync(administrador);
        await usuarioRepository.SalvarAlteracoesAsync();

        logger.LogWarning(
            "SeedAdmin: usuário Administrador '{Matricula}' criado automaticamente. " +
            "Troque a senha e depois defina \"SeedAdmin:Habilitado\": false no appsettings.",
            matricula);
    }
}
