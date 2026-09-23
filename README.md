# Catarinense — Controle de Limpeza Fina (Piloto)

## Progresso

- [x] Parte 1 — Estrutura do projeto + Domain layer
- [x] Parte 2 — Application layer
- [x] Parte 3 — Infrastructure layer
- [x] Parte 4 — API layer
- [x] **Parte 5 — Frontend (Next.js + Tailwind)** (este pacote)

## Parte 1 — O que foi criado

### `backend/src/Catarinense.Domain`

Núcleo do sistema. Não depende de nenhuma outra camada nem de bibliotecas externas (EF Core, etc.) — só C# puro. Contém:

**Entidades** (`Entities/`)
- `Usuario` — operador ou administrador (matrícula, perfil)
- `Onibus` — identificado pelo prefixo
- `EtapaPadrao` — catálogo fixo de etapas do checklist (ex.: Bancos, Piso, Teto...)
- `LimpezaFina` — **aggregate root**. Concentra as regras de negócio: cria as etapas obrigatórias ao ser iniciada, exige foto em cada etapa antes de permitir "Finalizar", só pode ser aprovada/reprovada depois de concluída, só pode ser notificada por e-mail depois de aprovada.
- `LimpezaEtapaExecucao` — execução de uma etapa dentro de um registro
- `FotoEtapa` — evidência fotográfica de uma etapa

**Enums** (`Enums/`)
- `PerfilUsuario` (Operador, Administrador)
- `StatusLimpeza` (EmAndamento, Concluida, Aprovada, Reprovada)

**Interfaces / contratos** (`Interfaces/`) — seguindo o **Dependency Inversion Principle**: o Domain define o que precisa, e é a Infrastructure (próxima parte) que vai implementar usando EF Core/MySQL.
- `IUsuarioRepository`, `IOnibusRepository`, `IEtapaPadraoRepository`, `ILimpezaFinaRepository`

**Exceções** (`Exceptions/`)
- `DomainException` — lançada quando uma regra de negócio é violada (ex.: tentar finalizar uma limpeza com etapas sem foto)

### Por que as regras já estão na entidade `LimpezaFina`?

Isso evita o "modelo anêmico" (entidades que são só um saco de propriedades, com toda a lógica solta em serviços). Colocando as regras na própria entidade:
- Fica impossível, por exemplo, criar uma limpeza aprovada sem antes passar por "Concluída".
- Fica impossível finalizar um registro com etapa sem foto — a regra é garantida em qualquer lugar do código que use essa entidade, não só na camada de API.

## Como abrir

Este pacote não inclui os binários do .NET SDK (ambiente não tinha acesso). Para rodar:

```bash
cd backend
dotnet build Catarinense.sln
```

(vai precisar do .NET 9 SDK instalado localmente)

## Parte 2 — O que foi criado

### `backend/src/Catarinense.Application`

Orquestra os casos de uso do sistema. Depende **só** do Domain (nunca de EF Core, ASP.NET Core, JWT, etc. diretamente) — tudo isso é abstraído por interfaces que a Infrastructure vai implementar na Parte 3.

**Interfaces / portas** (`Interfaces/`)
- `IPasswordHasher` — hash/verificação de senha (Infra vai implementar com BCrypt)
- `IJwtTokenGenerator` — geração do token de autenticação
- `IEmailService` + `EmailMensagem` — envio de e-mail (Infra vai implementar com SMTP)
- `IArmazenamentoArquivoService` — upload/armazenamento das fotos (Infra decide se é disco local, S3, Azure Blob, etc.)
- `UseCases/` — uma interface por caso de uso (Interface Segregation: cada consumidor depende só do que precisa)

**Casos de uso** (`UseCases/`) — um por ação do sistema, cada um com responsabilidade única (SRP):
- `AutenticarUsuarioUseCase` — login por matrícula + senha, retorna token
- `IniciarLimpezaFinaUseCase` — abre o registro, já criando as etapas do checklist pendentes
- `RegistrarFotoEtapaUseCase` — sobe a foto e marca a etapa como concluída
- `FinalizarLimpezaFinaUseCase` — fecha o registro (regra "todas etapas com foto" é validada na entidade)
- `AprovarLimpezaFinaUseCase` / `ReprovarLimpezaFinaUseCase` — só administrador pode avaliar
- `EnviarNotificacaoLimpezaUseCase` — **dispara o e-mail** avisando prefixo, O.S. e link com as fotos de cada etapa; só permite notificar limpeza já aprovada
- `ConsultarLimpezaFinaUseCase` — histórico por ônibus, por operador, por status, e detalhes de um registro (é essa consulta que alimenta a página pública do link enviado por e-mail)

**DTOs** (`DTOs/`) — objetos de entrada/saída dos casos de uso, isolando a Application do formato exato usado na API (JSON) e no Domain.

**Common** (`Common/`)
- `LimpezaFinaMapper` — converte entidade → DTO
- `DetalhesDtoBuilder` — monta o DTO completo (ônibus + operador + nomes das etapas) reaproveitado por vários casos de uso, evitando duplicação

## Parte 3 — O que foi criado

### `backend/src/Catarinense.Infrastructure`

Único projeto autorizado a depender de bibliotecas externas de infraestrutura (EF Core, MySQL, JWT, SMTP). Implementa tudo que Domain/Application definiram como interface.

**`Data/`**
- `AppDbContext` — DbContext do EF Core, com um `DbSet` por entidade
- `Configurations/` — mapeamento Fluent API de cada entidade (nomes de tabela em `snake_case`, tamanhos de coluna, índices). As coleções privadas (`_etapas`, `_fotos`) são mapeadas via *backing field*, então a entidade de domínio continua encapsulada (sem setter público de lista) mesmo com o EF Core gravando nela.

**`Repositories/`**
- `RepositorioBase<T>` — implementação genérica do `IRepositorioBase<T>`
- `UsuarioRepository`, `OnibusRepository`, `EtapaPadraoRepository`, `LimpezaFinaRepository` — cada um implementa a interface correspondente do Domain. `LimpezaFinaRepository` sempre traz etapas + fotos via `Include`, porque as regras de negócio da entidade dependem dessas coleções estarem carregadas.

**`Services/`**
- `PasswordHasher` (BCrypt)
- `JwtTokenGenerator` (System.IdentityModel.Tokens.Jwt)
- `EmailService` (SMTP nativo do .NET — é aqui que o e-mail de "limpeza fina concluída" realmente sai)
- `ArmazenamentoArquivoLocalService` — salva as fotos em disco (pasta configurável); trocar para S3/Azure Blob no futuro não afeta nenhum caso de uso, só essa classe

**`Options/`** — classes fortemente tipadas para `appsettings.json` (`JwtOptions`, `SmtpOptions`, `ArmazenamentoOptions`)

**`DependencyInjection/InfrastructureServiceCollectionExtensions.cs`** — um único método `AddInfrastructure(configuration)` que registra DbContext, repositórios, serviços e casos de uso. A API (Parte 4) só vai chamar esse método no `Program.cs`.

### `backend/appsettings.Example.json`
Modelo de configuração (connection string MySQL, JWT, SMTP, pasta de fotos). Copie para `Catarinense.API/appsettings.json` na Parte 4 e preencha com os dados reais.

### Como gerar o banco (depois de ter o .NET 9 SDK e MySQL rodando)

```bash
cd backend
dotnet tool install --global dotnet-ef   # se ainda não tiver
dotnet ef migrations add InitialCreate --project src/Catarinense.Infrastructure --startup-project src/Catarinense.API
dotnet ef database update --project src/Catarinense.Infrastructure --startup-project src/Catarinense.API
```
(Esses comandos só vão funcionar depois da Parte 4, quando o projeto `Catarinense.API` existir com o `Program.cs` configurado.)

## Parte 4 — O que foi criado

### `backend/src/Catarinense.API`

Camada de apresentação HTTP. Só orquestra: recebe a requisição, chama o caso de uso da Application, devolve a resposta. Nenhuma regra de negócio mora aqui.

**Controllers/**
- `AuthController` — `POST /api/auth/login`
- `UsuariosController` — `POST/GET /api/usuarios` (cadastro/listagem, só Administrador)
- `OnibusController` — `POST/GET /api/onibus`, `GET /api/onibus/{prefixo}/limpezas` (histórico)
- `EtapasPadraoController` — `POST/GET /api/etapas-padrao` (checklist configurável pelo admin)
- `LimpezasFinasController` — o coração do sistema:
  - `POST /api/limpezas-finas` → operador inicia o registro (prefixo + O.S.)
  - `POST /api/limpezas-finas/{id}/etapas/{etapaId}/foto` → upload da foto de cada etapa (multipart/form-data)
  - `POST /api/limpezas-finas/{id}/finalizar` → fecha o registro
  - `POST /api/limpezas-finas/{id}/aprovar` e `/reprovar` → só Administrador
  - `POST /api/limpezas-finas/{id}/notificar` → **o botão que você pediu**: dispara o e-mail com prefixo, O.S. e link
  - `GET /api/limpezas-finas/{id}` → detalhes com fotos de cada etapa (é a página que abre a partir do link do e-mail; liberada sem login porque o id é um GUID não sequencial — ok pro piloto, mas dá pra exigir login também se preferir mais segurança)
  - `GET /api/limpezas-finas?operadorId=...` ou `?status=...` → histórico/filtros

**Middleware/**
- `ExceptionHandlingMiddleware` — converte `DomainException`→400, `NotFoundException`→404, `NaoAutorizadoException`→403, qualquer outra coisa→500 (com log). Os controllers não precisam de try/catch.

**`Program.cs`**
- Chama `AddInfrastructure()` (registra tudo: DbContext, repositórios, serviços, casos de uso)
- Autenticação JWT (Bearer)
- CORS liberado para a origem do frontend Next.js (`Frontend:OrigensPermitidas` no appsettings)
- Swagger com suporte a Bearer token (facilita testar tudo antes do frontend existir)
- `UseStaticFiles()` para servir as fotos salvas em `wwwroot/uploads`

**`appsettings.json`** — já com placeholders para connection string MySQL, JWT, SMTP e a URL base do frontend usada no link do e-mail.

### Como rodar localmente (depois de configurar o `appsettings.json` com dados reais)

```bash
cd backend
dotnet ef migrations add InitialCreate --project src/Catarinense.Infrastructure --startup-project src/Catarinense.API
dotnet ef database update --project src/Catarinense.Infrastructure --startup-project src/Catarinense.API
dotnet run --project src/Catarinense.API
```
Depois abre `http://localhost:5080/swagger` pra testar os endpoints.

⚠️ Antes de usar de verdade: cadastre pelo menos 1 usuário Administrador direto no banco (ou crie um endpoint de seed) pra conseguir logar a primeira vez, já que `POST /api/usuarios` exige um Administrador autenticado.

## Parte 5 — O que foi criado

### `frontend/`

Next.js 15 (App Router) + React 19 + Tailwind. Build de produção testado neste ambiente (`npm run build`) — compilou sem erros de TypeScript nas 10 rotas.

Ver `frontend/README.md` para detalhes completos de estrutura, fluxo e como trocar o wordmark SVG pela logo oficial da Catarinense/Grupo JCA.

**Resumo do fluxo:**
- `/login` — matrícula + senha
- `/operador` → `/operador/nova` → `/operador/limpeza/[id]` — inicia e executa o checklist, com foto por etapa (usa a câmera do celular)
- `/admin` → `/admin/limpeza/[id]` — aprova/reprova e dispara o **botão de notificação por e-mail**
- `/admin/cadastros` — CRUD de ônibus, etapas do checklist e usuários
- `/limpezas/[id]` — **página pública**, sem login, é o destino do link enviado no e-mail

### Banco de dados: TiDB Cloud (Serverless)

Como combinado, a connection string em `appsettings.json`/`appsettings.Example.json` já está no formato do TiDB Cloud (`SslMode=Required`, porta 4000). É compatível com o Pomelo.EntityFrameworkCore.MySql sem mudar nenhum código — só preencher host/usuário/senha reais depois de criar o cluster gratuito.

## Importação em lote de ônibus (planilha CSV ou Excel)

Cadastrar a frota inteira um por um pela tela seria chato — dá pra importar via planilha:

- **Pela tela**: Cadastros → Ônibus → "Escolher planilha (.xlsx ou .csv)"
- **Direto pela API**: `POST /api/onibus/importar` (multipart/form-data, campo `arquivo`, só Administrador)

O sistema identifica a coluna do prefixo **pelo nome do cabeçalho** (reconhece variações como "Prefixo", "Veículo", "Ônibus", "Número", "Código", "Frota") e a da placa (coluna "Placa"), **não importa a ordem nem quantas outras colunas existam** na planilha. Veja os dois exemplos incluídos:

- `backend/exemplo-importacao-onibus.xlsx` — Excel real, com colunas de propósito fora de ordem e colunas extras (Garagem, Ano, Modelo, Observação) que o sistema ignora
- `backend/exemplo-importacao-onibus.csv` — CSV simples de duas colunas

Se nenhum cabeçalho reconhecido for encontrado, o sistema assume o formato antigo (1ª coluna = prefixo, 2ª coluna = placa, sem cabeçalho) — então planilhas simples continuam funcionando.

Comportamento em qualquer um dos dois formatos:
- Prefixos que já existem no banco são só relatados (não dá erro, não duplica)
- Se uma linha tiver problema, só aquela linha entra na lista de erros — o resto da planilha continua sendo processado
- A resposta mostra quantos foram criados, quantos já existiam e o detalhe de cada erro

## Etapas padrão do checklist — já vêm cadastradas

O processo de limpeza fina é sempre o mesmo, então o operador não precisa esperar o administrador configurar nada na primeira vez: ao subir a API pela primeira vez (tabela de etapas vazia), o sistema já cadastra automaticamente:

1. RETIRADA DAS CORTINAS
2. LIMPEZA DO BANHEIRO
3. LIMPEZA DA GELADEIRA
4. LIMPEZA INTERNA
5. LIMPEZA DA CABINE DO MOTORISTA
6. LIMPEZA EXTERNA DA FROTA
7. MÁQUINA DE OZÔNIO

Isso roda em `SeedEtapasPadraoIniciais` (chamado no `Program.cs`, logo após o seed do administrador) e só faz alguma coisa se a tabela estiver **totalmente vazia** — se você já cadastrou/editou etapas manualmente, ele não mexe em nada. O botão **Cadastros → Etapas do checklist** continua disponível pro administrador adicionar mais etapas ou, no futuro, ajustar pra uma filial com processo diferente.

## Fluxo da O.S. (sem integração com o Protheus)

A O.S. de limpeza fina é aberta no Protheus, e por enquanto não existe integração automática entre os dois sistemas. Por isso o fluxo é:

1. **Operador** inicia o registro informando **só o prefixo do ônibus** (sem O.S.)
2. Executa o checklist normalmente (foto por etapa) e finaliza
3. **Administrador**, ao revisar o registro concluído, consulta o número da O.S. no Protheus e preenche no campo "Número da O.S." da tela de avaliação (`POST /api/limpezas-finas/{id}/os`)
4. **Não é possível aprovar sem a O.S. preenchida** — essa regra está no Domain (`LimpezaFina.Aprovar()`), não só na tela, então nenhum outro ponto do sistema consegue burlar isso
5. Depois de aprovado e notificado por e-mail, a O.S. não pode mais ser alterada (trava também no Domain)

Se no futuro surgir uma forma de integrar com o Protheus (API REST exposta pelo time de TI/TOTVS, ou uma "Generic Query"), dá pra automatizar essa etapa 3 sem mexer no resto do sistema — bastaria um novo serviço que chama o Protheus e preenche a O.S. automaticamente, no lugar do administrador digitar manualmente.

## Segurança

Resumo do que está implementado e por quê:

| Proteção | O que faz |
|---|---|
| Senha com BCrypt | Senha nunca é armazenada nem comparada em texto puro |
| **Bloqueio de conta** | Após 5 senhas erradas seguidas para a mesma matrícula, a conta fica bloqueada por 15 minutos (`Usuario.RegistrarTentativaFalha()`) |
| **Rate limiting no login** | Máximo 10 tentativas de login por minuto por IP (`AddFixedWindowLimiter` no `Program.cs`) — trava tentativas de força bruta testando várias matrículas rapidamente |
| Mensagem de erro genérica | Login errado sempre responde "Matrícula ou senha inválida", nunca diz qual dos dois está errado (evita confirmar quais matrículas existem) |
| **Validação de conteúdo do arquivo** | Upload de foto verifica a assinatura binária real do arquivo (magic bytes), não só a extensão do nome — barra arquivo malicioso disfarçado de `.jpg` |
| Senha mínima | Cadastro de usuário exige 8+ caracteres |
| JWT + autorização por perfil | Toda rota sensível exige token válido; rotas de admin exigem `[Authorize(Roles = "Administrador")]` |
| **Cabeçalhos de segurança HTTP** | `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` em toda resposta; HSTS habilitado fora do ambiente de desenvolvimento |
| CORS restrito | Só a origem configurada em `Frontend:OrigensPermitidas` pode chamar a API |
| TLS no banco | Conexão com o TiDB exige SSL |
| **Segredos fora do Git** | `appsettings.json` real (com senha/chave JWT) está no `.gitignore` — só o `appsettings.Example.json` (com placeholders) é versionado |

⚠️ **Ação necessária**: a entidade `Usuario` ganhou dois campos novos (`TentativasFalhasLogin`, `BloqueadoAte`). Se você já rodou a migration inicial, gere uma nova:
```bash
dotnet ef migrations add SegurancaBloqueioLogin --project src/Catarinense.Infrastructure --startup-project src/Catarinense.API
dotnet ef database update --project src/Catarinense.Infrastructure --startup-project src/Catarinense.API
```

### O que ainda vale considerar (não crítico pro piloto)
- Rate limiting geral na API (hoje só o login tem)
- Log de auditoria de login/acesso
- Expiração de token mais curta (hoje 8h) + mecanismo de refresh
- Verificação antivírus nas fotos enviadas
- Trocar o JWT em texto no `localStorage` do navegador por cookie `httpOnly` (mais resistente a um eventual XSS)

## Como criar o primeiro usuário (Administrador)

Como `POST /api/usuarios` exige estar logado como Administrador, e ainda não existe nenhum na primeira vez, a própria API resolve isso sozinha na inicialização, via a seção `SeedAdmin` do `appsettings.json`:

```json
"SeedAdmin": {
  "Habilitado": true,
  "Matricula": "00001",
  "Nome": "Administrador Catarinense",
  "Senha": "TrocarDepois123!"
}
```

Ao rodar `dotnet run --project src/Catarinense.API` (depois das migrations aplicadas), se não existir usuário com essa matrícula, ele é criado automaticamente com a senha já em BCrypt — sem precisar de script externo nem tocar direto no banco.

Depois de logar pela primeira vez com essa matrícula/senha:
1. Troque a senha (ainda não tem endpoint de "trocar senha" — posso criar se quiser)
2. Coloque `"SeedAdmin": { "Habilitado": false }` no appsettings, pra não deixar uma senha em texto puro configurada
3. Daí em diante, use a tela **Cadastros → Usuários** no frontend pra criar os demais operadores/admins

Com isso, as 5 partes do escopo estão entregues: Domain, Application, Infrastructure, API e Frontend. Próximos passos ficam a critério de vocês — sugestões:
- Rodar as migrations num banco TiDB Cloud de teste e validar o fluxo ponta a ponta
- Trocar o wordmark pela logo oficial
- Definir a lista real de etapas do checklist com o time operacional da Catarinense (ponto que já estava em aberto no documento de escopo)
