const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

let startBtn1 = content.indexOf("<button\n                        onClick={() => setEditandoUsuario(u)}");
let endBtn2 = content.indexOf("</button>", content.indexOf("<Trash2", startBtn1)) + 9;

if (startBtn1 !== -1 && endBtn2 !== -1) {
    let newButtons = `<div className="flex items-center gap-1">
                        <button
                          onClick={() => setEditandoUsuario(u)}
                          className="p-2 text-ink/40 hover:text-brand hover:bg-brand/10 rounded-lg transition-colors"
                          title="Editar"
                        >
                          <Edit2 className="h-4 w-4" />
                        </button>
                        <button
                          onClick={() => setConfirmDelete({ aberto: true, id: u.id, nomeUsuario: u.nome })}
                          disabled={excluindoId === u.id}
                          className="p-2 text-ink/40 hover:text-danger hover:bg-danger/10 rounded-lg transition-colors disabled:opacity-50"
                          title="Excluir"
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      </div>`;
                      
    content = content.substring(0, startBtn1) + newButtons + content.substring(endBtn2);
    fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");
}