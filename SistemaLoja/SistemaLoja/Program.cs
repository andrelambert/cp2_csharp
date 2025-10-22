
using System;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using SistemaLoja.Lab12_ConexaoSQLServer;

namespace SistemaLoja
{
    /// <summary>
    /// Sistema de Gerenciamento de Loja
    /// Versão: 2.0
    /// Data: Outubro 2025
    /// 
    /// Este sistema implementa funcionalidades essenciais para
    /// gerenciamento de produtos, pedidos e clientes em uma loja.
    /// </summary>
    
    // ============================================
    // Sistema desenvolvido como parte do projeto
    // da disciplina de Desenvolvimento .NET
    // ============================================
    
    class Program
    {
        static void Main(string[] args)
        {
            // IMPORTANTE: Antes de executar, crie o banco de dados!
            // Execute o script SQL fornecido no arquivo setup.sql
            
            Console.WriteLine("=== LAB 12 - CONEXÃO SQL SERVER ===\n");
            
            var produtoRepo = new ProdutoRepository();
            var pedidoRepo = new PedidoRepository();
            
            bool continuar = true;
            
            while (continuar)
            {
                MostrarMenu();
                string opcao = Console.ReadLine();
                
                try
                {
                    switch (opcao)
                    {
                        case "1":
                            produtoRepo.ListarTodosProdutos();
                            break;
                            
                        case "2":
                            InserirNovoProduto(produtoRepo);
                            break;
                            
                        case "3":
                            AtualizarProdutoExistente(produtoRepo);
                            break;
                            
                        case "4":
                            DeletarProdutoExistente(produtoRepo);
                            break;
                            
                        case "5":
                            ListarPorCategoria(produtoRepo);
                            break;
                            
                        case "6":
                            CriarNovoPedido(pedidoRepo);
                            break;
                            
                        case "7":
                            ListarPedidosDeCliente(pedidoRepo);
                            break;
                            
                        case "8":
                            DetalhesDoPedido(pedidoRepo);
                            break;
                            
                        case "0":
                            continuar = false;
                            break;
                            
                        default:
                            Console.WriteLine("Opção inválida!");
                            break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"\n❌ Erro SQL: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Erro: {ex.Message}");
                }
                
                if (continuar)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            
            Console.WriteLine("\nPrograma finalizado!");
        }

        static void MostrarMenu()
        {
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║       MENU PRINCIPAL               ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║  PRODUTOS                          ║");
            Console.WriteLine("║  1 - Listar todos os produtos      ║");
            Console.WriteLine("║  2 - Inserir novo produto          ║");
            Console.WriteLine("║  3 - Atualizar produto             ║");
            Console.WriteLine("║  4 - Deletar produto               ║");
            Console.WriteLine("║  5 - Listar por categoria          ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  PEDIDOS                           ║");
            Console.WriteLine("║  6 - Criar novo pedido             ║");
            Console.WriteLine("║  7 - Listar pedidos de cliente     ║");
            Console.WriteLine("║  8 - Detalhes de um pedido         ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  0 - Sair                          ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.Write("\nEscolha uma opção: ");
        }

        // TODO: Implemente os métodos auxiliares abaixo
        
        static void InserirNovoProduto(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== INSERIR NOVO PRODUTO ===");
            
            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("Preço: ");
            decimal preco;
            while (!decimal.TryParse(Console.ReadLine(), out preco))
            {
                Console.Write("Valor inválido! Digite o preço: ");
            }

            Console.Write("Estoque: ");
            int estoque;
            while (!int.TryParse(Console.ReadLine(), out estoque))
            {
                Console.Write("Valor inválido! Digite o estoque: ");
            }

            Console.Write("CategoriaId: ");
            int categoriaId;
            while (!int.TryParse(Console.ReadLine(), out categoriaId))
            {
                Console.Write("Valor inválido! Digite o CategoriaId: ");
            }

            var produto = new Produto
            {
                Nome = nome,
                Preco = preco,
                Estoque = estoque,
                CategoriaId = categoriaId
            };

            repo.InserirProduto(produto);
        }

        static void AtualizarProdutoExistente(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== ATUALIZAR PRODUTO ===");

            Console.Write("ID do produto: ");
            string idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            var produto = repo.BuscarPorId(id);
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado!");
                return;
            }

            Console.WriteLine($"Nome atual: {produto.Nome}");
            Console.Write("Novo nome (Enter para manter): ");
            string nomeNovo = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nomeNovo))
                produto.Nome = nomeNovo;

            Console.WriteLine($"Preço atual: {produto.Preco}");
            Console.Write("Novo preço (Enter para manter): ");
            string precoNovo = Console.ReadLine();
            if (decimal.TryParse(precoNovo, out decimal precoVal))
                produto.Preco = precoVal;

            Console.WriteLine($"Estoque atual: {produto.Estoque}");
            Console.Write("Novo estoque (Enter para manter): ");
            string estoqueNovo = Console.ReadLine();
            if (int.TryParse(estoqueNovo, out int estoqueVal))
                produto.Estoque = estoqueVal;

            Console.WriteLine($"CategoriaId atual: {produto.CategoriaId}");
            Console.Write("Nova categoria (Enter para manter): ");
            string categoriaNovo = Console.ReadLine();
            if (int.TryParse(categoriaNovo, out int categoriaVal))
                produto.CategoriaId = categoriaVal;

            repo.AtualizarProduto(produto);
            Console.WriteLine("Produto atualizado!");
        }

        static void DeletarProdutoExistente(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== DELETAR PRODUTO ===");
            Console.Write("ID do produto: ");
            string idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out int id))
            {
                Console.WriteLine("ID inválido!");
                return;
            }

            var produto = repo.BuscarPorId(id);
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado!");
                return;
            }

            Console.WriteLine($"Produto encontrado: {produto.Nome} (ID: {produto.Id})");
            Console.Write("Tem certeza que deseja excluir? (s/n): ");
            string confirmacao = Console.ReadLine();
            if (confirmacao == null || !confirmacao.Trim().ToLower().StartsWith("s"))
            {
                Console.WriteLine("Exclusão cancelada.");
                return;
            }

            repo.DeletarProduto(id);
        }

        static void ListarPorCategoria(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== PRODUTOS POR CATEGORIA ===");
            Console.Write("Digite o ID da categoria: ");
            string categoriaStr = Console.ReadLine();
            if (!int.TryParse(categoriaStr, out int categoriaId))
            {
                Console.WriteLine("ID de categoria inválido!");
                return;
            }
            repo.ListarProdutosPorCategoria(categoriaId);
        }

        static void CriarNovoPedido(PedidoRepository repo)
        {
            Console.WriteLine("\n=== CRIAR NOVO PEDIDO ===");
            Console.Write("ID do cliente: ");
            string clienteStr = Console.ReadLine();
            if (!int.TryParse(clienteStr, out int clienteId))
            {
                Console.WriteLine("ID de cliente inválido!");
                return;
            }

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                DataPedido = DateTime.Now
            };

            var itens = new List<PedidoItem>();
            decimal valorTotal = 0;
            while (true)
            {
                Console.Write("ID do produto (Enter para finalizar): ");
                string prodStr = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(prodStr)) break;
                if (!int.TryParse(prodStr, out int produtoId))
                {
                    Console.WriteLine("ID de produto inválido!");
                    continue;
                }

                Console.Write("Quantidade: ");
                string qtdStr = Console.ReadLine();
                if (!int.TryParse(qtdStr, out int quantidade) || quantidade <= 0)
                {
                    Console.WriteLine("Quantidade inválida!");
                    continue;
                }

                Console.Write("Preço unitário: ");
                string precoStr = Console.ReadLine();
                if (!decimal.TryParse(precoStr, out decimal precoUnitario) || precoUnitario <= 0)
                {
                    Console.WriteLine("Preço inválido!");
                    continue;
                }

                itens.Add(new PedidoItem
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    PrecoUnitario = precoUnitario
                });
                valorTotal += quantidade * precoUnitario;
            }

            if (itens.Count == 0)
            {
                Console.WriteLine("Nenhum item informado. Pedido cancelado.");
                return;
            }

            pedido.ValorTotal = valorTotal;
            repo.CriarPedido(pedido, itens);
        }

        static void ListarPedidosDeCliente(PedidoRepository repo)
        {
            Console.WriteLine("\n=== PEDIDOS DO CLIENTE ===");
            Console.Write("Digite o ID do cliente: ");
            string clienteStr = Console.ReadLine();
            if (!int.TryParse(clienteStr, out int clienteId))
            {
                Console.WriteLine("ID de cliente inválido!");
                return;
            }
            repo.ListarPedidosCliente(clienteId);
        }

        static void DetalhesDoPedido(PedidoRepository repo)
        {
            Console.WriteLine("\n=== DETALHES DO PEDIDO ===");
            Console.Write("Digite o ID do pedido: ");
            string pedidoStr = Console.ReadLine();
            if (!int.TryParse(pedidoStr, out int pedidoId))
            {
                Console.WriteLine("ID de pedido inválido!");
                return;
            }
            repo.ObterDetalhesPedido(pedidoId);
        }
    }
}