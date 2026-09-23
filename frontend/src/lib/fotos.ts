export const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080";

/**
 * Monta a URL completa de uma foto retornada pela API. As fotos vêm como
 * caminho relativo (ex.: "/uploads/fotos-limpeza/xxx.jpg"), então precisam do
 * domínio da API na frente. Sem esse fallback, se NEXT_PUBLIC_API_URL não
 * estiver definido (.env.local ausente), o resultado vira "undefined/..." e a
 * imagem quebra silenciosamente.
 */
export function resolverUrlFoto(url: string): string {
  return url.startsWith("http") ? url : `${API_URL}${url}`;
}
