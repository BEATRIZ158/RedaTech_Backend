using Microsoft.EntityFrameworkCore;
using Redatech.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redatech.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Cpf), IsUnique = true)]
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(14)]
        public string Cpf { get; set; }
        public bool Status { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataNascimento { get; set; }
        public DateTime DataDeCriacao { get; set; } = DateTime.Now.ToLocalTime();
        public TipoUsuario TipoUsuario { get; set; }

        public string SenhaHash { get; set; }
    }
}
