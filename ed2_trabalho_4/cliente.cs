namespace MeuProjeto
{
    public class Cliente
    {
        public int CodCliente { get; set; }
        public string Nome { get; set; }   
        public int Idade { get; set; }
        public string Telefone { get; set; } 

        public static string AjustarNome(string nome)
        {
            if (string.IsNullOrEmpty(nome))
                nome = "";

            if (nome.Length > 40)
                return nome.Substring(0, 40);

            return nome.PadRight(40, ' ');
        }

        public static string AjustarTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
                telefone = "";

            if (telefone.Length > 20)
                return telefone.Substring(0, 20);

            return telefone.PadRight(20, ' ');
        }

        public override string ToString()
        {
            return $"{CodCliente} - {Nome} - Idade: {Idade} - Tel: {Telefone}";
        }
    }
}