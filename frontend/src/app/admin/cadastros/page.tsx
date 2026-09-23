"use client";

import { useState } from "react";
import { useAuth } from "@/lib/auth";
import { OnibusSection } from "@/components/admin/cadastros/OnibusSection";
import { EtapasSection } from "@/components/admin/cadastros/EtapasSection";
import { UsuariosSection } from "@/components/admin/cadastros/UsuariosSection";
import { Bus, ListChecks, Users } from "lucide-react";

type Aba = "onibus" | "etapas" | "usuarios";

const ABAS = [
  { id: "onibus" as Aba, rotulo: "Frota de Ônibus", icon: Bus },
  { id: "etapas" as Aba, rotulo: "Checklist Padrão", icon: ListChecks },
  { id: "usuarios" as Aba, rotulo: "Usuários e Acessos", icon: Users },
];

export default function CadastrosPage() {
  const { usuario } = useAuth();
  const [aba, setAba] = useState<Aba>("onibus");

  if (!usuario) return null;

  return (
    <div className="mx-auto max-w-6xl">
      <div className="mb-6">
        <h1 className="text-2xl font-bold tracking-tight text-ink">Cadastros Base</h1>
        <p className="text-sm text-ink/60">Gerencie ônibus, etapas do checklist e equipe do sistema.</p>
      </div>

      <div className="mb-8 flex gap-2 overflow-x-auto border-b border-line pb-px">
        {ABAS.map((a) => {
          const isSelected = aba === a.id;
          const Icon = a.icon;
          return (
            <button
              key={a.id}
              onClick={() => setAba(a.id)}
              className={`flex shrink-0 items-center gap-2 border-b-2 px-4 py-3 text-sm font-medium transition-colors ${
                isSelected
                  ? "border-brand text-brand"
                  : "border-transparent text-ink/60 hover:text-ink"
              }`}
            >
              <Icon className="h-4 w-4" />
              {a.rotulo}
            </button>
          );
        })}
      </div>

      {aba === "onibus" && <OnibusSection token={usuario.token} />}
      {aba === "etapas" && <EtapasSection token={usuario.token} />}
      {aba === "usuarios" && <UsuariosSection token={usuario.token} />}
    </div>
  );
}

