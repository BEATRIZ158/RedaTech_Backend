using Microsoft.EntityFrameworkCore;
using Redatech.Config;
using Redatech.DataContext;
using Redatech.Extensions;
using Redatech.Mapper;
using Redatech.Service.UsuarioService;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configura política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirOrigemLocal",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Redatech.API", Version = "v1" });

    // 🔐 Configuração para suportar JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando o esquema Bearer. 
                         Digite 'Bearer' [espaço] e então seu token.
                         Exemplo: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


//Quando eu fizer um inje��o de depend�ncia de IUsaurioInteface, estou querendo utilizar os m�todos do UsuarioService (L�gica)
builder.Services.AddScoped<IUsuarioInterface, UsuarioService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(UsuarioProfile));
builder.Services.RegisterServices();

builder.Services.AddAuthenticationConfiguration(builder.Configuration); // Aqui chama a config de Auth

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirOrigemLocal");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
