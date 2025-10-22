using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

/// <summary>
/// Gerencia a conexão com o banco de dados SQL Server
/// Responsável por fornecer conexões para as operações do sistema
/// </summary>
public class DatabaseConnection
{
    // Configuração da string de conexão com o SQL Server
    // Modificar estes valores de acordo com seu ambiente
    private static readonly string connectionString =
        "Server=localhost,1433;" +
        "Database=LojaDB;" +
        "User Id=sa;" +
        "Password=SqlServer2024!;" +
        "TrustServerCertificate=True;";

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }
}