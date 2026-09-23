"use client";

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { useRouter } from "next/navigation";
import { api } from "./api";
import type { Usuario } from "./types";

const STORAGE_KEY = "catarinense.limpeza.usuario";

interface AuthContextValue {
  usuario: Usuario | null;
  carregando: boolean;
  entrar: (matricula: string, senha: string) => Promise<Usuario>;
  sair: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [carregando, setCarregando] = useState(true);

  useEffect(() => {
    const salvo = localStorage.getItem(STORAGE_KEY);
    if (salvo) {
      try {
        setUsuario(JSON.parse(salvo));
      } catch {
        localStorage.removeItem(STORAGE_KEY);
      }
    }
    setCarregando(false);

    function handleUnauthorized() {
      localStorage.removeItem(STORAGE_KEY);
      setUsuario(null);
    }
    
    window.addEventListener("api-unauthorized", handleUnauthorized);
    return () => window.removeEventListener("api-unauthorized", handleUnauthorized);
  }, []);

  async function entrar(matricula: string, senha: string) {
    const resultado = await api.login(matricula, senha);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(resultado));
    setUsuario(resultado);
    return resultado;
  }

  function sair() {
    localStorage.removeItem(STORAGE_KEY);
    setUsuario(null);
  }

  return (
    <AuthContext.Provider value={{ usuario, carregando, entrar, sair }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const contexto = useContext(AuthContext);
  if (!contexto) throw new Error("useAuth precisa estar dentro de <AuthProvider>");
  return contexto;
}

export function useRequireAuth(perfilExigido?: "Administrador" | "Operador") {
  const { usuario, carregando } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (carregando) return;
    if (!usuario) {
      router.replace("/login");
      return;
    }
    if (perfilExigido && usuario.perfil !== perfilExigido) {
      router.replace(usuario.perfil === "Administrador" ? "/admin" : "/operador");
    }
  }, [usuario, carregando, perfilExigido, router]);

  return { usuario, carregando };
}