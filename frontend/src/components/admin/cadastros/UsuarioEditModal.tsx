import { useState, FormEvent, useEffect } from "react";
import { X, Check } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { api, ApiError } from "@/lib/api";
import type { UsuarioResumo } from "@/lib/types";

interface UsuarioEditModalProps {
  aberto: boolean;
  usuario: UsuarioResumo | null;
  onClose: () => void;
  onSuccess: (usuarioAtualizado: UsuarioResumo) => void;
  token: string;
}

export function UsuarioEditModal({ aberto, usuario, onClose, onSuccess, token }: UsuarioEditModalProps) {
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    if (usuario && aberto) {
      setNome(usuario.nome);
      setEmail(usuario.email || "");
      setSenha("");
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
        nome,
        email: email.trim() || null,
        senha: senha || null
      });
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
          <h3 className="font-bold text-lg text-ink">Editar Usuário</h3>
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
              <label className="block text-sm font-medium text-ink/70 mb-1">Matrícula</label>
              <Input value={usuario.matricula} disabled className="bg-ink/5 text-ink/50" />
            </div>

            <div>
              <label className="block text-sm font-medium text-ink/70 mb-1">Nome</label>
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
                type="password"
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
                placeholder="Deixe em branco para manter a atual"
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