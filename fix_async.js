const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");
content = content.replace("function executarExclusao() {", "async function executarExclusao() {");
fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");