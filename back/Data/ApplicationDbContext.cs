using Microsoft.EntityFrameworkCore;
using back.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace back.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Modelos de clínica veterinaria que mapean a las tablas existentes
        public DbSet<AnimalSpecies> AnimalSpecies { get; set; } = null!;
        public DbSet<VeterinaryConsultation> VeterinaryConsultations { get; set; } = null!;
        public DbSet<Treatment> Treatments { get; set; } = null!;
        public DbSet<HealthPlan> HealthPlans { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurar las tablas existentes para los modelos veterinarios
            modelBuilder.Entity<AnimalSpecies>().ToTable("Categories");
            modelBuilder.Entity<VeterinaryConsultation>().ToTable("Transactions");
            modelBuilder.Entity<Treatment>().ToTable("ExpenseCategories");
            modelBuilder.Entity<HealthPlan>().ToTable("Budgets");
        }
    }
}
