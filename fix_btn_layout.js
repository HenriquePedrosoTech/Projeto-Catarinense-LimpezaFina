const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

let oldButtons = `<button
                        onClick={() => setEditandoUsuario(u)}
                        className="p-1 text-ink/40 hover:text-brand hover:bg-brand/10 rounded transition-colors mr-2"
                        title="Editar Usuário"
                      >
                        <Edit2 className="h-4 w-4" />
                      </button>
                      <button
                        onClick={() => handleExcluir(u.id, u.nome)}
                        disabled={excluindoId === u.id}
                        className="text-ink/30 hover:text-danger disabled:opacity-50"
                        title="Excluir"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>`;

let newButtons = `<div className="flex items-center">
                        <button
                          onClick={() => setEditandoUsuario(u)}
                          className="p-2 text-ink/40 hover:text-brand hover:bg-brand/10 rounded-lg transition-colors"
                          title="Editar Usuário"
                        >
                          <Edit2 className="h-4 w-4" />
                        </button>
                        <button
                          onClick={() => handleExcluir(u.id, u.nome)}
                          disabled={excluindoId === u.id}
                          className="p-2 text-ink/40 hover:text-danger hover:bg-danger/10 rounded-lg transition-colors disabled:opacity-50"
                          title="Excluir"
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      </div>`;

content = content.replace(oldButtons, newButtons);
fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");