const fs = require("fs");
let c = fs.readFileSync('frontend/src/components/Lightbox.tsx', 'utf8');

c = c.replace(/fixed inset-0 z-50 flex items-center/, 'fixed inset-0 z-[110] flex items-center');

fs.writeFileSync('frontend/src/components/Lightbox.tsx', c, 'utf8');