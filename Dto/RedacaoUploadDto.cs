namespace Redatech.Dto
{
    public class RedacaoUploadDto
    {
        public int AlunoId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public IFormFile Arquivo { get; set; }
    }
}
