const fs = require("fs");
let c = fs.readFileSync('backend/src/Catarinense.API/Middleware/ExceptionHandlingMiddleware.cs', 'utf8');

c = c.replace('object corpo = _ambiente.IsDevelopment() && status == StatusCodes.Status500InternalServerError', 'object corpo = true // TEMPORARY DEBUG');

fs.writeFileSync('backend/src/Catarinense.API/Middleware/ExceptionHandlingMiddleware.cs', c, 'utf8');