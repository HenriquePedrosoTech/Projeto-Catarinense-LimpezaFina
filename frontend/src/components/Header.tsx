"use client";

import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { Logo } from "./Logo";

export function Header() {
  const { usuario, sair } = useAuth();

  return (
    <header className="border-b border-line bg-white">
      <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
        <Link href={usuario?.perfil === "Administrador" ? "/admin" : "/operador"}>
          <Logo />
        </Link>

        {usuario && (
          <div className="flex items-center gap-4">
            <div className="text-right leading-tight">
              <p className="text-sm font-medium text-ink">{usuario.nome}</p>
              <p className="text-xs text-ink/50">
                Matrícula {usuario.matricula} · {usuario.perfil}
              </p>
            </div>
            <button
              onClick={sair}
              className="rounded-md border border-line px-3 py-1.5 text-sm text-ink/70 transition hover:border-brand hover:text-brand"
            >
              Sair
            </button>
          </div>
        )}
      </div>
    </header>
  );
}
