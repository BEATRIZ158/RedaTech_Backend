using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Redatech.Models
{
    public class TurmaModel
    {
        [Key]
        public int Id { get; set; } //Id
        public string Nome { get; set; } //Nome
        public int ProfessorId { get; set; }

        // Propriedade de navegação para UsuarioModel
        [ForeignKey("ProfessorId")]
        public UsuarioModel Professor { get; set; }
        public DateTime DataDeCriacao { get; set; } = DateTime.Now.ToLocalTime(); //Data da criação
        public bool Status { get; set; } //Ativa/Inativo

        [Column(TypeName = "nvarchar(max)")]
        public string Descricao { get; set; }

        public List<TurmasAlunosModel> TurmasAlunos { get; set; } = new();
    }
}
