using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

/// <summary>
/// Repositório responsável pelo gerenciamento de produtos no sistema
/// Implementa operações CRUD e consultas especializadas
/// </summary>
public class ProdutoRepository
{
    /// <summary>
    /// Recupera e exibe todos os produtos cadastrados no sistema
    /// </summary>
    public void ListarTodosProdutos()
    {
        string sql = "SELECT * FROM Produtos";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Id\tNome\tPreco\tEstoque");
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string nome = reader["Nome"] as string ?? string.Empty;
                        decimal preco = Convert.ToDecimal(reader["Preco"]);
                        int estoque = Convert.ToInt32(reader["Estoque"]);
                        Console.WriteLine($"{id}\t{nome}\t{preco}\t{estoque}");
                    }
                }
            }
        }
    }

    // EXERCÍCIO 2: Inserir novo produto
    public void InserirProduto(Produto produto)
    {
        string sql = "INSERT INTO Produtos (Nome, Preco, Estoque, CategoriaId) " +
                     "VALUES (@Nome, @Preco, @Estoque, @CategoriaId)";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);
                cmd.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);

                int linhasAfetadas = cmd.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                    Console.WriteLine("Produto inserido com sucesso!");
                else
                    Console.WriteLine("Falha ao inserir produto.");
            }
        }
    }

    // EXERCÍCIO 3: Atualizar produto
    public void AtualizarProduto(Produto produto)
    {
        string sql = "UPDATE Produtos SET " +
                     "Nome = @Nome, " +
                     "Preco = @Preco, " +
                     "Estoque = @Estoque " +
                     "WHERE Id = @Id";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);
                cmd.Parameters.AddWithValue("@Id", produto.Id);

                int linhasAfetadas = cmd.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                    Console.WriteLine("Produto atualizado com sucesso!");
                else
                    Console.WriteLine("Produto não encontrado ou não atualizado.");
            }
        }
    }

    // EXERCÍCIO 4: Deletar produto
    public void DeletarProduto(int id)
    {
        // Verifica se há pedidos vinculados ao produto
        string sqlVerifica = "SELECT COUNT(*) FROM PedidoItens WHERE ProdutoId = @Id";
        string sqlDelete = "DELETE FROM Produtos WHERE Id = @Id";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmdVerifica = new SqlCommand(sqlVerifica, conn))
            {
                cmdVerifica.Parameters.AddWithValue("@Id", id);
                int pedidosVinculados = Convert.ToInt32(cmdVerifica.ExecuteScalar());
                if (pedidosVinculados > 0)
                {
                    Console.WriteLine("Não é possível excluir: existem pedidos vinculados a este produto.");
                    return;
                }
            }

            using (SqlCommand cmdDelete = new SqlCommand(sqlDelete, conn))
            {
                cmdDelete.Parameters.AddWithValue("@Id", id);
                int linhasAfetadas = cmdDelete.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                    Console.WriteLine("Produto excluído com sucesso!");
                else
                    Console.WriteLine("Produto não encontrado ou já excluído.");
            }
        }
    }

    // EXERCÍCIO 5: Buscar produto por ID
    public Produto? BuscarPorId(int id)
    {
        string sql = "SELECT * FROM Produtos WHERE Id = @Id";
        Produto? produto = null;

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        produto = new Produto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nome = reader["Nome"] as string ?? string.Empty,
                            Preco = Convert.ToDecimal(reader["Preco"]),
                            Estoque = Convert.ToInt32(reader["Estoque"]),
                            CategoriaId = Convert.ToInt32(reader["CategoriaId"])
                        };
                    }
                }
            }
        }
        return produto;
    }

    // EXERCÍCIO 6: Listar produtos por categoria
    public void ListarProdutosPorCategoria(int categoriaId)
    {
        string sql = @"SELECT p.*, c.Nome as NomeCategoria 
                          FROM Produtos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id
                          WHERE p.CategoriaId = @CategoriaId";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Id\tNome\tPreco\tEstoque\tCategoria");
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string nome = reader["Nome"] as string ?? string.Empty;
                        decimal preco = Convert.ToDecimal(reader["Preco"]);
                        int estoque = Convert.ToInt32(reader["Estoque"]);
                        string nomeCategoria = reader["NomeCategoria"] as string ?? string.Empty;
                        Console.WriteLine($"{id}\t{nome}\t{preco}\t{estoque}\t{nomeCategoria}");
                    }
                }
            }
        }
    }

    // DESAFIO 1: Buscar produtos com estoque baixo
    public void ListarProdutosEstoqueBaixo(int quantidadeMinima)
    {
        string sql = "SELECT * FROM Produtos WHERE Estoque < @QuantidadeMinima";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@QuantidadeMinima", quantidadeMinima);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Id\tNome\tPreco\tEstoque");
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string nome = reader["Nome"] as string ?? string.Empty;
                        decimal preco = Convert.ToDecimal(reader["Preco"]);
                        int estoque = Convert.ToInt32(reader["Estoque"]);
                        // Alerta visual: destaque o produto
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{id}\t{nome}\t{preco}\t{estoque} <-- ESTOQUE BAIXO!");
                        Console.ResetColor();
                    }
                }
            }
        }
    }

    // DESAFIO 2: Buscar produtos por nome (LIKE)
    public void BuscarProdutosPorNome(string termoBusca)
    {
        string sql = "SELECT * FROM Produtos WHERE Nome LIKE @TermoBusca";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TermoBusca", "%" + termoBusca + "%");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Id\tNome\tPreco\tEstoque");
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string nome = reader["Nome"] as string ?? string.Empty;
                        decimal preco = Convert.ToDecimal(reader["Preco"]);
                        int estoque = Convert.ToInt32(reader["Estoque"]);
                        Console.WriteLine($"{id}\t{nome}\t{preco}\t{estoque}");
                    }
                }
            }
        }
    }
}