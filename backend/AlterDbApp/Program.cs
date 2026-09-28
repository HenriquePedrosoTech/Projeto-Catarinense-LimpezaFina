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
                var createItensPadrao = @"
                    CREATE TABLE IF NOT EXISTS etapas_itens_padrao (
                        Id CHAR(36) COLLATE ascii_bin NOT NULL PRIMARY KEY,
                        EtapaPadraoId CHAR(36) COLLATE ascii_bin NOT NULL,
                        Texto VARCHAR(300) NOT NULL,
                        EnquadramentoFoto VARCHAR(300) NULL,
                        Ordem INT NOT NULL,
                        Ativo TINYINT(1) NOT NULL,
                        CriadoEm DATETIME(6) NOT NULL,
                        CONSTRAINT FK_etapas_itens_padrao_EtapaPadraoId FOREIGN KEY (EtapaPadraoId) REFERENCES etapas_padrao (Id) ON DELETE CASCADE
                    );";
                    
                var createItensExecucao = @"
                    CREATE TABLE IF NOT EXISTS etapas_itens_execucao (
                        Id CHAR(36) COLLATE ascii_bin NOT NULL PRIMARY KEY,
                        LimpezaEtapaExecucaoId CHAR(36) COLLATE ascii_bin NOT NULL,
                        EtapaItemPadraoId CHAR(36) COLLATE ascii_bin NOT NULL,
                        Status VARCHAR(50) NULL,
                        Funcionalidade VARCHAR(50) NULL,
                        RelatoProblema VARCHAR(1000) NULL,
                        FotoUrl VARCHAR(1000) NULL,
                        ConcluidaEm DATETIME(6) NULL,
                        CriadoEm DATETIME(6) NOT NULL,
                        CONSTRAINT FK_etapas_itens_execucao_LimpezaEtapaExecucaoId FOREIGN KEY (LimpezaEtapaExecucaoId) REFERENCES limpeza_etapa_execucoes (Id) ON DELETE CASCADE,
                        CONSTRAINT FK_etapas_itens_execucao_EtapaItemPadraoId FOREIGN KEY (EtapaItemPadraoId) REFERENCES etapas_itens_padrao (Id) ON DELETE RESTRICT
                    );";
                
                using var cmd1 = new MySqlCommand(createItensPadrao, connection);
                cmd1.ExecuteNonQuery();
                
                using var cmd2 = new MySqlCommand(createItensExecucao, connection);
                cmd2.ExecuteNonQuery();
                
                Console.WriteLine("Tabelas de Itens adicionadas!");
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}