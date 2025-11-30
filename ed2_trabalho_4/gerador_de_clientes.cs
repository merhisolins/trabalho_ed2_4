using System;
using System.Text;

namespace MeuProjeto
{
    public static class GeradorClientes
    {
        private static Random rnd = new Random();

        public static Cliente GerarCliente()
        {
            int cod = rnd.Next(1, 99999);

            string nome = GerarString(40);
            int idade = rnd.Next(18, 90);
            string telefone = GerarTelefone();

            return new Cliente
            {
                CodCliente = cod,
                Nome = nome,
                Idade = idade,
                Telefone = telefone
            };
        }

        private static string GerarString(int max)
        {
            const string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int tamanho = rnd.Next(5, max);  

            var sb = new StringBuilder();
            for (int i = 0; i < tamanho; i++)
                sb.Append(letras[rnd.Next(letras.Length)]);

            return sb.ToString();
        }

        private static readonly int[] DDDsValidos = new int[]
        {
            11,12,13,14,15,16,17,18,19,
            21,22,24,27,28,
            31,32,33,34,35,37,38,
            41,42,43,44,45,46,47,48,49,
            51,53,54,55,
            61,62,63,64,65,66,67,68,69,
            71,73,74,75,77,79,
            81,82,83,84,85,86,87,88,89,
            91,92,93,94,95,96,97,98,99
        };

        private static string GerarTelefone()
        {
            int ddd = DDDsValidos[rnd.Next(DDDsValidos.Length)];
            int parte1 = rnd.Next(10000, 99999);
            int parte2 = rnd.Next(1000, 9999);

            string fone = $"({ddd}) 9{parte1}-{parte2}";
            return fone.PadRight(20, ' '); 
        }
    }
}
