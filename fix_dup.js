const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");
content = content.replace("const [editandoUsuario, setEditandoUsuario] = useState<UsuarioResumo | null>(null);\n  const [editandoUsuario, setEditandoUsuario] = useState<UsuarioResumo | null>(null);", "const [editandoUsuario, setEditandoUsuario] = useState<UsuarioResumo | null>(null);");
fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");