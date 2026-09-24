const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

// Fix encoding
content = content.replace('titulo="Ateno"', 'titulo="Atenção"');
content = content.replace('titulo="Aten\ufffdo"', 'titulo="Atenção"');
content = content.replace('titulo="Excluir Usurio"', 'titulo="Excluir Usuário"');
content = content.replace('titulo="Excluir Usu\ufffdrio"', 'titulo="Excluir Usuário"');
content = content.replace('mensagem: "Usurio atualizado com sucesso!"', 'mensagem: "Usuário atualizado com sucesso!"');
content = content.replace('mensagem: "Usu\ufffdrio atualizado com sucesso!"', 'mensagem: "Usuário atualizado com sucesso!"');

// Fix buttons layout
let btn1 = `                    <button
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

let btn1b = btn1.replace('Usuário', 'Usurio');

let newBtn = `                    <div className="flex items-center gap-1">
                      <button
                        onClick={() => setEditandoUsuario(u)}
                        className="p-2 text-ink/40 hover:text-brand hover:bg-brand/10 rounded-lg transition-colors"
                        title="Editar"
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

content = content.replace(btn1, newBtn).replace(btn1b, newBtn);

fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");