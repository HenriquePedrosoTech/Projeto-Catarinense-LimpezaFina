const fs = require("fs");

function fixFile(path) {
    let content = fs.readFileSync(path, "utf8");
    
    // Fix literal `n
    content = content.replace(/`n/g, "\n");
    
    // Fix literal replacement chars (I'll just replace the whole broken phrases)
    content = content.replace(/registro do ï¿½nibus/g, "registro do ônibus");
    content = content.replace(/registro do nibus/g, "registro do ônibus");
    content = content.replace(/o usuï¿½rio/g, "o usuário");
    content = content.replace(/o usurio/g, "o usuário");
    content = content.replace(/Excluir Usuï¿½rio/g, "Excluir Usuário");
    content = content.replace(/Excluir Usurio/g, "Excluir Usuário");
    content = content.replace(/Atenï¿½ï¿½o/g, "Atenção");
    content = content.replace(/Ateno/g, "Atenção");
    
    fs.writeFileSync(path, content, "utf8");
}

fixFile("frontend/src/app/admin/page.tsx");
fixFile("frontend/src/components/admin/cadastros/UsuariosSection.tsx");
fixFile("frontend/src/components/admin/cadastros/OnibusSection.tsx");
fixFile("frontend/src/components/admin/cadastros/EtapasSection.tsx");

console.log("Done");