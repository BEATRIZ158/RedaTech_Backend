using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Redatech.Enums;

namespace Redatech.Models
{
    public class CorrecaoModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Redacao")]
        public int RedacaoId { get; set; }
        public RedacaoModel Redacao { get; set; }

        [ForeignKey("Professor")]
        public int ProfessorId { get; set; }
        public UsuarioModel Professor { get; set; }

        public string Comentarios { get; set; }
        
        [Range(0, 1000, ErrorMessage = "A nota deve estar entre 0 e 1000.")]
        public decimal Nota { get; set; }

        public StatusCorrecao Status { get; set; } = StatusCorrecao.EmAndamento;
        public DateTime DataDeCorrecao { get; set; } = DateTime.Now;
    }
}
