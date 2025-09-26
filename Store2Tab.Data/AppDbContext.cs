using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public DbSet<PassPianteCeeNumerazione> PassPianteCeeNumerazioni { get; set; }
        public DbSet<PassPianteCeeSpecie> PassPianteCeeSpecie { get; set; }
        public DbSet<PassPianteCeeVarieta> PassPianteCeeVarieta { get; set; }
        public DbSet<PassPianteCEE_Portinnesto> PassPianteCeePortinnesto { get; set; }
        public DbSet<PassPianteCeeTipo> PassPianteCeeTipo { get; set; }

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

            #region PassPianteCeeNumerazione
            modelBuilder.Entity<PassPianteCeeNumerazione>(entity =>
            {
                entity.HasKey(e => e.IdPassPianteCEE_Numerazione);
                entity.Property(e => e.IdPassPianteCEE_Numerazione)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdPassPianteCEE_Numerazione");
                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("Descrizione");
                entity.Property(e => e.Sigla)
                    .HasMaxLength(10)
                    .HasColumnName("Sigla");
                entity.Property(e => e.Prefisso)
                    .HasMaxLength(10)
                    .HasColumnName("Prefisso");
                entity.ToTable("TPassPianteCEE_Numerazione");
            });
            #endregion

            #region PassPianteCeeSpecie
            modelBuilder.Entity<PassPianteCeeSpecie>(entity =>
            {
                entity.HasKey(e => e.IdPassPianteCEE_Specie);
                entity.Property(e => e.IdPassPianteCEE_Specie)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdPassPianteCEE_Specie");
                entity.Property(e => e.Specie)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Specie");
                entity.ToTable("TPassPianteCEE_Specie");
            });
            #endregion

            #region PassPianteCeeVarieta
            modelBuilder.Entity<PassPianteCeeVarieta>(entity =>
            {
                entity.HasKey(e => e.IdPassPianteCEE_Varieta);

                entity.Property(e => e.IdPassPianteCEE_Varieta)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdPassPianteCEE_Varieta");

                entity.Property(e => e.IdPassPianteCEE_Specie)
                    .IsRequired()
                    .HasColumnName("IdPassPianteCEE_Specie");

                entity.Property(e => e.Varieta)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Varieta");

                // Configurazione della relazione con PassPianteCeeSpecie
                entity.HasOne(e => e.SpecieBotanica)
                    .WithMany()
                    .HasForeignKey(e => e.IdPassPianteCEE_Specie)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("TPassPianteCEE_Varieta");
            });
            #endregion

            #region PassPianteCeePortinnesto
            modelBuilder.Entity<PassPianteCEE_Portinnesto>(entity =>
            {
                entity.HasKey(e => e.IdPassPianteCEE_Portinnesto);

                entity.Property(e => e.IdPassPianteCEE_Portinnesto)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdPassPianteCEE_Portinnesto");

                entity.Property(e => e.IdPassPianteCEE_Specie)
                    .IsRequired()
                    .HasColumnName("IdPassPianteCEE_Specie");

                entity.Property(e => e.Portinnesto)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Portinnesto");

                // Configurazione della relazione con PassPianteCeeSpecie
                entity.HasOne(e => e.SpecieBotanica)
                    .WithMany()
                    .HasForeignKey(e => e.IdPassPianteCEE_Specie)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("TPassPianteCEE_Portinnesto");
            });
            #endregion

            #region PassPianteCeeTipo
            modelBuilder.Entity<PassPianteCeeTipo>(entity =>
            {
                entity.HasKey(e => e.IdPassPianteCEE_Tipo);

                entity.Property(e => e.IdPassPianteCEE_Tipo)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdPassPianteCEE_Tipo");

                entity.Property(e => e.IdPassPianteCEE_Numerazione)
                    .IsRequired()
                    .HasColumnName("IdPassPianteCEE_Numerazione");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(80)
                    .HasColumnName("Descrizione");

                entity.Property(e => e.ServizioFitosanitario)
                    .HasMaxLength(50)
                    .HasColumnName("ServizioFitosanitario");

                entity.Property(e => e.CodiceProduttore)
                    .HasMaxLength(50)
                    .HasColumnName("CodiceProduttore");

                entity.Property(e => e.CodiceProduttoreOrig)
                    .HasMaxLength(50)
                    .HasColumnName("CodiceProduttoreOrig");

                entity.Property(e => e.PaeseOrigine)
                    .HasMaxLength(50)
                    .HasColumnName("PaeseOrigine");

                entity.Property(e => e.StampaTesserino)
                    .IsRequired()
                    .HasColumnName("StampaTesserino");

                entity.Property(e => e.PassaportoCEE)
                    .IsRequired()
                    .HasColumnName("PassaportoCEE");

                entity.Property(e => e.DocumentoCommerc)
                    .IsRequired()
                    .HasColumnName("DocumentoCommerc");

                entity.Property(e => e.CatCertCAC)
                    .IsRequired()
                    .HasColumnName("CatCertCAC");

                entity.Property(e => e.Dal)
                    .HasColumnName("Dal");

                entity.Property(e => e.Al)
                    .HasColumnName("Al");

                entity.Property(e => e.DescrizioneStamp)
                    .HasMaxLength(80)
                    .HasColumnName("DescrizioneStamp");

                entity.Property(e => e.Raggruppamento)
                    .IsRequired()
                    .HasColumnName("Raggruppamento");

                // Configurazione della relazione con PassPianteCeeNumerazione
                entity.HasOne(e => e.Numerazione)
                    .WithMany()
                    .HasForeignKey(e => e.IdPassPianteCEE_Numerazione)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("TPassPianteCEE_Tipo");
            });
            #endregion

            #region TipiAttivita
            modelBuilder.Entity<TipiAttivita>(entity =>
            {
                // Configurata la chiave primaria
                entity.HasKey(e => e.IdAnagraficaAttivita);

                // IMPORTANTE: Configurato l'ID come IDENTITY (auto-incremento)
                entity.Property(e => e.IdAnagraficaAttivita)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdAnagraficaAttivita");

                // Configurato il campo descrizione
                entity.Property(e => e.Attivita)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasDefaultValue("")
                    .HasColumnName("AnagraficaAttivita");

                // Configurato il nome della tabella
                entity.ToTable("TAnagraficaAttivita");
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