namespace Redatech.Dto
{
    public class AlunoNaTurmaDto
    {
        public int Id { get; set; } // ID do usuário
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataVinculo { get; set; } // Data que entrou na turma
    }
}
