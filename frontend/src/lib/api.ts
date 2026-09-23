import type {
  EtapaPadraoResumo,
  LimpezaFinaDetalhes,
  LimpezaFinaResumo,
  OnibusResumo,
  ResultadoImportacaoOnibus,
  Usuario,
  UsuarioResumo,
} from "./types";
import { API_URL } from "./fotos";

export class ApiError extends Error {
  constructor(message: string, public status: number) {
    super(message);
  }
}

async function request<T>(
  path: string,
  options: RequestInit & { token?: string | null } = {}
): Promise<T> {
  const { token, headers, ...rest } = options;

  const resposta = await fetch(`${API_URL}${path}`, {
    ...rest,
    headers: {
      ...(rest.body && !(rest.body instanceof FormData) ? { "Content-Type": "application/json" } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
  });

  if (!resposta.ok) {
    if (resposta.status === 401 && typeof window !== "undefined") {
      window.dispatchEvent(new CustomEvent("api-unauthorized"));
    }
    let mensagem = `Erro ${resposta.status} ao chamar ${path}`;
    try {
      const corpo = await resposta.json();
      if (corpo?.erro) mensagem = corpo.erro;
    } catch {
      // corpo sem JSON (ex.: 204 ou erro de rede) — mantém mensagem padrão
    }
    throw new ApiError(mensagem, resposta.status);
  }

  if (resposta.status === 204) return undefined as T;
  return (await resposta.json()) as T;
}

export const api = {
  login: (matricula: string, senha: string) =>
    request<Usuario>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({ matricula, senha }),
    }),

  // --- Limpezas finas ---
  iniciarLimpeza: (token: string, prefixoOnibus: string) =>
    request<LimpezaFinaDetalhes>("/api/limpezas-finas", {
      method: "POST",
      token,
      body: JSON.stringify({ prefixoOnibus }),
    }),

  definirNumeroOS: (token: string, limpezaId: string, numeroOS: string) =>
    request<LimpezaFinaDetalhes>(`/api/limpezas-finas/${limpezaId}/os`, {
      method: "POST",
      token,
      body: JSON.stringify({ numeroOS }),
    }),

  enviarFotoEtapa: (token: string, limpezaId: string, etapaPadraoId: string, arquivo: File) => {
    const form = new FormData();
    form.append("arquivo", arquivo);
    return request<LimpezaFinaDetalhes>(`/api/limpezas-finas/${limpezaId}/etapas/${etapaPadraoId}/foto`, {
      method: "POST",
      token,
      body: form,
    });
  },

  removerFotoEtapa: (token: string, limpezaId: string, etapaPadraoId: string, urlArquivo: string) =>
    request<LimpezaFinaDetalhes>(
      `/api/limpezas-finas/${limpezaId}/etapas/${etapaPadraoId}/foto?urlArquivo=${encodeURIComponent(urlArquivo)}`,
      { method: "DELETE", token }
    ),

  sinalizarCortinas: (token: string, limpezaId: string, retiradas: boolean) =>
    request<LimpezaFinaDetalhes>(
      `/api/limpezas-finas/${limpezaId}/cortinas?retiradas=${retiradas}`,
      { method: "POST", token }
    ),

  finalizarLimpeza: (token: string, limpezaId: string) =>
    request<LimpezaFinaDetalhes>(`/api/limpezas-finas/${limpezaId}/finalizar`, { method: "POST", token }),

  aprovarLimpeza: (token: string, limpezaId: string) =>
    request<LimpezaFinaDetalhes>(`/api/limpezas-finas/${limpezaId}/aprovar`, { method: "POST", token }),

  reprovarLimpeza: (token: string, limpezaId: string, motivo: string) =>
    request<LimpezaFinaDetalhes>(`/api/limpezas-finas/${limpezaId}/reprovar`, {
      method: "POST",
      token,
      body: JSON.stringify({ motivo }),
    }),

  notificarLimpeza: (token: string, limpezaId: string, destinatariosEmail: string[]) =>
    request<void>(`/api/limpezas-finas/${limpezaId}/notificar`, {
      method: "POST",
      token,
      body: JSON.stringify({ destinatariosEmail }),
    }),

  obterDetalhesLimpeza: (limpezaId: string, token?: string | null) =>
    request<LimpezaFinaDetalhes>(`/api/limpezas-finas/${limpezaId}`, { token }),

  listarLimpezasPorOperador: (token: string, operadorId: string) =>
    request<LimpezaFinaResumo[]>(`/api/limpezas-finas?operadorId=${operadorId}`, { token }),

  listarLimpezasPorStatus: (token: string, status: string) =>
    request<LimpezaFinaResumo[]>(`/api/limpezas-finas?status=${status}`, { token }),

  obterRelatorioLimpezas: (
    token: string,
    filtros: { dataInicio?: string; dataFim?: string; status?: string; operadorId?: string }
  ) => {
    const params = new URLSearchParams();
    if (filtros.dataInicio) params.append("dataInicio", filtros.dataInicio);
    if (filtros.dataFim) params.append("dataFim", filtros.dataFim);
    if (filtros.status) params.append("status", filtros.status);
    if (filtros.operadorId) params.append("operadorId", filtros.operadorId);

    const qs = params.toString() ? `?${params.toString()}` : "";
    return request<LimpezaFinaResumo[]>(`/api/limpezas-finas/relatorio${qs}`, { token });
  },

  excluirLimpezaFina: (token: string, limpezaId: string) =>
    request<void>(`/api/limpezas-finas/${limpezaId}`, { method: "DELETE", token }),

  // --- Dados mestres ---
  listarEtapasPadrao: (token: string) =>
    request<EtapaPadraoResumo[]>("/api/etapas-padrao", { token }),

  cadastrarEtapaPadrao: (token: string, nome: string, ordem: number, descricao?: string, linkVideo?: string) =>
    request<EtapaPadraoResumo>("/api/etapas-padrao", {
      method: "POST",
      token,
      body: JSON.stringify({ nome, ordem, descricao, linkVideo }),
    }),

  editarEtapaPadrao: (token: string, etapaId: string, nome: string, ordem: number, descricao?: string, linkVideo?: string) =>
    request<EtapaPadraoResumo>(`/api/etapas-padrao/${etapaId}`, {
      method: "PUT",
      token,
      body: JSON.stringify({ nome, ordem, descricao, linkVideo }),
    }),

  excluirEtapaPadrao: (token: string, etapaId: string) =>
    request<void>(`/api/etapas-padrao/${etapaId}`, { method: "DELETE", token }),

  listarOnibus: (token: string) => request<OnibusResumo[]>("/api/onibus", { token }),

  cadastrarOnibus: (token: string, prefixo: string, placa: string) =>
    request<OnibusResumo>("/api/onibus", {
      method: "POST",
      token,
      body: JSON.stringify({ prefixo, placa: placa || null }),
    }),

  excluirOnibus: (token: string, onibusId: string) =>
    request<void>(`/api/onibus/${onibusId}`, { method: "DELETE", token }),

  importarOnibusPlanilha: (token: string, arquivo: File) => {
    const form = new FormData();
    form.append("arquivo", arquivo);
    return request<ResultadoImportacaoOnibus>("/api/onibus/importar", {
      method: "POST",
      token,
      body: form,
    });
  },

  listarUsuarios: (token: string) => request<UsuarioResumo[]>("/api/usuarios", { token }),

  cadastrarUsuario: (
    token: string,
    matricula: string,
    nome: string,
    senha: string,
    perfil: "Operador" | "Administrador",
    email: string
  ) =>
    request<UsuarioResumo>("/api/usuarios", {
      method: "POST",
      token,
      body: JSON.stringify({ matricula, nome, senha, perfil, email: email || null }),
    }),

  excluirUsuario: (token: string, usuarioId: string) =>
    request<void>(`/api/usuarios/${usuarioId}`, { method: "DELETE", token }),
};
