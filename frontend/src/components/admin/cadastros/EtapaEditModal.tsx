import { useState, type FormEvent, useEffect } from "react";
import { api, ApiError } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { X } from "lucide-react";
import type { EtapaPadraoResumo } from "@/lib/types";

interface EtapaEditModalProps {
  token: string;
  etapa: EtapaPadraoResumo | null;
  onClose: () => void;
  onSuccess: () => void;
}

export function EtapaEditModal({ token, etapa, onClose, onSuccess }: EtapaEditModalProps) {
  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [linkVideo, setLinkVideo] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [salvando, setSalvando] = useState(false);

  useEffect(() => {
    if (etapa) {
      setNome(etapa.nome);
      setDescricao(etapa.descricao || "");
      setLinkVideo(etapa.linkVideo || "");
      setErro(null);
    }
  }, [etapa]);

  if (!etapa) return null;

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setSalvando(true);
    try {
      await api.editarEtapaPadrao(
        token, 
        etapa!.id, 
        nome.trim(), 
        etapa!.ordem, 
        descricao.trim() || undefined,
        linkVideo.trim() || undefined
      );
      onSuccess();
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao editar a etapa.");
    } finally {
      setSalvando(false);
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-ink/50 backdrop-blur-sm p-4">
      <div className="relative w-full max-w-lg rounded-xl bg-white p-6 shadow-2xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 rounded-md p-1 text-ink/50 hover:bg-surface hover:text-ink transition-colors"
        >
          <X className="h-5 w-5" />
        </button>

        <h2 className="mb-4 text-xl font-bold text-ink">Editar Etapa</h2>

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div>
            <label className="mb-1 block text-sm font-medium text-ink">Nome da Etapa</label>
            <Input
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              placeholder="Ex.: Limpeza do Banheiro"
              required
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-ink">Procedimento Operacional (P.O.P)</label>
            <textarea
              className="w-full rounded-md border border-line bg-surface p-3 text-sm focus:border-brand focus:outline-none focus:ring-1 focus:ring-brand"
              rows={4}
              placeholder="Descreva o passo a passo..."
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-ink">Link do Vídeo (Opcional)</label>
            <Input
              type="url"
              value={linkVideo}
              onChange={(e) => setLinkVideo(e.target.value)}
              placeholder="Ex.: https://youtube.com/..."
            />
          </div>

          {erro && <p className="rounded-md bg-danger/10 p-3 text-sm text-danger">{erro}</p>}

          <div className="mt-2 flex justify-end gap-3">
            <Button type="button" variant="outline" onClick={onClose} disabled={salvando}>
              Cancelar
            </Button>
            <Button type="submit" loading={salvando}>
              Salvar
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}

