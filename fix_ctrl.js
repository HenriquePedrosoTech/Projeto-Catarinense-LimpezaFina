const fs = require("fs");
let content = fs.readFileSync("backend/src/Catarinense.API/Controllers/UsuariosController.cs", "utf8");

content = content.replace("private readonly IExcluirUsuarioUseCase _excluirUseCase;", "private readonly IExcluirUsuarioUseCase _excluirUseCase;\n    private readonly IEditarUsuarioUseCase _editarUseCase;");
content = content.replace("IExcluirUsuarioUseCase excluirUseCase)", "IExcluirUsuarioUseCase excluirUseCase, IEditarUsuarioUseCase editarUseCase)");
content = content.replace("_excluirUseCase = excluirUseCase;", "_excluirUseCase = excluirUseCase;\n        _editarUseCase = editarUseCase;");

fs.writeFileSync("backend/src/Catarinense.API/Controllers/UsuariosController.cs", content, "utf8");