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
                using var cmd = new MySqlCommand("ALTER TABLE etapas_padrao ADD COLUMN LinkVideo LONGTEXT NULL;", connection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Coluna adicionada!");
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}