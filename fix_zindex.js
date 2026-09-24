const fs = require("fs");
let c = fs.readFileSync('frontend/src/components/EtapaCard.tsx', 'utf8');

c = c.replace(/fixed inset-0 z-50 flex items-end/, 'fixed inset-0 z-[100] flex items-end');

fs.writeFileSync('frontend/src/components/EtapaCard.tsx', c, 'utf8');