const fs = require("fs");
let c = fs.readFileSync('backend/src/Catarinense.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs', 'utf8');

c = c.replace('services.AddScoped<IEmailService, EmailService>();', `
        var resendApiKey = configuration["Resend:ApiKey"];
        if (!string.IsNullOrWhiteSpace(resendApiKey))
        {
            services.AddHttpClient<IEmailService, ResendEmailService>();
        }
        else
        {
            services.AddScoped<IEmailService, EmailService>();
        }
`);

fs.writeFileSync('backend/src/Catarinense.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs', c, 'utf8');