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

        private static string GerarTelefone()
        {
            return $"+55 ({rnd.Next(11, 99)}) 9{rnd.Next(1000, 9999)}-{rnd.Next(1000, 9999)}";
        }
    }
}
