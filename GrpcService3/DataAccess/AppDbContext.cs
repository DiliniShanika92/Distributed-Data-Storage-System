using Microsoft.EntityFrameworkCore;
using SupportService.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GrpcService3.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<nodeItems> nodes { get; set; }
        public DbSet<valueItems> values { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //OnConfiguring metheod and Coonection str    
        {

            optionsBuilder.UseSqlServer("Server=DESKTOP-SJKQCJ3;Initial Catalog=db3;Integrated Security=True;TrustServerCertificate=True");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<valueItems>()
        .Property(p => p.Id)
        .ValueGeneratedNever();
        }


    }
}
