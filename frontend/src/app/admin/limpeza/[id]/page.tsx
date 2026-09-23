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
import { Check, X, ArrowLeft, Mail, Info, User, Calendar, BusFront } from "lucide-react";
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
              {limpeza.etapas.map((etapa) => (
                <Card key={etapa.etapaPadraoId}>
                  <CardContent className="p-5">
                    <div className="mb-3 flex items-center gap-2">
                      <div className="flex h-6 w-6 items-center justify-center rounded-full bg-success/20 text-success">
                        <Check className="h-4 w-4" />
                      </div>
                      <p className="font-semibold text-ink">{etapa.nome}</p>
                    </div>
                    <div className="flex gap-3 overflow-x-auto">
                      {etapa.fotos.length === 0 && (
                        <p className="text-sm text-ink/40">Sem fotos registradas.</p>
                      )}
                      {etapa.fotos.map((url) => {
                        const urlCompleta = resolverUrlFoto(url);
                        return (
                          // eslint-disable-next-line @next/next/no-img-element
                          <img
                            key={url}
                            src={urlCompleta}
                            alt="Evidência"
                            onClick={() => abrir(urlCompleta)}
                            className="h-28 w-28 shrink-0 cursor-zoom-in rounded-md border border-line object-cover transition hover:opacity-80"
                          />
                        );
                      })}
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
                      disabled={processando || !limpeza.numeroOS}
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