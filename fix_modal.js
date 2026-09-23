const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuarioEditModal.tsx", "utf8");
content = content.replace("const response = await api.put<UsuarioResumo>(`/api/usuarios/${usuario!.id}`, {", "const response = await api.editarUsuario(token, usuario!.id, {");
content = content.replace("senha: senha || null\n      }, {\n        headers: { Authorization: `Bearer ${token}` }\n      });", "senha: senha || null\n      });");
fs.writeFileSync("frontend/src/components/admin/cadastros/UsuarioEditModal.tsx", content, "utf8");