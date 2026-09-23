"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";
import { api, ApiError } from "@/lib/api";
import { StatusBadge } from "@/components/StatusBadge";
import { EtapaCard } from "@/components/EtapaCard";
import { Skeleton } from "@/components/ui/Skeleton";
import { Button } from "@/components/ui/Button";
import { Card, CardContent } from "@/components/ui/Card";
import { Bus, CheckCircle, ArrowLeft, User, Calendar } from "lucide-react";
import type { LimpezaFinaDetalhes } from "@/lib/types";
import { ToastAlerta } from "@/components/ToastAlerta";

export default function ChecklistLimpezaPage() {
  const { usuario } = useAuth();
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const [limpeza, setLimpeza] = useState<LimpezaFinaDetalhes | null>(null);
  const [alerta, setAlerta] = useState<{ titulo: string; mensagem: string } | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [finalizando, setFinalizando] = useState(false);

  useEffect(() => {
    if (!usuario) return;
    api
      .obterDetalhesLimpeza(id, usuario.token)
      .then(setLimpeza)
      .catch((err) => setErro(err instanceof ApiError ? err.message : "Falha ao carregar o registro."));
  }, [usuario, id]);

  const etapasObrigatorias = limpeza?.etapas.filter(e => !(e.nome.toUpperCase().includes("CORTINA") && !limpeza.cortinasRetiradas)) ?? [];
  const totalEtapas = etapasObrigatorias.length;
  const concluidas = etapasObrigatorias.filter((e) => e.concluida).length;
  const progresso = totalEtapas > 0 ? (concluidas / totalEtapas) * 100 : 0;
  const todasConcluidas = concluidas === totalEtapas && totalEtapas > 0;
  const emAndamento = limpeza?.status === "EmAndamento";

  async function handleEnviarFoto(etapaPadraoId: string, arquivo: File) {
    if (!usuario) return;
    try {
      const atualizado = await api.enviarFotoEtapa(usuario.token, id, etapaPadraoId, arquivo);
      setLimpeza(atualizado);
    } catch (err) {
      const nomeDaEtapa = limpeza?.etapas.find(e => e.etapaPadraoId === etapaPadraoId)?.nome || "Etapa desconhecida";
      setAlerta({
        titulo: "Erro ao cadastrar foto",
        mensagem: `Ocorreu um erro ao enviar a foto para a etapa: ${nomeDaEtapa}. Verifique sua conexão e tente novamente.`
      });
    }
  }

  async function handleRemoverFoto(etapaPadraoId: string, url: string) {
    if (!usuario) return;
    try {
      const atualizado = await api.removerFotoEtapa(usuario.token, id, etapaPadraoId, url);
      setLimpeza(atualizado);
    } catch (err) {
      setAlerta({
        titulo: "Erro ao remover foto",
        mensagem: "Não foi possível remover a foto. Tente novamente."
      });
    }
  }

  async function handleFinalizar() {
    if (!usuario) return;

    if (!todasConcluidas) {
      const faltantes = etapasObrigatorias.filter(e => !e.concluida).map(e => e.nome).join(", ");
      setAlerta({
        titulo: "Checklist incompleto!",
        mensagem: `Você não anexou as fotos exigidas nas seguintes etapas: ${faltantes}.`
      });
      return;
    }

    setErro(null);
    setFinalizando(true);
    try {
      await api.finalizarLimpeza(usuario.token, id);
      router.push("/operador");
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível finalizar.");
      setFinalizando(false);
    }
  }

  if (!usuario) return null;

  return (
    <div className="mx-auto max-w-2xl pb-24 relative">
      {alerta && (
        <ToastAlerta 
          titulo={alerta.titulo} 
          mensagem={alerta.mensagem} 
          onClose={() => setAlerta(null)} 
        />
      )}

      <button
        onClick={() => router.push("/operador")}
        className="mb-6 flex items-center gap-2 text-sm text-ink/60 transition hover:text-brand"
      >
        <ArrowLeft className="h-4 w-4" /> Voltar para Minhas Limpezas
      </button>

      {erro && (
        <div className="mb-6 rounded-md bg-danger/10 p-4 text-sm text-danger">{erro}</div>
      )}

      {!limpeza && !erro && (
        <div className="space-y-4">
          <Skeleton className="h-40 w-full rounded-card" />
          <Skeleton className="h-32 w-full rounded-card" />
          <Skeleton className="h-32 w-full rounded-card" />
        </div>
      )}

      {limpeza && (
        <>
          <Card className="mb-6 shadow-sm border-line">
            <CardContent className="p-5">
              <div className="mb-4 flex items-start justify-between">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-full bg-brand/10">
                    <Bus className="h-6 w-6 text-brand" />
                  </div>
                  <div>
                    <h1 className="text-xl font-bold text-ink">Prefixo {limpeza.prefixo}</h1>
                    <p className="text-sm font-medium text-ink/60">
                      {limpeza.numeroOS ? `O.S. ${limpeza.numeroOS}` : "O.S. Pendente"}
                    </p>
                  </div>
                </div>
                <StatusBadge status={limpeza.status} />
              </div>

              <div className="mb-4 grid grid-cols-2 gap-3 rounded-lg bg-surface p-3 text-sm text-ink/80">
                <div className="flex items-center gap-2">
                  <User className="h-4 w-4 text-ink/40" />
                  <span className="truncate"><strong>Op:</strong> {limpeza.nomeOperador}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Calendar className="h-4 w-4 text-ink/40" />
                  <span className="truncate">
                    <strong>Início:</strong>{" "}
                    {new Date(limpeza.iniciadaEm).toLocaleDateString("pt-BR")}
                  </span>
                </div>
              </div>

              <div className="mt-4">
                <div className="mb-2 flex items-center justify-between text-sm">
                  <span className="font-semibold text-ink">Progresso do Checklist</span>
                  <span className="font-bold text-brand">{concluidas} / {totalEtapas}</span>
                </div>
                <div className="h-2.5 w-full overflow-hidden rounded-full bg-surface">
                  <div 
                    className="h-full bg-brand transition-all duration-500 ease-out"
                    style={{ width: `${progresso}%` }}
                  />
                </div>
              </div>

              <div className="mt-6 flex items-center gap-3 border-t border-line pt-4">
                <input
                  type="checkbox"
                  id="cortinas"
                  checked={limpeza.cortinasRetiradas}
                  disabled={!emAndamento}
                  onChange={async (e) => {
                    if (!usuario) return;
                    const val = e.target.checked;
                    try {
                      const atualizado = await api.sinalizarCortinas(usuario.token, id, val);
                      setLimpeza(atualizado);
                    } catch (err) {
                      setErro("Falha ao atualizar status das cortinas.");
                    }
                  }}
                  className="h-5 w-5 rounded border-line text-brand focus:ring-brand"
                />
                <label htmlFor="cortinas" className="flex flex-col cursor-pointer">
                  <span className="text-sm font-semibold text-ink">Cortinas foram retiradas?</span>
                  <span className="text-xs text-ink/60">Marque caso as cortinas tenham sido removidas do veículo</span>
                </label>
              </div>
            </CardContent>
          </Card>

          <div className="space-y-4">
            {limpeza.etapas.map((etapa) => (
              <EtapaCard
                key={etapa.etapaPadraoId}
                etapa={etapa}
                desabilitado={!emAndamento}
                onEnviarFoto={(arquivo) => handleEnviarFoto(etapa.etapaPadraoId, arquivo)}
                onRemoverFoto={(url) => handleRemoverFoto(etapa.etapaPadraoId, url)}
              />
            ))}
          </div>

          {emAndamento && (
            <div className="fixed bottom-16 left-0 right-0 z-40 border-t border-line bg-white/90 p-4 backdrop-blur-md pb-safe">
              <div className="mx-auto max-w-2xl">
                <Button
                  className="w-full py-6 text-lg shadow-sm"
                  onClick={handleFinalizar}
                  loading={finalizando}
                  disabled={finalizando}
                >
                  <CheckCircle className="mr-2 h-5 w-5" /> 
                  Finalizar Limpeza Fina
                </Button>
              </div>
            </div>
          )}

          {!emAndamento && (
            <div className="mt-8 rounded-lg border border-dashed border-line bg-surface p-6 text-center">
              <CheckCircle className="mx-auto mb-2 h-8 w-8 text-success/60" />
              <p className="font-semibold text-ink">Checklist Concluído</p>
              <p className="mt-1 text-sm text-ink/60">
                Este registro já foi finalizado e aguarda a avaliação do administrador.
              </p>
            </div>
          )}
        </>
      )}
    </div>
  );
}