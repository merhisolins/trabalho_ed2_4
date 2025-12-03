using System;

namespace MeuProjeto
{
    public class Cliente
    {
        public const int NomeLen = 40;
        public const int TelefoneLen = 20;

        public int CodCliente { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Telefone { get; set; } = string.Empty;

        public static string AjustarNome(string nome)
        {
            nome ??= string.Empty;
            nome = nome.Trim();
            return nome.Length >= NomeLen
                ? nome.Substring(0, NomeLen)
                : nome.PadRight(NomeLen, ' ');
        }

        public static string AjustarTelefone(string telefone)
        {
            telefone ??= string.Empty;
            telefone = telefone.Trim();
            return telefone.Length >= TelefoneLen
                ? telefone.Substring(0, TelefoneLen)
                : telefone.PadRight(TelefoneLen, ' ');
        }

        public override string ToString()
            => $"{CodCliente} - {Nome} - Idade: {Idade} - Tel: {Telefone}";
    }
}