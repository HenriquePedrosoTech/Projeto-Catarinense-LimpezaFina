const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

content = content.replace(
`<ConfirmDialog 
        titulo="Excluir Usuário"
        mensagem={\`Tem certeza que deseja excluir o usuario "\${confirmDelete.nomeUsuario}"?\`}
        mensagem={\`Tem certeza que deseja excluir o usuario "\${confirmDelete.nomeUsuario}"?\`}`,
`<ConfirmDialog 
        aberto={confirmDelete.aberto}
        titulo="Excluir Usuário"
        mensagem={\`Tem certeza que deseja excluir o usuario "\${confirmDelete.nomeUsuario}"?\`}`
);

fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");