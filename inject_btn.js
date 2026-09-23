const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

let trashIndex = content.indexOf("<Trash2 className=\"h-4 w-4\" />");
let buttonStart = content.lastIndexOf("<button", trashIndex);

let newBtn = "<button\n                      onClick={() => setEditandoUsuario(u)}\n                      className=\"p-1 text-ink/40 hover:text-brand hover:bg-brand/10 rounded transition-colors mr-2\"\n                      title=\"Editar Usuário\"\n                    >\n                      <Edit2 className=\"h-4 w-4\" />\n                    </button>\n                    ";

let insertPos = buttonStart;
let part1 = content.substring(0, insertPos);
let part2 = content.substring(insertPos);
content = part1 + newBtn + part2;

fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");