using System;
using MySqlConnector;

namespace AlterDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var cs = "Server=gateway01.sa-east-1.prod.aws.tidbcloud.com;Port=4000;Database=catarinense_limpeza;User=HwPSkJfTt3LTMvZ.root;Password=F5Zf3wc9s5011MDu;SslMode=Preferred;";
            using var connection = new MySqlConnection(cs);
            connection.Open();

            try {
                using var cmd = new MySqlCommand("ALTER TABLE usuarios ADD COLUMN PrecisaTrocarSenha TINYINT(1) NOT NULL DEFAULT 1;", connection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Coluna PrecisaTrocarSenha adicionada!");
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }

            try {
                var sql = @"
CREATE TABLE etapas_itens_padrao (
    Id CHAR(36) COLLATE ascii_bin NOT NULL,
    EtapaPadraoId CHAR(36) COLLATE ascii_bin NOT NULL,
    Texto VARCHAR(255) NOT NULL,
    Ordem INT NOT NULL,
    EnquadramentoFoto VARCHAR(500) NULL,
    Ativo TINYINT(1) NOT NULL DEFAULT 1,
    DataCriacao DATETIME(6) NOT NULL,
    DataAtualizacao DATETIME(6) NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_EtapaItemPadrao_EtapaPadrao FOREIGN KEY (EtapaPadraoId) REFERENCES etapas_padrao (Id) ON DELETE CASCADE
);";
                using var cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Tabela etapas_itens_padrao criada!");
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }

            try {
                var sql = @"
CREATE TABLE etapas_itens_execucao (
    Id CHAR(36) COLLATE ascii_bin NOT NULL,
    LimpezaEtapaExecucaoId CHAR(36) COLLATE ascii_bin NOT NULL,
    EtapaItemPadraoId CHAR(36) COLLATE ascii_bin NOT NULL,
    Status VARCHAR(50) NOT NULL,
    Funcionalidade VARCHAR(50) NOT NULL,
    RelatoProblema VARCHAR(1000) NULL,
    FotoUrl VARCHAR(1000) NULL,
    Concluida TINYINT(1) NOT NULL,
    DataCriacao DATETIME(6) NOT NULL,
    DataAtualizacao DATETIME(6) NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_EtapaItemExecucao_LimpezaEtapaExecucao FOREIGN KEY (LimpezaEtapaExecucaoId) REFERENCES limpeza_etapa_execucoes (Id) ON DELETE CASCADE
);";
                using var cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Tabela etapas_itens_execucao criada!");
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
