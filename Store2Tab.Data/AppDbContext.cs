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
        public DbSet<TipiAttivita> TipiAttivita { get; set; }
        public DbSet<NotaDocumento> NotaDocumento { get; set; }
        public DbSet<Protocollo> Protocolli { get; set; }
        public DbSet<ProtocolloContatore> ProtocolliContatori { get; set; }
        public DbSet<SchedaTrasporto> SchedeTrasporto { get; set; }
        public DbSet<NumerazioneOrdini> OrdineNumerazione { get; set; }
        public DbSet<DocEmessoNumerazione> DocEmessoNumerazioni { get; set; }

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

            #region NoteDocumento
            modelBuilder.Entity<NotaDocumento>(entity =>
            {
                // Configurata la chiave primaria
                entity.HasKey(e => e.IdNotaDocumento);

                // IMPORTANTE: Configurato l'ID come IDENTITY (auto-incremento)
                entity.Property(e => e.IdNotaDocumento)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdNotaDocumento");

                // Configurato il campo descrizione
                entity.Property(e => e.Nota)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasDefaultValue("")
                    .HasColumnName("NotaDocumento");

                // Configurato il nome della tabella
                entity.ToTable("TNotaDocumento");
            });
            #endregion

            #region Protocolli
            modelBuilder.Entity<Protocollo>(entity =>
            {
                // Configurata la chiave primaria
                entity.HasKey(e => e.IdProtocollo);
                // IMPORTANTE: Configurato l'ID come IDENTITY (auto-incremento)
                entity.Property(e => e.IdProtocollo)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdProtocollo");
                // Configurato il campo descrizione
                entity.Property(e => e.NomeProtocollo)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasDefaultValue("")
                    .HasColumnName("Protocollo");
                // Configurato il nome della tabella
                entity.ToTable("TProtocollo");
            });
            #endregion

            #region ProtocolliContatori
            modelBuilder.Entity<ProtocolloContatore>(entity =>
            {
                // Configurazione chiave primaria composta
                entity.HasKey(e => new { e.IdProtocollo, e.Esercizio });

                entity.Property(e => e.IdProtocollo)
                    .HasColumnName("IdProtocollo");

                entity.Property(e => e.Esercizio)
                    .HasColumnName("Esercizio");

                entity.Property(e => e.Contatore)
                    .IsRequired()
                    .HasDefaultValue(0)
                    .HasColumnName("Contatore");

                // Configurazione relazione con Protocollo
                entity.HasOne(e => e.Protocollo)
                    .WithMany()
                    .HasForeignKey(e => e.IdProtocollo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("TProtocolloContatore");
            });
            #endregion

            #region SchedaTrasporto
            modelBuilder.Entity<SchedaTrasporto>(entity =>
            {
                // Configura la chiave primaria
                entity.HasKey(e => e.IdSchedaTrasporto);

                // IMPORTANTE: Configura l'ID come IDENTITY (auto-incremento)
                entity.Property(e => e.IdSchedaTrasporto)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("IdSchedaTrasporto");

                // Dati Vettore
                entity.Property(e => e.VettoreDescrizione)
                    .IsRequired()
                    .HasMaxLength(800)
                    .HasDefaultValue("")
                    .HasColumnName("Vettore_Descrizione");

                entity.Property(e => e.VettorePartitaIva)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("")
                    .HasColumnName("Vettore_PartitaIva");

                entity.Property(e => e.VettoreAlboAutotrasportatori)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("")
                    .HasColumnName("Vettore_AlboAutotrasportatori");

                // Dati Committente
                entity.Property(e => e.CommittenteDescrizione)
                    .IsRequired()
                    .HasMaxLength(800)
                    .HasDefaultValue("")
                    .HasColumnName("Committente_Descrizione");

                entity.Property(e => e.CommittentePartitaIva)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("")
                    .HasColumnName("Committente_PartitaIva");

                // Dati Caricatore
                entity.Property(e => e.CaricatoreDescrizione)
                    .IsRequired()
                    .HasMaxLength(800)
                    .HasDefaultValue("")
                    .HasColumnName("Caricatore_Descrizione");

                entity.Property(e => e.CaricatorePartitaIva)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("")
                    .HasColumnName("Caricatore_PartitaIva");

                // Dati Proprietario
                entity.Property(e => e.ProprietarioDescrizione)
                    .IsRequired()
                    .HasMaxLength(800)
                    .HasDefaultValue("")
                    .HasColumnName("Proprietario_Descrizione");

                entity.Property(e => e.ProprietarioPartitaIva)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("")
                    .HasColumnName("Proprietario_PartitaIva");

                entity.Property(e => e.Dichiarazioni)
                    .IsRequired()
                    .HasMaxLength(800)
                    .HasDefaultValue("")
                    .HasColumnName("Dichiarazioni");

                // Dati Merce
                entity.Property(e => e.MerceTipologia)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("")
                    .HasColumnName("Merce_Tipologia");

                entity.Property(e => e.MerceQuantitaPeso)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("")
                    .HasColumnName("Merce_QuantitaPeso");

                entity.Property(e => e.MerceLuogoCarico)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("")
                    .HasColumnName("Merce_LuogoCarico");

                entity.Property(e => e.MerceLuogoScarico)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("")
                    .HasColumnName("Merce_LuogoScarico");

                // Dati Compilazione
                entity.Property(e => e.Luogo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("")
                    .HasColumnName("Luogo");

                entity.Property(e => e.Compilatore)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("")
                    .HasColumnName("Compilatore");

                // Configura il nome della tabella
                entity.ToTable("TSchedaTrasporto");
            });
            #endregion

            #region NumerazioneOrdini
            modelBuilder.Entity<NumerazioneOrdini>(entity =>
            {
                entity.ToTable("TOrdineNumerazione");
                entity.HasKey(e => e.IdOrdineNumerazione);
                entity.Property(e => e.IdOrdineNumerazione)
                    .HasColumnName("IdOrdineNumerazione")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.OrdineNumerazione)
                    .HasColumnName("OrdineNumerazione")
                    .HasMaxLength(25)
                    .IsRequired();
                entity.Property(e => e.NumerazioneSigla)
                    .HasColumnName("NumerazioneSigla")
                    .HasMaxLength(5);
                entity.Property(e => e.DefaultCliente)
                    .HasColumnName("DefaultCliente");
                entity.Property(e => e.DefaultFornitore)
                    .HasColumnName("DefaultFornitore");
                entity.Property(e => e.StampaDestinatarioTel)
                    .HasColumnName("Stampa_DestinatarioTel");
                entity.Property(e => e.StampaLogo)
                    .HasColumnName("Stampa_Logo")
                    .HasMaxLength(50);
                entity.Property(e => e.StampaCodiciArticolo)
                    .HasColumnName("Stampa_CodiciArticolo");
            });
            #endregion

            #region DocEmessoNumerazioni
            modelBuilder.Entity<DocEmessoNumerazione>(entity =>
            {
                entity.ToTable("TDocEmessoNumerazione");
                entity.HasKey(e => e.IdDocEmessoNumerazione);
                entity.Property(e => e.IdDocEmessoNumerazione)
                    .HasColumnName("IdDocEmessoNumerazione")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.DocEmessoNumerazioneDescrizione)
                    .HasColumnName("DocEmessoNumerazione")
                    .HasMaxLength(25)
                    .IsRequired();
                entity.Property(e => e.NumerazioneSigla)
                    .HasColumnName("NumerazioneSigla")
                    .HasMaxLength(5)
                    .IsRequired();
                entity.Property(e => e.DocumentoElettronico)
                    .HasColumnName("DocumentoElettronico")
                    .IsRequired();
                entity.Property(e => e.FE_TipoDoc)
                    .HasColumnName("FE_TipoDoc")
                    .HasMaxLength(5)
                    .IsRequired();
                entity.Property(e => e.DocEmessoTipo_Stampa)
                    .HasColumnName("DocEmessoTipo_Stampa")
                    .HasMaxLength(50)
                    .IsRequired();
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