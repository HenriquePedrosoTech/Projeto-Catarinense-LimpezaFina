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
});

builder.Services.AddAuthorization();

// ---------- Rate limiting no login (anti força-bruta) ----------
// Limita tentativas de login por IP: 10 requisições por minuto, sem fila (a 11ª
// requisição no mesmo minuto recebe 429 imediatamente). Isso é uma camada A MAIS
// do bloqueio por conta que já existe no AutenticarUsuarioUseCase — aqui protege
// contra tentar matrículas diferentes rapidamente; lá protege uma conta específica.
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

// ---------- Seed do primeiro Administrador (só roda se SeedAdmin:Habilitado = true) ----------
await SeedAdministradorInicial.ExecutarAsync(app.Services, app.Configuration, app.Logger);

// ---------- Seed das etapas padrão do checklist (roda sempre que a tabela estiver vazia) ----------
await SeedEtapasPadraoIniciais.ExecutarAsync(app.Services, app.Logger);

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

app.MapControllers();

app.Run();


