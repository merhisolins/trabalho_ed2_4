using System;
using System.Collections.Generic;
using System.IO;
using MeuProjeto;

class Program
{
    static BPlusTree<int, Cliente> arvore = new BPlusTree<int, Cliente>(2);

    static void Main(string[] args)
    {
        bool sair = false;

        while (!sair)
        {
            Console.WriteLine("\n======== MENU ÁRVORE B+ ========");
            Console.WriteLine("1 - Inserir cliente manualmente");
            Console.WriteLine("2 - Inserir cliente aleatório");
            Console.WriteLine("3 - Buscar cliente");
            Console.WriteLine("4 - Remover cliente");
            Console.WriteLine("5 - Listar clientes");
            Console.WriteLine("6 - Executar teste automático");
            Console.WriteLine("7 - Salvar arquivos (metadados/indice/dados)");
            Console.WriteLine("8 - Sair");
            Console.Write("Escolha: ");

            string op = Console.ReadLine();
            Console.WriteLine();

            switch (op)
            {
                case "1": InserirClienteManual(); break;
                case "2": InserirClienteAleatorio(); break;
                case "3": BuscarCliente(); break;
                case "4": RemoverCliente(); break;
                case "5": ListarClientes(); break;
                case "6": TesteAutomatico(); break;
                case "7": SalvarArquivos(arvore, "metadados.txt", "indice.txt", "dados.txt"); break;
                case "8": sair = true; break;
                default: Console.WriteLine("Opção inválida."); break;
            }
        }
    }
    static void InserirClienteManual()
    {
        Console.Write("Código: ");
        if (!int.TryParse(Console.ReadLine(), out int cod))
        {
            Console.WriteLine("Código inválido.");
            return;
        }
    
        string nome;
        while (true)
        {
            Console.Write("Nome: ");
            nome = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Nome inválido! Digite novamente.");
                continue; 
            }

            break; 
        }
    

        Console.Write("Idade: ");
        int idade = int.Parse(Console.ReadLine());

        Console.Write("Telefone: ");
        string telefone = Console.ReadLine();

        nome = Cliente.AjustarNome(nome);
        telefone = Cliente.AjustarTelefone(telefone);

        var cli = new Cliente
        {
            CodCliente = cod,
            Nome = nome,
            Idade = idade,
            Telefone = telefone
        };

        arvore.Insert(cod, cli);
        Console.WriteLine("Cliente inserido.");
    }

    static void InserirClienteAleatorio()
    {
        Cliente cli = GeradorClientes.GerarCliente();
        arvore.Insert(cli.CodCliente, cli);
        Console.WriteLine($"Inserido automaticamente: {cli}");
    }
    static void BuscarCliente()
    {
        Console.Write("Código para buscar: ");
        if (!int.TryParse(Console.ReadLine(), out int cod))
        {
            Console.WriteLine("Código inválido.");
            return;
        }

        if (arvore.TryGetValue(cod, out Cliente encontrado))
            Console.WriteLine($"Encontrado: {encontrado}");
        else
            Console.WriteLine("Cliente não encontrado.");
    }


    static void RemoverCliente()
    {
        Console.Write("Código para remover: ");
        if (!int.TryParse(Console.ReadLine(), out int cod))
        {
            Console.WriteLine("Código inválido.");
            return;
        }

        arvore.Remove(cod);
        Console.WriteLine("Cliente removido (se existia).");
    }

    static void ListarClientes()
    {
        bool vazio = true;

        arvore.ForEachInOrder((cod, cli) =>
        {
            vazio = false;
            Console.WriteLine(cli);
        });

        if (vazio)
            Console.WriteLine("Árvore vazia.");
    }

    static void TesteAutomatico()
    {
        arvore = new BPlusTree<int, Cliente>(2);
        var codigosInseridos = new List<int>();

        Console.WriteLine("Inserindo 10 clientes aleatórios...\n");

        for (int i = 0; i < 10; i++)
        {
            Cliente cli = GeradorClientes.GerarCliente();
            arvore.Insert(cli.CodCliente, cli);
            codigosInseridos.Add(cli.CodCliente);

            Console.WriteLine($"Inserido: {cli}");
        }

        Console.WriteLine("\n--- Teste de Busca ---");
        int codigoBusca = codigosInseridos[3];

        if (arvore.TryGetValue(codigoBusca, out Cliente encontrado))
            Console.WriteLine($"Busca por {codigoBusca}: ENCONTRADO → {encontrado}");
        else
            Console.WriteLine($"Busca por {codigoBusca}: NÃO encontrado");

        Console.WriteLine("\n--- Teste de Exclusão ---");
        int codigoRemocao = codigosInseridos[5];
        arvore.Remove(codigoRemocao);

        if (arvore.TryGetValue(codigoRemocao, out _))
            Console.WriteLine($"ERRO: {codigoRemocao} ainda está na árvore!");
        else
            Console.WriteLine($"Ok: {codigoRemocao} removido com sucesso.");

        Console.WriteLine("\nFim do teste automático.");
    }
    static void SalvarArquivos(BPlusTree<int, Cliente> arvore, string arqMetadados, string arqIndice, string arqDados)
    {
        var listaClientes = new List<Cliente>();
        int quantidadeRegistros = 0;

        arvore.ForEachInOrder((cod, cli) =>
        {
            quantidadeRegistros++;
            listaClientes.Add(cli);
        });

        using (var meta = new StreamWriter(arqMetadados))
        {
            meta.WriteLine($"Ordem={arvore.Ordem}");
            meta.WriteLine($"QuantidadeRegistros={quantidadeRegistros}");
            meta.WriteLine($"ArquivoIndice={arqIndice}");
            meta.WriteLine($"ArquivoDados={arqDados}");
        }

        using (var idx = new StreamWriter(arqIndice))
        {
            foreach (var cli in listaClientes)
                idx.WriteLine(cli.CodCliente);
        }

        using (var dados = new StreamWriter(arqDados))
        {
            foreach (var cli in listaClientes)
                dados.WriteLine($"{cli.CodCliente};{cli.Nome};{cli.Idade};{cli.Telefone}");
        }

        Console.WriteLine("Arquivos salvos com sucesso.");
    }
}