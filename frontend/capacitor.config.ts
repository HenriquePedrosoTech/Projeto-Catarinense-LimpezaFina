import type { CapacitorConfig } from "@capacitor/cli";

/**
 * IMPORTANTE: troque a URL abaixo pelo endereço real onde o frontend Next.js
 * está rodando — hoje, pra teste, o IP do seu PC na rede (ex.: http://192.168.0.15:3000).
 * Mais pra frente, quando o sistema estiver publicado num servidor de verdade,
 * troque pela URL de produção (idealmente https://...).
 *
 * O app Android não roda o Next.js "dentro" dele — ele abre essa URL numa WebView
 * em tela cheia, sem barra de navegador. Por isso o servidor (seu PC, ou o
 * servidor de produção depois) precisa estar acessível e rodando.
 */
const config: CapacitorConfig = {
  appId: "br.com.jcaholding.catarinense.limpezafina",
  appName: "Limpeza Fina",
  webDir: "public", // não é usado de fato quando "server.url" está definido, mas o Capacitor exige o campo
  server: {
    url: "http://SEU_IP_AQUI:3000",
    cleartext: true, // permite HTTP (sem TLS) — necessário pra testar localmente; remova ao publicar com HTTPS
  },
  android: {
    allowMixedContent: true,
  },
};

export default config;
