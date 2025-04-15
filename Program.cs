using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Extensions;
using Redatech.Mapper;
using Redatech.Service.UsuarioService;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//Quando eu fizer um inje��o de depend�ncia de IUsaurioInteface, estou querendo utilizar os m�todos do UsuarioService (L�gica)
builder.Services.AddScoped<IUsuarioInterface, UsuarioService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(UsuarioProfile));
builder.Services.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.UseStaticFiles();

app.Run();
