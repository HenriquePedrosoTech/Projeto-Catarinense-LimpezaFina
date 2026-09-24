const fs = require("fs");
let c = fs.readFileSync('frontend/src/components/EtapaCard.tsx', 'utf8');

c = c.replace(/onClick=\{\(\) => \{ inputCameraRef.current\?\.click\(\); \}\}/, 'onClick={() => { inputCameraRef.current?.click(); setMostrarOpcoesFoto(false); }}');
c = c.replace(/onClick=\{\(\) => \{ inputGaleriaRef.current\?\.click\(\); \}\}/, 'onClick={() => { inputGaleriaRef.current?.click(); setMostrarOpcoesFoto(false); }}');

fs.writeFileSync('frontend/src/components/EtapaCard.tsx', c, 'utf8');