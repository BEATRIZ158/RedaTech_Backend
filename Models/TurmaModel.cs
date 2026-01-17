using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Redatech.Models
{
    public class TurmaModel
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public int ProfessorId { get; set; }

        [ForeignKey("ProfessorId")]
        public UsuarioModel Professor { get; set; }
        public DateTime DataDeCriacao { get; set; } = DateTime.Now.ToLocalTime();
        public bool Status { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Descricao { get; set; }

        public List<TurmasAlunosModel> TurmasAlunos { get; set; } = new();
    }
}
