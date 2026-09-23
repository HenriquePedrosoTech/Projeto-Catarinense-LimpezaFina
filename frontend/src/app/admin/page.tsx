"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { api, ApiError } from "@/lib/api";
import { StatusBadge } from "@/components/StatusBadge";
import { ConfirmDialog } from "@/components/ui/Dialogs";
import { Card, CardContent } from "@/components/ui/Card";
import { Skeleton } from "@/components/ui/Skeleton";
import { Trash2, Bus, Clock, CheckCircle, XCircle } from "lucide-react";
import type { LimpezaFinaResumo, StatusLimpeza } from "@/lib/types";

type FiltroItem = { valor: StatusLimpeza; rotulo: string; icon: React.ElementType };

const FILTROS: FiltroItem[] = [
  { valor: "Concluida", rotulo: "Aguardando Avaliação", icon: Clock },
  { valor: "EmAndamento", rotulo: "Em Andamento", icon: Bus },
  { valor: "Aprovada", rotulo: "Aprovadas", icon: CheckCircle },
  { valor: "Reprovada", rotulo: "Reprovadas", icon: XCircle },
];

export default function AdminDashboardPage() {
  const { usuario } = useAuth();
  const [filtro, setFiltro] = useState<StatusLimpeza>("Concluida");
  const [limpezas, setLimpezas] = useState<LimpezaFinaResumo[] | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [excluindoId, setExcluindoId] = useState<string | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<{aberto: boolean, id: string, prefixo: string}>({aberto: false, id: "", prefixo: ""});

  useEffect(() => {
    if (!usuario) return;
    setLimpezas(null);
    api
      .listarLimpezasPorStatus(usuario.token, filtro)
      .then(setLimpezas)
      .catch((err) => setErro(err instanceof ApiError ? err.message : "Falha ao carregar a lista."));
  }, [usuario, filtro]);

  async function handleExcluir(e: React.MouseEvent, id: string, prefixo: string) {
    e.preventDefault();
    if (!usuario) return;
    setConfirmDelete({ aberto: true, id, prefixo });
  }

  async function executarExclusao() {
    if (!usuario) return;
    const { id } = confirmDelete;
    setErro(null);
    setExcluindoId(id);
    try {
      await api.excluirLimpezaFina(usuario.token, id);
      setLimpezas((atual) => atual?.filter((l) => l.id !== id) ?? null);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao excluir.");
    } finally {
      setExcluindoId(null);
    }
  }

  if (!usuario) return null;

  return (
    <div className="mx-auto max-w-5xl">`n      <ConfirmDialog `n        aberto={confirmDelete.aberto}`n        titulo="Excluir Limpeza"`n        mensagem={`Tem certeza que deseja excluir o registro do �nibus ${confirmDelete.prefixo}?`}`n        tipo="danger"`n        textoConfirmar="Excluir"`n        onClose={() => setConfirmDelete({ ...confirmDelete, aberto: false })}`n        onConfirmar={executarExclusao}`n      />
      <div className="mb-8">
        <h1 className="text-2xl font-bold tracking-tight text-ink">Dashboard de Limpezas</h1>
        <p className="text-ink/60">Acompanhe e avalie as execuções de limpeza fina da frota.</p>
      </div>

      <div className="mb-6 flex gap-2 overflow-x-auto pb-2">
        {FILTROS.map((f) => {
          const Icon = f.icon;
          const isSelected = filtro === f.valor;
          return (
            <button
              key={f.valor}
              onClick={() => setFiltro(f.valor)}
              className={`flex shrink-0 items-center gap-2 rounded-full border px-4 py-2 text-sm font-medium transition-colors ${
                isSelected
                  ? "border-brand bg-brand text-white shadow-md"
                  : "border-line bg-white text-ink/70 hover:bg-surface"
              }`}
            >
              <Icon className="h-4 w-4" />
              {f.rotulo}
            </button>
          );
        })}
      </div>

      {erro && (
        <div className="mb-6 rounded-md bg-danger/10 p-4 text-sm text-danger">{erro}</div>
      )}

      {!limpezas && !erro && (
        <div className="space-y-3">
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} className="h-24 w-full rounded-card" />
          ))}
        </div>
      )}

      {limpezas && limpezas.length === 0 && (
        <div className="flex flex-col items-center justify-center rounded-card border border-dashed border-line bg-white py-16 text-center">
          <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-surface">
            <CheckCircle className="h-6 w-6 text-ink/40" />
          </div>
          <h3 className="font-semibold text-ink">Tudo limpo por aqui!</h3>
          <p className="mt-1 text-sm text-ink/50">Nenhum registro encontrado neste status.</p>
        </div>
      )}

      {limpezas && limpezas.length > 0 && (
        <div className="grid gap-3">
          {limpezas.map((l) => (
            <Link key={l.id} href={`/admin/limpeza/${l.id}`}>
              <Card className="group transition-all hover:border-brand/40 hover:shadow-md">
                <CardContent className="flex items-center justify-between p-4 sm:p-5">
                  <div className="flex items-center gap-4 sm:gap-6">
                    <div className="hidden h-12 w-12 items-center justify-center rounded-full bg-brand/5 sm:flex">
                      <Bus className="h-6 w-6 text-brand" />
                    </div>

                    <div>
                      <div className="mb-1 flex items-center gap-3">
                        <span className="text-lg font-bold text-ink">{l.prefixo}</span>
                        <StatusBadge status={l.status} />
                      </div>
                      <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-ink/60">
                        <span className="flex items-center gap-1.5">
                          <span className="flex h-5 w-5 items-center justify-center rounded-full bg-ink/10 text-[10px] font-bold text-ink">
                            {l.nomeOperador.charAt(0)}
                          </span>
                          {l.nomeOperador}
                        </span>
                        <span className="text-ink/30">·</span>
                        <span>
                          {new Date(l.iniciadaEm).toLocaleDateString("pt-BR", {
                            day: "2-digit",
                            month: "short",
                            hour: "2-digit",
                            minute: "2-digit",
                          })}
                        </span>
                        {l.numeroOS && (
                          <>
                            <span className="text-ink/30">·</span>
                            <span className="font-medium text-ink/80">O.S. {l.numeroOS}</span>
                          </>
                        )}
                        {l.notificacaoEnviada && (
                          <span className="ml-1 rounded bg-success/10 px-1.5 py-0.5 text-xs font-semibold text-success">
                            ✓ Notificado
                          </span>
                        )}
                      </div>
                    </div>
                  </div>

                  <button
                    onClick={(e) => handleExcluir(e, l.id, l.prefixo)}
                    disabled={excluindoId === l.id}
                    className="ml-4 rounded-md p-2 text-ink/30 transition-colors hover:bg-danger/10 hover:text-danger disabled:opacity-50"
                    title="Excluir registro"
                  >
                    <Trash2 className="h-5 w-5" />
                  </button>
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}



