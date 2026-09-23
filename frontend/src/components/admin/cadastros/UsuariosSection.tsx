"use client";

import { useEffect, useState, type FormEvent } from "react";
import { api, ApiError } from "@/lib/api";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { ConfirmDialog, AlertDialog } from "@/components/ui/Dialogs";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/Card";
import { Users, Trash2, ShieldCheck, User as UserIcon, Edit2 } from "lucide-react";
import { UsuarioEditModal } from "./UsuarioEditModal";
import type { UsuarioResumo } from "@/lib/types";

export function UsuariosSection({ token }: { token: string }) {
  const [lista, setLista] = useState<UsuarioResumo[]>([]);
  const [matricula, setMatricula] = useState("");
  const [nome, setNome] = useState("");
  const [senha, setSenha] = useState("");
  const [email, setEmail] = useState("");
  const [perfil, setPerfil] = useState<"Operador" | "Administrador">("Operador");
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);
  const [excluindoId, setExcluindoId] = useState<string | null>(null);
  const [editandoUsuario, setEditandoUsuario] = useState<UsuarioResumo | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<{aberto: boolean, id: string, nomeUsuario: string}>({aberto: false, id: "", nomeUsuario: ""});
  const [alertInfo, setAlertInfo] = useState<{aberto: boolean, mensagem: string}>({aberto: false, mensagem: ""});

  const carregar = () => {
    api.listarUsuarios(token).then(setLista).catch(() => {});
  };

  useEffect(carregar, [token]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setEnviando(true);
    try {
      await api.cadastrarUsuario(token, matricula.trim(), nome.trim(), senha, perfil, email.trim());
      setMatricula("");
      setNome("");
      setSenha("");
      setEmail("");
      carregar();
    } catch (err) {
      setErro(err instanceof ApiError ? err.message : "Falha ao cadastrar.");
    } finally {
      setEnviando(false);
    }
  }

  async function handleExcluir(id: string, nomeUsuario: string) {
    setConfirmDelete({ aberto: true, id, nomeUsuario });
  }

  async function handleEditSuccess(usuarioAtualizado: UsuarioResumo) {
    setLista(lista.map(u => u.id === usuarioAtualizado.id ? usuarioAtualizado : u));
    setAlertInfo({ aberto: true, mensagem: "Usuário atualizado com sucesso!" });
  }

  async function executarExclusao() {
    const { id } = confirmDelete;
    setExcluindoId(id);
    try {
      await api.excluirUsuario(token, id);
      setLista((atual) => atual.filter((u) => u.id !== id));
    } catch (err) {
      setAlertInfo({ aberto: true, mensagem: err instanceof ApiError ? err.message : "Falha ao excluir." });
    } finally {
      setExcluindoId(null);
    }
  }

  return (
    <div className="grid gap-6 lg:grid-cols-3">
      <ConfirmDialog 
        aberto={confirmDelete.aberto}
        titulo="Excluir Usu�rio"
        mensagem={`Tem certeza que deseja excluir o usuario "${confirmDelete.nomeUsuario}"?`}
        tipo="danger"
        textoConfirmar="Excluir"
        onClose={() => setConfirmDelete({ ...confirmDelete, aberto: false })}
        onConfirmar={executarExclusao}
      />
      <AlertDialog 
        aberto={alertInfo.aberto}
        titulo="Aten��o"
        mensagem={alertInfo.mensagem}
        tipo="danger"
        onClose={() => setAlertInfo({ ...alertInfo, aberto: false })}
      />
      {/* Coluna de Cadastro */}
      <div className="lg:col-span-1">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-lg">
              <Users className="h-5 w-5 text-ink/50" />
              Novo Usuário
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
              <div className="flex gap-2">
                <div className="flex-1">
                  <Input
                    label="Matrícula"
                    value={matricula}
                    onChange={(e) => setMatricula(e.target.value)}
                    required
                  />
                </div>
                <div className="flex-1">
                  <label className="mb-1.5 block text-sm font-medium text-ink/80">Perfil</label>
                  <select
                    value={perfil}
                    onChange={(e) => setPerfil(e.target.value as "Operador" | "Administrador")}
                    className="flex h-10 w-full rounded-md border border-line bg-white px-3 py-2 text-sm text-ink outline-none focus:border-brand focus:ring-2 focus:ring-brand focus:ring-offset-1"
                  >
                    <option value="Operador">Operador</option>
                    <option value="Administrador">Admin</option>
                  </select>
                </div>
              </div>
              <Input
                label="Nome Completo"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
                required
              />
              <Input
                label="E-mail (Opcional)"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
              <Input
                label="Senha Provisória"
                type="password"
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
                required
              />
              
              {erro && <p className="text-sm text-danger">{erro}</p>}
              
              <Button type="submit" loading={enviando} className="mt-2">
                Cadastrar Usuário
              </Button>
            </form>
          </CardContent>
        </Card>
      </div>

      {/* Coluna da Lista */}
      <div className="lg:col-span-2">
        <Card>
          <CardHeader>
            <CardTitle className="text-lg">Equipe Cadastrada</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid gap-3 sm:grid-cols-2">
              {lista.map((u) => (
                <div key={u.id} className="flex flex-col justify-between rounded-md border border-line bg-surface/30 p-4 transition-colors hover:bg-surface/60">
                  <div className="mb-3 flex items-start justify-between gap-2">
                    <div className="flex items-center gap-2">
                      <div className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-full ${u.perfil === 'Administrador' ? 'bg-navy text-white' : 'bg-brand/10 text-brand'}`}>
                        {u.perfil === 'Administrador' ? <ShieldCheck className="h-4 w-4" /> : <UserIcon className="h-4 w-4" />}
                      </div>
                      <div>
                        <p className="font-bold text-ink leading-tight">{u.nome}</p>
                        <p className="text-xs font-medium text-ink/50">{u.perfil}</p>
                      </div>
                    </div>
                    <button
                      onClick={() => setEditandoUsuario(u)}
                      className="p-1 text-ink/40 hover:text-brand hover:bg-brand/10 rounded transition-colors mr-2"
                      title="Editar Usuário"
                    >
                      <Edit2 className="h-4 w-4" />
                    </button>
                    <button
                      onClick={() => handleExcluir(u.id, u.nome)}
                      disabled={excluindoId === u.id}
                      className="text-ink/30 hover:text-danger disabled:opacity-50"
                      title="Excluir"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                  <div className="text-sm text-ink/60">
                    <p>Matrícula: {u.matricula}</p>
                    {u.email && <p className="truncate">E-mail: {u.email}</p>}
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
      <UsuarioEditModal
        aberto={!!editandoUsuario}
        usuario={editandoUsuario}
        token={token}
        onClose={() => setEditandoUsuario(null)}
        onSuccess={handleEditSuccess}
      />
    </div>
  );
}



