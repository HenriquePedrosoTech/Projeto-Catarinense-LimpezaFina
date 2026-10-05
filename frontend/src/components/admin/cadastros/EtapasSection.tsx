"use client";

import { useEffect, useState, type FormEvent } from "react";
import { api, ApiError } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { ConfirmDialog, AlertDialog } from "@/components/ui/Dialogs";
import { Input } from "@/components/ui/Input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { ListChecks, Trash2, Edit2, Info, GripVertical } from "lucide-react";
import type { EtapaPadraoResumo } from "@/lib/types";
import { EtapaEditModal } from "./EtapaEditModal";
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
  useSortable
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';

function SortableItem({ id, e, i, onEdit, onDelete, excluindoId }: any) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
  } = useSortable({ id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <li ref={setNodeRef} style={style} className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between p-4 hover:bg-surface/50 bg-white z-10">
      <div className="flex flex-1 items-center gap-4">
        <button {...attributes} {...listeners} className="cursor-grab text-ink/40 hover:text-ink/80 touch-none">
          <GripVertical className="h-5 w-5" />
        </button>
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
          onClick={() => onEdit(e)}
          className="flex items-center gap-1 text-sm text-ink/50 transition hover:text-brand px-2 py-1 rounded hover:bg-surface"
          title="Editar Etapa"
        >
          <Edit2 className="h-4 w-4" />
          <span className="sm:hidden">Editar</span>
        </button>
        
        <button
          onClick={() => onDelete(e.id, e.nome)}
          disabled={excluindoId === e.id}
          className="flex items-center gap-1 text-sm text-ink/50 transition hover:text-danger disabled:opacity-50 px-2 py-1 rounded hover:bg-surface"
          title="Excluir Etapa"
        >
          <Trash2 className="h-4 w-4" />
          <span className="sm:hidden">Excluir</span>
        </button>
      </div>
    </li>
  );
}

export function EtapasSection({ token }: { token: string }) {
  const [lista, setLista] = useState<EtapaPadraoResumo[]>([]);
  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [linkVideo, setLinkVideo] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);
  const [excluindoId, setExcluindoId] = useState<string | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<{aberto: boolean, id: string, nomeEtapa: string}>({aberto: false, id: "", nomeEtapa: ""});
  const [alertInfo, setAlertInfo] = useState<{aberto: boolean, mensagem: string}>({aberto: false, mensagem: ""});
  
  const [etapaEditando, setEtapaEditando] = useState<EtapaPadraoResumo | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const carregar = () => {
    api.listarEtapasPadrao(token).then(data => {
      // Garantir que estao ordenadas pela Ordem
      const ordenadas = [...data].sort((a, b) => a.ordem - b.ordem);
      setLista(ordenadas);
    }).catch(() => {});
  };

  useEffect(carregar, [token]);

  async function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;

    if (over && active.id !== over.id) {
      setLista((items) => {
        const oldIndex = items.findIndex((i) => i.id === active.id);
        const newIndex = items.findIndex((i) => i.id === over.id);
        
        const newItems = arrayMove(items, oldIndex, newIndex);
        
        // Agora mapeamos a nova ordem
        const payload = newItems.map((item, index) => ({
          id: item.id,
          ordem: index
        }));

        // Dispara a chamada de API sem bloquear a interface imediatamente
        api.reordenarEtapasPadrao(token, payload).catch(err => {
          setErro("Falha ao salvar a nova ordem. Recarregando...");
          carregar(); // Recarrega se falhar
        });

        // Atualiza as ordens locais para renderizar corretamente os nÃºmeros
        return newItems.map((item, index) => ({...item, ordem: index}));
      });
    }
  }

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
    setConfirmDelete({ aberto: true, id, nomeEtapa });
  }

  async function executarExclusao() {
    const { id } = confirmDelete;
    setExcluindoId(id);
    try {
      await api.excluirEtapaPadrao(token, id);
      setLista((atual) => atual.filter((u) => u.id !== id));
    } catch (err) {
      setAlertInfo({ aberto: true, mensagem: err instanceof ApiError ? err.message : "Falha ao excluir." });
    } finally {
      setExcluindoId(null);
    }
  }

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <ConfirmDialog 
        aberto={confirmDelete.aberto}
        titulo="Excluir Etapa Padrao"
        mensagem={`Tem certeza que deseja excluir a etapa "${confirmDelete.nomeEtapa}"?`}
        tipo="danger"
        textoConfirmar="Excluir"
        onClose={() => setConfirmDelete({ ...confirmDelete, aberto: false })}
        onConfirmar={executarExclusao}
      />
      <AlertDialog 
        aberto={alertInfo.aberto}
        titulo="Atenção"
        mensagem={alertInfo.mensagem}
        tipo="danger"
        onClose={() => setAlertInfo({ ...alertInfo, aberto: false })}
      />
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-lg">
            <ListChecks className="h-5 w-5 text-ink/50" />
            Checklist Padrão
          </CardTitle>
          <p className="text-sm text-ink/50">
            A ordem em que você adiciona as etapas define a ordem exigida no aplicativo do operador. Você pode clicar e arrastar para reordenar.
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
            
            <DndContext 
              sensors={sensors}
              collisionDetection={closestCenter}
              onDragEnd={handleDragEnd}
            >
              <SortableContext 
                items={lista.map(i => i.id)}
                strategy={verticalListSortingStrategy}
              >
                <ul className="divide-y divide-line">
                  {lista.map((e, i) => (
                    <SortableItem 
                      key={e.id}
                      id={e.id}
                      e={e}
                      i={i}
                      onEdit={setEtapaEditando}
                      onDelete={handleExcluir}
                      excluindoId={excluindoId}
                    />
                  ))}
                </ul>
              </SortableContext>
            </DndContext>
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
