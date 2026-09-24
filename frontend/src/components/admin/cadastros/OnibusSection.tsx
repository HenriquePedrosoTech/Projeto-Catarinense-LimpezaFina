"use client";

import { useEffect, useState, type ChangeEvent, type FormEvent } from "react";
import { api, ApiError } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { ConfirmDialog, AlertDialog } from "@/components/ui/Dialogs";
import { Input } from "@/components/ui/Input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { Bus, UploadCloud, Trash2, CheckCircle2, AlertTriangle } from "lucide-react";
import type { OnibusResumo, ResultadoImportacaoOnibus } from "@/lib/types";

export function OnibusSection({ token }: { token: string }) {
  const [lista, setLista] = useState<OnibusResumo[]>([]);
  const [busca, setBusca] = useState("");
  const [prefixo, setPrefixo] = useState("");
  const [placa, setPlaca] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);
  const [excluindoId, setExcluindoId] = useState<string | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<{aberto: boolean, id: string, prefixo: string}>({aberto: false, id: "", prefixo: ""});
  const [alertInfo, setAlertInfo] = useState<{aberto: boolean, mensagem: string}>({aberto: false, mensagem: ""});

  const listaFiltrada = lista.filter(o => o.prefixo.toLowerCase().includes(busca.toLowerCase()));

  const [importando, setImportando] = useState(false);
  const [resultadoImportacao, setResultadoImportacao] = useState<ResultadoImportacaoOnibus | null>(null);
  const [erroImportacao, setErroImportacao] = useState<string | null>(null);

  const carregar = () => {
    api.listarOnibus(token).then(setLista).catch(() => {});
  };

  useEffect(carregar, [token]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setEnviando(true);
    try {
      await api.cadastrarOnibus(token, prefixo.trim().toUpperCase(), placa.trim());
      setPrefixo("");
      setPlaca("");
      carregar();
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao cadastrar.");
    } finally {
      setEnviando(false);
    }
  }

  async function handleImportarPlanilha(e: ChangeEvent<HTMLInputElement>) {
    const arquivo = e.target.files?.[0];
    if (!arquivo) return;

    setErroImportacao(null);
    setResultadoImportacao(null);
    setImportando(true);
    try {
      const resultado = await api.importarOnibusPlanilha(token, arquivo);
      setResultadoImportacao(resultado);
      carregar();
    } catch (err) {
      setErroImportacao(err instanceof ApiError ? err.message : "Falha ao importar o arquivo.");
    } finally {
      setImportando(false);
      e.target.value = "";
    }
  }

  async function handleExcluir(id: string, prefixo: string) {
    setConfirmDelete({ aberto: true, id, prefixo });
  }

  async function executarExclusao() {
    const { id } = confirmDelete;
    setExcluindoId(id);
    try {
      await api.excluirOnibus(token, id);
      setLista((atual) => atual.filter((u) => u.id !== id));
    } catch (err) {
      setAlertInfo({ aberto: true, mensagem: err instanceof ApiError ? err.message : "Falha ao excluir." });
    } finally {
      setExcluindoId(null);
    }
  }

  return (
    <div className="grid gap-6 md:grid-cols-2">
      <ConfirmDialog 
        aberto={confirmDelete.aberto}
        titulo="Excluir Onibus"
        mensagem={`Tem certeza que deseja excluir o onibus ${confirmDelete.prefixo}?`}
        tipo="danger"
        textoConfirmar="Excluir"
        onClose={() => setConfirmDelete({ ...confirmDelete, aberto: false })}
        onConfirmar={executarExclusao}
      />
      <AlertDialog 
        aberto={alertInfo.aberto}
        titulo="Atenção"
        mensagem={alertInfo.mensagem}
        tipo="danger"
        onClose={() => setAlertInfo({ ...alertInfo, aberto: false })}
      />
      {/* Coluna Esquerda: Cadastro e Importação */}
      <div className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-lg">
              <Bus className="h-5 w-5 text-ink/50" />
              Cadastrar Manualmente
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
              <div className="flex gap-3">
                <Input
                  value={prefixo}
                  onChange={(e) => setPrefixo(e.target.value.toUpperCase())}
                  placeholder="Prefixo (ex.: 3228)"
                  required
                />
                <Input
                  value={placa}
                  onChange={(e) => setPlaca(e.target.value.toUpperCase())}
                  placeholder="Placa (opc.)"
                />
              </div>
              {erro && <p className="text-sm text-danger">{erro}</p>}
              <Button type="submit" loading={enviando} className="w-full">
                Adicionar Ônibus
              </Button>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-lg">Importar via Planilha</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4 text-sm text-ink/60">
              Aceita <code className="rounded bg-surface px-1">.xlsx</code> (Excel) ou{" "}
              <code className="rounded bg-surface px-1">.csv</code>. O sistema identifica a coluna "Prefixo" e "Placa" pelo nome.
            </p>
            <input
              type="file"
              accept=".csv,.xlsx"
              onChange={handleImportarPlanilha}
              disabled={importando}
              className="hidden"
              id="importar-csv-onibus"
            />
            <label
              htmlFor="importar-csv-onibus"
              className="flex w-full cursor-pointer items-center justify-center gap-2 rounded-md border border-dashed border-brand/50 bg-brand/5 py-4 text-sm font-medium text-brand transition-colors hover:bg-brand/10 hover:border-brand"
            >
              <UploadCloud className="h-5 w-5" />
              {importando ? "Importando aguarde..." : "Escolher arquivo (.xlsx ou .csv)"}
            </label>

            {erroImportacao && (
              <p className="mt-4 rounded-md bg-danger/10 p-3 text-sm text-danger">{erroImportacao}</p>
            )}

            {resultadoImportacao && (
              <div className="mt-4 space-y-2 rounded-md bg-surface p-4 text-sm">
                <p className="flex items-center gap-2 font-medium text-success">
                  <CheckCircle2 className="h-4 w-4" /> 
                  {resultadoImportacao.criados.length} criados com sucesso.
                </p>
                {resultadoImportacao.jaExistentes.length > 0 && (
                  <p className="flex items-start gap-2 text-ink/60">
                    <AlertTriangle className="mt-0.5 h-4 w-4 shrink-0 text-warning" />
                    <span>{resultadoImportacao.jaExistentes.length} ignorados (já existiam).</span>
                  </p>
                )}
                {resultadoImportacao.erros.length > 0 && (
                  <div className="mt-2 text-danger">
                    <p className="font-medium">{resultadoImportacao.erros.length} linha(s) com erro:</p>
                    <ul className="ml-4 list-disc opacity-80">
                      {resultadoImportacao.erros.slice(0, 5).map((e, i) => (
                        <li key={i}>{e.prefixo}: {e.motivo}</li>
                      ))}
                      {resultadoImportacao.erros.length > 5 && (
                        <li>e mais {resultadoImportacao.erros.length - 5}...</li>
                      )}
                    </ul>
                  </div>
                )}
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Coluna Direita: Lista */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Frota Cadastrada ({lista.length})</CardTitle>
          <div className="pt-2">
            <Input 
              placeholder="Buscar prefixo..." 
              value={busca}
              onChange={(e) => setBusca(e.target.value)}
              className="w-full bg-surface"
            />
          </div>
        </CardHeader>
        <CardContent>
          <div className="max-h-[500px] overflow-y-auto pr-2">
            <ul className="space-y-2">
              {listaFiltrada.map((o) => (
                <li key={o.id} className="flex items-center justify-between rounded-md border border-line bg-surface/30 px-4 py-2 text-sm transition-colors hover:bg-surface">
                  <div className="flex gap-3">
                    <span className="font-bold text-ink">{o.prefixo}</span>
                    <span className="text-ink/40">{o.placa ?? "Sem placa"}</span>
                  </div>
                  <button
                    onClick={() => handleExcluir(o.id, o.prefixo)}
                    disabled={excluindoId === o.id}
                    className="text-ink/30 transition-colors hover:text-danger disabled:opacity-50"
                    title="Excluir"
                  >
                    <Trash2 className="h-4 w-4" />
                  </button>
                </li>
              ))}
              {listaFiltrada.length === 0 && (
                <li className="text-center text-sm text-ink/40 py-4">Nenhum ônibus encontrado.</li>
              )}
            </ul>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}


