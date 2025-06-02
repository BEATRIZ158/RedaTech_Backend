namespace Redatech.Dto
{
    public class AuthResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataExpiracao {get; set; }
    }
}
