using System.ComponentModel.DataAnnotations;

namespace Redatech.Models
{
    public class RefreshTokenModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string Token { get; set; }
        
        public DateTime DataCriacao { get; set; }
        public DateTime DataExpiracao { get; set; }
        public DateTime? DataRevogado { get; set; }
        public string SubstituidoPor { get; set; }
        public bool Ativo => DataRevogado == null && !Expirado;
        public bool Expirado => DateTime.UtcNow >= DataExpiracao;
    }
}
