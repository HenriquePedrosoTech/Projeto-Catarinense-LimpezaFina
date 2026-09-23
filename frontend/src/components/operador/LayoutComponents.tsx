"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { ClipboardList, PlusCircle, LogOut } from "lucide-react";
import { useAuth } from "@/lib/auth";
import { Logo } from "@/components/Logo";

export function OperadorTopbar() {
  const { usuario, sair } = useAuth();

  return (
    <header className="fixed left-0 right-0 top-0 z-50 flex h-16 items-center justify-between border-b border-line bg-white px-4 shadow-sm">
      <Logo />
      
      {usuario && (
        <div className="flex items-center gap-3">
          <span className="hidden text-sm font-medium text-ink/80 sm:block">Olá, {usuario.nome.split(" ")[0]}</span>
          <button
            onClick={sair}
            className="flex h-9 w-9 items-center justify-center rounded-full bg-surface text-ink/60 transition hover:bg-danger/10 hover:text-danger"
            title="Sair"
          >
            <LogOut className="h-4 w-4" />
          </button>
        </div>
      )}
    </header>
  );
}

export function OperadorBottomNav() {
  const pathname = usePathname();

  const links = [
    { href: "/operador", label: "Minhas Limpezas", icon: ClipboardList },
    { href: "/operador/nova", label: "Nova Limpeza", icon: PlusCircle },
  ];

  return (
    <nav className="fixed bottom-0 left-0 right-0 z-50 flex h-16 border-t border-line bg-white pb-safe">
      {links.map((link) => {
        const isActive = pathname === link.href;
        const Icon = link.icon;
        return (
          <Link
            key={link.href}
            href={link.href}
            className={`flex flex-1 flex-col items-center justify-center gap-1 transition-colors ${
              isActive ? "text-brand" : "text-ink/50 hover:text-ink/80"
            }`}
          >
            <Icon className={`h-6 w-6 ${isActive ? "fill-brand/10" : ""}`} />
            <span className="text-[10px] font-medium">{link.label}</span>
          </Link>
        );
      })}
    </nav>
  );
}

