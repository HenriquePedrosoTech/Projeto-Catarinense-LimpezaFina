import { useState, useEffect } from "react";
import { AlertCircle, X } from "lucide-react";

export function ToastAlerta({ 
  titulo, 
  mensagem, 
  onClose 
}: { 
  titulo: string; 
  mensagem: string; 
  onClose: () => void 
}) {
  return (
    <div className="fixed top-4 left-1/2 -translate-x-1/2 z-50 w-[90%] max-w-sm animate-in fade-in slide-in-from-top-10 duration-300">
      <div className="flex items-start gap-3 rounded-lg border-l-4 border-danger bg-white p-4 shadow-xl">
        <AlertCircle className="h-6 w-6 text-danger shrink-0" />
        <div className="flex-1">
          <h3 className="font-bold text-ink">{titulo}</h3>
          <p className="text-sm text-ink/70 mt-1">{mensagem}</p>
        </div>
        <button onClick={onClose} className="text-ink/40 hover:text-ink shrink-0">
          <X className="h-5 w-5" />
        </button>
      </div>
    </div>
  );
}