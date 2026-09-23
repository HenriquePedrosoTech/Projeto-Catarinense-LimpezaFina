"use client";

import { useRequireAuth } from "@/lib/auth";
import { OperadorBottomNav, OperadorTopbar } from "@/components/operador/LayoutComponents";

export default function OperadorLayout({ children }: { children: React.ReactNode }) {
  const { usuario, carregando } = useRequireAuth("Operador");

  if (carregando || !usuario) {
    return null;
  }

  return (
    <div className="flex min-h-screen flex-col bg-surface">
      <OperadorTopbar />
      {/* pt-16 (topbar) + pb-20 (bottom nav com folga) */}
      <main className="flex-1 overflow-y-auto px-4 pb-20 pt-20">
        {children}
      </main>
      <OperadorBottomNav />
    </div>
  );
}

