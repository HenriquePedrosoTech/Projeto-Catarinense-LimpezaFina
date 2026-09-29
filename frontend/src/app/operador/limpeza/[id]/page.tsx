"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { api, ApiError } from "@/lib/api";
import { useAuth } from "@/lib/auth";
import type { LimpezaFinaDetalhes } from "@/lib/types";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Loader2, CheckCircle, AlertTriangle, Bus, User, Calendar, Info } from "lucide-react";
import { EtapaCard } from "@/components/EtapaCard";

export default function ChecklistLimpezaPage() {
  const { id } = useParams() as { id: string };
  const { usuario } = useAuth();
  const router = useRouter();

  const [limpeza, setLimpeza] = useState<LimpezaFinaDetalhes | null>(null);
  const [carregando, setCarregando] = useState(true);
  const [alerta, setAlerta] = useState<{ titulo: string; mensagem: string } | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [finalizando, setFinalizando] = useState(false);
  const [etapaAtivaId, setEtapaAtivaId] = useState<string | null>(null);

  useEffect(() => {
    if (!usuario) return;
    api.obterDetalhesLimpeza(id, usuario.token)
      .then((data) => { setLimpeza(data); setCarregando(false); if (data.etapas.length > 0) setEtapaAtivaId(data.etapas[0].etapaPadraoId); })
      .catch((err) => setErro(err instanceof ApiError ? err.message : "Falha ao carregar o registro.")).finally(() => setCarregando(false));
  }, [usuario, id]);

  const etapasObrigatorias = limpeza?.etapas.filter(e => {
    if (e.obrigatoria === false) return false;
    if (e.nome.toUpperCase().includes("CORTINA") && !limpeza.cortinasRetiradas) return false;
    return true;
  }) ?? [];
  const totalEtapas = etapasObrigatorias.length;
  const concluidas = etapasObrigatorias.filter((e) => e.concluida).length;
  const progresso = totalEtapas > 0 ? (concluidas / totalEtapas) * 100 : 0;
  const todasConcluidas = totalEtapas > 0 && concluidas === totalEtapas;
  const emAndamento = limpeza?.status === "EmAndamento";

  async function handleEnviarFoto(etapaPadraoId: string, arquivo: File) {
    if (!usuario || !limpeza) return;
    try {
      await api.enviarFotoEtapa(usuario.token, limpeza.id, etapaPadraoId, arquivo);
      const data = await api.obterDetalhesLimpeza(id, usuario.token);
      setLimpeza(data);
    } catch (err) {
      const nomeDaEtapa = limpeza?.etapas.find(e => e.etapaPadraoId === etapaPadraoId)?.nome || "Etapa desconhecida";
      setAlerta({
        titulo: "Erro ao cadastrar foto",
        mensagem: `Ocorreu um erro ao enviar a foto para a etapa: ${nomeDaEtapa}. Verifique sua conexão e tente novamente.`
      });
    }
  }

  async function handleEnviarItemExecucao(etapaPadraoId: string, itemId: string, status: string, funcionalidade: string, relato: string | null, arquivo: File | null) {
    if (!usuario || !limpeza) return;
    try {
      await api.enviarItemExecucao(usuario.token, limpeza.id, etapaPadraoId, itemId, status, funcionalidade, relato, arquivo);
      const data = await api.obterDetalhesLimpeza(id, usuario.token);
      setLimpeza(data);
    } catch (err: any) {
      throw new Error(err instanceof ApiError ? err.message : "Erro ao salvar item");
    }
  }

  async function handleRemoverFoto(etapaPadraoId: string, url: string) {
    if (!usuario || !limpeza) return;
    try {
      await api.removerFotoEtapa(usuario.token, limpeza.id, etapaPadraoId, url);
      const data = await api.obterDetalhesLimpeza(id, usuario.token);
      setLimpeza(data);
    } catch (err) {
      setAlerta({
        titulo: "Erro ao remover foto",
        mensagem: "Não foi possível remover a foto. Tente novamente."
      });
    }
  }

  async function handleFinalizarLimpeza() {
    if (!usuario || !limpeza) return;
    
    if (!todasConcluidas) {
      const faltantes = etapasObrigatorias.filter(e => !e.concluida).map(e => e.nome).join(", ");
      setAlerta({
        titulo: "Checklist incompleto!",
        mensagem: `Você não anexou as fotos exigidas nas seguintes etapas: ${faltantes}.`
      });
      return;
    }

    setFinalizando(true);
    setErro(null);
    try {
      await api.finalizarLimpeza(usuario.token, id);
      router.push("/operador");
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível finalizar.");
      setFinalizando(false);
    }
  }

  async function toggleCortinas(retiradas: boolean) {
    if (!usuario || !limpeza || !emAndamento) return;
    try {
      await api.sinalizarCortinas(usuario.token, id, retiradas);
      setLimpeza({ ...limpeza, cortinasRetiradas: retiradas });
    } catch (err) {
      setAlerta({
        titulo: "Falha ao atualizar cortinas",
        mensagem: "Houve um erro ao salvar o status das cortinas. Tente novamente."
      });
    }
  }

  if (carregando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-surface">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-10 w-10 animate-spin text-brand" />
          <p className="font-semibold text-ink/60 animate-pulse">Carregando checklist...</p>
        </div>
      </div>
    );
  }

  if (!limpeza) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center p-4 bg-surface">
        <div className="rounded-xl bg-white p-8 text-center shadow-lg border border-line">
          <AlertTriangle className="mx-auto mb-4 h-12 w-12 text-danger" />
          <h2 className="mb-2 text-xl font-bold text-ink">Registro não encontrado</h2>
          <p className="mb-6 text-ink/60">{erro || "Não foi possível localizar esta limpeza."}</p>
          <Button onClick={() => router.push("/operador")}>Voltar para Início</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-surface pb-24">
      <div className="bg-brand text-white shadow-md">
        <div className="mx-auto max-w-3xl px-4 py-4 sm:px-6 flex items-center justify-between">
          <button onClick={() => router.push("/operador")} className="text-sm font-semibold text-white/80 hover:text-white flex items-center gap-1">
             &larr; Voltar para Minhas Limpezas
          </button>
        </div>
      </div>

      <main className="mx-auto max-w-3xl px-4 py-6 sm:px-6">
        
        <div className="mb-6 rounded-2xl bg-white p-5 shadow-sm border border-line">
          <div className="flex flex-col gap-4">
            
            <div className="flex items-start justify-between">
              <div className="flex items-center gap-3">
                <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-brand/10 text-brand">
                  <Bus className="h-6 w-6" />
                </div>
                <div>
                  <h1 className="text-xl font-bold text-ink">Prefixo {limpeza.prefixo}</h1>
                  <p className="text-sm font-medium text-ink/50">
                    {limpeza.numeroOS ? `O.S. ${limpeza.numeroOS}` : "O.S. Pendente"}
                  </p>
                </div>
              </div>
              <span className={`inline-flex items-center rounded-full px-2.5 py-1 text-xs font-bold ${emAndamento ? 'bg-brand/10 text-brand' : 'bg-success/10 text-success'}`}>
                {limpeza.status === "EmAndamento" ? "Em Andamento" : limpeza.status}
              </span>
            </div>

            <div className="flex items-center gap-6 rounded-xl bg-surface/50 p-3 text-sm font-medium text-ink/80">
              <div className="flex items-center gap-2">
                <User className="h-4 w-4 text-ink/40" />
                <span className="truncate max-w-[120px]">Op: {limpeza.nomeOperador}</span>
              </div>
              <div className="h-4 w-px bg-line/50"></div>
              <div className="flex items-center gap-2">
                <Calendar className="h-4 w-4 text-ink/40" />
                <span className="truncate">
                  <strong>Início:</strong>{" "}
                  {new Date(limpeza.iniciadaEm).toLocaleDateString("pt-BR")}
                </span>
              </div>
            </div>

            <div className="mt-2">
              <div className="flex items-end justify-between mb-2">
                <span className="text-sm font-bold text-ink">Progresso do Checklist</span>
                <span className="text-sm font-bold text-brand">{concluidas} / {totalEtapas}</span>
              </div>
              <div className="h-3 w-full overflow-hidden rounded-full bg-surface">
                <div 
                  className="h-full bg-brand transition-all duration-500 ease-out" 
                  style={{ width: `${progresso}%` }}
                ></div>
              </div>
            </div>

          </div>
        </div>

        {erro && <div className="mb-6 rounded-xl bg-danger/10 p-4 text-sm font-medium text-danger border border-danger/20">{erro}</div>}

        <div className="space-y-6">
          <Card className="border-line shadow-sm overflow-hidden border-2 border-brand/20">
            <CardContent className="p-4 bg-brand/5">
              <div className="flex items-start gap-3">
                <input
                  type="checkbox"
                  id="cortinas"
                  checked={limpeza.cortinasRetiradas}
                  onChange={(e) => toggleCortinas(e.target.checked)}
                  disabled={!emAndamento}
                  className="mt-1 h-5 w-5 rounded border-line text-brand focus:ring-brand cursor-pointer"
                />
                <label htmlFor="cortinas" className="flex flex-col cursor-pointer">
                  <span className="text-sm font-semibold text-ink">Cortinas foram retiradas?</span>
                  <span className="text-xs text-ink/60">Marque caso as cortinas tenham sido removidas do veículo</span>
                </label>
              </div>
            </CardContent>
          </Card>

          <div className="flex gap-2 flex-wrap pb-2">
            {limpeza.etapas.map((etapa, idx) => (
              <button
                key={etapa.etapaPadraoId}
                onClick={() => setEtapaAtivaId(etapa.etapaPadraoId)}
                className={`shrink-0 px-4 py-3 rounded-xl border text-sm font-semibold transition-all ${etapaAtivaId === etapa.etapaPadraoId ? "bg-brand text-white border-brand shadow-md" : "bg-white text-ink border-line hover:border-brand/50"} ${etapa.concluida ? "opacity-75" : ""}`}
              >
                <div className="flex items-center gap-2">
                  <span className={`flex h-5 w-5 items-center justify-center rounded-full text-xs ${etapaAtivaId === etapa.etapaPadraoId ? "bg-white text-brand" : "bg-surface text-ink/60"}`}>
                    {idx + 1}
                  </span>
                  {etapa.nome}
                  {etapa.concluida && <CheckCircle className="h-4 w-4 ml-1 text-success" />}
                </div>
              </button>
            ))}
          </div>

          <div className="mt-4">
            {limpeza.etapas.map((etapa) => (
              <div key={etapa.etapaPadraoId} className={etapaAtivaId === etapa.etapaPadraoId ? "block" : "hidden"}>
                <EtapaCard
                  etapa={etapa}
                  desabilitado={!emAndamento}
                  onEnviarFoto={(arquivo) => handleEnviarFoto(etapa.etapaPadraoId, arquivo)}
                  onRemoverFoto={(url) => handleRemoverFoto(etapa.etapaPadraoId, url)}
                  onEnviarItem={(itemId, status, funcionalidade, relato, arquivo) => handleEnviarItemExecucao(etapa.etapaPadraoId, itemId, status, funcionalidade, relato, arquivo)}
                />
              </div>
            ))}
          </div>

          {emAndamento && (
            <div className="sticky bottom-4 left-0 right-0 mt-8 z-40 bg-surface/80 p-2 rounded-2xl backdrop-blur-md border border-line shadow-lg">
              <Button
                size="lg"
                className="w-full h-14 text-lg font-bold rounded-xl shadow-md transition-all hover:scale-[1.02]"
                onClick={handleFinalizarLimpeza}
                disabled={!todasConcluidas || finalizando}
                loading={finalizando}
              >
                {todasConcluidas ? (
                  <>
                    <CheckCircle className="mr-2 h-6 w-6" /> Concluir e Enviar
                  </>
                ) : (
                  <>Finalizar Limpeza ({concluidas}/{totalEtapas})</>
                )}
              </Button>
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
        </div>
      </main>

      {alerta && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-ink/50 backdrop-blur-sm p-4">
          <div className="w-full max-w-sm rounded-2xl bg-white p-6 shadow-2xl animate-in zoom-in-95">
            <h3 className="mb-2 text-lg font-bold text-ink">{alerta.titulo}</h3>
            <p className="mb-6 text-sm text-ink/70">{alerta.mensagem}</p>
            <div className="flex justify-end">
              <Button onClick={() => setAlerta(null)}>Entendi</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
