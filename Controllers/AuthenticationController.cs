using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redatech.Dto;
using Redatech.Service.AuthenticationService;

namespace Redatech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationInterface _authService;

        public AuthenticationController(IAuthenticationInterface authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Faz login do usuário com email e senha.
        /// </summary>
        /// <param name="loginDto">Dados de login do usuário.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.Login(loginDto);

            if (!response.Sucesso)
            {
                return BadRequest(new { message = response.Mensagem });
            }

            return Ok(response);
        }

        /// <summary>
        /// Gerar novo token depois de expirar o token atual
        /// </summary>
        /// <param name="refreshToken">Dados de login do usuário.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> GerarNovoTokenDepoisDeExpirar([FromBody] RefreshTokenDto refreshToken)
        {
            var response = await _authService.GerarNovoTokenDepoisDeExpirar(refreshToken);
            if (!response.Sucesso)
            {
                return BadRequest(new { message = response.Mensagem });
            }
            return Ok(response);
        }
    }
}
