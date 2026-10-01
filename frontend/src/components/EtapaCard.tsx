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
  onRegistrarProblemaExtra?: (etapaId: string, descricao: string, foto: File | null) => Promise<void>;
}

export function EtapaCard({ etapa, desabilitado, onEnviarFoto, onRemoverFoto, onEnviarItem, onRegistrarProblemaExtra }: Props) {
  const inputCameraRef = useRef<HTMLInputElement>(null);
  const inputGaleriaRef = useRef<HTMLInputElement>(null);
  
  const [enviando, setEnviando] = useState(false);
  const [removendo, setRemovendo] = useState<string | null>(null);
  const [mostrarDesc, setMostrarDesc] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [mostrarOpcoesFoto, setMostrarOpcoesFoto] = useState(false);
  const [mostrarProblemaExtra, setMostrarProblemaExtra] = useState(false);
  const [relatoExtra, setRelatoExtra] = useState("");
  const [fotoExtra, setFotoExtra] = useState<File | null>(null);
  const [enviandoProblemaExtra, setEnviandoProblemaExtra] = useState(false);
  const inputFotoExtraRef = useRef<HTMLInputElement>(null);
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

  const possuiItens = etapa.itens && etapa.itens.length > 0;

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

            {etapa.concluida && !desabilitado && !enviando && (
              <div className="relative">
                <input ref={inputCameraRef} type="file" accept="image/*" capture="environment" onChange={handleArquivo} className="hidden" />
                <input ref={inputGaleriaRef} type="file" accept="image/*" onChange={handleArquivo} className="hidden" />
                <button
                  type="button"
                  onClick={() => window.innerWidth >= 640 ? inputGaleriaRef.current?.click() : setMostrarOpcoesFoto(true)}
                  className="flex h-10 w-10 items-center justify-center rounded-full bg-brand/10 text-brand hover:bg-brand hover:text-white transition-colors"
                >
                  <ImagePlus className="h-5 w-5" />
                </button>
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
              onAbrirFoto={abrir}              />
            ))}
            
            {/* Sessao de Problema Extra */}
          {etapa.problemaExtraDescricao ? (
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
          ) : (
            <div className="mt-2 pt-2 border-t border-line border-dashed">
              {!mostrarProblemaExtra ? (
                <button
                  type="button"
                  onClick={() => setMostrarProblemaExtra(true)}
                  disabled={desabilitado}
                  className="text-sm text-primary hover:text-primary-hover font-medium flex items-center gap-1.5"
                >
                  <AlertTriangle className="w-4 h-4" /> Encontrou outro problema? Reporte
                </button>
              ) : (
                <div className="p-4 rounded-xl border border-danger/20 bg-danger/5 flex flex-col gap-3">
                  <h4 className="text-sm font-semibold text-danger flex items-center gap-2">
                    <AlertTriangle className="w-4 h-4" /> Reportar Problema Extra
                  </h4>
                  
                  <div>
                    <label className="text-xs font-semibold mb-1 block text-danger/80 uppercase">Descri��o *</label>
                    <textarea
                      value={relatoExtra}
                      onChange={(e) => setRelatoExtra(e.target.value)}
                      disabled={desabilitado || enviandoProblemaExtra}
                      placeholder="Descreva o problema encontrado..."
                      className="w-full min-h-[80px] p-2.5 rounded-lg border border-line bg-surface text-sm text-ink focus:border-primary focus:ring-1 focus:ring-primary outline-none resize-y transition-shadow"
                    />
                  </div>

                  <div>
                    <label className="text-[11px] font-semibold mb-1 block text-danger/80 uppercase">Foto (Opcional)</label>
                    <input
                      type="file"
                      accept="image/*"
                      ref={inputFotoExtraRef}
                      className="hidden"
                      onChange={(e) => {
                        if (e.target.files && e.target.files[0]) {
                          setFotoExtra(e.target.files[0]);
                        }
                      }}
                    />
                    
                    {fotoExtra ? (
                      <div className="relative w-full h-24 rounded-lg overflow-hidden border border-line">
                        <img src={URL.createObjectURL(fotoExtra)} alt="Preview" className="w-full h-full object-cover" />
                        <button
                          type="button"
                          onClick={() => setFotoExtra(null)}
                          className="absolute top-2 right-2 bg-black/60 text-white rounded-full p-1.5 hover:bg-black/80"
                        >
                          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>
                        </button>
                      </div>
                    ) : (
                      <button
                        type="button"
                        onClick={() => inputFotoExtraRef.current?.click()}
                        disabled={desabilitado || enviandoProblemaExtra}
                        className="w-full flex items-center justify-center gap-2 border-2 border-dashed border-danger/20 bg-white/50 text-danger/80 py-3 rounded-lg hover:bg-danger/5 transition-colors text-sm font-medium"
                      >
                        <Camera className="w-4 h-4" /> Anexar Foto
                      </button>
                    )}
                  </div>

                  <div className="flex gap-2 mt-1">
                    <button
                      type="button"
                      disabled={desabilitado || enviandoProblemaExtra}
                      onClick={() => {
                        setMostrarProblemaExtra(false);
                        setRelatoExtra("");
                        setFotoExtra(null);
                      }}
                      className="flex-1 py-2.5 rounded-lg border border-danger/20 text-danger/80 font-semibold text-sm hover:bg-danger/5"
                    >
                      Cancelar
                    </button>
                    <button
                      type="button"
                      disabled={desabilitado || enviandoProblemaExtra || !relatoExtra.trim()}
                      onClick={async () => {
                         if (!onRegistrarProblemaExtra) return;
                         setEnviandoProblemaExtra(true);
                         try {
                           await onRegistrarProblemaExtra(etapa.etapaPadraoId, relatoExtra, fotoExtra);
                           setMostrarProblemaExtra(false);
                         } catch (e) {} finally { setEnviandoProblemaExtra(false); }
                      }}
                      className="flex-[2] py-2.5 rounded-lg bg-danger text-white font-semibold text-sm hover:bg-danger-hover disabled:opacity-50 flex justify-center items-center gap-2"
                    >
                      {enviandoProblemaExtra ? <Loader2 className="w-4 h-4 animate-spin" /> : "Enviar Problema"}
                    </button>
                  </div>
                </div>
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
                  onClick={() => handleRemover(fotoUrl)}
                  disabled={removendo === fotoUrl}
                  className="absolute right-1 top-1 flex h-7 w-7 items-center justify-center rounded-full bg-ink/70 text-white opacity-100 transition-all hover:bg-danger md:opacity-0 md:group-hover:opacity-100"
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
