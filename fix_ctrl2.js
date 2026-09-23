const fs = require("fs");
let content = fs.readFileSync("backend/src/Catarinense.API/Controllers/UsuariosController.cs", "utf8");
content = content.replace("[HttpDelete(\"{id:guid}\")]\n    [HttpPut(\"{id:guid}\")]", "[HttpPut(\"{id:guid}\")]");
fs.writeFileSync("backend/src/Catarinense.API/Controllers/UsuariosController.cs", content, "utf8");