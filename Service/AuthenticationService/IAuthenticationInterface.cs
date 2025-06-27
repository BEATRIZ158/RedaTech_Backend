using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.AuthenticationService
{
    public interface IAuthenticationInterface
    {
        Task<ServiceResponse<AuthResponseDto>> Login(LoginDto loginDto);//Retorna informações do usuário para o Front-End
        Task<ServiceResponse<AuthResponseDto>> GerarNovoTokenDepoisDeExpirar(RefreshTokenDto refreshToken);
    }
}
