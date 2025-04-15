using System.Security.Cryptography;
using System.Text;

namespace Redatech.Estaticos.Login
{
    public static class CriptografiaHash
    {
        public static string GerarHash(string senha)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(senha);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public static bool VerificarSenha(string senhaDigitada, string senhaHashArmazenada)
        {
            var hashDigitada = GerarHash(senhaDigitada);
            return hashDigitada == senhaHashArmazenada;
        }
    }
}
