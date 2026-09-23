"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, FileText, Database, Settings, LogOut, Menu } from "lucide-react";
import { useAuth } from "@/lib/auth";
import { Logo } from "@/components/Logo";

export function AdminSidebar({ isOpen, onClose }: { isOpen?: boolean; onClose?: () => void }) {
  const pathname = usePathname();
  const { sair } = useAuth();

  const links = [
    { href: "/admin", label: "Dashboard", icon: LayoutDashboard },
    { href: "/admin/relatorio", label: "Relatórios", icon: FileText },
    { href: "/admin/cadastros", label: "Cadastros Base", icon: Database },
  ];

  const sidebarContent = (
    <>
      <div className="flex h-16 shrink-0 items-center border-b border-line px-6">
        <Logo />
      </div>

      <nav className="flex-1 space-y-1 p-4 overflow-y-auto">
        {links.map((link) => {
          const isActive = pathname === link.href;
          const Icon = link.icon;
          return (
            <Link
              key={link.href}
              href={link.href}
              onClick={onClose}
              className={`flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors ${
                isActive
                  ? "bg-brand/10 text-brand"
                  : "text-ink/60 hover:bg-surface hover:text-ink"
              }`}
            >
              <Icon className="h-5 w-5" />
              {link.label}
            </Link>
          );
        })}
      </nav>

      <div className="shrink-0 border-t border-line p-4">
        <button
          onClick={sair}
          className="flex w-full items-center gap-3 rounded-md px-3 py-2 text-sm font-medium text-ink/60 transition-colors hover:bg-danger/10 hover:text-danger"
        >
          <LogOut className="h-5 w-5" />
          Sair do sistema
        </button>
      </div>
    </>
  );

  return (
    <>
      {/* Desktop Sidebar */}
      <aside className="hidden w-72 flex-col border-r border-line bg-white md:flex">
        {sidebarContent}
      </aside>

      {/* Mobile Sidebar Overlay */}
      {isOpen && (
        <div className="fixed inset-0 z-50 flex md:hidden">
          {/* Backdrop */}
          <div 
            className="fixed inset-0 bg-ink/50 backdrop-blur-sm transition-opacity" 
            onClick={onClose}
          />
          {/* Menu Panel */}
          <aside className="relative flex w-[80%] max-w-sm flex-col bg-white shadow-xl">
            {onClose && (
              <button 
                onClick={onClose}
                className="absolute right-4 top-4 rounded-md p-2 text-ink/50 hover:bg-surface hover:text-ink"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>
              </button>
            )}
            {sidebarContent}
          </aside>
        </div>
      )}
    </>
  );
}

export function AdminTopbar({ onOpenMenu }: { onOpenMenu?: () => void }) {
  const { usuario } = useAuth();

  return (
    <header className="flex h-16 shrink-0 items-center justify-between border-b border-line bg-white px-4 md:px-6">
      <div className="flex items-center gap-4 md:hidden">
        <button onClick={onOpenMenu} className="p-1 -ml-1 text-ink/70 hover:bg-surface rounded-md">
          <Menu className="h-6 w-6" />
        </button>
        <Logo />
      </div>

      <div className="hidden flex-1 md:block" />

      {usuario && (
        <div className="flex items-center gap-3">
          <div className="hidden text-right sm:block">
            <p className="text-sm font-semibold text-ink">{usuario.nome}</p>
            <p className="text-xs text-ink/50">{usuario.perfil}</p>
          </div>
          <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-brand/10 font-bold text-brand">
            {usuario.nome.charAt(0).toUpperCase()}
          </div>
        </div>
      )}
    </header>
  );
}

