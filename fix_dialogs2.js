const fs = require("fs");

function fixFile(file) {
  let c = fs.readFileSync(file, 'utf8');
  c = c.replace(/<AlertDialog[\s\S]*?\/>/, `<AlertDialog 
        aberto={alertInfo.aberto}
        titulo="Atenção"
        mensagem={alertInfo.mensagem}
        tipo="danger"
        onClose={() => setAlertInfo({ ...alertInfo, aberto: false })}
      />`);
  fs.writeFileSync(file, c, 'utf8');
}

fixFile("frontend/src/components/admin/cadastros/EtapasSection.tsx");
fixFile("frontend/src/components/admin/cadastros/OnibusSection.tsx");