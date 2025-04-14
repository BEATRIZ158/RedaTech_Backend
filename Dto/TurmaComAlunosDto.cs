namespace Redatech.Dto
{
    public class TurmaComAlunosDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public List<AlunoNaTurmaDto> Alunos { get; set; }
    }
}
