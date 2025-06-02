using Microsoft.AspNetCore.Authentication;
using Redatech.provider;
using Redatech.Service.AuthenticationService;
using Redatech.Service.CorrecaoService;
using Redatech.Service.RedacaoService;
using Redatech.Service.TurmaService;
using Redatech.Service.UsuarioService;

namespace Redatech.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioInterface, UsuarioService>();
            services.AddScoped<IRedacaoInterface, RedacaoService>();
            services.AddScoped<ITurmaInterface, TurmaService>();
            services.AddScoped<ICorrecaoInterface, CorrecaoService>();
            services.AddScoped<IAuthenticationInterface, AuthService>();
            services.AddScoped<TokenProvider>();
            services.AddHttpContextAccessor();

        }
    }
}
