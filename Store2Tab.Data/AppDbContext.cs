using Microsoft.EntityFrameworkCore;
using Store2Tab.Data.Models;

namespace Store2Tab.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Banca> Banche { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazione esplicita per Banca
            modelBuilder.Entity<Banca>(entity =>
            {
                // Configura la chiave primaria
                entity.HasKey(e => e.ID);

                // IMPORTANTE: Configura l'ID come IDENTITY (auto-incremento)
                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdBanca");

                // Configura i campi come NOT NULL con valori di default
                entity.Property(e => e.Codice)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValue("")
                    .HasColumnName("Codice");

                entity.Property(e => e.Denominazione)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("")
                    .HasColumnName("Banca");

                entity.Property(e => e.Agenzia)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("")
                    .HasColumnName("Agenzia");

                entity.Property(e => e.ABI)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasDefaultValue("")
                    .HasColumnName("ABI");

                entity.Property(e => e.CAB)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasDefaultValue("")
                    .HasColumnName("CAB");

                entity.Property(e => e.CC)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("")
                    .HasColumnName("CC");

                entity.Property(e => e.CIN)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasDefaultValue("")
                    .HasColumnName("CIN");

                entity.Property(e => e.IBAN)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("")
                    .HasColumnName("IBAN");

                entity.Property(e => e.SWIFT)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasDefaultValue("")
                    .HasColumnName("SWIFT");

                entity.Property(e => e.NoteInterne)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValue("")
                    .HasColumnName("NoteInterne");

                entity.Property(e => e.Predefinita)
                    .IsRequired()
                    .HasColumnName("Predefinita");

                // Configura il nome della tabella
                entity.ToTable("TBanca");
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Connection string di fallback
                optionsBuilder.UseSqlServer("Server=SERVER2019\\PSERVICE;Database=pss_b_marcello;User Id=sa;Password=barcatfilcat;TrustServerCertificate=true;Connection Timeout=30;");
            }

#if DEBUG
            // Logging per debug
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
#endif
        }
    }
}