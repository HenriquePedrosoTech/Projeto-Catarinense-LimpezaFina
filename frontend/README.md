# Catarinense · Limpeza Fina — Frontend

Next.js 15 (App Router) + React 19 + Tailwind CSS.

## Como rodar

```bash
cd frontend
cp .env.local.example .env.local   # ajuste NEXT_PUBLIC_API_URL se a API não estiver em localhost:5080
npm install
npm run dev
```

Acesse `http://localhost:3000`.

## Estrutura

```
src/
├── app/
│   ├── login/                     → tela de login
│   ├── operador/
│   │   ├── page.tsx                → "minhas limpezas"
│   │   ├── nova/                   → iniciar limpeza (só o prefixo)
│   │   └── limpeza/[id]/           → checklist com upload de foto por etapa
│   ├── admin/
│   │   ├── page.tsx                → lista de limpezas por status
│   │   ├── limpeza/[id]/           → aprovar/reprovar + botão de notificar por e-mail
│   │   └── cadastros/              → CRUD de ônibus, etapas do checklist e usuários
│   └── limpezas/[id]/              → página PÚBLICA (é o link enviado no e-mail)
├── components/                     → Logo, Header, StatusBadge, EtapaCard
└── lib/
    ├── api.ts                      → cliente HTTP (fetch) de toda a API
    ├── auth.tsx                    → contexto de autenticação (localStorage + JWT)
    └── types.ts                    → tipos espelhando os DTOs do backend
```

## Identidade visual

Usei a paleta da Catarinense (vermelho de frota + navy) em `tailwind.config.ts`. A logo por enquanto é um wordmark em SVG (`src/components/Logo.tsx`) — para usar a logo oficial:

1. Baixe o arquivo em `https://jcaholding.com.br/wp-content/uploads/2021/07/catarinense.png` (ou peça o material de marca oficial pro time de marketing).
2. Salve como `public/logo-catarinense.png`.
3. Em `src/components/Logo.tsx`, troque o bloco do SVG por `<Image src="/logo-catarinense.png" alt="Catarinense" width={172} height={40} priority />` (o comentário no próprio arquivo já mostra o trecho pronto).

## Fluxo resumido

1. **Operador** loga → "Nova limpeza" → informa só o prefixo do ônibus → tira foto de cada etapa do checklist (câmera do celular via `capture="environment"`) → finaliza.
2. **Administrador** loga → vê a lista "Aguardando avaliação" → abre o registro, vê as fotos de cada etapa, consulta a O.S. no Protheus e preenche o número → aprova ou reprova (com motivo) → se aprovar, aparece o botão **"Notificar limpeza concluída"**, que dispara o e-mail com o link desta mesma página pública.
3. Quem recebe o e-mail clica no link e cai em `/limpezas/[id]` — sem precisar de login — e vê o prefixo, a O.S. e as fotos de cada etapa.

## Testar no celular (mesma rede Wi-Fi)

1. Descubra o IP do seu PC na rede (`ipconfig` no Windows, procure "Endereço IPv4")
2. Crie/edite `frontend/.env.local`: `NEXT_PUBLIC_API_URL=http://SEU_IP:5080`
3. No `appsettings.json` da API, adicione `http://SEU_IP:3000` em `Frontend:OrigensPermitidas`
4. No `launchSettings.json` da API, `applicationUrl` já está em `http://0.0.0.0:5080` (aceita conexão de fora do PC)
5. `npm run dev` (já roda em `0.0.0.0:3000` por padrão)
6. No celular, na mesma rede Wi-Fi, acesse `http://SEU_IP:3000`

Depois de abrir pelo Chrome do Android, dá pra usar **"Adicionar à tela inicial"** pra ganhar um ícone e abrir em tela cheia (usa o `manifest.json` + ícones já configurados em `public/`).

## Empacotar como app Android nativo (Capacitor)

Isso gera um `.apk` de verdade que você instala no celular como app nativo (ícone próprio, tela cheia, sem barra de navegador) — diferente do "Adicionar à tela inicial" do navegador.

**Como funciona:** o app Android abre o frontend Next.js dentro de uma WebView em tela cheia, apontando pra uma URL (configurada em `capacitor.config.ts`). Ou seja, o servidor (seu PC durante o teste, ou um servidor de produção depois) precisa estar rodando e acessível — o app não roda o Next.js "dentro" dele.

### Pré-requisitos (instalar uma vez, na sua máquina)
- [Android Studio](https://developer.android.com/studio) (inclui o Android SDK)
- JDK 17+ (o Android Studio já instala um compatível)

### Passo a passo

1. Edite `capacitor.config.ts` e troque `http://SEU_IP_AQUI:3000` pelo IP do seu PC na rede (o mesmo IP da seção "Testar no celular" acima).

2. Instale as dependências e gere o projeto Android nativo:
   ```bash
   npm install
   npm run cap:add
   ```
   Isso cria uma pasta `android/` com um projeto Android Studio completo.

3. Sempre que mudar o `capacitor.config.ts` (ex.: trocar a URL), rode:
   ```bash
   npm run cap:sync
   ```

4. Abra o projeto no Android Studio:
   ```bash
   npm run cap:open
   ```
   (ou abra a pasta `android/` manualmente pelo Android Studio)

5. No Android Studio, conecte o celular via USB (com "Depuração USB" ativada nas Opções de Desenvolvedor do Android) e clique em **Run ▶** — instala e abre o app direto no celular.

6. Pra gerar um `.apk` que você pode mandar por WhatsApp/e-mail e instalar sem estar conectado no PC: **Build → Build Bundle(s) / APK(s) → Build APK(s)**. O arquivo fica em `android/app/build/outputs/apk/debug/app-debug.apk`. Copie pro celular e instale (o Android vai pedir pra liberar "instalar de fontes desconhecidas" — normal fora da Play Store).

### Publicar na Play Store (mais pra frente, opcional)
Exige: ícone/nome finais, política de privacidade, conta de desenvolvedor Google (taxa única), build **assinado** (release, não debug), e a URL do `server.url` apontando pra um domínio HTTPS de verdade (não dá pra publicar apontando pro IP local do seu PC). Quando chegar nessa fase, me avise que eu ajudo a configurar o build de release.

## Observações

- Não há middleware de rota no servidor: a proteção de `/admin` e `/operador` é feita client-side (`useRequireAuth`), redirecionando para `/login` se não autenticado ou para o painel correto se o perfil não bater. Suficiente para o piloto; se quiser proteção também no servidor, dá pra migrar para cookies httpOnly + `middleware.ts` do Next.js depois.
- O upload de foto usa `<input type="file" accept="image/*" capture="environment">`, que no celular abre direto a câmera traseira.
