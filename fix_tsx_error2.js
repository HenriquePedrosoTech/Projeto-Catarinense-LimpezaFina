const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

let lines = content.split('\n');

for(let i=0; i<lines.length; i++) {
  if(lines[i].includes('ConfirmDialog')) {
    lines[i+1] = '        aberto={confirmDelete.aberto}';
    lines[i+2] = '        titulo="Excluir Usuário"';
    lines[i+3] = '        mensagem={`Tem certeza que deseja excluir o usuario "${confirmDelete.nomeUsuario}"?`}';
    lines[i+4] = '        tipo="danger"';
    lines[i+5] = '        textoConfirmar="Excluir"';
    lines[i+6] = '        onClose={() => setConfirmDelete({ ...confirmDelete, aberto: false })}';
    lines[i+7] = '        onConfirmar={executarExclusao}';
    break;
  }
}

for(let i=0; i<lines.length; i++) {
  if(lines[i].includes('AlertDialog')) {
    lines[i+1] = '        aberto={alertInfo.aberto}';
    lines[i+2] = '        titulo="Atenção"';
    lines[i+3] = '        mensagem={alertInfo.mensagem}';
    lines[i+4] = '        tipo="danger"';
    lines[i+5] = '        onClose={() => setAlertInfo({ ...alertInfo, aberto: false })}';
    break;
  }
}

content = lines.join('\n');
fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");