using Microsoft.EntityFrameworkCore;
using System.Text;
using Catarinense.API.Middleware;
using Catarinense.API.Startup;
using Catarinense.Infrastructure.DependencyInjection;
using Catarinense.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Infraestrutura (DbContext, repositórios, serviços, casos de uso) ----------
builder.Services.AddInfrastructure(builder.Configuration);

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- CORS (libera o frontend Next.js) ----------
const string CorsPolicyFrontend = "FrontendPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyFrontend, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- Autenticação JWT ----------
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Emissor,
        ValidAudience = jwtOptions.Audiencia,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Chave))
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var matricula = context.Principal?.FindFirst("matricula")?.Value;
            if (string.Equals(matricula?.Trim(), Catarinense.Domain.Entities.Usuario.MatriculaMaster, StringComparison.OrdinalIgnoreCase) &&
                context.Principal?.Identity is System.Security.Claims.ClaimsIdentity identity &&
                !identity.HasClaim(System.Security.Claims.ClaimTypes.Role, "AdminMaster"))
            {
                identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "AdminMaster"));
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ---------- Rate limiting no login (anti força-bruta) ----------
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Catarinense - Limpeza Fina API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Informe: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------- Inicialização de Schema e Seeds em Background (não trava o boot do Kestrel) ----------
_ = Task.Run(async () =>
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Catarinense.Infrastructure.Data.AppDbContext>();
        var sqlComandos = new[]
        {
            "ALTER TABLE etapas_itens_padrao ADD COLUMN Descricao LONGTEXT NULL;",
            "ALTER TABLE limpeza_etapa_execucoes ADD COLUMN ProblemaExtraDescricao LONGTEXT NULL;",
            "ALTER TABLE limpeza_etapa_execucoes ADD COLUMN ProblemaExtraFotoUrl VARCHAR(1000) NULL;",
            "ALTER TABLE limpeza_etapa_execucoes ADD COLUMN ProblemaExtraFotoDuplicadaOriginalPrefixo VARCHAR(100) NULL;",
            "ALTER TABLE etapas_itens_execucao ADD COLUMN FotoDuplicadaOriginalPrefixo VARCHAR(100) NULL;",
            "ALTER TABLE etapas_padrao ADD COLUMN ExigeFoto TINYINT(1) NOT NULL DEFAULT 0;",
            "ALTER TABLE etapas_itens_padrao ADD COLUMN ExigeFoto TINYINT(1) NOT NULL DEFAULT 0;",
            "ALTER TABLE limpezas_finas ADD COLUMN ColaboradoresIds VARCHAR(1000) NULL;",
            "ALTER TABLE limpezas_finas ADD COLUMN ColaboradoresNomes VARCHAR(1000) NULL;"
        };

        foreach (var sql in sqlComandos)
        {
            try { await dbContext.Database.ExecuteSqlRawAsync(sql); } catch { /* coluna já existe */ }
        }

        await SeedAdministradorInicial.ExecutarAsync(app.Services, app.Configuration, app.Logger);
        await SeedEtapasPadraoIniciais.ExecutarAsync(app.Services, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Erro não crítico na verificação inicial de schema/seed.");
    }
});

// ---------- Pipeline ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // HSTS: instrui o navegador a só falar com a API por HTTPS pelos próximos 30 dias.
    app.UseHsts();
}

app.UseHttpsRedirection();

// ---------- Cabeçalhos de segurança HTTP em toda resposta ----------
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";      // impede o navegador de "adivinhar" o tipo de um arquivo
    headers["X-Frame-Options"] = "DENY";                 // impede que a API seja carregada dentro de um <iframe> (clickjacking)
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["X-XSS-Protection"] = "0";                   // header antigo e hoje enganoso; desabilita explicitamente
    await next();
});

app.UseRateLimiter();

// Serve as fotos salvas em wwwroot/uploads (ArmazenamentoArquivoLocalService) como arquivos estáticos.
app.UseStaticFiles();

app.UseCors(CorsPolicyFrontend);

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();
