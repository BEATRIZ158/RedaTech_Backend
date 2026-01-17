using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Redatech.Models
{
    public class RedacaoModel
    {
        [Key]
        public int Id { get; set; }
        public string CaminhoArquivo { get; set; } = string.Empty;
        public DateTime DataDeEnvio { get; set; } = DateTime.Now.ToLocalTime();
        public int AlunoId { get; set; }

        [ForeignKey("AlunoId")]
        public UsuarioModel Aluno { get; set; }
        public string Descricao { get; set; }
        public string Titulo { get; set; }
    }
}
