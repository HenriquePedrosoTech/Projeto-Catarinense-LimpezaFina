"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { api, ApiError } from "@/lib/api";
import { StatusBadge } from "@/components/StatusBadge";
import { Card, CardContent } from "@/components/ui/Card";
import { Skeleton } from "@/components/ui/Skeleton";
import { ClipboardCheck, Bus, Search } from "lucide-react";
import type { LimpezaFinaResumo } from "@/lib/types";

export default function OperadorHomePage() {
  const { usuario } = useAuth();
  const [limpezas, setLimpezas] = useState<LimpezaFinaResumo[] | null>(null);
  const [pesquisa, setPesquisa] = useState("");
  const [dataPesquisa, setDataPesquisa] = useState("");
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    if (!usuario) return;
    api
      .listarLimpezasPorOperador(usuario.token, usuario.usuarioId)
      .then(setLimpezas)
      .catch((err) => setErro(err instanceof ApiError ? err.message : "Falha ao carregar suas limpezas."));
  }, [usuario]);

  
  const limpezasFiltradas = limpezas?.filter((l) => {
    const matchPrefixo = l.prefixo.toLowerCase().includes(pesquisa.toLowerCase()) || 
                         (l.numeroOS?.toLowerCase().includes(pesquisa.toLowerCase()));
    
    if (dataPesquisa) {
      const dataFormatada = new Date(l.iniciadaEm).toISOString().split('T')[0];
      return matchPrefixo && dataFormatada === dataPesquisa;
    }
    
    return matchPrefixo;
  });

  if (!usuario) return null;

  return (
    <div className="mx-auto max-w-lg">
      <div className="mb-6">
        <h1 className="text-2xl font-bold tracking-tight text-ink">Histórico</h1>
        <p className="text-sm text-ink/60">Acompanhe as limpezas que você realizou.</p>
      </div>


      <div className="mb-4 flex flex-col sm:flex-row gap-2">
        <div className="relative flex-1">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Search className="h-4 w-4 text-ink/40" />
          </div>
          <input
            type="text"
            placeholder="Pesquisar prefixo ou O.S..."
            value={pesquisa}
            onChange={(e) => setPesquisa(e.target.value)}
            className="block w-full pl-9 pr-3 py-2 border border-line rounded-xl text-sm focus:ring-brand-blue focus:border-brand-blue bg-white"
          />
        </div>
        <input
          type="date"
          value={dataPesquisa}
          onChange={(e) => setDataPesquisa(e.target.value)}
          className="block w-full sm:w-auto px-3 py-2 border border-line rounded-xl text-sm focus:ring-brand-blue focus:border-brand-blue bg-white"
        />
      </div>


      {erro && <div className="mb-4 rounded-md bg-danger/10 p-4 text-sm text-danger">{erro}</div>}

      {!limpezas && !erro && (
        <div className="space-y-3">
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} className="h-24 w-full rounded-card" />
          ))}
        </div>
      )}

      {limpezasFiltradas && limpezasFiltradas.length === 0 && (
        <div className="flex flex-col items-center justify-center rounded-card border border-dashed border-line bg-white py-12 text-center shadow-sm">
          <div className="mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-surface">
            <ClipboardCheck className="h-6 w-6 text-ink/40" />
          </div>
          <h3 className="font-semibold text-ink">Nenhuma limpeza ainda</h3>
          <p className="mt-1 text-sm text-ink/50">Toque em "Nova Limpeza" abaixo para começar.</p>
        </div>
      )}

      {limpezasFiltradas && limpezasFiltradas.length > 0 && (
        <div className="grid gap-3">
          {limpezasFiltradas.map((l) => (
            <Link
              key={l.id}
              href={l.status === "EmAndamento" ? `/operador/limpeza/${l.id}` : `/limpezas/${l.id}`}
            >
              <Card
                className={`group transition-all active:scale-[0.98] ${
                  l.status === "EmAndamento" ? "border-brand-blue/40 shadow-sm" : ""
                }`}
              >
                <CardContent className="p-4">
                  <div className="mb-2 flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <Bus className="h-5 w-5 text-ink/40" />
                      <span className="text-lg font-bold text-ink">{l.prefixo}</span>
                    </div>
                    <StatusBadge status={l.status} />
                  </div>
                  <div className="text-sm text-ink/60">
                    <p>
                      Iniciada em{" "}
                      {new Date(l.iniciadaEm).toLocaleDateString("pt-BR", {
                        day: "2-digit",
                        month: "short",
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </p>
                    {l.numeroOS && (
                      <p className="mt-0.5 font-medium text-ink/80">O.S. {l.numeroOS}</p>
                    )}
                  </div>
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}

