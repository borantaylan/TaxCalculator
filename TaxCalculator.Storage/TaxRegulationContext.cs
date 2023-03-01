using Microsoft.EntityFrameworkCore;
using TaxCalculator.Domain;

namespace TaxCalculator.Storage
{
    internal class TaxRegulationContext : DbContext
    {
        public TaxRegulationContext(DbContextOptions<TaxRegulationContext> options) : base(options)
        { }

        public DbSet<TaxRegulation> TaxRegulations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxRegulation>()
                .HasIndex(taxRegulation => taxRegulation.CountryCode)
                .IsUnique();

            modelBuilder.Entity<TaxRegulation>()
                .OwnsMany(taxRegulation => taxRegulation.TaxRates);

            //Shadow property
            modelBuilder.Entity<TaxRegulation>()
                .Property<long>("ID")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<TaxRegulation>().HasKey("ID");
            //
        }
    }
}
