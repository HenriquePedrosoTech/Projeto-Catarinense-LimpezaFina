import type { StatusLimpeza } from "@/lib/types";

const CONFIG: Record<StatusLimpeza, { texto: string; classe: string }> = {
  EmAndamento: { texto: "Em andamento", classe: "bg-navy/10 text-navy border-navy/20" },
  Concluida: { texto: "Aguardando avaliação", classe: "bg-warning/10 text-warning border-warning/20" },
  Aprovada: { texto: "Aprovada", classe: "bg-success/10 text-success border-success/20" },
  Reprovada: { texto: "Reprovada", classe: "bg-danger/10 text-danger border-danger/20" },
};

export function StatusBadge({ status, className = "" }: { status: StatusLimpeza; className?: string }) {
  const { texto, classe } = CONFIG[status];
  return (
    <span
      className={`inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold tracking-wide ${classe} ${className}`}
    >
      {texto}
    </span>
  );
}
