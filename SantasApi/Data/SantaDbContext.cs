using Microsoft.EntityFrameworkCore;
using SantaApi.Models;
using System.Data;
using System.Reflection.Emit;

namespace SantaApi.Data
{
    public class SantaDbContext : DbContext
    {
        public SantaDbContext(DbContextOptions<SantaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GiftRequest>().HasKey(gr => new { gr.Name, gr.Age });
        }

        public DbSet<GiftRequest> GiftRequests { get; set; }
    }
}
