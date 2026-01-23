using Microsoft.AspNetCore.Authentication;
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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEssayService, EssayService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ICorrectionService, CorrectionService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}
