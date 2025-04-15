using Redatech.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Redatech.Models
{
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; } //Id
        public string Nome { get; set; } //Nome
        public string Email { get; set; } //email
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
