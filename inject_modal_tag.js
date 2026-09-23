const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

content = content.replace("    </div>\n  );\n}", "      <UsuarioEditModal\n        aberto={!!editandoUsuario}\n        usuario={editandoUsuario}\n        token={token}\n        onClose={() => setEditandoUsuario(null)}\n        onSuccess={handleEditSuccess}\n      />\n    </div>\n  );\n}");

fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");