using Microsoft.EntityFrameworkCore;
using iTransition4.Models;
using System.Reflection.Emit;

namespace iTransition4.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public DbSet<User> UsersDb { get; set; }
    }
}
