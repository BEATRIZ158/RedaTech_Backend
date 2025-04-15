using Redatech.Enums;

namespace Redatech.Dto
{
    public class UsuarioDto
    {
        //Aqui ficam apenas os dados que o usuário precisa preencher
        public int Id { get; set; }
        public string Nome { get; set; } //Nome
        public string Email { get; set; } //email
        public string Cpf { get; set; } //CPF
        public DateTime DataNascimento { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public string SenhaHash { get; set; }
        public bool Status { get; set; }
    }
}
