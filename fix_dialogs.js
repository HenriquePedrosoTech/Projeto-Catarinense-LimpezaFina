const fs = require("fs");
let c = fs.readFileSync('frontend/src/components/admin/cadastros/UsuariosSection.tsx', 'utf8');

c = c.replace(/<ConfirmDialog[\s\S]*?\/>/, `<ConfirmDialog
        aberto={confirmDelete.aberto}
        titulo="Excluir Usuário"
        mensagem={\`Tem certeza que deseja excluir o usuario "\${confirmDelete.nomeUsuario}"?\`}
        tipo="danger"
        textoConfirmar="Excluir"
        onClose={() => setConfirmDelete({ ...confirmDelete, aberto: false })}
        onConfirmar={executarExclusao}
      />`);
      
c = c.replace(/<AlertDialog[\s\S]*?\/>/, `<AlertDialog 
        aberto={alertInfo.aberto}
        titulo="Atenção"
        mensagem={alertInfo.mensagem}
        tipo="danger"
        onClose={() => setAlertInfo({ ...alertInfo, aberto: false })}
      />`);

fs.writeFileSync('frontend/src/components/admin/cadastros/UsuariosSection.tsx', c, 'utf8');