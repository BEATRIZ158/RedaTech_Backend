using Microsoft.EntityFrameworkCore;
using Redatech.Models;

namespace Redatech.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Essay> Essays { get; set; }
        public DbSet<Correction> Corrections { get; set; }
        public DbSet<ClassStudent> ClassesStudents { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    }
}
