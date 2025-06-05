using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Estaticos.Login;
using Redatech.Models;
using Redatech.provider;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Redatech.Service.AuthenticationService
{
    public class AuthService : IAuthenticationInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenProvider _tokenProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext context, TokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public Task<ServiceResponse<AuthResponseDto>> GerarNovoTokenDepoisDeExpirar(RefreshTokenDto refreshToken)
        {
            var serviceResponse = new ServiceResponse<AuthResponseDto>();

            //Gerando novo Token
            var dadosUsuario = new AuthResponseDto
            {
                Nome = usuario.Nome,
                Role = usuario.TipoUsuario.ToString(),
                Token = _tokenProvider.GerarToken(usuarioLogado),
                RefreshToken = refreshToken.Token,
                DataCriacao = refreshToken.DataCriacao,
                DataExpiracao = refreshToken.DataExpiracao
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var serviceResponse = new ServiceResponse<AuthResponseDto>();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null || !CriptografiaHash.VerificarSenha(loginDto.Senha, usuario.SenhaHash))
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = "Usuário ou senha inválidos!";
                return serviceResponse;
            }

            var usuarioLogado = _mapper.Map<UsuarioLogadoDto>(usuario);
            var refreshToken = _tokenProvider.GerarRefreshToken();

            // Dados para retorno ao Front
            var dadosUsuario = new AuthResponseDto
            {
                Nome = usuario.Nome,
                Role = usuario.TipoUsuario.ToString(),
                Token = _tokenProvider.GerarToken(usuarioLogado),
                RefreshToken = refreshToken.Token,
                DataCriacao = refreshToken.DataCriacao,
                DataExpiracao = refreshToken.DataExpiracao
            };

            // Dados para salvar na tabela de RefreshTokens
            var refreshTokenModel = new RefreshTokenModel
            {
                UsuarioId = usuario.Id,
                Token = refreshToken.Token,
                DataCriacao = refreshToken.DataCriacao,
                DataExpiracao = refreshToken.DataExpiracao,
                DataRevogado = null,
                SubstituidoPor = ""
            };

            _context.RefreshTokens.Add(refreshTokenModel);
            await _context.SaveChangesAsync();

            serviceResponse.Sucesso = true;
            serviceResponse.Mensagem = "Login realizado com sucesso!";
            serviceResponse.Dados = dadosUsuario;

            return serviceResponse;
        }
    }
}
