const fs = require('fs');
const file = 'frontend/src/lib/api.ts';
let content = fs.readFileSync(file, 'utf8');

const search = 'excluirEtapaPadrao: (token: string, etapaId: string) =>\r\n    request<void>(`/api/etapas-padrao/${etapaId}`, { method: "DELETE", token }),';
const replace = search + '\r\n\r\n  reordenarEtapasPadrao: (token: string, etapas: { id: string; ordem: number }[]) =>\r\n    request<void>("/api/etapas-padrao/reordenar", { method: "PUT", token, body: JSON.stringify(etapas) }),';

content = content.replace(search, replace);
fs.writeFileSync(file, content, 'utf8');
