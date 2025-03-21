using Microsoft.EntityFrameworkCore;
using WebApplication6.Models.Entities;

namespace WebApplication6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Auth> Auths { get; set; }
    }
}
