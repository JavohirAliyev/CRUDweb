using Microsoft.EntityFrameworkCore;
using iTransition4.Models;

namespace iTransition4.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> UsersDb { get; set; }
    }
}
