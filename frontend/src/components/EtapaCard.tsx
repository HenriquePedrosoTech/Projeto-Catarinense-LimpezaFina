"use client";

import { useRef, useState } from "react";
import type { EtapaResumo } from "@/lib/types";
import { resolverUrlFoto } from "@/lib/fotos";
import { Lightbox, useLightbox } from "./Lightbox";
import { Camera, Check, Loader2, ImagePlus, Info, Upload, AlertTriangle } from "lucide-react";
import { EtapaItemCard } from "./EtapaItemCard";

interface Props {
  etapa: EtapaResumo;
  desabilitado: boolean;
  onEnviarFoto: (arquivo: File) => Promise<void>;
  onRemoverFoto?: (url: string) => Promise<void>;
  onEnviarItem?: (itemId: string, status: string, funcionalidade: string, relato: string | null, arquivo: File | null) => Promise<void>;
}

export function EtapaCard({ etapa, desabilitado, onEnviarFoto, onRemoverFoto, onEnviarItem }: Props) {
  const inputCameraRef = useRef<HTMLInputElement>(null);
  const inputGaleriaRef = useRef<HTMLInputElement>(null);

  const [mostrarOpcoesFoto, setMostrarOpcoesFoto] = useState(false);
  const [enviando, setEnviando] = useState(false);
  const [removendo, setRemovendo] = useState<string | null>(null);
  const [mostrarDesc, setMostrarDesc] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const { urlAberta, abrir, fechar } = useLightbox();

  async function handleArquivo(e: React.ChangeEvent<HTMLInputElement>) {
    if (!e.target.files || e.target.files.length === 0) return;
    setEnviando(true);
    setErro(null);
    try {
      await onEnviarFoto(e.target.files[0]);
    } catch (err: any) {
      setErro(err.message || "Erro ao enviar foto");
    } finally {
      setEnviando(false);
      setMostrarOpcoesFoto(false);
      if (inputCameraRef.current) inputCameraRef.current.value = "";
      if (inputGaleriaRef.current) inputGaleriaRef.current.value = "";
    }
  }

  const possuiItens = etapa.itens && etapa.itens.length > 0;

  return (
    <div className="rounded-2xl border border-line bg-white p-4 shadow-sm sm:p-6 transition-all hover:shadow-md">
      <div className="flex items-start justify-between gap-4">
        <div className="flex-1">
          <div className="flex items-center gap-3">
            <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-surface text-sm font-bold text-ink/50">
              {etapa.ordem}
            </span>
            <h3 className="text-base font-bold uppercase text-ink sm:text-lg">{etapa.nome}</h3>
            
            {etapa.descricao && (
              <button 
                type="button" 
                onClick={() => setMostrarDesc(!mostrarDesc)}
                className="text-ink/40 hover:text-brand transition-colors"
                title="Ver instru��o"
              >
                <Info className="h-5 w-5" />
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
              Ver instru��o
            </a>
          )}
        </div>

        {!possuiItens && (
          <div className="flex shrink-0 items-center">
            {enviando && <Loader2 className="h-5 w-5 animate-spin text-brand" />}
            
            {!etapa.concluida && !desabilitado && !enviando && (
              <div className="relative">
                <input ref={inputCameraRef} type="file" accept="image/*" capture="environment" onChange={handleArquivo} className="hidden" />
                <input ref={inputGaleriaRef} type="file" accept="image/*" onChange={handleArquivo} className="hidden" />
                <button
                  type="button"
                  onClick={() => window.innerWidth >= 640 ? inputGaleriaRef.current?.click() : setMostrarOpcoesFoto(true)}
                  className="flex h-10 w-10 items-center justify-center rounded-full bg-surface text-ink/40 hover:bg-brand/10 hover:text-brand transition-colors"
                >
                  <Camera className="h-5 w-5" />
                </button>
              </div>
            )}

            {etapa.concluida && (
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-success/10 text-success">
                <Check className="h-5 w-5" />
              </div>
            )}
          </div>
        )}
      </div>

      {erro && <p className="mt-3 text-sm font-medium text-danger">{erro}</p>}

      {mostrarDesc && etapa.descricao && (
        <div className="mt-3 rounded-lg bg-surface/50 p-3 text-sm text-ink/70">
          {etapa.descricao}
        </div>
      )}

      {possuiItens && (
        <div className="mt-4 flex flex-col gap-3 border-t border-line pt-4">
          {etapa.itens?.map((item) => (
            <EtapaItemCard
              key={item.id}
              item={item}
              desabilitado={desabilitado}
              onEnviarItem={onEnviarItem}
              onAbrirFoto={abrir}
            />
          ))}

          {etapa.problemaExtraDescricao && (
             <div className="mt-4 p-4 rounded-xl border border-danger/20 bg-danger/5">
                <h4 className="text-sm font-semibold text-danger flex items-center gap-2 mb-2">
                  <AlertTriangle className="w-4 h-4" /> Problema Extra Reportado
                </h4>
                <p className="text-sm text-ink/80">{etapa.problemaExtraDescricao}</p>
                {etapa.problemaExtraFotoUrl && (
                  <button onClick={() => abrir(etapa.problemaExtraFotoUrl!)} className="mt-3 block w-full">
                    <img src={resolverUrlFoto(etapa.problemaExtraFotoUrl)} alt="Problema extra" className="w-full h-32 object-cover rounded-lg border border-line" />
                  </button>
                )}
             </div>
          )}
        </div>
      )}

      {!possuiItens && etapa.fotos && etapa.fotos.length > 0 && (
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
                  onClick={async (e) => {
                    e.stopPropagation();
                    setRemovendo(fotoUrl);
                    try {
                      await onRemoverFoto(fotoUrl);
                    } finally {
                      setRemovendo(null);
                    }
                  }}
                  className="absolute right-1 top-1 flex h-6 w-6 items-center justify-center rounded-full bg-danger text-white shadow-sm transition-opacity opacity-0 group-hover:opacity-100 disabled:opacity-50"
                  disabled={removendo === fotoUrl}
                >
                  {removendo === fotoUrl ? <Loader2 className="h-3 w-3 animate-spin" /> : <span className="text-sm font-bold">X</span>}
                </button>
              )}
            </div>
          ))}
        </div>
      )}

      <Lightbox url={urlAberta} onClose={fechar} />

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
                <Camera className="h-5 w-5" /> Tirar Foto
              </button>
              <button
                type="button"
                onClick={() => { inputGaleriaRef.current?.click(); setMostrarOpcoesFoto(false); }}
                className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 hover:text-brand transition-colors text-ink font-semibold"
              >
                <Upload className="h-5 w-5" /> Escolher da Galeria
              </button>
              <button type="button" onClick={() => setMostrarOpcoesFoto(false)} className="mt-2 w-full p-4 rounded-xl bg-ink/5 text-ink/60 hover:bg-ink/10 font-bold transition-colors">
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}