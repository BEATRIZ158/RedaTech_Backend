namespace Redatech.Dto
{
    public class CorrecaoDto
    {
        public int Id { get; set; }
        public int RedacaoId { get; set; }
        public int ProfessorId { get; set; }
        public string Comentarios { get; set; }
        public double Nota { get; set; }
        public string Status { get; set; }
    }
}
