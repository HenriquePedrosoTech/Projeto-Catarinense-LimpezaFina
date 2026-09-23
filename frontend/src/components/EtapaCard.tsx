"use client";

import { useRef, useState } from "react";
import type { EtapaResumo } from "@/lib/types";
import { resolverUrlFoto } from "@/lib/fotos";
import { Lightbox, useLightbox } from "./Lightbox";
import { Camera, Check, Loader2, ImagePlus, Info } from "lucide-react";

interface Props {
  etapa: EtapaResumo;
  desabilitado: boolean;
  onEnviarFoto: (arquivo: File) => Promise<void>;
  onRemoverFoto?: (url: string) => Promise<void>;
}

export function EtapaCard({ etapa, desabilitado, onEnviarFoto, onRemoverFoto }: Props) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [enviando, setEnviando] = useState(false);
  const [removendo, setRemovendo] = useState<string | null>(null);
  const [mostrarDesc, setMostrarDesc] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const { urlAberta, abrir, fechar } = useLightbox();

  async function handleArquivo(e: React.ChangeEvent<HTMLInputElement>) {
    const arquivo = e.target.files?.[0];
    if (!arquivo) return;

    setErro(null);
    setEnviando(true);
    try {
      await onEnviarFoto(arquivo);
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Falha ao enviar a foto.");
    } finally {
      setEnviando(false);
      if (inputRef.current) inputRef.current.value = "";
    }
  }

  async function handleRemover(url: string) {
    if (!onRemoverFoto) return;
    setErro(null);
    setRemovendo(url);
    try {
      await onRemoverFoto(url);
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Falha ao remover a foto.");
    } finally {
      setRemovendo(null);
    }
  }

  return (
    <div
      className={`rounded-card border p-5 transition-all ${
        etapa.concluida ? "border-success/40 bg-success/5 shadow-sm" : "border-line bg-white shadow-sm"
      }`}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="flex items-center gap-3">
          <span
            className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-full text-base font-bold transition-colors ${
              etapa.concluida ? "bg-success text-white" : "bg-surface text-ink/40"
            }`}
          >
            {etapa.concluida ? <Check className="h-5 w-5" /> : etapa.ordem + 1}
          </span>
          <div className="flex flex-col">
            <div className="flex items-center gap-2">
              <p className="font-semibold text-ink">{etapa.nome}</p>
              {etapa.descricao && (
                <button 
                  onClick={() => setMostrarDesc(!mostrarDesc)}
                  className="text-brand hover:bg-brand/10 p-1 rounded-full transition"
                  title="Ver Procedimento Operacional"
                >
                  <Info className="h-4 w-4" />
                </button>
              )}
            </div>
          </div>
        </div>
      </div>

      {mostrarDesc && etapa.descricao && (
        <div className="mt-3 pl-13 text-sm text-ink/70 bg-surface/50 p-3 rounded-md border border-line">
          <strong>P.O.P (Procedimento Padrão):</strong>
          <p className="mt-1 whitespace-pre-line">{etapa.descricao}</p>
          {etapa.linkVideo && (
            <a 
              href={etapa.linkVideo} 
              target="_blank" 
              rel="noopener noreferrer"
              className="mt-3 inline-flex items-center gap-1.5 font-medium text-brand hover:underline"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="m22 8-6 4 6 4V8Z"/><rect width="14" height="12" x="2" y="6" rx="2" ry="2"/></svg>
              Assistir Vídeo / Link
            </a>
          )}
        </div>
      )}

      <div className="mt-4 pl-13 flex flex-col gap-3">
        {etapa.fotos.length > 0 && (
          <div className="flex gap-3 overflow-x-auto pt-4 pr-4 pb-2 -mt-4">
            {etapa.fotos.map((url) => {
              const urlCompleta = resolverUrlFoto(url);
              return (
                <div key={url} className="relative shrink-0">
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={urlCompleta}
                    alt={`Evidência - ${etapa.nome}`}
                    onClick={() => abrir(urlCompleta)}
                    className={`h-24 w-24 cursor-zoom-in rounded-lg border border-line object-cover transition-all hover:opacity-80 shadow-sm ${removendo === url ? "opacity-50 blur-sm" : ""}`}
                  />
                  {!desabilitado && onRemoverFoto && (
                    <button
                      onClick={() => handleRemover(url)}
                      disabled={removendo !== null}
                      className="absolute -right-3 -top-3 flex h-8 w-8 items-center justify-center rounded-full bg-danger text-white border-2 border-white shadow-md hover:bg-danger/90 transition-transform active:scale-90"
                      title="Remover foto"
                    >
                      <span className="text-lg font-bold leading-none pb-0.5">×</span>
                    </button>
                  )}
                </div>
              );
            })}
          </div>
        )}

        {!etapa.concluida && !desabilitado && (
          <div>
            <input
              ref={inputRef}
              type="file"
              accept="image/*"
              onChange={handleArquivo}
              className="hidden"
              id={`foto-${etapa.etapaPadraoId}`}
            />
            <label
              htmlFor={`foto-${etapa.etapaPadraoId}`}
              className={`inline-flex cursor-pointer items-center gap-2 rounded-md px-5 py-2.5 text-sm font-semibold transition-all active:scale-[0.98] ${
                enviando 
                  ? "bg-surface text-ink/40 pointer-events-none" 
                  : "bg-brand text-white hover:bg-brand/90 hover:shadow-md"
              }`}
            >
              {enviando ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" /> Enviando...
                </>
              ) : (
                <>
                  <Camera className="h-5 w-5" /> Tirar foto
                </>
              )}
            </label>
            {erro && <p className="mt-2 text-sm font-medium text-danger">{erro}</p>}
          </div>
        )}

        {etapa.concluida && !desabilitado && (
          <div>
             <input
              ref={inputRef}
              type="file"
              accept="image/*"
              onChange={handleArquivo}
              className="hidden"
              id={`foto-add-${etapa.etapaPadraoId}`}
            />
            <label
              htmlFor={`foto-add-${etapa.etapaPadraoId}`}
              className={`inline-flex cursor-pointer items-center gap-2 rounded-md px-4 py-2 text-xs font-semibold transition-colors active:scale-[0.98] ${
                enviando 
                  ? "bg-surface text-ink/40 pointer-events-none" 
                  : "bg-surface text-ink/60 hover:bg-line"
              }`}
            >
              {enviando ? (
                <>
                  <Loader2 className="h-3 w-3 animate-spin" /> Enviando...
                </>
              ) : (
                <>
                  <ImagePlus className="h-4 w-4" /> Adicionar mais fotos
                </>
              )}
            </label>
          </div>
        )}
      </div>

      <Lightbox url={urlAberta} onClose={fechar} />
    </div>
  );
}
