"use client";

import { useState, useRef } from "react";
import type { EtapaItemResumo } from "@/lib/types";
import { resolverUrlFoto } from "@/lib/fotos";
import { Camera, Check, Upload, Loader2, ImagePlus } from "lucide-react";
import { Button } from "./ui/Button";

interface Props {
  item: EtapaItemResumo;
  desabilitado: boolean;
  onEnviarItem?: (itemId: string, status: string, funcionalidade: string, relato: string | null, arquivo: File | null) => Promise<void>;
  onAbrirFoto?: (url: string) => void;
}

export function EtapaItemCard({ item, desabilitado, onEnviarItem, onAbrirFoto }: Props) {
  const [status, setStatus] = useState(item.status || "Conforme");
  const [funcionalidade, setFuncionalidade] = useState(item.funcionalidade || "OK / Funcional");
  const [relato, setRelato] = useState(item.relatoProblema || "");
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [enviando, setEnviando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  const inputCameraRef = useRef<HTMLInputElement>(null);
  const inputGaleriaRef = useRef<HTMLInputElement>(null);
  const [mostrarOpcoesFoto, setMostrarOpcoesFoto] = useState(false);

  const precisaRelato = status === "Não Conforme" || funcionalidade === "Com Defeito";

  async function handleSalvar() {
    if (!onEnviarItem) return;
    setErro(null);

    if (precisaRelato && !relato.trim()) {
      setErro("É obrigatório descrever o problema encontrado.");
      return;
    }

    if (precisaRelato && !arquivo && !item.fotoUrl) {
      setErro("É obrigatório enviar uma foto evidenciando o problema.");
      return;
    }

    setEnviando(true);
    try {
      await onEnviarItem(item.id, status, funcionalidade, relato.trim(), arquivo);
    } catch (err: any) {
      setErro(err.message || "Erro ao salvar item.");
    } finally {
      setEnviando(false);
    }
  }

  return (
    <div className={`p-4 rounded-lg border ${item.concluida ? "bg-success/5 border-success/40" : "bg-white border-line"}`}>
      <div className="flex justify-between items-start mb-3">
        <h4 className="font-semibold text-ink text-sm pr-4">{item.ordem + 1}º {item.texto}</h4>
        {item.concluida && <Check className="h-5 w-5 text-success shrink-0" />}
      </div>

      {item.enquadramentoFoto && (
        <p className="text-xs text-ink/60 mb-3 bg-surface p-2 rounded">
          <strong>Foto:</strong> {item.enquadramentoFoto}
        </p>
      )}

      <div className="grid grid-cols-2 gap-3 mb-4">
        <div>
          <label className="text-xs font-semibold mb-1 block">Conformidade</label>
          <select 
            value={status} 
            onChange={(e) => setStatus(e.target.value)}
            disabled={desabilitado || item.concluida || enviando}
            className="w-full text-sm border-line rounded-md"
          >
            <option value="Conforme">Conforme</option>
            <option value="Não Conforme">Não Conforme</option>
            <option value="N/A">N/A</option>
          </select>
        </div>
        <div>
          <label className="text-xs font-semibold mb-1 block">Funcionalidade</label>
          <select 
            value={funcionalidade} 
            onChange={(e) => setFuncionalidade(e.target.value)}
            disabled={desabilitado || item.concluida || enviando}
            className="w-full text-sm border-line rounded-md"
          >
            <option value="OK / Funcional">OK / Funcional</option>
            <option value="Com Defeito">Com Defeito</option>
          </select>
        </div>
      </div>

      {precisaRelato && (
        <div className="mb-4">
          <label className="text-xs font-semibold mb-1 block text-danger">Descreva o Problema *</label>
          <textarea
            value={relato}
            onChange={(e) => setRelato(e.target.value)}
            disabled={desabilitado || item.concluida || enviando}
            className="w-full text-sm border-line rounded-md min-h-[80px]"
            placeholder="Qual é o problema ou avaria?"
          />
        </div>
      )}

      {erro && <p className="text-xs text-danger mb-3 font-semibold">{erro}</p>}

      {!item.concluida && !desabilitado && (
        <div className="flex flex-col gap-2">
          {precisaRelato && (
            <div className="flex gap-2 mb-2">
               <input ref={inputCameraRef} type="file" accept="image/*" capture="environment" onChange={(e) => { setArquivo(e.target.files?.[0] || null); setMostrarOpcoesFoto(false); }} className="hidden" />
               <input ref={inputGaleriaRef} type="file" accept="image/*" onChange={(e) => { setArquivo(e.target.files?.[0] || null); setMostrarOpcoesFoto(false); }} className="hidden" />
               <Button type="button" variant="outline" className="w-full flex-1" onClick={() => window.innerWidth >= 640 ? inputGaleriaRef.current?.click() : setMostrarOpcoesFoto(true)}>
                 {arquivo ? "Foto Selecionada" : <><Camera className="w-4 h-4 mr-2" /> Evidência *</>}
               </Button>
            </div>
          )}
          <Button type="button" onClick={handleSalvar} disabled={enviando} className="w-full">
             {enviando ? <Loader2 className="w-4 h-4 animate-spin" /> : "Salvar Item"}
          </Button>
        </div>
      )}

      {item.concluida && item.fotoUrl && (
        <div className="mt-2">
          <p className="text-xs font-semibold mb-1">Evidência:</p>
          <img
            src={resolverUrlFoto(item.fotoUrl)}
            alt="Evidência"
            className="h-20 w-20 rounded-md object-cover cursor-pointer border border-line"
            onClick={() => onAbrirFoto?.(item.fotoUrl!)}
          />
        </div>
      )}

      {mostrarOpcoesFoto && (
        <div className="fixed inset-0 z-[100] flex items-end justify-center bg-ink/50 backdrop-blur-sm sm:items-center animate-in fade-in">
          <div className="w-full bg-white rounded-t-3xl sm:rounded-2xl sm:max-w-sm overflow-hidden p-6 pb-12 sm:pb-6 animate-in slide-in-from-bottom-10 sm:zoom-in-95">
            <h3 className="text-lg font-bold text-ink mb-4 text-center">Como deseja enviar a foto?</h3>
            <div className="flex flex-col gap-3">
              <button onClick={() => { inputCameraRef.current?.click(); }} className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 hover:text-brand font-semibold text-ink"><Camera className="h-5 w-5" /> Tirar Foto</button>
              <button onClick={() => { inputGaleriaRef.current?.click(); }} className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 hover:text-brand font-semibold text-ink"><Upload className="h-5 w-5" /> Escolher da Galeria</button>
              <button onClick={() => setMostrarOpcoesFoto(false)} className="mt-2 w-full p-4 rounded-xl bg-ink/5 text-ink/60 hover:bg-ink/10 font-bold">Cancelar</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
