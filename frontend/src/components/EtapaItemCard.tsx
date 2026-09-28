"use client";

import { useRef, useState } from "react";
import type { EtapaItemResumo } from "@/lib/types";
import { Camera, Check, Loader2, Upload, AlertCircle } from "lucide-react";
import { Button } from "./ui/Button";
import { resolverUrlFoto } from "@/lib/fotos";

interface Props {
  item: EtapaItemResumo;
  desabilitado: boolean;
  onEnviarItem?: (itemId: string, status: string, funcionalidade: string, relato: string | null, arquivo: File | null) => Promise<void>;
  onAbrirFoto: (url: string) => void;
}

export function EtapaItemCard({ item, desabilitado, onEnviarItem, onAbrirFoto }: Props) {
  const [status, setStatus] = useState<string>(item.status || "");
  const [funcionalidade, setFuncionalidade] = useState<string>(item.funcionalidade || "");
  const [relato, setRelato] = useState<string>(item.relatoProblema || "");
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(item.fotoUrl ? resolverUrlFoto(item.fotoUrl) : null);
  const [enviando, setEnviando] = useState(false);
  
  const inputCameraRef = useRef<HTMLInputElement>(null);
  const inputGaleriaRef = useRef<HTMLInputElement>(null);
  const [mostrarOpcoesFoto, setMostrarOpcoesFoto] = useState(false);

  const isNaoConforme = status === "Não Conforme" || funcionalidade === "Com Defeito";
  const editando = !item.concluida && !desabilitado;

  function handleArquivo(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    setMostrarOpcoesFoto(false);
    if (!file) return;

    setArquivo(file);
    const reader = new FileReader();
    reader.onload = () => setPreview(reader.result as string);
    reader.readAsDataURL(file);
  }

  async function handleSalvar() {
    if (!onEnviarItem) return;
    
    // Validacoes
    if (!status) return alert("Selecione a conformidade.");
    if (!funcionalidade) return alert("Selecione o estado da funcionalidade.");
    if (isNaoConforme && !relato) return alert("Por favor, descreva o problema no relato livre.");
    if (!preview) return alert("A foto de evidência é obrigatória.");

    setEnviando(true);
    try {
      await onEnviarItem(item.id, status, funcionalidade, isNaoConforme ? relato : null, arquivo);
    } catch (e) {
      alert("Erro ao salvar o item");
    } finally {
      setEnviando(false);
    }
  }

  return (
    <div className={`p-4 rounded-lg border ${item.concluida ? "bg-success/5 border-success/30" : "bg-white border-line"}`}>
      <div className="flex gap-3 mb-3">
        <span className="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-surface text-ink text-xs font-bold">
          {item.ordem + 1}
        </span>
        <div>
          <p className="text-sm font-semibold text-ink leading-tight">{item.texto}</p>
          {item.enquadramentoFoto && (
            <p className="text-xs text-ink/60 mt-1 italic">
              📸 {item.enquadramentoFoto}
            </p>
          )}
        </div>
      </div>

      {editando ? (
        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-2">
            <select
              value={status}
              onChange={(e) => setStatus(e.target.value)}
              className="text-sm rounded border border-line p-2 text-ink"
            >
              <option value="" disabled>Conformidade...</option>
              <option value="Conforme">Conforme</option>
              <option value="Não Conforme">Não Conforme</option>
              <option value="N/A">N/A</option>
            </select>
            <select
              value={funcionalidade}
              onChange={(e) => setFuncionalidade(e.target.value)}
              className="text-sm rounded border border-line p-2 text-ink"
            >
              <option value="" disabled>Funcionamento...</option>
              <option value="OK">OK / Funcional</option>
              <option value="Com Defeito">Com Defeito</option>
            </select>
          </div>

          {isNaoConforme && (
            <div>
              <textarea
                placeholder="Descreva o problema encontrado (Obrigatório)..."
                value={relato}
                onChange={(e) => setRelato(e.target.value)}
                className="w-full text-sm rounded border border-line p-2 min-h-[80px]"
              />
            </div>
          )}

          <div className="flex items-center gap-3">
            {preview ? (
              <div className="relative h-20 w-20 shrink-0 overflow-hidden rounded-lg bg-surface border border-line">
                <img src={preview} alt="Evidência" className="h-full w-full object-cover" />
                <button
                  type="button"
                  onClick={() => { setArquivo(null); setPreview(null); }}
                  className="absolute top-1 right-1 flex h-6 w-6 items-center justify-center rounded-full bg-ink/70 text-white hover:bg-danger text-xs"
                >
                  X
                </button>
              </div>
            ) : (
              <div className="relative">
                <input ref={inputCameraRef} type="file" accept="image/*" capture="environment" onChange={handleArquivo} className="hidden" />
                <input ref={inputGaleriaRef} type="file" accept="image/*" onChange={handleArquivo} className="hidden" />
                <button
                  type="button"
                  onClick={() => window.innerWidth >= 640 ? inputGaleriaRef.current?.click() : setMostrarOpcoesFoto(true)}
                  className="flex items-center gap-2 rounded bg-surface px-4 py-2 text-sm text-ink hover:bg-brand/10 transition-colors"
                >
                  <Camera className="h-4 w-4" /> Anexar Foto
                </button>
              </div>
            )}
            
            <div className="flex-1 flex justify-end">
              <Button onClick={handleSalvar} loading={enviando} disabled={enviando}>Salvar Item</Button>
            </div>
          </div>
        </div>
      ) : (
        <div className="mt-2 space-y-2 text-sm">
          <div className="flex gap-2">
            <span className={`px-2 py-0.5 rounded text-xs font-semibold ${item.status === "Não Conforme" ? "bg-danger/10 text-danger" : "bg-success/10 text-success"}`}>
              {item.status}
            </span>
            <span className={`px-2 py-0.5 rounded text-xs font-semibold ${item.funcionalidade === "Com Defeito" ? "bg-danger/10 text-danger" : "bg-success/10 text-success"}`}>
              {item.funcionalidade}
            </span>
          </div>

          {(item.status === "Não Conforme" || item.funcionalidade === "Com Defeito") && item.relatoProblema && (
            <div className="bg-danger/5 p-2 rounded text-danger text-xs flex items-start gap-2">
              <AlertCircle className="h-4 w-4 shrink-0" />
              <p>{item.relatoProblema}</p>
            </div>
          )}

          {item.fotoUrl && (
            <img 
              src={resolverUrlFoto(item.fotoUrl)} 
              alt="Evidência" 
              className="mt-2 h-20 w-20 cursor-pointer object-cover rounded-lg border border-line" 
              onClick={() => onAbrirFoto(item.fotoUrl!)}
            />
          )}
        </div>
      )}

      {mostrarOpcoesFoto && (
        <div className="fixed inset-0 z-[100] flex items-end justify-center bg-ink/50 backdrop-blur-sm sm:items-center animate-in fade-in">
          <div className="w-full bg-white rounded-t-3xl sm:rounded-2xl sm:max-w-sm overflow-hidden p-6 pb-12 sm:pb-6 animate-in slide-in-from-bottom-10 sm:zoom-in-95">
            <h3 className="text-lg font-bold text-ink mb-4 text-center">Como deseja enviar a foto?</h3>
            <div className="flex flex-col gap-3">
              <button onClick={() => { inputCameraRef.current?.click(); setMostrarOpcoesFoto(false); }} className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 transition-colors">
                <Camera className="h-5 w-5" /> Tirar Foto
              </button>
              <button onClick={() => { inputGaleriaRef.current?.click(); setMostrarOpcoesFoto(false); }} className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 transition-colors">
                <Upload className="h-5 w-5" /> Escolher da Galeria
              </button>
              <button onClick={() => setMostrarOpcoesFoto(false)} className="mt-2 w-full p-4 rounded-xl bg-ink/5 text-ink/60 hover:bg-ink/10 font-bold transition-colors">
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
