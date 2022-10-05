using Challange.Models;
using Microsoft.EntityFrameworkCore;

namespace Challange.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Direccion> Direcciones { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
