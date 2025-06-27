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

        public async Task<ServiceResponse<AuthResponseDto>> GerarNovoTokenDepoisDeExpirar(RefreshTokenDto refreshTokenDto)
        {
            var serviceResponse = new ServiceResponse<AuthResponseDto>();

            try
            {
                // 🔍 Busca o refresh token no banco
                var refreshTokenModel = await _context.RefreshTokens
                    .FirstOrDefaultAsync(x => x.Token == refreshTokenDto.Token);

                if (refreshTokenModel == null)
                {
                    serviceResponse.Sucesso = false;
                    serviceResponse.Mensagem = "Refresh Token inválido!";
                    return serviceResponse;
                }

                // ⌛ Verifica se o refresh token está expirado
                if (refreshTokenModel.DataExpiracao < DateTime.UtcNow)
                {
                    serviceResponse.Sucesso = false;
                    serviceResponse.Mensagem = "Refresh Token expirado!";
                    return serviceResponse;
                }

                // 👤 Busca o usuário
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == refreshTokenModel.UsuarioId);

                if (usuario == null)
                {
                    serviceResponse.Sucesso = false;
                    serviceResponse.Mensagem = "Usuário não encontrado!";
                    return serviceResponse;
                }

                // 🔐 Gera novo Access Token (JWT)
                var usuarioLogado = new UsuarioLogadoDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    TipoUsuario = usuario.TipoUsuario
                };

                var novoToken = _tokenProvider.GerarToken(usuarioLogado);

                // ♻️ Gera novo Refresh Token
                var novoRefreshToken = _tokenProvider.GerarRefreshToken();

                // 💾 Atualiza o Refresh Token no banco
                refreshTokenModel.Token = novoRefreshToken.Token;
                refreshTokenModel.DataCriacao = novoRefreshToken.DataCriacao;
                refreshTokenModel.DataExpiracao = novoRefreshToken.DataExpiracao;

                _context.RefreshTokens.Update(refreshTokenModel);
                await _context.SaveChangesAsync();

                // 📦 Monta a resposta
                var authResponse = new AuthResponseDto
                {
                    Nome = usuario.Nome,
                    Role = usuario.TipoUsuario.ToString(),
                    Token = novoToken,
                    RefreshToken = novoRefreshToken.Token,
                    DataCriacao = novoRefreshToken.DataCriacao,
                    DataExpiracao = novoRefreshToken.DataExpiracao
                };

                serviceResponse.Dados = authResponse;
                serviceResponse.Sucesso = true;
                serviceResponse.Mensagem = "Novo token gerado com sucesso!";
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Ocorreu um erro ao gerar novo token: {ex.Message}";
            }

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
