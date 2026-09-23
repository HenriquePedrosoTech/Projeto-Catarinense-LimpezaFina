const fs = require("fs");

function fixStr(file, searchStr, replaceStr) {
    let content = fs.readFileSync(file, "utf8");
    content = content.split(searchStr).join(replaceStr);
    fs.writeFileSync(file, content, "utf8");
}

let f1 = "frontend/src/app/admin/page.tsx";
let c1 = fs.readFileSync(f1, "utf8");
let i1 = c1.indexOf("Tem certeza que deseja excluir o registro do ");
let e1 = c1.indexOf("${confirmDelete.prefixo}", i1);
if(i1 > -1 && e1 > -1) {
    let badStr = c1.substring(i1, e1);
    fixStr(f1, badStr, "Tem certeza que deseja excluir o registro do onibus ");
}

let f2 = "frontend/src/components/admin/cadastros/OnibusSection.tsx";
let c2 = fs.readFileSync(f2, "utf8");
let i2 = c2.indexOf("Tem certeza que deseja excluir o ");
let e2 = c2.indexOf("${confirmDelete.prefixo}", i2);
if(i2 > -1 && e2 > -1) {
    let badStr2 = c2.substring(i2, e2);
    fixStr(f2, badStr2, "Tem certeza que deseja excluir o onibus ");
}
fixStr(f2, "Excluir nibus", "Excluir Onibus");
fixStr(f2, "Ateno", "Atencao");

let f3 = "frontend/src/components/admin/cadastros/UsuariosSection.tsx";
let c3 = fs.readFileSync(f3, "utf8");
let i3 = c3.indexOf("Tem certeza que deseja excluir o usu");
let e3 = c3.indexOf("${confirmDelete.nomeUsuario}", i3);
if(i3 > -1 && e3 > -1) {
    let badStr3 = c3.substring(i3, e3);
    fixStr(f3, badStr3, "Tem certeza que deseja excluir o usuario \"");
}
fixStr(f3, "Excluir Usurio", "Excluir Usuario");
fixStr(f3, "Ateno", "Atencao");

let f4 = "frontend/src/components/admin/cadastros/EtapasSection.tsx";
let c4 = fs.readFileSync(f4, "utf8");
fixStr(f4, "Excluir Etapa Padro", "Excluir Etapa Padrao");
fixStr(f4, "Ateno", "Atencao");