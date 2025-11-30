namespace MeuProjeto
{
    public class Cliente
    {
        public int CodCliente { get; set; }
        public string Nome { get; set; }   
        public int Idade { get; set; }
        public string Telefone { get; set; } 

        public override string ToString()
        {
            return $"{CodCliente} - {Nome} - Idade: {Idade} - Tel: {Telefone}";
        }
    }
}