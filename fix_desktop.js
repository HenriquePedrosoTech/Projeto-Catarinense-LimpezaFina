const fs = require("fs");
let c = fs.readFileSync('frontend/src/components/EtapaCard.tsx', 'utf8');

// The main button for uncompleted step:
// <button type="button" onClick={() => setMostrarOpcoesFoto(true)} className="flex h-10 w-10 items-center justify-center rounded-full bg-brand/10 text-brand hover:bg-brand hover:text-white transition-colors">
// The camera button for completed step:
// <button type="button" onClick={() => setMostrarOpcoesFoto(true)} className="flex h-10 w-10 items-center justify-center rounded-full bg-surface text-ink/40 hover:bg-brand/10 hover:text-brand transition-colors">

c = c.replace(/onClick=\{\(\) => setMostrarOpcoesFoto\(true\)\}/g, `onClick={() => {
                    if (window.innerWidth >= 640) {
                      inputGaleriaRef.current?.click();
                    } else {
                      setMostrarOpcoesFoto(true);
                    }
                  }}`);

fs.writeFileSync('frontend/src/components/EtapaCard.tsx', c, 'utf8');