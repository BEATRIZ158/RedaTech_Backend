using Redatech.Enums;

namespace Redatech.Dto
{
    public class UsuarioLogadoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
    }
}
