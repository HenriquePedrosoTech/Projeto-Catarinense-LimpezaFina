const fs = require("fs");
let content = fs.readFileSync("backend/src/Catarinense.API/Controllers/UsuariosController.cs", "utf8");

content = content.replace("private readonly IExcluirUsuarioUseCase _excluirUsuarioUseCase;", "private readonly IExcluirUsuarioUseCase _excluirUsuarioUseCase;\n    private readonly IEditarUsuarioUseCase _editarUseCase;");
content = content.replace("IExcluirUsuarioUseCase excluirUsuarioUseCase,", "IExcluirUsuarioUseCase excluirUsuarioUseCase, IEditarUsuarioUseCase editarUseCase,");
content = content.replace("_excluirUsuarioUseCase = excluirUsuarioUseCase;", "_excluirUsuarioUseCase = excluirUsuarioUseCase;\n        _editarUseCase = editarUseCase;");

fs.writeFileSync("backend/src/Catarinense.API/Controllers/UsuariosController.cs", content, "utf8");