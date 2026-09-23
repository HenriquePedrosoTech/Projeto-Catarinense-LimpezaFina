"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";

export default function HomePage() {
  const { usuario, carregando } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (carregando) return;
    if (!usuario) {
      router.replace("/login");
    } else {
      router.replace(usuario.perfil === "Administrador" ? "/admin" : "/operador");
    }
  }, [usuario, carregando, router]);

  return (
    <div className="flex min-h-screen items-center justify-center">
      <p className="text-ink/50">Carregando…</p>
    </div>
  );
}
