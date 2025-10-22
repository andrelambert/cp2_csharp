using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

/// <summary>
/// Repositório responsável por gerenciar todas as operações relacionadas a pedidos
/// Implementa transações para garantir a integridade dos dados
/// </summary>
public class PedidoRepository
{
    /// <summary>
    /// Cria um novo pedido com seus itens em uma única transação
    /// Atualiza o estoque automaticamente
    /// </summary>
    public void CriarPedido(Pedido pedido, List<PedidoItem> itens)
    {
        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();
            try
            {
                // 1. Inserir pedido e obter ID
                string sqlPedido = "INSERT INTO Pedidos (ClienteId, DataPedido, ValorTotal) OUTPUT INSERTED.Id VALUES (@ClienteId, @DataPedido, @ValorTotal)";
                int pedidoId = 0;
                using (SqlCommand cmd = new SqlCommand(sqlPedido, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                    cmd.Parameters.AddWithValue("@DataPedido", pedido.DataPedido);
                    cmd.Parameters.AddWithValue("@ValorTotal", pedido.ValorTotal);
                    pedidoId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 2. Inserir itens do pedido
                string sqlItem = "INSERT INTO PedidoItens (PedidoId, ProdutoId, Quantidade, PrecoUnitario) VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario)";
                string sqlEstoque = "UPDATE Produtos SET Estoque = Estoque - @Quantidade WHERE Id = @ProdutoId";
                foreach (var item in itens)
                {
                    using (SqlCommand cmdItem = new SqlCommand(sqlItem, conn, transaction))
                    {
                        cmdItem.Parameters.AddWithValue("@PedidoId", pedidoId);
                        cmdItem.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                        cmdItem.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmdItem.Parameters.AddWithValue("@PrecoUnitario", item.PrecoUnitario);
                        cmdItem.ExecuteNonQuery();
                    }
                    // 3. Atualizar estoque dos produtos
                    using (SqlCommand cmdEstoque = new SqlCommand(sqlEstoque, conn, transaction))
                    {
                        cmdEstoque.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmdEstoque.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                        cmdEstoque.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                Console.WriteLine("Pedido criado com sucesso!");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Erro ao criar pedido: {ex.Message}");
                throw;
            }
        }
    }

    // EXERCÍCIO 8: Listar pedidos de um cliente
    public void ListarPedidosCliente(int clienteId)
    {
        string sql = "SELECT * FROM Pedidos WHERE ClienteId = @ClienteId ORDER BY DataPedido DESC";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ClienteId", clienteId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Id\tData\tValorTotal");
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        DateTime data = Convert.ToDateTime(reader["DataPedido"]);
                        decimal valorTotal = Convert.ToDecimal(reader["ValorTotal"]);
                        Console.WriteLine($"{id}\t{data:dd/MM/yyyy}\t{valorTotal}");
                    }
                }
            }
        }
    }

    // EXERCÍCIO 9: Obter detalhes completos de um pedido
    public void ObterDetalhesPedido(int pedidoId)
    {
        string sql = @"SELECT 
                            pi.*, 
                            p.Nome as NomeProduto,
                            (pi.Quantidade * pi.PrecoUnitario) as Subtotal
                          FROM PedidoItens pi
                          INNER JOIN Produtos p ON pi.ProdutoId = p.Id
                          WHERE pi.PedidoId = @PedidoId";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Produto\tQtd\tUnitário\tSubtotal");
                    while (reader.Read())
                    {
                        string nomeProduto = reader["NomeProduto"] as string ?? string.Empty;
                        int quantidade = Convert.ToInt32(reader["Quantidade"]);
                        decimal precoUnitario = Convert.ToDecimal(reader["PrecoUnitario"]);
                        decimal subtotal = Convert.ToDecimal(reader["Subtotal"]);
                        Console.WriteLine($"{nomeProduto}\t{quantidade}\t{precoUnitario}\t{subtotal}");
                    }
                }
            }
        }
    }

    // DESAFIO 3: Calcular total de vendas por período
    public void TotalVendasPorPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        string sql = "SELECT SUM(ValorTotal) FROM Pedidos WHERE DataPedido BETWEEN @DataInicio AND @DataFim";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);
                object resultado = cmd.ExecuteScalar();
                decimal total = resultado != DBNull.Value ? Convert.ToDecimal(resultado) : 0;
                Console.WriteLine($"Total de vendas de {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}: {total}");
            }
        }
    }
}