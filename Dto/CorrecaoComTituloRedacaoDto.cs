namespace Redatech.Dto
{
    public class CorrecaoComTituloRedacaoDto
    {
        public int Id { get; set; }
        public int RedacaoId { get; set; }
        public string TituloRedacao { get; set; }
        public int ProfessorId { get; set; }
        public string Comentarios { get; set; }
        public double Nota { get; set; }
        public string Status { get; set; }
    }
}
