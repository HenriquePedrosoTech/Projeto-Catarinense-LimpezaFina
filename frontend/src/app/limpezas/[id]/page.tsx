"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { api, ApiError } from "@/lib/api";
import { resolverUrlFoto } from "@/lib/fotos";
import { StatusBadge } from "@/components/StatusBadge";
import { Lightbox, useLightbox } from "@/components/Lightbox";
import { Card, CardContent } from "@/components/ui/Card";
import { Skeleton } from "@/components/ui/Skeleton";
import { Bus, CheckCircle, Calendar, User, Info } from "lucide-react";
import type { LimpezaFinaDetalhes } from "@/lib/types";

/**
 * Página sem exigência de login - é o link que vai no corpo do e-mail de notificação.
 * O acesso é protegido apenas pela imprevisibilidade do GUID na URL.
 */
export default function DetalhesPublicosPage() {
  const { id } = useParams<{ id: string }>();
  const [limpeza, setLimpeza] = useState<LimpezaFinaDetalhes | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const { urlAberta, abrir, fechar } = useLightbox();

  useEffect(() => {
    api
      .obterDetalhesLimpeza(id)
      .then(setLimpeza)
      .catch((err) => setErro(err instanceof ApiError ? err.message : "Registro não encontrado."));
  }, [id]);

  return (
    <div className="min-h-screen bg-surface selection:bg-brand/20">
      <header className="border-b border-line bg-white shadow-sm">
        <div className="mx-auto flex max-w-3xl items-center justify-between px-6 py-4">
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-md bg-brand text-white font-bold">
              C
            </div>
            <span className="font-bold tracking-tight text-ink">Catarinense</span>
          </div>
          <span className="text-xs font-semibold uppercase tracking-wider text-ink/40">
            Relatório de Limpeza Fina
          </span>
        </div>
      </header>

      <main className="mx-auto max-w-3xl px-4 py-8">
        {erro && (
          <div className="rounded-md bg-danger/10 p-4 text-center text-sm font-medium text-danger">
            {erro}
          </div>
        )}

        {!limpeza && !erro && (
          <div className="space-y-6">
            <Skeleton className="h-48 w-full rounded-card" />
            <Skeleton className="h-64 w-full rounded-card" />
          </div>
        )}

        {limpeza && (
          <>
            <Card className="mb-8 overflow-hidden shadow-md border-line">
              <div className="h-2 bg-brand w-full" />
              <CardContent className="p-6">
                <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
                  <div className="flex items-center gap-4">
                    <div className="flex h-16 w-16 items-center justify-center rounded-full bg-brand/10">
                      <Bus className="h-8 w-8 text-brand" />
                    </div>
                    <div>
                      <h1 className="text-2xl font-bold text-ink">Ônibus {limpeza.prefixo}</h1>
                      <p className="font-medium text-ink/60">
                        O.S. {limpeza.numeroOS ?? "Não informada"}
                      </p>
                    </div>
                  </div>
                  <StatusBadge status={limpeza.status} />
                </div>

                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 rounded-lg bg-surface p-4">
                  <div className="flex items-center gap-3">
                    <User className="h-5 w-5 text-ink/40" />
                    <div>
                      <p className="text-xs font-medium text-ink/50 uppercase tracking-wide">Operador</p>
                      <p className="font-semibold text-ink">{limpeza.nomeOperador}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <Calendar className="h-5 w-5 text-ink/40" />
                    <div>
                      <p className="text-xs font-medium text-ink/50 uppercase tracking-wide">Finalização</p>
                      <p className="font-semibold text-ink">
                        {limpeza.finalizadaEm 
                          ? new Date(limpeza.finalizadaEm).toLocaleString("pt-BR")
                          : "Em Andamento"}
                      </p>
                    </div>
                  </div>
                </div>

                {limpeza.status === "Aprovada" && (
                  <div className="mt-4 flex items-center gap-2 rounded-md bg-success/10 p-3 text-sm text-success">
                    <CheckCircle className="h-5 w-5 shrink-0" />
                    <p>Esta limpeza foi inspecionada e <strong>aprovada</strong> pelo controle de qualidade.</p>
                  </div>
                )}
                {limpeza.status === "Reprovada" && limpeza.observacaoAvaliacao && (
                  <div className="mt-4 flex items-start gap-2 rounded-md bg-danger/10 p-3 text-sm text-danger">
                    <Info className="mt-0.5 h-5 w-5 shrink-0" />
                    <div>
                      <p className="font-bold">Limpeza Reprovada</p>
                      <p className="mt-1">{limpeza.observacaoAvaliacao}</p>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>

            <h2 className="mb-4 text-lg font-bold text-ink">Evidências do Checklist</h2>

            <div className="grid gap-4 sm:grid-cols-2">
              {limpeza.etapas.map((etapa) => (
                <Card key={etapa.etapaPadraoId} className="overflow-hidden hover:shadow-md transition-shadow">
                  <CardContent className="p-0">
                    <div className="border-b border-line bg-surface/50 p-4">
                      <div className="flex items-center gap-2">
                        <CheckCircle className="h-5 w-5 text-success" />
                        <p className="font-semibold text-ink">{etapa.nome}</p>
                      </div>
                    </div>
                    <div className="p-4">
                      {etapa.fotos.length === 0 ? (
                        <p className="text-center text-sm text-ink/40 py-8">Nenhuma foto registrada</p>
                      ) : (
                        <div className="grid grid-cols-2 gap-2">
                          {etapa.fotos.map((url) => {
                            const urlCompleta = resolverUrlFoto(url);
                            return (
                              // eslint-disable-next-line @next/next/no-img-element
                              <img
                                key={url}
                                src={urlCompleta}
                                alt={`Evidência - ${etapa.nome}`}
                                onClick={() => abrir(urlCompleta)}
                                className="aspect-square w-full cursor-zoom-in rounded-md border border-line object-cover transition hover:opacity-80 shadow-sm"
                              />
                            );
                          })}
                        </div>
                      )}
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          </>
        )}

        <div className="mt-12 mb-8 text-center">
          <p className="text-xs font-medium uppercase tracking-widest text-ink/30">
            Grupo JCA • Controle de Qualidade
          </p>
        </div>
      </main>

      <Lightbox url={urlAberta} onClose={fechar} />
    </div>
  );
}
