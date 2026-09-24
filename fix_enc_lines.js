const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

let lines = content.split('\n');
lines[76] = '        mensagem={`Tem certeza que deseja excluir o usuario "${confirmDelete.nomeUsuario}"?`}';
lines[75] = '        titulo="Excluir Usuário"';
lines[85] = '        titulo="Atenção"';

content = lines.join('\n');
fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");

// Also check EtapasSection and OnibusSection
let etapas = fs.readFileSync("frontend/src/components/admin/cadastros/EtapasSection.tsx", "utf8");
etapas = etapas.replace(/titulo="Aten.*"/g, 'titulo="Atenção"');
fs.writeFileSync("frontend/src/components/admin/cadastros/EtapasSection.tsx", etapas, "utf8");

let onibus = fs.readFileSync("frontend/src/components/admin/cadastros/OnibusSection.tsx", "utf8");
onibus = onibus.replace(/titulo="Aten.*"/g, 'titulo="Atenção"');
fs.writeFileSync("frontend/src/components/admin/cadastros/OnibusSection.tsx", onibus, "utf8");
