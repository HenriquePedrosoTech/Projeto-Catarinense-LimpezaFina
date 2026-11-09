import { useState, FormEvent, useEffect } from "react";
import { X, Check, Eye, EyeOff } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { api, ApiError } from "@/lib/api";
import { useAuth } from "@/lib/auth";
import type { UsuarioResumo } from "@/lib/types";

interface UsuarioEditModalProps {
  aberto: boolean;
  usuario: UsuarioResumo | null;
  onClose: () => void;
  onSuccess: (usuarioAtualizado: UsuarioResumo) => void;
  token: string;
}

export function UsuarioEditModal({ aberto, usuario, onClose, onSuccess, token }: UsuarioEditModalProps) {
  const { usuario: usuarioLogado, atualizarUsuario } = useAuth();
  const [matricula, setMatricula] = useState("");
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [mostrarSenha, setMostrarSenha] = useState(false);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    if (usuario && aberto) {
      setMatricula(usuario.matricula);
      setNome(usuario.nome);
      setEmail(usuario.email || "");
      setSenha("");
      setMostrarSenha(false);
      setErro(null);
    }
  }, [usuario, aberto]);

  if (!aberto || !usuario) return null;

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);
    setSalvando(true);

    try {
      const response = await api.editarUsuario(token, usuario!.id, {
        id: usuario!.id,
        matricula: matricula.trim(),
        nome: nome.trim(),
        email: email.trim() || null,
        senha: senha || null
      });
      if (usuarioLogado && usuarioLogado.usuarioId === response.id) {
        atualizarUsuario({
          ...usuarioLogado,
          nome: response.nome,
          matricula: response.matricula,
          isMaster: response.isMaster ?? usuarioLogado.isMaster,
        });
      }
      onSuccess(response);
      onClose();
    } catch (err) {
      if (err instanceof ApiError) {
        setErro(err.message);
      } else {
        setErro("Ocorreu um erro ao atualizar o usuário.");
      }
    } finally {
      setSalvando(false);
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-ink/40 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-md overflow-hidden flex flex-col max-h-[90vh]">
        <div className="flex items-center justify-between p-4 border-b border-ink/10">
          <h3 className="font-bold text-lg text-ink">Editar Usuário e Acesso</h3>
          <button onClick={onClose} className="p-2 text-ink/50 hover:text-ink hover:bg-ink/5 rounded-full transition-colors">
            <X className="h-5 w-5" />
          </button>
        </div>
        
        <div className="p-6 overflow-y-auto">
          {erro && (
            <div className="mb-4 p-3 bg-danger/10 text-danger text-sm rounded-lg border border-danger/20">
              {erro}
            </div>
          )}

          <form id="edit-user-form" onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-ink/70 mb-1">Login / Matrícula de Acesso</label>
              <Input
                value={matricula}
                onChange={(e) => setMatricula(e.target.value)}
                placeholder="Ex.: 00123 ou joao.silva"
                required
              />
              <p className="text-xs text-ink/50 mt-1">Pode ser um número de matrícula ou um login personalizado.</p>
            </div>

            <div>
              <label className="block text-sm font-medium text-ink/70 mb-1">Nome de Usuário (também válido para login)</label>
              <Input
                value={nome}
                onChange={(e) => setNome(e.target.value)}
                required
              />
            </div>

            {usuario.perfil === "Administrador" && (
              <div>
                <label className="block text-sm font-medium text-ink/70 mb-1">E-mail</label>
                <Input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="admin@empresa.com"
                />
                <p className="text-xs text-ink/50 mt-1">Necessário para receber notificações de limpeza concluída.</p>
              </div>
            )}

            <div>
              <label className="block text-sm font-medium text-ink/70 mb-1">Nova Senha</label>
              <Input
                type={mostrarSenha ? "text" : "password"}
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
                placeholder="Deixe em branco para manter a atual"
                rightElement={
                  <button
                    type="button"
                    onClick={() => setMostrarSenha(!mostrarSenha)}
                    className="text-ink/40 hover:text-ink/60 focus:outline-none"
                  >
                    {mostrarSenha ? <EyeOff size={18} /> : <Eye size={18} />}
                  </button>
                }
              />
            </div>
          </form>
        </div>

        <div className="p-4 border-t border-ink/10 bg-ink/5 flex justify-end gap-3">
          <Button variant="outline" onClick={onClose} disabled={salvando}>
            Cancelar
          </Button>
          <Button type="submit" form="edit-user-form" loading={salvando} className="flex items-center gap-2">
            {!salvando && <Check className="h-4 w-4" />}
            Salvar Alterações
          </Button>
        </div>
      </div>
    </div>
  );
}
