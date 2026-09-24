const fs = require("fs");
let c = fs.readFileSync('frontend/src/components/EtapaCard.tsx', 'utf8');

c = c.replace(/overflow-hidden p-6 animate-in slide-in-from-bottom-10/, 'overflow-hidden p-6 pb-12 sm:pb-6 animate-in slide-in-from-bottom-10');

fs.writeFileSync('frontend/src/components/EtapaCard.tsx', c, 'utf8');