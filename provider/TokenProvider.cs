using Microsoft.IdentityModel.Tokens;
using Redatech.Dto;
using Redatech.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Redatech.provider
{
    public class TokenProvider
    {
        private readonly IConfiguration _configuration;

        public TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(UsuarioLogadoDto usuario)
        {
            //Array de objetos Claim que serão adicionados ao token JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
            };

            //Gerando a assinatura do token JWT
            // Convertendo a string para bytes, chave de segurança do JWT
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //o token será assinado com a chave gerada e usando o algoritmo HMAC SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Definindo a data de expiração do token, nesse caso 2 horas
            var expiration = DateTime.UtcNow.AddHours(2);

            //Montagem do token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            //converte o token, que até então era um objeto, em uma string no formato JWT
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal DecodificaToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            return principal;
        }

        public RefreshTokenModel GerarRefreshToken(string ipAddress)
        {
            var refreshToken = new RefreshTokenModel
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                DataCriacao = DateTime.UtcNow,
                DataExpiracao = DateTime.UtcNow.AddDays(7),
                IpCriacao = ipAddress
            };

            return refreshToken;
        }
    }
}
