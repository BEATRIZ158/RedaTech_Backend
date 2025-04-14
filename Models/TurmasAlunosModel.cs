using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Redatech.Models
{
    public class TurmasAlunosModel
    {
        [Key]
        public int Id { get; set; }

        public int TurmaId { get; set; }

        [ForeignKey("TurmaId")]
        public TurmaModel Turma { get; set; }

        public int AlunoId { get; set; }

        [ForeignKey("AlunoId")]
        public UsuarioModel Aluno { get; set; }

        public DateTime DataAcao { get; set; } = DateTime.Now.ToLocalTime();
    }
}
