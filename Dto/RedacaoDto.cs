namespace Redatech.Dto
{
    public class RedacaoDto
    {
        public int Id { get; set; }
        public string? CaminhoArquivo { get; set; }
        public string NomeArquivo { get; set; } = string.Empty; // ← novo campo
        public int AlunoId { get; set; }
        public string Descricao { get; set; }
        public string Titulo { get; set; }
    }
}
