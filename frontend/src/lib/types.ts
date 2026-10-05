export type Perfil = "Operador" | "Administrador";

export type StatusLimpeza = "EmAndamento" | "Concluida" | "Aprovada" | "Reprovada";

export interface Usuario {
  usuarioId: string;
  nome: string;
  matricula: string;
  perfil: Perfil;
  token: string;
  precisaTrocarSenha: boolean;
}

export interface EtapaResumo {
  obrigatoria?: boolean;
  etapaPadraoId: string;
  nome: string;
  descricao: string | null;
  linkVideo?: string;
  ordem: number;
  concluida: boolean;
  fotos: string[];
  itens: EtapaItemResumo[];
  problemaExtraDescricao?: string | null;
  problemaExtraFotoUrl?: string | null;
}

export interface EtapaItemResumo {
  id: string;
  texto: string;
  descricao: string | null;
  enquadramentoFoto: string | null;
  ordem: number;
  status: string | null;
  funcionalidade: string | null;
  relatoProblema: string | null;
  fotoUrl: string | null;
  fotoDuplicadaPrefixo?: string | null;
  concluida: boolean;
  obrigatorio?: boolean;
}

export interface LimpezaFinaDetalhes {
  id: string;
  prefixo: string;
  numeroOS: string | null;
  status: StatusLimpeza;
  nomeOperador: string;
  iniciadaEm: string;
  finalizadaEm: string | null;
  avaliadaEm: string | null;
  observacaoAvaliacao: string | null;
  notificacaoEnviada: boolean;
  cortinasRetiradas: boolean;
  etapas: EtapaResumo[];
}

export interface LimpezaFinaResumo {
  id: string;
  prefixo: string;
  numeroOS: string | null;
  status: StatusLimpeza;
  nomeOperador: string;
  iniciadaEm: string;
  finalizadaEm: string | null;
  notificacaoEnviada: boolean;
  cortinasRetiradas: boolean;
}

export interface OnibusResumo {
  id: string;
  prefixo: string;
  placa: string | null;
  ativo: boolean;
}

export interface EtapaItemPadraoResumo {
  id?: string;
  obrigatorio?: boolean;
  texto: string;
  ordem: number;
  descricao?: string;
  enquadramentoFoto?: string;
}

export interface EtapaPadraoResumo {
  itens?: EtapaItemPadraoResumo[];
  obrigatoria?: boolean;
  id: string;
  nome: string;
  descricao: string | null;
  linkVideo?: string;
  ordem: number;
  ativo: boolean;
}

export interface UsuarioResumo {
  id: string;
  matricula: string;
  nome: string;
  email: string | null;
  perfil: Perfil;
  ativo: boolean;
}

export interface ItemImportacaoErro {
  prefixo: string;
  motivo: string;
}

export interface ResultadoImportacaoOnibus {
  totalLinhasProcessadas: number;
  criados: OnibusResumo[];
  jaExistentes: string[];
  erros: ItemImportacaoErro[];
}
