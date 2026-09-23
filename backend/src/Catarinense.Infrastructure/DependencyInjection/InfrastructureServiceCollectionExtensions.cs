using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Catarinense.Infrastructure.Options;
using Catarinense.Infrastructure.Repositories;
using Catarinense.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catarinense.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registra DbContext (MySQL), repositórios e serviços de infraestrutura.
    /// Chamado uma única vez no Program.cs da API: services.AddInfrastructure(Configuration).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Connection string 'MySql' não configurada em appsettings.json.");

        // AutoDetect funciona com TiDB Cloud (compatível com o protocolo MySQL 5.7/8.0).
        // Se o auto-detect falhar em algum ambiente, troque por um ServerVersion fixo, ex.:
        // new MySqlServerVersion(new Version(8, 0, 11))
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.Configure<ArmazenamentoOptions>(configuration.GetSection(ArmazenamentoOptions.SectionName));

        // Repositórios (Domain define a interface, Infrastructure implementa — DIP)
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IOnibusRepository, OnibusRepository>();
        services.AddScoped<IEtapaPadraoRepository, EtapaPadraoRepository>();
        services.AddScoped<ILimpezaFinaRepository, LimpezaFinaRepository>();

        // Serviços de infraestrutura
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IArmazenamentoArquivoService, ArmazenamentoArquivoLocalService>();

        // Casos de uso (Application)
        services.AddScoped<IAutenticarUsuarioUseCase, Catarinense.Application.UseCases.AutenticarUsuarioUseCase>();
        services.AddScoped<IIniciarLimpezaFinaUseCase, Catarinense.Application.UseCases.IniciarLimpezaFinaUseCase>();
        services.AddScoped<IDefinirNumeroOSUseCase, Catarinense.Application.UseCases.DefinirNumeroOSUseCase>();
        services.AddScoped<IRegistrarFotoEtapaUseCase, Catarinense.Application.UseCases.RegistrarFotoEtapaUseCase>();
        services.AddScoped<IRemoverFotoEtapaUseCase, Catarinense.Application.UseCases.RemoverFotoEtapaUseCase>();
        services.AddScoped<Catarinense.Application.UseCases.ISinalizarCortinasUseCase, Catarinense.Application.UseCases.SinalizarCortinasUseCase>();
        services.AddScoped<IFinalizarLimpezaFinaUseCase, Catarinense.Application.UseCases.FinalizarLimpezaFinaUseCase>();
        services.AddScoped<IAprovarLimpezaFinaUseCase, Catarinense.Application.UseCases.AprovarLimpezaFinaUseCase>();
        services.AddScoped<IReprovarLimpezaFinaUseCase, Catarinense.Application.UseCases.ReprovarLimpezaFinaUseCase>();
        services.AddScoped<IEnviarNotificacaoLimpezaUseCase, Catarinense.Application.UseCases.EnviarNotificacaoLimpezaUseCase>();
        services.AddScoped<IConsultarLimpezaFinaUseCase, Catarinense.Application.UseCases.ConsultarLimpezaFinaUseCase>();
        services.AddScoped<IExcluirLimpezaFinaUseCase, Catarinense.Application.UseCases.ExcluirLimpezaFinaUseCase>();
        services.AddScoped<Catarinense.Application.Common.DetalhesDtoBuilder>();

        // Cadastro / consulta de dados mestres (usuários, ônibus, etapas padrão)
        services.AddScoped<ICadastrarUsuarioUseCase, Catarinense.Application.UseCases.CadastrarUsuarioUseCase>();
        services.AddScoped<IEditarUsuarioUseCase, Catarinense.Application.UseCases.EditarUsuarioUseCase>();
        services.AddScoped<IExcluirUsuarioUseCase, Catarinense.Application.UseCases.ExcluirUsuarioUseCase>();
        services.AddScoped<ICadastrarOnibusUseCase, Catarinense.Application.UseCases.CadastrarOnibusUseCase>();
        services.AddScoped<IExcluirOnibusUseCase, Catarinense.Application.UseCases.ExcluirOnibusUseCase>();
        services.AddScoped<IImportarOnibusEmLoteUseCase, Catarinense.Application.UseCases.ImportarOnibusEmLoteUseCase>();
        services.AddScoped<ICadastrarEtapaPadraoUseCase, Catarinense.Application.UseCases.CadastrarEtapaPadraoUseCase>();
        services.AddScoped<Catarinense.Application.Interfaces.UseCases.IEditarEtapaPadraoUseCase, Catarinense.Application.UseCases.EditarEtapaPadraoUseCase>();
        services.AddScoped<IExcluirEtapaPadraoUseCase, Catarinense.Application.UseCases.ExcluirEtapaPadraoUseCase>();
        services.AddScoped<IConsultarDadosMestresUseCase, Catarinense.Application.UseCases.ConsultarDadosMestresUseCase>();

        return services;
    }
}

