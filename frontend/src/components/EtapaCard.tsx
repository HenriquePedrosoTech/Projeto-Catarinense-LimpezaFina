"use client";

import { useRef, useState, useEffect } from "react";
import type { EtapaResumo } from "@/lib/types";
import { resolverUrlFoto } from "@/lib/fotos";
import { Lightbox, useLightbox } from "./Lightbox";
import { Camera, Check, Loader2, ImagePlus, Info, Upload } from "lucide-react";

interface Props {
  etapa: EtapaResumo;
  desabilitado: boolean;
  onEnviarFoto: (arquivo: File) => Promise<void>;
  onRemoverFoto?: (url: string) => Promise<void>;
}

export function EtapaCard({ etapa, desabilitado, onEnviarFoto, onRemoverFoto }: Props) {
  const inputCameraRef = useRef<HTMLInputElement>(null);
  const inputGaleriaRef = useRef<HTMLInputElement>(null);
  
  const [enviando, setEnviando] = useState(false);
  const [removendo, setRemovendo] = useState<string | null>(null);
  const [mostrarDesc, setMostrarDesc] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [mostrarOpcoesFoto, setMostrarOpcoesFoto] = useState(false);
  const { urlAberta, abrir, fechar } = useLightbox();

  async function handleArquivo(e: React.ChangeEvent<HTMLInputElement>) {
    const arquivo = e.target.files?.[0];
    setMostrarOpcoesFoto(false);
    if (!arquivo) return;

    setErro(null);
    setEnviando(true);
    try {
      await onEnviarFoto(arquivo);
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Falha ao enviar a foto.");
    } finally {
      setEnviando(false);
      if (inputCameraRef.current) inputCameraRef.current.value = "";
      if (inputGaleriaRef.current) inputGaleriaRef.current.value = "";
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
                  type="button"
                  onClick={() => setMostrarDesc(!mostrarDesc)}
                  className="text-ink/40 hover:text-brand transition-colors"
                >
                  <Info className="h-4 w-4" />
                </button>
              )}
            </div>
            {etapa.linkVideo && (
              <a
                href={etapa.linkVideo}
                target="_blank"
                rel="noreferrer"
                className="mt-0.5 text-xs font-semibold text-brand hover:underline"
              >
                Ver instrução
              </a>
            )}
          </div>
        </div>

        <div className="flex shrink-0 items-center">
          {enviando && <Loader2 className="h-5 w-5 animate-spin text-brand" />}
          
          {!etapa.concluida && !desabilitado && !enviando && (
            <div className="relative">
              <input
                ref={inputCameraRef}
                type="file"
                accept="image/*"
                capture="environment"
                onChange={handleArquivo}
                className="hidden"
              />
              <input
                ref={inputGaleriaRef}
                type="file"
                accept="image/*"
                onChange={handleArquivo}
                className="hidden"
              />
              <button
                type="button"
                onClick={() => setMostrarOpcoesFoto(true)}
                className="flex h-10 w-10 items-center justify-center rounded-full bg-surface text-ink/40 hover:bg-brand/10 hover:text-brand transition-colors"
              >
                <Camera className="h-5 w-5" />
              </button>
            </div>
          )}

          {etapa.concluida && !desabilitado && !enviando && (
            <div className="relative">
               <input
                ref={inputCameraRef}
                type="file"
                accept="image/*"
                capture="environment"
                onChange={handleArquivo}
                className="hidden"
              />
              <input
                ref={inputGaleriaRef}
                type="file"
                accept="image/*"
                onChange={handleArquivo}
                className="hidden"
              />
              <button
                type="button"
                onClick={() => setMostrarOpcoesFoto(true)}
                className="flex h-10 w-10 items-center justify-center rounded-full bg-brand/10 text-brand hover:bg-brand hover:text-white transition-colors"
              >
                <ImagePlus className="h-5 w-5" />
              </button>
            </div>
          )}
        </div>
      </div>

      {erro && (
        <p className="mt-3 text-sm font-medium text-danger">{erro}</p>
      )}

      {mostrarDesc && etapa.descricao && (
        <div className="mt-3 rounded-lg bg-surface/50 p-3 text-sm text-ink/70">
          {etapa.descricao}
        </div>
      )}

      {etapa.fotos.length > 0 && (
        <div className="mt-4 flex gap-3 overflow-x-auto pb-2">
          {etapa.fotos.map((fotoUrl) => (
            <div key={fotoUrl} className="group relative h-20 w-20 shrink-0 overflow-hidden rounded-lg bg-surface border border-line">
              <img
                src={resolverUrlFoto(fotoUrl)}
                alt="Foto da etapa"
                className="h-full w-full cursor-pointer object-cover"
                onClick={() => abrir(fotoUrl)}
              />
              {onRemoverFoto && !desabilitado && (
                <button
                  type="button"
                  onClick={() => handleRemover(fotoUrl)}
                  disabled={removendo === fotoUrl}
                  className="absolute right-1 top-1 flex h-7 w-7 items-center justify-center rounded-full bg-ink/70 text-white opacity-100 transition-all hover:bg-danger md:opacity-0 md:group-hover:opacity-100"
                >
                  {removendo === fotoUrl ? (
                    <Loader2 className="h-3 w-3 animate-spin" />
                  ) : (
                    <span className="text-sm font-bold">X</span>
                  )}
                </button>
              )}
            </div>
          ))}
        </div>
      )}

      <Lightbox 
        url={urlAberta} 
        onClose={fechar} 
      />

      {/* Modal de Escolha de Foto */}
      {mostrarOpcoesFoto && (
        <div className="fixed inset-0 z-[100] flex items-end justify-center bg-ink/50 backdrop-blur-sm sm:items-center animate-in fade-in">
          <div className="w-full bg-white rounded-t-3xl sm:rounded-2xl sm:max-w-sm overflow-hidden p-6 pb-12 sm:pb-6 animate-in slide-in-from-bottom-10 sm:zoom-in-95">
            <h3 className="text-lg font-bold text-ink mb-4 text-center">Como deseja enviar a foto?</h3>
            <div className="flex flex-col gap-3">
              <button
                type="button"
                onClick={() => { inputCameraRef.current?.click(); setMostrarOpcoesFoto(false); }}
                className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 hover:text-brand transition-colors text-ink font-semibold"
              >
                <Camera className="h-5 w-5" />
                Tirar Foto
              </button>
              <button
                type="button"
                onClick={() => { inputGaleriaRef.current?.click(); setMostrarOpcoesFoto(false); }}
                className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 hover:text-brand transition-colors text-ink font-semibold"
              >
                <Upload className="h-5 w-5" />
                Escolher da Galeria
              </button>
              <button
                type="button"
                onClick={() => setMostrarOpcoesFoto(false)}
                className="mt-2 w-full p-4 rounded-xl bg-ink/5 text-ink/60 hover:bg-ink/10 font-bold transition-colors"
              >
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}