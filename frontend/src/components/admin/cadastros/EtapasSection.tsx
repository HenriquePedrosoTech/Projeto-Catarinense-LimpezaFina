"use client";

import { useEffect, useState, type FormEvent } from "react";
import { api, ApiError } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { ListChecks, Trash2, Edit2, Info } from "lucide-react";
import type { EtapaPadraoResumo } from "@/lib/types";
import { EtapaEditModal } from "./EtapaEditModal";

export function EtapasSection({ token }: { token: string }) {
  const [lista, setLista] = useState<EtapaPadraoResumo[]>([]);
  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [linkVideo, setLinkVideo] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);
  const [excluindoId, setExcluindoId] = useState<string | null>(null);
  
  const [etapaEditando, setEtapaEditando] = useState<EtapaPadraoResumo | null>(null);

  const carregar = () => {
    api.listarEtapasPadrao(token).then(setLista).catch(() => {});
  };

  useEffect(carregar, [token]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setEnviando(true);
    try {
      await api.cadastrarEtapaPadrao(
        token, 
        nome.trim(), 
        lista.length, 
        descricao.trim() || undefined, 
        linkVideo.trim() || undefined
      );
      setNome("");
      setDescricao("");
      setLinkVideo("");
      carregar();
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao cadastrar.");
    } finally {
      setEnviando(false);
    }
  }

  async function handleExcluir(id: string, nomeEtapa: string) {
    if (!confirm(`Excluir a etapa "${nomeEtapa}"?`)) return;
    setExcluindoId(id);
    try {
      await api.excluirEtapaPadrao(token, id);
      setLista((atual) => atual.filter((e) => e.id !== id));
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Falha ao excluir.");
    } finally {
      setExcluindoId(null);
    }
  }

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-lg">
            <ListChecks className="h-5 w-5 text-ink/50" />
            Checklist Padrão
          </CardTitle>
          <p className="text-sm text-ink/50">
            A ordem em que você adiciona as etapas define a ordem exigida no aplicativo do operador.
          </p>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="mb-6 flex flex-col gap-3">
            <div className="flex gap-3">
              <div className="flex-1">
                <Input
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                  placeholder="Ex.: Limpeza do Banheiro"
                  required
                />
              </div>
              <Button type="submit" loading={enviando} className="shrink-0">
                Adicionar Etapa
              </Button>
            </div>
            <textarea
              className="w-full rounded-md border border-line bg-surface p-3 text-sm focus:border-brand focus:outline-none focus:ring-1 focus:ring-brand"
              rows={3}
              placeholder="Procedimento Operacional Padrão (P.O.P) opcional. Ex: Descrever como limpar, quais produtos usar..."
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
            <Input
              type="url"
              className="w-full"
              placeholder="Link para Vídeo do YouTube / Drive (opcional)"
              value={linkVideo}
              onChange={(e) => setLinkVideo(e.target.value)}
            />
          </form>

          {erro && <p className="mb-4 rounded-md bg-danger/10 p-3 text-sm text-danger">{erro}</p>}

          <div className="rounded-md border border-line bg-surface/20">
            {lista.length === 0 && (
              <p className="p-6 text-center text-sm text-ink/50">Nenhuma etapa cadastrada ainda.</p>
            )}
            <ul className="divide-y divide-line">
              {lista.map((e, i) => (
                <li key={e.id} className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between p-4 hover:bg-surface/50">
                  <div className="flex flex-1 items-center gap-4">
                    <span className="flex h-7 w-7 items-center justify-center rounded bg-ink text-xs font-bold text-white shadow-sm shrink-0">
                      {i + 1}
                    </span>
                    <div className="flex flex-col">
                      <span className="font-medium text-ink">{e.nome}</span>
                      {e.descricao && (
                        <span className="text-xs text-ink/60 flex items-center gap-1 mt-0.5">
                          <Info className="h-3 w-3" /> P.O.P configurado
                        </span>
                      )}
                    </div>
                  </div>
                  
                  <div className="flex items-center justify-end gap-2 sm:mt-0 mt-3 sm:w-auto w-full border-t sm:border-t-0 border-line/50 pt-2 sm:pt-0">
                    <button
                      onClick={() => setEtapaEditando(e)}
                      className="flex items-center gap-1 text-sm text-ink/50 transition hover:text-brand px-2 py-1 rounded hover:bg-surface"
                      title="Editar Etapa"
                    >
                      <Edit2 className="h-4 w-4" />
                      <span className="sm:hidden">Editar</span>
                    </button>
                    
                    <button
                      onClick={() => handleExcluir(e.id, e.nome)}
                      disabled={excluindoId === e.id}
                      className="flex items-center gap-1 text-sm text-ink/50 transition hover:text-danger disabled:opacity-50 px-2 py-1 rounded hover:bg-surface"
                      title="Excluir Etapa"
                    >
                      <Trash2 className="h-4 w-4" />
                      <span className="sm:hidden">Excluir</span>
                    </button>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        </CardContent>
      </Card>

      <EtapaEditModal 
        token={token}
        etapa={etapaEditando}
        onClose={() => setEtapaEditando(null)}
        onSuccess={() => {
          setEtapaEditando(null);
          carregar();
        }}
      />
    </div>
  );
}