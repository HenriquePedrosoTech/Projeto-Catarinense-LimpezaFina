"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";
import { api, ApiError } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { Card, CardContent } from "@/components/ui/Card";
import { Input } from "@/components/ui/Input";
import { BusFront, ArrowRight } from "lucide-react";

export default function NovaLimpezaPage() {
  const { usuario } = useAuth();
  const router = useRouter();
  const [prefixo, setPrefixo] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!usuario) return;
    
    const prefStr = prefixo.trim().toUpperCase();
    if (!prefStr) {
      setErro("Informe o prefixo do ônibus");
      return;
    }

    setErro(null);
    setCarregando(true);

    try {
      const limpeza = await api.iniciarLimpeza(usuario.token, prefStr);
      router.push(`/operador/limpeza/${limpeza.id}`);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao iniciar limpeza.");
      setCarregando(false);
    }
  }

  if (!usuario) return null;

  return (
    <main className="flex min-h-[calc(100vh-140px)] items-center justify-center p-4">
      <div className="w-full max-w-sm space-y-6">
        <div className="text-center">
          <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-brand/10">
            <BusFront className="h-8 w-8 text-brand" />
          </div>
          <h1 className="text-2xl font-bold tracking-tight text-ink">Nova Limpeza</h1>
          <p className="mt-1 text-sm text-ink/60">
            Informe o prefixo do veículo que será limpo.
          </p>
        </div>

        <Card className="border-brand/20 shadow-md">
          <CardContent className="p-6">
            <form onSubmit={handleSubmit} className="space-y-5">
              {erro && (
                <div className="rounded-md bg-danger/10 p-3 text-sm text-danger">
                  {erro}
                </div>
              )}
              
              <div className="space-y-1">
                <label className="text-sm font-semibold text-ink">
                  Prefixo do Ônibus
                </label>
                <input
                  type="text"
                  placeholder="EX: 4192"
                  value={prefixo}
                  onChange={(e) => setPrefixo(e.target.value.toUpperCase())}
                  className="w-full rounded-md border border-line bg-surface p-3 text-center text-2xl font-bold uppercase tracking-widest text-ink focus:border-brand focus:bg-white focus:outline-none focus:ring-1 focus:ring-brand"
                  autoFocus
                />
              </div>

              <Button
                type="submit"
                className="w-full py-6 text-lg"
                loading={carregando}
                disabled={!prefixo.trim()}
              >
                Iniciar Checklist <ArrowRight className="ml-2 h-5 w-5" />
              </Button>
            </form>
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
