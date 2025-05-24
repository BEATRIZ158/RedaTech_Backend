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
        public int Id { get; set; } //Id

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; } //Nome

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } //email

        [MaxLength(14)]
        public string Cpf { get; set; } //CPF
        public bool Status { get; set; } //Ativa/Inativo

        [Column(TypeName = "date")]
        public DateTime DataNascimento { get; set; }
        public DateTime DataDeCriacao { get; set; } = DateTime.Now.ToLocalTime(); //Data da criação
        public TipoUsuario TipoUsuario { get; set; }

        //Senha criptografada
        public string SenhaHash { get; set; }
    }
}
