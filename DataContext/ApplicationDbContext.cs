using Microsoft.EntityFrameworkCore;
using Redatech.Models;

namespace Redatech.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<TurmaModel> Turmas { get; set; }
        public DbSet<RedacaoModel> Redacoes { get; set; }
        public DbSet<CorrecaoModel> Correcoes { get; set; }
        public DbSet<TurmasAlunosModel> TurmasAlunos { get; set; }
    }
}
