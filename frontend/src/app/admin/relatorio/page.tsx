"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/lib/auth";
import { api, ApiError } from "@/lib/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { StatusBadge } from "@/components/StatusBadge";
import { Download, Search, FileText, CheckCircle, XCircle, TrendingUp } from "lucide-react";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from "recharts";
import Papa from "papaparse";
import type { LimpezaFinaResumo, UsuarioResumo, StatusLimpeza } from "@/lib/types";

type GraficoVisao = "Total" | "Aprovadas" | "Reprovadas";

export default function RelatorioPage() {
  const { usuario } = useAuth();
  const [limpezas, setLimpezas] = useState<LimpezaFinaResumo[]>([]);
  const [operadores, setOperadores] = useState<UsuarioResumo[]>([]);
  const [carregando, setCarregando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  const [dataInicio, setDataInicio] = useState("");
  const [dataFim, setDataFim] = useState("");
  const [status, setStatus] = useState<StatusLimpeza | "">("");
  const [operadorId, setOperadorId] = useState("");
  
  const [graficoVisao, setGraficoVisao] = useState<GraficoVisao>("Total");

  useEffect(() => {
    if (!usuario) return;
    api.listarUsuarios(usuario.token).then(us => {
      setOperadores(us.filter(u => u.perfil === "Operador"));
    }).catch(console.error);

    const hj = new Date();
    const dFim = hj.toISOString().split("T")[0];
    const dIni = new Date(hj.setDate(hj.getDate() - 7)).toISOString().split("T")[0];
    setDataInicio(dIni);
    setDataFim(dFim);
  }, [usuario]);

  useEffect(() => {
    if (dataInicio && dataFim) handleBuscar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dataInicio, dataFim]);

  async function handleBuscar() {
    if (!usuario) return;
    setErro(null);
    setCarregando(true);
    try {
      const res = await api.obterRelatorioLimpezas(usuario.token, { 
        dataInicio, 
        dataFim, 
        status: status || undefined, 
        operadorId: operadorId || undefined 
      });
      setLimpezas(res);
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao buscar relatório.");
    } finally {
      setCarregando(false);
    }
  }

  function exportarCSV() {
    const csv = Papa.unparse(limpezas.map(l => ({
      Prefixo: l.prefixo,
      OS: l.numeroOS || "",
      Operador: l.nomeOperador,
      Status: l.status,
      Iniciada: new Date(l.iniciadaEm).toLocaleString("pt-BR"),
      Finalizada: l.finalizadaEm ? new Date(l.finalizadaEm).toLocaleString("pt-BR") : "",
      Cortinas: l.cortinasRetiradas ? "Sim" : "Não",
    })));
    const blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", `relatorio_limpezas_${new Date().getTime()}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  const limpezasFiltradasPorVisao = limpezas.filter(l => {
    if (graficoVisao === "Aprovadas") return l.status === "Aprovada";
    if (graficoVisao === "Reprovadas") return l.status === "Reprovada";
    return true; // Total
  });

  const chartDataMap = new Map<string, number>();
  limpezasFiltradasPorVisao.forEach(l => {
    const dia = new Date(l.iniciadaEm).toLocaleDateString("pt-BR", { day: "2-digit", month: "2-digit" });
    chartDataMap.set(dia, (chartDataMap.get(dia) || 0) + 1);
  });
  
  const chartData = Array.from(chartDataMap.entries())
    .map(([dia, total]) => ({ dia, total }))
    .reverse();

  const total = limpezas.length;
  const aprovadas = limpezas.filter(l => l.status === "Aprovada").length;
  const reprovadas = limpezas.filter(l => l.status === "Reprovada").length;
  
  const corDoGrafico = graficoVisao === "Aprovadas" ? "#16a34a" : graficoVisao === "Reprovadas" ? "#dc2626" : "#C41230";

  if (!usuario) return null;

  return (
    <div className="mx-auto max-w-6xl pb-10">
      <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-ink">Relatórios</h1>
          <p className="text-sm text-ink/60">Análise de execuções de limpeza fina.</p>
        </div>
        <Button onClick={exportarCSV} disabled={limpezas.length === 0} variant="outline" className="shrink-0 bg-white">
          <Download className="mr-2 h-4 w-4" /> Exportar CSV
        </Button>
      </div>

      <Card className="mb-8">
        <CardContent className="p-5">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-5 items-end">
            <div>
              <label className="mb-1 block text-sm font-medium text-ink">Data Início</label>
              <input 
                type="date" 
                value={dataInicio} 
                onChange={e => setDataInicio(e.target.value)}
                className="w-full rounded-md border border-line bg-white p-2 text-sm text-ink outline-none focus:border-brand"
              />
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium text-ink">Data Fim</label>
              <input 
                type="date" 
                value={dataFim} 
                onChange={e => setDataFim(e.target.value)}
                className="w-full rounded-md border border-line bg-white p-2 text-sm text-ink outline-none focus:border-brand"
              />
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium text-ink">Status</label>
              <select 
                value={status} 
                onChange={e => setStatus(e.target.value as StatusLimpeza)}
                className="w-full rounded-md border border-line bg-white p-2 text-sm text-ink outline-none focus:border-brand"
              >
                <option value="">Todos</option>
                <option value="EmAndamento">Em Andamento</option>
                <option value="Concluida">Aguardando Avaliação</option>
                <option value="Aprovada">Aprovada</option>
                <option value="Reprovada">Reprovada</option>
              </select>
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium text-ink">Operador</label>
              <select 
                value={operadorId} 
                onChange={e => setOperadorId(e.target.value)}
                className="w-full rounded-md border border-line bg-white p-2 text-sm text-ink outline-none focus:border-brand"
              >
                <option value="">Todos</option>
                {operadores.map(op => (
                  <option key={op.id} value={op.id}>{op.nome}</option>
                ))}
              </select>
            </div>
            <Button onClick={handleBuscar} loading={carregando} className="w-full">
              <Search className="mr-2 h-4 w-4" /> Filtrar
            </Button>
          </div>
        </CardContent>
      </Card>

      {erro && (
        <div className="mb-6 rounded-md bg-danger/10 p-4 text-sm text-danger">{erro}</div>
      )}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3 mb-8">
        <Card 
          className={`cursor-pointer transition-all hover:-translate-y-1 ${graficoVisao === "Total" ? "ring-2 ring-brand border-transparent shadow-md" : "hover:border-brand/40"}`}
          onClick={() => setGraficoVisao("Total")}
        >
          <CardContent className="flex items-center gap-4 p-6">
            <div className="flex h-12 w-12 items-center justify-center rounded-full bg-brand/10">
              <FileText className="h-6 w-6 text-brand" />
            </div>
            <div>
              <p className="text-sm font-medium text-ink/60">Total de Limpezas</p>
              <p className="text-2xl font-bold text-ink">{total}</p>
            </div>
          </CardContent>
        </Card>
        
        <Card 
          className={`cursor-pointer transition-all hover:-translate-y-1 ${graficoVisao === "Aprovadas" ? "ring-2 ring-success border-transparent shadow-md" : "hover:border-success/40"}`}
          onClick={() => setGraficoVisao("Aprovadas")}
        >
          <CardContent className="flex items-center gap-4 p-6">
            <div className="flex h-12 w-12 items-center justify-center rounded-full bg-success/10">
              <CheckCircle className="h-6 w-6 text-success" />
            </div>
            <div>
              <p className="text-sm font-medium text-ink/60">Aprovadas</p>
              <p className="text-2xl font-bold text-ink">{aprovadas}</p>
            </div>
          </CardContent>
        </Card>
        
        <Card 
          className={`cursor-pointer transition-all hover:-translate-y-1 ${graficoVisao === "Reprovadas" ? "ring-2 ring-danger border-transparent shadow-md" : "hover:border-danger/40"}`}
          onClick={() => setGraficoVisao("Reprovadas")}
        >
          <CardContent className="flex items-center gap-4 p-6">
            <div className="flex h-12 w-12 items-center justify-center rounded-full bg-danger/10">
              <XCircle className="h-6 w-6 text-danger" />
            </div>
            <div>
              <p className="text-sm font-medium text-ink/60">Reprovadas</p>
              <p className="text-2xl font-bold text-ink">{reprovadas}</p>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-8 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="text-base flex items-center gap-2">
              <TrendingUp className="h-5 w-5" style={{ color: corDoGrafico }} />
              Execuções por Dia: {graficoVisao}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-[300px] w-full">
              {chartData.length > 0 ? (
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={chartData} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#E4E4E7" />
                    <XAxis dataKey="dia" axisLine={false} tickLine={false} tick={{ fontSize: 12, fill: '#71717A' }} dy={10} />
                    <YAxis axisLine={false} tickLine={false} tick={{ fontSize: 12, fill: '#71717A' }} />
                    <Tooltip 
                      cursor={{ fill: '#F4F4F5' }}
                      contentStyle={{ borderRadius: '8px', border: '1px solid #E4E4E7', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
                    />
                    <Bar dataKey="total" fill={corDoGrafico} radius={[4, 4, 0, 0]} barSize={40} />
                  </BarChart>
                </ResponsiveContainer>
              ) : (
                <div className="flex h-full items-center justify-center text-sm text-ink/40">
                  Nenhum dado para exibir no período.
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        <Card className="flex flex-col">
          <CardHeader>
            <CardTitle className="text-base">Registros Recentes ({graficoVisao})</CardTitle>
          </CardHeader>
          <CardContent className="flex-1 overflow-auto p-0">
            {limpezasFiltradasPorVisao.length === 0 ? (
              <div className="p-6 text-center text-sm text-ink/40">Nenhum registro encontrado.</div>
            ) : (
              <div className="divide-y divide-line">
                {limpezasFiltradasPorVisao.slice(0, 10).map(l => (
                  <div key={l.id} className="flex items-center justify-between p-4 hover:bg-surface/50">
                    <div>
                      <p className="font-bold text-ink">{l.prefixo}</p>
                      <p className="text-xs text-ink/60">{new Date(l.iniciadaEm).toLocaleDateString("pt-BR")}</p>
                    </div>
                    <StatusBadge status={l.status} />
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}