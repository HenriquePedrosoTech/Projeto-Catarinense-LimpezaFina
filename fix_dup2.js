const fs = require("fs");
let content = fs.readFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", "utf8");

let firstIndex = content.indexOf("function handleEditSuccess");
if(firstIndex > -1) {
    let secondIndex = content.indexOf("function handleEditSuccess", firstIndex + 1);
    if(secondIndex > -1) {
        let endIndex = content.indexOf("}", secondIndex) + 1;
        content = content.substring(0, secondIndex) + content.substring(endIndex);
    }
}
fs.writeFileSync("frontend/src/components/admin/cadastros/UsuariosSection.tsx", content, "utf8");