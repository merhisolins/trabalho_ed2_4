using System;
using System.Collections.Generic;
using System.IO;
using MeuProjeto;  

class Program
{
    static void Main(string[] args)
    {
        var arvore = new BPlusTree<int, Cliente>(2);

        var codigosInseridos = new List<int>();


        Console.WriteLine("Inserindo clientes aleatórios...\n");

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
        {
            Console.WriteLine($"Busca por {codigoBusca}: ENCONTRADO -> {encontrado}");
        }
        else
        {
            Console.WriteLine($"Busca por {codigoBusca}: NÃO encontrado");
        }

        Console.WriteLine("\n--- Teste de Exclusão ---");
        int codigoRemocao = codigosInseridos[5];

        Console.WriteLine($"Removendo cliente com código {codigoRemocao}...");
        arvore.Remove(codigoRemocao);

        if (arvore.TryGetValue(codigoRemocao, out _))
        {
            Console.WriteLine($"ERRO: cliente {codigoRemocao} ainda está na árvore!");
        }
        else
        {
            Console.WriteLine($"Ok: cliente {codigoRemocao} foi removido.");
        }

        SalvarArquivos(arvore, "metadados.txt", "indice.txt", "dados.txt");

        Console.WriteLine("\nArquivos gerados:");
        Console.WriteLine(" - metadados.txt");
        Console.WriteLine(" - indice.txt");
        Console.WriteLine(" - dados.txt");
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
            {
                idx.WriteLine(cli.CodCliente);
            }
        }

        
        using (var dados = new StreamWriter(arqDados))
        {
            foreach (var cli in listaClientes)
            {
                dados.WriteLine($"{cli.CodCliente};{cli.Nome};{cli.Idade};{cli.Telefone}");
            }
        }
    }
}
