const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

if (!content.includes("UsuarioEditModal")) {
    // Inject Import
    content = content.replace("import { Users, Trash2, ShieldCheck, User as UserIcon } from \"lucide-react\";", "import { Users, Trash2, ShieldCheck, User as UserIcon, Edit2 } from \"lucide-react\";\nimport { UsuarioEditModal } from \"./UsuarioEditModal\";");

    // Inject state
    content = content.replace("const [confirmDelete, setConfirmDelete]", "const [editandoUsuario, setEditandoUsuario] = useState<UsuarioResumo | null>(null);\n  const [confirmDelete, setConfirmDelete]");

    // Inject handleEditSuccess
    content = content.replace("async function executarExclusao() {", "function handleEditSuccess(usuarioAtualizado: UsuarioResumo) {\n    setLista(lista.map(u => u.id === usuarioAtualizado.id ? usuarioAtualizado : u));\n    setAlertInfo({ aberto: true, mensagem: \"Usuário atualizado com sucesso!\" });\n  }\n\n  async function executarExclusao() {");

    // Inject edit button
    const oldButton = `<button
                      onClick={() => setConfirmDelete({ aberto: true, id: u.id, nomeUsuario: u.nome })}
                      disabled={excluindoId === u.id}
                      className="text-ink/30 hover:text-danger disabled:opacity-50"
                      title="Excluir"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>`;
    
    const newButtons = `<button
                      onClick={() => setEditandoUsuario(u)}
                      className="p-1 text-ink/40 hover:text-brand hover:bg-brand/10 rounded transition-colors mr-2"
                      title="Editar Usuário"
                    >
                      <Edit2 className="h-4 w-4" />
                    </button>\n                    ` + oldButton;

    content = content.replace(oldButton, newButtons);

    // Inject modal component at end
    content = content.replace("</section>", "  <UsuarioEditModal\n        aberto={!!editandoUsuario}\n        usuario={editandoUsuario}\n        token={token}\n        onClose={() => setEditandoUsuario(null)}\n        onSuccess={handleEditSuccess}\n      />\n    </section>");

    fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");
}