export const runtime = 'edge';
"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";
import { api, ApiError } from "@/lib/api";
import { resolverUrlFoto } from "@/lib/fotos";
import { StatusBadge } from "@/components/StatusBadge";
import { Lightbox, useLightbox } from "@/components/Lightbox";
import { Button } from "@/components/ui/Button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { Input } from "@/components/ui/Input";
import { Skeleton } from "@/components/ui/Skeleton";
import { Check, X, ArrowLeft, Mail, Info, User, Calendar, BusFront, AlertTriangle } from "lucide-react";
import type { LimpezaFinaDetalhes } from "@/lib/types";

export default function AvaliarLimpezaPage() {
  const { usuario } = useAuth();
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const [limpeza, setLimpeza] = useState<LimpezaFinaDetalhes | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [processando, setProcessando] = useState(false);
  const [motivoReprovacao, setMotivoReprovacao] = useState("");
  const [mostrarReprovacao, setMostrarReprovacao] = useState(false);
  const [emails, setEmails] = useState("");
  const [mostrarNotificar, setMostrarNotificar] = useState(false);
  const [etapasAprovadas, setEtapasAprovadas] = useState<Record<string, boolean>>({});
  const [numeroOSInput, setNumeroOSInput] = useState("");
  const [salvandoOS, setSalvandoOS] = useState(false);

  const { urlAberta, abrir, fechar } = useLightbox();

  useEffect(() => {
    if (!usuario) return;
    api
      .obterDetalhesLimpeza(id, usuario.token)
      .then((dados) => {
        setLimpeza(dados);
        setNumeroOSInput(dados.numeroOS ?? "");
      })
      .catch((err) =>
        setErro(err instanceof ApiError ? err.message : "Falha ao carregar o registro.")
      );
  }, [usuario, id]);

  async function handleSalvarOS() {
    if (!usuario || !numeroOSInput.trim()) return;
    setErro(null);
    setSalvandoOS(true);
    try {
      const atualizado = await api.definirNumeroOS(usuario.token, id, numeroOSInput.trim());
      setLimpeza(atualizado);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível salvar a O.S.");
    } finally {
      setSalvandoOS(false);
    }
  }

  async function handleAprovar() {
    if (!usuario) return;
    setErro(null);
    setProcessando(true);
    try {
      const atualizado = await api.aprovarLimpeza(usuario.token, id);
      setLimpeza(atualizado);
      setMostrarNotificar(true);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível aprovar.");
    } finally {
      setProcessando(false);
    }
  }

  async function handleReprovar() {
    if (!usuario || !motivoReprovacao.trim()) return;
    setErro(null);
    setProcessando(true);
    try {
      const atualizado = await api.reprovarLimpeza(usuario.token, id, motivoReprovacao.trim());
      setLimpeza(atualizado);
      setMostrarReprovacao(false);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível reprovar.");
    } finally {
      setProcessando(false);
    }
  }

  async function handleNotificar() {
    if (!usuario) return;
    const destinatarios = emails.split(",").map((e) => e.trim()).filter(Boolean);
    if (destinatarios.length === 0) {
      setErro("Informe ao menos um e-mail.");
      return;
    }
    setErro(null);
    setProcessando(true);
    try {
      await api.notificarLimpeza(usuario.token, id, destinatarios);
      setLimpeza((prev) => (prev ? { ...prev, notificacaoEnviada: true } : prev));
      setMostrarNotificar(false);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível enviar notificação.");
    } finally {
      setProcessando(false);
    }
  }

  if (!usuario) return null;

  return (
    <div className="mx-auto max-w-4xl pb-10">
      <button
        onClick={() => router.push("/admin")}
        className="mb-6 flex items-center gap-2 text-sm text-ink/60 transition hover:text-brand"
      >
        <ArrowLeft className="h-4 w-4" /> Voltar para Dashboard
      </button>

      {erro && (
        <div className="mb-6 rounded-md bg-danger/10 p-4 text-sm text-danger">{erro}</div>
      )}

      {!limpeza && !erro && (
        <div className="space-y-4">
          <Skeleton className="h-32 w-full rounded-card" />
          <Skeleton className="h-64 w-full rounded-card" />
        </div>
      )}

      {limpeza && (
        <div className="grid gap-6 md:grid-cols-3">
          <div className="space-y-6 md:col-span-2">
            <Card>
              <CardContent className="p-6">
                <div className="mb-4 flex items-start justify-between">
                  <div className="flex items-center gap-3">
                    <div className="flex h-12 w-12 items-center justify-center rounded-full bg-brand/10">
                      <BusFront className="h-6 w-6 text-brand" />
                    </div>
                    <div>
                      <h1 className="text-2xl font-bold text-ink">Prefixo {limpeza.prefixo}</h1>
                      <p className="text-sm font-medium text-ink/60">
                        {limpeza.numeroOS ? `O.S. ${limpeza.numeroOS}` : "Aguardando O.S."}
                      </p>
                    </div>
                  </div>
                  <StatusBadge status={limpeza.status} />
                </div>

                <div className="grid grid-cols-2 gap-4 rounded-lg bg-surface p-4 text-sm text-ink/80">
                  <div className="flex items-center gap-2">
                    <User className="h-4 w-4 text-ink/40" />
                    <span><strong>Operador:</strong> {limpeza.nomeOperador}</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Calendar className="h-4 w-4 text-ink/40" />
                    <span>
                      <strong>Data:</strong>{" "}
                      {new Date(limpeza.iniciadaEm).toLocaleDateString("pt-BR")}
                    </span>
                  </div>
                </div>

                {limpeza.status === "Reprovada" && limpeza.observacaoAvaliacao && (
                  <div className="mt-4 rounded-md bg-danger/10 p-4 text-sm text-danger">
                    <strong>Motivo:</strong> {limpeza.observacaoAvaliacao}
                  </div>
                )}
              </CardContent>
            </Card>

            <h2 className="text-lg font-bold text-ink">Checklist e Evidências</h2>
            <div className="space-y-4">
              {limpeza.status === "Concluida" && (
                <div className="mb-4 flex items-center justify-between p-3 rounded-xl border border-line bg-surface">
                  <span className="text-sm font-bold text-ink">Marcar todos como aprovados</span>
                  <input 
                    type="checkbox"
                    checked={limpeza.etapas.length > 0 && limpeza.etapas.every(e => etapasAprovadas[e.etapaPadraoId])}
                    onChange={(e) => {
                      const checked = e.target.checked;
                      const newState: Record<string, boolean> = {};
                      limpeza.etapas.forEach(et => newState[et.etapaPadraoId] = checked);
                      setEtapasAprovadas(newState);
                    }}
                    className="w-5 h-5 rounded text-success focus:ring-success cursor-pointer"
                  />
                </div>
              )}
              {limpeza.etapas.map((etapa) => (
                <Card key={etapa.etapaPadraoId} className={etapasAprovadas[etapa.etapaPadraoId] ? "border-success/30 bg-success/5 transition-colors" : "transition-colors"}>
                  <CardContent className="p-5">
                    <div className="mb-3 flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <div className="flex h-6 w-6 items-center justify-center rounded-full bg-success/20 text-success">
                          <Check className="h-4 w-4" />
                        </div>
                        <p className="font-semibold text-ink">{etapa.nome}</p>
                      </div>
                      
                      {limpeza.status === "Concluida" && (
                        <div className="flex items-center gap-2">
                          <label className="text-sm font-semibold text-ink/70 cursor-pointer select-none" htmlFor={`check-${etapa.etapaPadraoId}`}>
                            Aprovar etapa
                          </label>
                          <input 
                            type="checkbox"
                            id={`check-${etapa.etapaPadraoId}`}
                            checked={!!etapasAprovadas[etapa.etapaPadraoId]}
                            onChange={(e) => setEtapasAprovadas(prev => ({...prev, [etapa.etapaPadraoId]: e.target.checked}))}
                            className="w-5 h-5 rounded text-brand focus:ring-brand cursor-pointer"
                          />
                        </div>
                      )}
                    </div>
                    
                    {etapa.problemaExtraDescricao && (
                      <div className="mb-4 rounded-lg border border-danger/20 bg-danger/5 p-4 text-sm">
                        <div className="font-semibold text-danger mb-1">Problema Extra Reportado:</div>
                        <p className="text-danger/80">{etapa.problemaExtraDescricao}</p>
                        {etapa.problemaExtraFotoUrl && (
                          <div className="mt-2 block">
                            <button onClick={() => abrir(resolverUrlFoto(etapa.problemaExtraFotoUrl!))} className="block cursor-zoom-in">
                              <img src={resolverUrlFoto(etapa.problemaExtraFotoUrl)} alt="Problema" className="h-16 rounded border border-line" />
                            </button>
                          </div>
                        )}
                      </div>
                    )}

                    {(!etapa.itens || etapa.itens.length === 0) && (
                      <p className="text-sm text-ink/40">Sem itens registrados.</p>
                    )}
                    
                    <div className="flex flex-col gap-3 w-full mt-2">
                      {etapa.itens?.map((item) => (
                        <div key={item.id} className={`p-3 rounded-lg border ${item.concluida ? 'border-success/20 bg-success/5' : 'border-line bg-surface/30'}`}>
                          <div className="flex gap-2">
                            {item.concluida ? (
                              <Check className="w-4 h-4 text-success mt-0.5 shrink-0" />
                            ) : (
                              <div className="w-4 h-4 rounded-sm border-2 border-line mt-0.5 shrink-0" />
                            )}
                            <div className="flex-1">
                              <p className={`text-sm font-medium ${item.concluida ? 'text-ink' : 'text-ink/60'}`}>{item.texto}</p>
                              
                              {item.concluida && (item.status || item.funcionalidade || item.relatoProblema) && (
                                <div className="mt-2 grid grid-cols-2 gap-2 text-xs">
                                  {item.status && item.status !== "N/A" && (
                                    <div><span className="font-semibold opacity-70">Status:</span> {item.status}</div>
                                  )}
                                  {item.funcionalidade && item.funcionalidade !== "N/A" && (
                                    <div><span className="font-semibold opacity-70">Funcionalidade:</span> {item.funcionalidade}</div>
                                  )}
                                  {item.relatoProblema && (
                                    <div className="col-span-2 text-danger"><span className="font-bold">Problema:</span> {item.relatoProblema}</div>
                                  )}
                                </div>
                              )}
                              
                              {item.concluida && item.fotoUrl && (
                                <div className="mt-2">
                                  {item.fotoDuplicadaPrefixo && (
                                      <div className="mb-2 inline-flex items-center gap-1.5 rounded-md bg-warning/10 px-2.5 py-1 text-xs font-bold text-warning border border-warning/20">
                                          <AlertTriangle className="h-3.5 w-3.5" />
                                          Foto duplicada! Usada primeiro no prefixo {item.fotoDuplicadaPrefixo}
                                      </div>
                                  )}
                                  <button onClick={() => abrir(resolverUrlFoto(item.fotoUrl!))} className="block cursor-zoom-in">
                                    <img src={resolverUrlFoto(item.fotoUrl)} alt="Evidência do item" className="h-20 w-20 object-cover rounded border border-line hover:opacity-80 transition" />
                                  </button>
                                </div>
                              )}
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>

          <div className="space-y-6">
            <Card>
              <CardHeader className="border-b border-line bg-surface/50">
                <CardTitle className="flex items-center gap-2 text-base">
                  <Info className="h-4 w-4" /> Ações do Avaliador
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-5 p-5">
                {!limpeza.notificacaoEnviada ? (
                  <div className="space-y-3">
                    <Input
                      label="Nº da O.S. (Protheus)"
                      placeholder="Ex.: OS-2026-00891"
                      value={numeroOSInput}
                      onChange={(e) => setNumeroOSInput(e.target.value)}
                    />
                    <Button
                      variant="secondary"
                      className="w-full"
                      onClick={handleSalvarOS}
                      loading={salvandoOS}
                      disabled={!numeroOSInput.trim() || numeroOSInput.trim() === limpeza.numeroOS}
                    >
                      Salvar O.S.
                    </Button>
                  </div>
                ) : (
                  <div className="rounded bg-surface p-3 text-center text-sm text-ink/60">
                    O.S. não pode ser alterada após notificação.
                  </div>
                )}

                <div className="h-px bg-line" />

                {limpeza.status === "Concluida" && (
                  <div className="space-y-3">
                    {!limpeza.numeroOS && (
                      <p className="text-center text-xs text-danger">
                        Preencha a O.S. para aprovar.
                      </p>
                    )}
                    <Button
                      className="w-full bg-success hover:bg-success/90"
                      onClick={handleAprovar}
                      disabled={processando || !limpeza.numeroOS || !limpeza.etapas.every(e => etapasAprovadas[e.etapaPadraoId])}
                    >
                      <Check className="mr-2 h-4 w-4" /> Aprovar Limpeza
                    </Button>
                    <Button
                      variant="outline"
                      className="w-full border-danger text-danger hover:bg-danger/10"
                      onClick={() => setMostrarReprovacao(!mostrarReprovacao)}
                      disabled={processando}
                    >
                      <X className="mr-2 h-4 w-4" /> Reprovar
                    </Button>
                    {mostrarReprovacao && (
                      <div className="space-y-2 rounded-md bg-danger/5 p-3">
                        <textarea
                          value={motivoReprovacao}
                          onChange={(e) => setMotivoReprovacao(e.target.value)}
                          rows={3}
                          className="w-full rounded-md border border-line p-2 text-sm focus:border-brand focus:outline-none"
                          placeholder="Descreva o motivo..."
                        />
                        <Button
                          variant="danger"
                          size="sm"
                          className="w-full"
                          onClick={handleReprovar}
                          disabled={processando || !motivoReprovacao.trim()}
                        >
                          Confirmar Reprovação
                        </Button>
                      </div>
                    )}
                  </div>
                )}

                {limpeza.status === "Aprovada" && (
                  <div className="space-y-3">
                    {limpeza.notificacaoEnviada ? (
                      <div className="flex flex-col items-center gap-1 rounded-md bg-success/10 p-4 text-center text-success">
                        <Check className="h-6 w-6" />
                        <span className="text-sm font-medium">Notificação Enviada</span>
                      </div>
                    ) : mostrarNotificar ? (
                      <div className="space-y-2">
                        <Input
                          label="E-mails (vírgula)"
                          placeholder="email@empresa.com"
                          value={emails}
                          onChange={(e) => setEmails(e.target.value)}
                        />
                        <Button className="w-full" onClick={handleNotificar} loading={processando}>
                          Enviar E-mail
                        </Button>
                      </div>
                    ) : (
                      <Button className="w-full" onClick={() => setMostrarNotificar(true)}>
                        <Mail className="mr-2 h-4 w-4" /> Notificar Conclusão
                      </Button>
                    )}
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </div>
      )}

      <Lightbox url={urlAberta} onClose={fechar} />
    </div>
  );
}