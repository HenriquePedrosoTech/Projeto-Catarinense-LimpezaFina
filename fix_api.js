const fs = require("fs");
let content = fs.readFileSync("frontend/src/lib/api.ts", "utf8");
content = content.replace("excluirUsuario: (token: string, usuarioId: string) =>", "editarUsuario: (token: string, usuarioId: string, dados: any) =>\n    request<UsuarioResumo>(`/api/usuarios/${usuarioId}`, { method: \"PUT\", token, body: JSON.stringify(dados) }),\n\n  excluirUsuario: (token: string, usuarioId: string) =>");
fs.writeFileSync("frontend/src/lib/api.ts", content, "utf8");