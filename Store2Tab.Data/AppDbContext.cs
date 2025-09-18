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
        public DbSet<CausaleMovimento> CausaliMovimento { get; set; }
        public DbSet<PagamentoMezzo> PagamentiMezzo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Banche
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
            #endregion

            #region CausaliMovimento
            // Configurazione per CausaleMovimento
            modelBuilder.Entity<CausaleMovimento>(entity =>
            {
                // Configura la chiave primaria
                entity.HasKey(e => e.Codice);

                // Configura le proprietà
                entity.Property(e => e.Codice)
                    .IsRequired()
                    .HasMaxLength(4)
                    .HasColumnName("IdContCausale");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasDefaultValue("")
                    .HasColumnName("ContCausale");

                entity.Property(e => e.CodiceControMovimento)
                    .HasMaxLength(4)
                    .HasDefaultValue("")
                    .HasColumnName("IdContCausaleContro");

                entity.Property(e => e.Utilizzabile)
                    .IsRequired()
                    .HasDefaultValue((byte)1)
                    .HasColumnName("Utilizzabile");

                // Configura la relazione auto-referenziale per il contro movimento
                entity.HasOne(e => e.ControMovimento)
                    .WithMany()
                    .HasForeignKey(e => e.CodiceControMovimento)
                    .OnDelete(DeleteBehavior.NoAction);

                // Configura il nome della tabella
                entity.ToTable("TContCausale");
            });
            #endregion

            #region PagamentoMezzo
            modelBuilder.Entity<PagamentoMezzo>(entity =>
            {
                entity.HasKey(e => e.IdPagamentoMezzo);

                entity.Property(e => e.IdPagamentoMezzo)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdPagamentoMezzo");

                entity.Property(e => e.Gruppo)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("Gruppo");

                entity.Property(e => e.pagamentoMezzo)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("PagamentoMezzo");

                entity.Property(e => e.PagamentoInterbancario)
                    .IsRequired()
                    .HasColumnName("PagamentoInterbancario");

                entity.Property(e => e.IdBanca_Emesso)
                    .IsRequired()
                    .HasColumnName("IdBanca_Emesso");

                entity.Property(e => e.IdBanca_Ricevuto)
                    .IsRequired()
                    .HasColumnName("IdBanca_Ricevuto");

                entity.Property(e => e.FE_ModPag)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("FE_ModPag");

                entity.ToTable("TPagamentoMezzo");
            });
            #endregion
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