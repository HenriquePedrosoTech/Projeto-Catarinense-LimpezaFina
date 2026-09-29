"use client";

import { useState, useRef, useEffect } from "react";
import type { EtapaItemResumo } from "@/lib/types";
import { resolverUrlFoto } from "@/lib/fotos";
import { Camera, Check, Upload, Loader2, X, AlertTriangle } from "lucide-react";
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
  const [arquivoPreview, setArquivoPreview] = useState<string | null>(null);
  
  const [enviandoNormal, setEnviandoNormal] = useState(false);
  const [enviandoAvaria, setEnviandoAvaria] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  const [mostrarAvaria, setMostrarAvaria] = useState(false);
  const [mostrarOpcoesFoto, setMostrarOpcoesFoto] = useState(false);

  const inputCameraRef = useRef<HTMLInputElement>(null);
  const inputGaleriaRef = useRef<HTMLInputElement>(null);

  const temAvariaRegistrada = item.status === "Não Conforme" || item.funcionalidade === "Com Defeito";

  useEffect(() => {
    if (arquivo) {
      const url = URL.createObjectURL(arquivo);
      setArquivoPreview(url);
      return () => URL.revokeObjectURL(url);
    } else {
      setArquivoPreview(null);
    }
  }, [arquivo]);

  async function handleMarcarConcluido() {
    if (!onEnviarItem || item.concluida || desabilitado) return;
    
    // Se há uma instrução de foto (enquadramentoFoto) é porque a foto é obrigatória mesmo no fluxo normal
    if (item.enquadramentoFoto && !arquivo && !item.fotoUrl) {
      setErro("É necessário anexar a foto solicitada acima antes de marcar como concluído.");
      return;
    }

    setErro(null);
    setEnviandoNormal(true);
    try {
      // Quando marca pelo checkbox normal, é porque está tudo OK.
      await onEnviarItem(item.id, "Conforme", "OK / Funcional", null, arquivo);
    } catch (err: any) {
      setErro(err.message || "Erro ao salvar item.");
    } finally {
      setEnviandoNormal(false);
    }
  }

  async function handleSalvarAvaria() {
    if (!onEnviarItem) return;
    setErro(null);

    if (!relato.trim()) {
      setErro("É obrigatório descrever o problema encontrado.");
      return;
    }

    if (!arquivo && !item.fotoUrl) {
      setErro("É obrigatório enviar uma foto evidenciando o problema.");
      return;
    }

    setEnviandoAvaria(true);
    try {
      await onEnviarItem(item.id, status === "Conforme" ? "Não Conforme" : status, funcionalidade === "OK / Funcional" ? "Com Defeito" : funcionalidade, relato.trim(), arquivo);
      setMostrarAvaria(false);
    } catch (err: any) {
      setErro(err.message || "Erro ao salvar avaria.");
    } finally {
      setEnviandoAvaria(false);
    }
  }

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    if (e.target.files && e.target.files[0]) {
      setArquivo(e.target.files[0]);
    }
    setMostrarOpcoesFoto(false);
  }

  const isEnviando = enviandoNormal || enviandoAvaria;

  return (
    <div className={`p-4 rounded-xl border ${item.concluida ? (temAvariaRegistrada ? "bg-danger/5 border-danger/30" : "bg-success/5 border-success/40") : "bg-white border-line shadow-sm"}`}>
      
      {/* Cabeçalho do Item (Checkbox e Título) */}
      <div className="flex items-start gap-3">
        <button 
          type="button"
          onClick={handleMarcarConcluido}
          disabled={desabilitado || item.concluida || isEnviando}
          className={`mt-0.5 relative flex h-6 w-6 shrink-0 items-center justify-center rounded-md border-2 transition-all ${
            item.concluida 
              ? (temAvariaRegistrada ? "border-danger bg-danger text-white" : "border-success bg-success text-white") 
              : "border-line bg-surface hover:border-brand"
          } ${isEnviando ? "opacity-50 cursor-not-allowed" : "cursor-pointer"}`}
        >
          {enviandoNormal ? (
            <Loader2 className="h-3 w-3 animate-spin" />
          ) : (
            item.concluida && <Check className="h-4 w-4" strokeWidth={3} />
          )}
        </button>
        
        <div className="flex-1 flex flex-col pt-0.5">
          <h4 className={`font-semibold text-sm leading-tight ${item.concluida ? "text-ink/70" : "text-ink"}`}>
            {item.ordem + 1}º {item.texto}
            {!item.obrigatorio && <span className="text-[10px] font-normal text-ink/50 bg-ink/5 px-2 py-0.5 rounded-full ml-2 uppercase">Opcional</span>}
          </h4>
          
          {item.enquadramentoFoto && !item.concluida && (
            <p className="text-xs text-ink/60 mt-1.5 flex items-start gap-1.5 bg-surface/50 p-2 rounded-md border border-line">
              <Camera className="w-3.5 h-3.5 shrink-0 mt-0.5" /> 
              <span><strong>Foto necessária:</strong> {item.enquadramentoFoto}</span>
            </p>
          )}
        </div>
      </div>

      {erro && !mostrarAvaria && <p className="text-xs text-danger mt-3 font-semibold ml-9">{erro}</p>}

      {/* Inputs Injetados (Camera e Galeria ocultos usados por todos os botões de foto) */}
      {!item.concluida && (
        <>
          <input ref={inputCameraRef} type="file" accept="image/*" capture="environment" onChange={handleFileChange} className="hidden" />
          <input ref={inputGaleriaRef} type="file" accept="image/*" onChange={handleFileChange} className="hidden" />
        </>
      )}

      {/* Botões de Ação Normais (Foto e Relatar Problema) */}
      {!item.concluida && !mostrarAvaria && !desabilitado && (
        <div className="mt-3 ml-9 flex flex-col gap-3">
          
          {/* Se a etapa exige foto para o fluxo normal, exibe o upload aqui */}
          {item.enquadramentoFoto && (
             <div className="flex flex-col gap-2">
              {arquivoPreview ? (
                <div className="relative inline-block w-24 h-24 rounded-lg overflow-hidden border border-line">
                  <img src={arquivoPreview} alt="Preview" className="w-full h-full object-cover" />
                  <button 
                    type="button" 
                    onClick={() => { setArquivo(null); setArquivoPreview(null); }} 
                    className="absolute top-1 right-1 bg-danger text-white rounded-full p-1 shadow-md hover:bg-danger/80"
                  >
                    <X className="w-3 h-3" />
                  </button>
                </div>
              ) : (
                <button 
                  type="button" 
                  onClick={() => window.innerWidth >= 640 ? inputGaleriaRef.current?.click() : setMostrarOpcoesFoto(true)}
                  className="flex items-center justify-center gap-2 w-full sm:w-auto px-4 py-2 rounded-lg border border-line text-ink hover:bg-surface transition-colors text-xs font-semibold"
                >
                  <Camera className="w-3.5 h-3.5" /> Anexar Foto
                </button>
              )}
             </div>
          )}

          <div className="flex items-center">
            <button 
              type="button" 
              onClick={() => {
                setMostrarAvaria(true);
                if (status === "Conforme") setStatus("Não Conforme");
                if (funcionalidade === "OK / Funcional") setFuncionalidade("Com Defeito");
              }}
              className="flex items-center gap-1.5 text-xs font-semibold text-danger/80 hover:text-danger bg-danger/5 hover:bg-danger/10 px-3 py-1.5 rounded-lg transition-colors"
            >
              <AlertTriangle className="w-3.5 h-3.5" /> Relatar Problema
            </button>
          </div>
        </div>
      )}

      {/* Foto no estado concluído (seja avaria ou normal) */}
      {item.concluida && item.fotoUrl && !temAvariaRegistrada && (
        <div className="mt-2 ml-9">
          <img
            src={resolverUrlFoto(item.fotoUrl)}
            alt="Evidência"
            className="h-16 w-16 rounded-md object-cover cursor-pointer border border-line hover:opacity-80 transition-opacity"
            onClick={() => onAbrirFoto?.(item.fotoUrl!)}
          />
        </div>
      )}

      {/* Box de Avaria (Visível apenas se abrir ou se já tiver avaria registrada) */}
      {(mostrarAvaria || (item.concluida && temAvariaRegistrada)) && (
        <div className="mt-4 ml-9 p-3 sm:p-4 bg-danger/5 border border-danger/20 rounded-xl flex flex-col gap-3 relative">
          
          <div className="flex items-center justify-between mb-1">
            <h5 className="text-xs font-bold text-danger uppercase tracking-wider flex items-center gap-1.5">
              <AlertTriangle className="w-3.5 h-3.5" /> 
              {item.concluida ? "Problema Registrado" : "Relatório de Avaria"}
            </h5>
            {!item.concluida && (
              <button onClick={() => setMostrarAvaria(false)} className="text-danger/40 hover:text-danger p-1">
                <X className="w-4 h-4" />
              </button>
            )}
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <label className="text-[11px] font-semibold mb-1 block text-danger/80 uppercase">Conformidade</label>
              <select 
                value={status} 
                onChange={(e) => setStatus(e.target.value)}
                disabled={desabilitado || item.concluida || isEnviando}
                className="w-full text-sm border-danger/20 bg-white rounded-lg text-ink focus:border-danger focus:ring-danger"
              >
                <option value="Conforme">Conforme</option>
                <option value="Não Conforme">Não Conforme</option>
                <option value="N/A">N/A</option>
              </select>
            </div>
            <div>
              <label className="text-[11px] font-semibold mb-1 block text-danger/80 uppercase">Funcionalidade</label>
              <select 
                value={funcionalidade} 
                onChange={(e) => setFuncionalidade(e.target.value)}
                disabled={desabilitado || item.concluida || isEnviando}
                className="w-full text-sm border-danger/20 bg-white rounded-lg text-ink focus:border-danger focus:ring-danger"
              >
                <option value="OK / Funcional">OK / Funcional</option>
                <option value="Com Defeito">Com Defeito</option>
              </select>
            </div>
          </div>

          <div>
            <label className="text-[11px] font-semibold mb-1 block text-danger/80 uppercase">Descrição do Problema *</label>
            <textarea
              value={relato}
              onChange={(e) => setRelato(e.target.value)}
              disabled={desabilitado || item.concluida || isEnviando}
              className="w-full text-sm border-danger/20 bg-white rounded-lg min-h-[70px] focus:border-danger focus:ring-danger placeholder:text-ink/30"
              placeholder="Descreva a avaria ou o problema encontrado..."
            />
          </div>

          {/* Área da Foto na Avaria */}
          {!item.concluida && (
            <div className="flex flex-col gap-2 mt-1">
              <label className="text-[11px] font-semibold text-danger/80 uppercase">Evidência Fotográfica *</label>
              {arquivoPreview ? (
                <div className="relative inline-block w-24 h-24 rounded-lg overflow-hidden border-2 border-danger/30">
                  <img src={arquivoPreview} alt="Preview" className="w-full h-full object-cover" />
                  <button 
                    type="button" 
                    onClick={() => { setArquivo(null); setArquivoPreview(null); }} 
                    className="absolute top-1 right-1 bg-danger text-white rounded-full p-1 shadow-md hover:bg-danger/80"
                  >
                    <X className="w-3 h-3" />
                  </button>
                </div>
              ) : (
                <button 
                  type="button" 
                  onClick={() => window.innerWidth >= 640 ? inputGaleriaRef.current?.click() : setMostrarOpcoesFoto(true)}
                  className="flex items-center justify-center gap-2 w-full sm:w-auto px-4 py-2.5 rounded-lg border-2 border-dashed border-danger/30 text-danger hover:bg-danger hover:text-white transition-colors text-sm font-semibold"
                >
                  <Camera className="w-4 h-4" /> Adicionar Foto do Problema
                </button>
              )}
            </div>
          )}

          {item.concluida && item.fotoUrl && (
            <div className="mt-1">
              <label className="text-[11px] font-semibold text-danger/80 uppercase block mb-1">Evidência Fotográfica</label>
              <img
                src={resolverUrlFoto(item.fotoUrl)}
                alt="Evidência"
                className="h-20 w-20 rounded-lg object-cover cursor-pointer border border-danger/20 hover:opacity-80 transition-opacity"
                onClick={() => onAbrirFoto?.(item.fotoUrl!)}
              />
            </div>
          )}

          {erro && mostrarAvaria && <p className="text-xs text-danger font-semibold">{erro}</p>}

          {!item.concluida && (
            <div className="mt-2 flex justify-end">
              <Button type="button" onClick={handleSalvarAvaria} disabled={isEnviando} className="bg-danger hover:bg-danger/90 text-white w-full sm:w-auto">
                {enviandoAvaria ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : <AlertTriangle className="w-4 h-4 mr-2" />}
                Registrar Problema
              </Button>
            </div>
          )}
        </div>
      )}

      {/* Modal de Escolha de Foto */}
      {mostrarOpcoesFoto && (
        <div className="fixed inset-0 z-[100] flex items-end justify-center bg-ink/50 backdrop-blur-sm sm:items-center animate-in fade-in">
          <div className="w-full bg-white rounded-t-3xl sm:rounded-2xl sm:max-w-sm overflow-hidden p-6 pb-12 sm:pb-6 animate-in slide-in-from-bottom-10 sm:zoom-in-95 shadow-2xl">
            <h3 className="text-lg font-bold text-ink mb-4 text-center">Adicionar Evidência</h3>
            <div className="flex flex-col gap-3">
              <button onClick={() => { inputCameraRef.current?.click(); }} className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 hover:text-brand font-semibold text-ink transition-colors">
                <Camera className="h-5 w-5" /> Tirar Foto
              </button>
              <button onClick={() => { inputGaleriaRef.current?.click(); }} className="flex items-center gap-3 w-full p-4 rounded-xl bg-surface hover:bg-brand/10 hover:text-brand font-semibold text-ink transition-colors">
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
