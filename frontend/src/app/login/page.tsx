"use client";

import { useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";
import { ApiError } from "@/lib/api";
import { Logo, LogoJCA } from "@/components/Logo";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { ShieldCheck } from "lucide-react";

export default function LoginPage() {
  const { entrar } = useAuth();
  const router = useRouter();

  const [matricula, setMatricula] = useState("");
  const [senha, setSenha] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setEnviando(true);
    try {
      const usuario = await entrar(matricula, senha);
      router.push(usuario.perfil === "Administrador" ? "/admin" : "/operador");
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Não foi possível conectar ao servidor.");
    } finally {
      setEnviando(false);
    }
  }

  return (
    <div className="flex min-h-screen">
      {/* Painel Esquerdo — Identidade Visual */}
      <div className="relative hidden w-1/2 flex-col justify-between bg-ink p-12 lg:flex">
        {/* Padrão de grade sutil no fundo */}
        <div
          className="absolute inset-0 opacity-5"
          style={{
            backgroundImage:
              "linear-gradient(#ffffff 1px, transparent 1px), linear-gradient(90deg, #ffffff 1px, transparent 1px)",
            backgroundSize: "40px 40px",
          }}
        />

        {/* Logo no topo */}
        <div className="relative">
          <div className="flex items-center gap-3">
            <svg width="36" height="36" viewBox="0 0 28 28" fill="none" aria-hidden="true">
              <rect width="28" height="28" rx="6" fill="#C41230" />
              <path
                d="M6 19.5V8.5H14.5C17 8.5 19 10.3 19 13C19 14.6 18.2 15.9 17 16.7L19.5 19.5H16.3L14.3 17.2H9.2V19.5H6ZM9.2 14.6H14.2C15.2 14.6 15.9 13.9 15.9 13C15.9 12.1 15.2 11.4 14.2 11.4H9.2V14.6Z"
                fill="white"
              />
            </svg>
            <div>
              <p className="text-lg font-bold text-white">Catarinense</p>
              <p className="text-xs font-medium text-white/50 tracking-widest uppercase">Limpeza Fina</p>
            </div>
          </div>
        </div>

        {/* Mensagem Central */}
        <div className="relative">
          <div className="mb-6 flex h-14 w-14 items-center justify-center rounded-2xl bg-brand shadow-lg shadow-brand/30">
            <ShieldCheck className="h-8 w-8 text-white" />
          </div>
          <h2 className="mb-4 text-4xl font-bold leading-tight text-white">
            Controle total da sua frota
          </h2>
          <p className="text-lg leading-relaxed text-white/60">
            Registre, comprove e aprove cada etapa da limpeza fina dos ônibus com evidências fotográficas.
          </p>

          <div className="mt-10 flex gap-6">
            {[
              { label: "Registro Digital", desc: "Checklists com fotos" },
              { label: "Rastreabilidade", desc: "Quem fez e quando" },
              { label: "Aprovação Rápida", desc: "Workflow em tempo real" },
            ].map((item) => (
              <div key={item.label}>
                <p className="text-sm font-bold text-white">{item.label}</p>
                <p className="text-xs text-white/50">{item.desc}</p>
              </div>
            ))}
          </div>
        </div>

        {/* Rodapé */}
        <div className="relative text-xs text-white/30">
          <LogoJCA />
        </div>
      </div>

      {/* Painel Direito — Formulário */}
      <div className="flex flex-1 flex-col items-center justify-center bg-surface px-6 py-12">
        {/* Logo visível apenas no mobile */}
        <div className="mb-10 lg:hidden">
          <Logo />
        </div>

        <div className="w-full max-w-sm">
          <div className="mb-8">
            <h1 className="text-2xl font-bold tracking-tight text-ink">Entrar no sistema</h1>
            <p className="mt-1 text-sm text-ink/60">
              Use sua matrícula e senha para acessar.
            </p>
          </div>

          <form onSubmit={handleSubmit} className="flex flex-col gap-5">
            <Input
              id="matricula"
              label="Matrícula"
              type="text"
              required
              autoFocus
              value={matricula}
              onChange={(e) => setMatricula(e.target.value)}
              placeholder="Ex.: 00123"
            />

            <Input
              id="senha"
              label="Senha"
              type="password"
              required
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              placeholder="••••••••"
            />

            {erro && (
              <div
                className="rounded-md border border-danger/20 bg-danger/10 px-4 py-3 text-sm text-danger"
                role="alert"
              >
                {erro}
              </div>
            )}

            <Button
              type="submit"
              size="lg"
              className="mt-2 w-full"
              loading={enviando}
            >
              {enviando ? "Autenticando..." : "Acessar o Sistema"}
            </Button>
          </form>

          <p className="mt-8 text-center text-xs text-ink/40">
            Problemas para acessar? Fale com o administrador do sistema.
          </p>
        </div>
      </div>
    </div>
  );
}
