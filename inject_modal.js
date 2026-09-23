const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

if (!content.includes("UsuarioEditModal")) {
    content = content.replace("import { Trash2, Shield, User, Info } from \"lucide-react\";", "import { Trash2, Edit2, Shield, User, Info } from \"lucide-react\";\nimport { UsuarioEditModal } from \"./UsuarioEditModal\";");
    content = content.replace("const [confirmDelete, setConfirmDelete]", "const [editandoUsuario, setEditandoUsuario] = useState<UsuarioResumo | null>(null);\n  const [confirmDelete, setConfirmDelete]");
    content = content.replace("function executarExclusao() {", "function handleEditSuccess(usuarioAtualizado: UsuarioResumo) {\n    setLista(lista.map(u => u.id === usuarioAtualizado.id ? usuarioAtualizado : u));\n    setAlertInfo({ aberto: true, mensagem: \"Usuário atualizado com sucesso!\" });\n  }\n\n  function executarExclusao() {");
    
    // add edit button before trash button
    content = content.replace("<button\n                    onClick={() => setConfirmDelete(", "<button\n                    onClick={() => setEditandoUsuario(u)}\n                    className=\"p-2 text-ink/40 hover:text-brand hover:bg-brand/10 rounded-lg transition-colors\"\n                    title=\"Editar Usuário\"\n                  >\n                    <Edit2 className=\"h-4 w-4\" />\n                  </button>\n                  <button\n                    onClick={() => setConfirmDelete(");

    // append modal at bottom
    content = content.replace("</section>", "  <UsuarioEditModal\n        aberto={!!editandoUsuario}\n        usuario={editandoUsuario}\n        token={token}\n        onClose={() => setEditandoUsuario(null)}\n        onSuccess={handleEditSuccess}\n      />\n    </section>");
    
    fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");
}