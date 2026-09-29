import { useState, type FormEvent, useEffect } from "react";
import { X, Plus, Trash2 } from "lucide-react";
import { api, ApiError } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import type { EtapaPadraoResumo } from "@/lib/types";

interface EtapaEditModalProps {
  token: string;
  etapa: EtapaPadraoResumo | null;
  onClose: () => void;
  onSuccess: () => void;
}

export function EtapaEditModal({ token, etapa, onClose, onSuccess }: EtapaEditModalProps) {
  if (!etapa) return null;
  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [linkVideo, setLinkVideo] = useState("");
  const [obrigatoria, setObrigatoria] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [salvando, setSalvando] = useState(false);
  
  const [itens, setItens] = useState<{ id?: string, texto: string, enquadramentoFoto: string, ordem: number, obrigatorio: boolean }[]>([]);

  useEffect(() => {
    if (etapa) {
      setNome(etapa.nome);
      setDescricao(etapa.descricao || "");
      setLinkVideo(etapa.linkVideo || "");
      setObrigatoria(etapa.obrigatoria ?? true);
      setItens(
        (etapa.itens || []).map((i) => ({
          id: i.id,
          texto: i.texto,
          enquadramentoFoto: i.enquadramentoFoto || "",
          ordem: i.ordem,
          obrigatorio: i.obrigatorio ?? true,
        }))
      );
      setErro(null);
    }
  }, [etapa]);


  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setSalvando(true);
    try {
      await api.editarEtapaPadrao(
        token, 
        etapa!.id,
        nome,
        etapa!.ordem,
        descricao || undefined,
        linkVideo || undefined,
        obrigatoria,
        itens.map(i => ({
          id: i.id,
          texto: i.texto,
          enquadramentoFoto: i.enquadramentoFoto || undefined,
          ordem: i.ordem,
          obrigatorio: i.obrigatorio
        }))
      );
      onSuccess();
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao editar a etapa.");
    } finally {
      setSalvando(false);
    }
  }

  function adicionarItem() {
    setItens([...itens, { texto: "", enquadramentoFoto: "", ordem: itens.length, obrigatorio: true }]);
  }

  function removerItem(index: number) {
    setItens(itens.filter((_, i) => i !== index));
  }

  function atualizarItem(index: number, campo: "texto" | "enquadramentoFoto" | "obrigatorio", valor: any) {
    const novos = [...itens];
    (novos[index] as any)[campo] = valor;
    setItens(novos);
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-ink/50 backdrop-blur-sm p-4 overflow-y-auto">
      <div className="relative w-full max-w-2xl rounded-xl bg-white p-6 shadow-2xl my-8">
        <button
          type="button"
          onClick={onClose}
          className="absolute right-4 top-4 rounded-md p-1 text-ink/50 hover:bg-surface hover:text-ink transition-colors"
        >
          <X className="h-5 w-5" />
        </button>
        <h2 className="mb-4 text-xl font-bold text-ink">Editar Etapa e Itens</h2>

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="mb-1 block text-sm font-medium text-ink">Nome da Seção</label>
              <Input
                value={nome}
                onChange={(e) => setNome(e.target.value)}
                placeholder="Ex.: Limpeza Interna"
                required
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
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-ink">Procedimento Geral (Opcional)</label>
            <textarea
              className="w-full rounded-lg border border-line bg-surface/50 p-3 text-ink placeholder:text-ink/40 focus:border-brand focus:outline-none focus:ring-1 focus:ring-brand"
              rows={2}
              placeholder="Descreva o passo a passo..."
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
          </div>

          <div className="flex items-center gap-2 mt-2">
            <input 
              type="checkbox" 
              id="obrigatoria" 
              checked={obrigatoria} 
              onChange={(e) => setObrigatoria(e.target.checked)} 
              className="h-4 w-4 rounded border-line text-brand focus:ring-brand" 
            />
            <label htmlFor="obrigatoria" className="text-sm font-medium text-ink">Esta etapa é obrigatória para finalizar a limpeza</label>
          </div>

          <div className="border-t border-line pt-4 mt-2">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-lg font-semibold text-ink">Itens do Checklist</h3>
              <Button type="button" variant="outline" size="sm" onClick={adicionarItem} className="flex gap-1 items-center">
                <Plus className="h-4 w-4" /> Novo Item
              </Button>
            </div>

            <div className="flex flex-col gap-3 max-h-80 overflow-y-auto p-1">
              {itens.length === 0 ? (
                <p className="text-sm text-ink/60 text-center py-4">Nenhum item cadastrado.</p>
              ) : (
                itens.map((item, index) => (
                  <div key={index} className="flex flex-col gap-2 rounded-lg border border-line p-3 bg-surface/30">
                    <div className="flex items-start gap-3">
                      <div className="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-ink text-xs font-bold text-white">
                        {index + 1}
                      </div>
                      <div className="flex-1 flex flex-col gap-2">
                        <Input 
                          placeholder="Nome da tarefa (ex: Limpar Poltronas)" 
                          value={item.texto} 
                          onChange={(e) => atualizarItem(index, "texto", e.target.value)} 
                          required 
                        />
                        <Input 
                          placeholder="Instrução da Foto (ex: Foto pegando da poltrona para trás)" 
                          value={item.enquadramentoFoto} 
                          onChange={(e) => atualizarItem(index, "enquadramentoFoto", e.target.value)} 
                        />
                      </div>
                      <button
                        type="button"
                        onClick={() => removerItem(index)}
                        className="text-ink/40 hover:text-danger transition-colors p-1"
                        title="Remover item"
                      >
                        <Trash2 className="h-5 w-5" />
                      </button>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>

          {erro && <p className="rounded-md bg-danger/10 p-3 text-sm text-danger">{erro}</p>}

          <div className="mt-4 flex justify-end gap-3 border-t border-line pt-4">
            <Button type="button" variant="outline" onClick={onClose} disabled={salvando}>
              Cancelar
            </Button>
            <Button type="submit" loading={salvando}>
              Salvar Alterações
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
