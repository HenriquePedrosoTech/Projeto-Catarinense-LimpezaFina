"use client";

import { useState } from "react";
import { useRequireAuth } from "@/lib/auth";
import { AdminSidebar, AdminTopbar } from "@/components/admin/LayoutComponents";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const { usuario, carregando } = useRequireAuth("Administrador");
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  if (carregando || !usuario) {
    return null;
  }

  return (
    <div className="flex h-screen overflow-hidden bg-surface">
      <AdminSidebar isOpen={isMobileMenuOpen} onClose={() => setIsMobileMenuOpen(false)} />
      <div className="flex flex-1 flex-col overflow-hidden">
        <AdminTopbar onOpenMenu={() => setIsMobileMenuOpen(true)} />
        <main className="flex-1 overflow-y-auto p-4 md:p-6 lg:p-8">
          {children}
        </main>
      </div>
    </div>
  );
}