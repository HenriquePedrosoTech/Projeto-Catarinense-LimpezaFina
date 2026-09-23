"use client";

import { useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { useAuth } from "@/lib/auth";
import { ApiError } from "@/lib/api";
import { Logo, LogoJCA } from "@/components/Logo";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";

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

        {/* Topo vazio para espaçamento */}
        <div className="relative"></div>

        {/* Mensagem Central com Logo Grande */}
        <div className="relative">
          <div className="mb-8 flex items-center gap-4">
            <Image 
              src="/logo.png" 
              alt="Logo Catarinense" 
              width={72} 
              height={72} 
              className="rounded-lg object-contain shrink-0 bg-white p-1" 
              priority 
            />
            <div>
              <p className="text-3xl font-bold text-white tracking-tight">Catarinense</p>
              <p className="text-sm font-bold text-white/60 tracking-[0.2em] uppercase mt-1">Limpeza Fina</p>
            </div>
          </div>
          
          <h2 className="mb-4 text-3xl font-bold leading-tight text-white/90">
            Controle total da sua frota
          </h2>
          <p className="text-lg leading-relaxed text-white/60 max-w-md">
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
        <div className="mb-10 lg:hidden scale-110">
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