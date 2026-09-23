"use client";

import { useEffect } from "react";
import { AlertCircle, CheckCircle, Info, X } from "lucide-react";
import { Button } from "./Button";

export type DialogType = "info" | "success" | "warning" | "danger";

interface BaseDialogProps {
  aberto: boolean;
  titulo: string;
  mensagem: string;
  tipo?: DialogType;
  onClose: () => void;
}

const ICONS = {
  info: <Info className="h-6 w-6 text-brand" />,
  success: <CheckCircle className="h-6 w-6 text-success" />,
  warning: <AlertCircle className="h-6 w-6 text-warning" />,
  danger: <AlertCircle className="h-6 w-6 text-danger" />
};

export function AlertDialog({ aberto, titulo, mensagem, tipo = "info", onClose }: BaseDialogProps) {
  useEffect(() => {
    if (aberto) document.body.style.overflow = "hidden";
    else document.body.style.overflow = "";
    return () => { document.body.style.overflow = ""; };
  }, [aberto]);

  if (!aberto) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-ink/40 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm overflow-hidden animate-in zoom-in-95 duration-200">
        <div className="p-6 flex flex-col items-center text-center">
          <div className="mb-4">
            {ICONS[tipo]}
          </div>
          <h3 className="text-xl font-bold text-ink mb-2">{titulo}</h3>
          <p className="text-ink/60 text-sm mb-6">{mensagem}</p>
          <Button onClick={onClose} className="w-full" variant={tipo === "danger" ? "danger" : "primary"}>
            Entendi
          </Button>
        </div>
      </div>
    </div>
  );
}

interface ConfirmDialogProps extends BaseDialogProps {
  textoConfirmar?: string;
  textoCancelar?: string;
  onConfirmar: () => void;
}

export function ConfirmDialog({
  aberto,
  titulo,
  mensagem,
  tipo = "warning",
  textoConfirmar = "Confirmar",
  textoCancelar = "Cancelar",
  onClose,
  onConfirmar
}: ConfirmDialogProps) {
  useEffect(() => {
    if (aberto) document.body.style.overflow = "hidden";
    else document.body.style.overflow = "";
    return () => { document.body.style.overflow = ""; };
  }, [aberto]);

  if (!aberto) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-ink/40 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm overflow-hidden animate-in zoom-in-95 duration-200">
        <div className="p-6 flex flex-col items-center text-center">
          <div className="mb-4">
            {ICONS[tipo]}
          </div>
          <h3 className="text-xl font-bold text-ink mb-2">{titulo}</h3>
          <p className="text-ink/60 text-sm mb-6">{mensagem}</p>
          <div className="flex w-full gap-3">
            <Button onClick={onClose} variant="outline" className="flex-1">
              {textoCancelar}
            </Button>
            <Button onClick={() => { onConfirmar(); onClose(); }} variant={tipo === "danger" ? "danger" : "primary"} className="flex-1">
              {textoConfirmar}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}