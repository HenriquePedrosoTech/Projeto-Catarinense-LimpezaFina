const fs = require("fs");
function fixAten(file) {
  let lines = fs.readFileSync(file, "utf8").split('\n');
  for(let i=0; i<lines.length; i++) {
    if(lines[i].includes('AlertDialog')) {
      lines[i+2] = '        titulo="Atenção"';
    }
  }
  fs.writeFileSync(file, lines.join('\n'), "utf8");
}
fixAten("frontend/src/components/admin/cadastros/EtapasSection.tsx");
fixAten("frontend/src/components/admin/cadastros/OnibusSection.tsx");