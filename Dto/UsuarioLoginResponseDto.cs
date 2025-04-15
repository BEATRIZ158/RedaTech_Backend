namespace Redatech.Dto
{
    public class UsuarioLoginResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string TipoUsuario { get; set; } // "Aluno", "Professor", etc.
    }
}
