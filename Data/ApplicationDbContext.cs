using KidusGranite.Models;
using Microsoft.EntityFrameworkCore;

namespace KidusGranite.Data
{
    public class ApplicationDbContext : DbContext
    {
      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
      {        
      }   

      public DbSet<Category> Categories { get; set; }
      public DbSet<ApplicationType> ApplicationTypes { get; set; }
      public DbSet<Product> Products { get; set; }
    }
}