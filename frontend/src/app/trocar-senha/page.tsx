"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth, useRequireAuth } from "@/lib/auth";
import { api } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Eye, EyeOff } from "lucide-react";

export default function TrocarSenhaPage() {
  const { usuario, carregando } = useRequireAuth(undefined, true);
  const { atualizarUsuario, sair } = useAuth();
  const router = useRouter();

  const [senha, setSenha] = useState("");
  const [confirmarSenha, setConfirmarSenha] = useState("");
  const [mostrarSenha, setMostrarSenha] = useState(false);
  const [mostrarConfirmarSenha, setMostrarConfirmarSenha] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [salvando, setSalvando] = useState(false);

  if (carregando || !usuario) return <p className="p-8 text-ink/50 text-center">Carregando...</p>;

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErro(null);

    if (senha.length < 8) {
      setErro("A nova senha deve ter no mínimo 8 caracteres.");
      return;
    }

    if (senha !== confirmarSenha) {
      setErro("As senhas não coincidem.");
      return;
    }

    setSalvando(true);
    try {
      await api.trocarSenha(usuario!.token, senha);
      atualizarUsuario({ ...usuario!, precisaTrocarSenha: false });
      router.replace(usuario!.perfil === "Administrador" ? "/admin" : "/operador");
    } catch (error: any) {
      setErro(error.message || "Erro ao trocar senha.");
    } finally {
      setSalvando(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-surface p-4">
      <div className="w-full max-w-sm rounded-xl border bg-white p-8 shadow-sm">
        <h1 className="mb-2 text-2xl font-bold tracking-tight text-ink">Troca Obrigatória</h1>
        <p className="mb-6 text-sm text-ink/60">
          Como este é o seu primeiro acesso, é necessário cadastrar uma nova senha para continuar.
        </p>

        {erro && <div className="mb-4 rounded-md bg-red-50 p-3 text-sm text-red-600">{erro}</div>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-1.5">
            <label className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70" htmlFor="senha">Nova Senha</label>
            <Input
              id="senha"
              type={mostrarSenha ? "text" : "password"}
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              disabled={salvando}
              required
              rightElement={
                <button
                  type="button"
                  onClick={() => setMostrarSenha(!mostrarSenha)}
                  className="text-ink/40 hover:text-ink/60 focus:outline-none"
                  tabIndex={-1}
                >
                  {mostrarSenha ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              }
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70" htmlFor="confirmarSenha">Confirmar Nova Senha</label>
            <Input
              id="confirmarSenha"
              type={mostrarConfirmarSenha ? "text" : "password"}
              value={confirmarSenha}
              onChange={(e) => setConfirmarSenha(e.target.value)}
              disabled={salvando}
              required
              rightElement={
                <button
                  type="button"
                  onClick={() => setMostrarConfirmarSenha(!mostrarConfirmarSenha)}
                  className="text-ink/40 hover:text-ink/60 focus:outline-none"
                  tabIndex={-1}
                >
                  {mostrarConfirmarSenha ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              }
            />
          </div>

          <div className="pt-2 flex flex-col gap-2">
            <Button type="submit" className="w-full" disabled={salvando}>
              {salvando ? "Salvando..." : "Salvar Senha e Entrar"}
            </Button>
            <Button type="button" variant="outline" className="w-full" disabled={salvando} onClick={sair}>
              Sair
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
