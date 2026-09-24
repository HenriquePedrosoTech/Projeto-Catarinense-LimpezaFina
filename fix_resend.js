const fs = require("fs");
let c = fs.readFileSync('backend/src/Catarinense.Infrastructure/Services/ResendEmailService.cs', 'utf8');

c = c.replace('_remetenteEmail = configuration["Smtp:RemetenteEmail"] ?? "onboarding@resend.dev";', '_remetenteEmail = "onboarding@resend.dev";');

fs.writeFileSync('backend/src/Catarinense.Infrastructure/Services/ResendEmailService.cs', c, 'utf8');