using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VotingIrregularities.Domain.Models
{
    public partial class VotingContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminOng>(entity =>
            {
                entity.HasKey(e => e.IdAdminOng)
                    .HasName("PK_AdminONG");

                entity.ToTable("AdminONG");

                entity.Property(e => e.IdAdminOng)
                    .HasColumnName("IdAdminONG")
                    .ValueGeneratedNever();

                entity.Property(e => e.Cont)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Parola)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdOngNavigation)
                    .WithMany(p => p.AdminOng)
                    .HasForeignKey(d => d.IdOng)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AdminONG_ONG");
            });

            modelBuilder.Entity<Intrebare>(entity =>
            {
                entity.HasKey(e => e.IdIntrebare)
                    .HasName("PK_Intrebare");

                entity.HasIndex(e => e.IdSectiune)
                    .HasName("IX_Intrebare_IdSectiune");

                entity.Property(e => e.IdIntrebare).ValueGeneratedNever();

                entity.Property(e => e.CodFormular)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.TextIntrebare)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.IdSectiuneNavigation)
                    .WithMany(p => p.Intrebare)
                    .HasForeignKey(d => d.IdSectiune)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Judet>(entity =>
            {
                entity.HasKey(e => e.IdJudet)
                    .HasName("PK_Judet");

                entity.Property(e => e.IdJudet).ValueGeneratedNever();

                entity.Property(e => e.CodJudet)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Nume)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Nota>(entity =>
            {
                entity.HasKey(e => e.IdNota)
                    .HasName("PK_Nota");

                entity.HasIndex(e => e.IdIntrebare)
                    .HasName("IX_Nota_IdIntrebare");

                entity.HasIndex(e => e.IdObservator)
                    .HasName("IX_Nota_IdObservator");

                entity.HasIndex(e => e.IdSectieDeVotare)
                    .HasName("IX_Nota_IdSectieDeVotare");

                entity.Property(e => e.CaleFisierAtasat).HasMaxLength(1000);

                entity.Property(e => e.DataUltimeiModificari).HasColumnType("datetime");

                entity.HasOne(d => d.IdIntrebareNavigation)
                    .WithMany(p => p.Nota)
                    .HasForeignKey(d => d.IdIntrebare)
                    .HasConstraintName("FK_Nota_Intrebare");

                entity.HasOne(d => d.IdObservatorNavigation)
                    .WithMany(p => p.Nota)
                    .HasForeignKey(d => d.IdObservator)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.IdSectieDeVotareNavigation)
                    .WithMany(p => p.Nota)
                    .HasForeignKey(d => d.IdSectieDeVotare)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Observator>(entity =>
            {
                entity.HasKey(e => e.IdObservator)
                    .HasName("PK_Observator");

                entity.HasIndex(e => e.IdOng)
                    .HasName("IX_Observator_IdOng");

                entity.Property(e => e.IdObservator).ValueGeneratedNever();

                entity.Property(e => e.DataInregistrariiDispozitivului).HasColumnType("datetime");

                entity.Property(e => e.EsteDinEchipa).HasDefaultValueSql("0");

                entity.Property(e => e.IdDispozitivMobil).HasColumnType("varchar(500)");

                entity.Property(e => e.NumarTelefon)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.NumeIntreg)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Pin)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdOngNavigation)
                    .WithMany(p => p.Observator)
                    .HasForeignKey(d => d.IdOng)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ong>(entity =>
            {
                entity.HasKey(e => e.IdOng)
                    .HasName("PK_ONG");

                entity.ToTable("ONG");

                entity.Property(e => e.IdOng).ValueGeneratedNever();

                entity.Property(e => e.AbreviereNumeOng)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.NumeOng)
                    .IsRequired()
                    .HasColumnName("NumeONG")
                    .HasMaxLength(200);

                entity.Property(e => e.Organizator).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Optiune>(entity =>
            {
                entity.HasKey(e => e.IdOptiune)
                    .HasName("PK_Optiune");

                entity.Property(e => e.IdOptiune).ValueGeneratedNever();

                entity.Property(e => e.SeIntroduceText).HasDefaultValueSql("0");

                entity.Property(e => e.TextOptiune)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Raspuns>(entity =>
            {
                entity.HasKey(e => new { e.IdObservator, e.IdRaspunsDisponibil, e.IdSectieDeVotare })
                    .HasName("PK_Raspuns_1");

                entity.HasIndex(e => e.IdObservator)
                    .HasName("IX_Raspuns_IdObservator");

                entity.HasIndex(e => e.IdRaspunsDisponibil)
                    .HasName("IX_Raspuns_IdRaspunsDisponibil");

                entity.HasIndex(e => e.IdSectieDeVotare)
                    .HasName("IX_Raspuns_IdSectieDeVotare");

                entity.Property(e => e.DataUltimeiModificari)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Value).HasMaxLength(1000);

                entity.Property(e => e.CodJudet).HasMaxLength(2);

                entity.HasOne(d => d.IdObservatorNavigation)
                    .WithMany(p => p.Raspuns)
                    .HasForeignKey(d => d.IdObservator)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Raspuns_Observator");

                entity.HasOne(d => d.IdRaspunsDisponibilNavigation)
                    .WithMany(p => p.Raspuns)
                    .HasForeignKey(d => d.IdRaspunsDisponibil)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.IdSectieDeVotareNavigation)
                    .WithMany(p => p.Raspuns)
                    .HasForeignKey(d => d.IdSectieDeVotare)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RaspunsDisponibil>(entity =>
            {
                entity.HasKey(e => e.IdRaspunsDisponibil)
                    .HasName("IX_IdOptiune_IdIntrebare");

                entity.HasIndex(e => e.IdIntrebare)
                    .HasName("IX_RaspunsDisponibil_IdIntrebare");

                entity.HasIndex(e => e.IdOptiune)
                    .HasName("IX_RaspunsDisponibil_IdOptiune");

                entity.HasIndex(e => new { e.IdOptiune, e.IdIntrebare })
                    .HasName("IX_RaspunsDisponibil")
                    .IsUnique();

                entity.Property(e => e.IdRaspunsDisponibil).ValueGeneratedNever();

                entity.Property(e => e.RaspunsCuFlag).HasDefaultValueSql("0");

                entity.HasOne(d => d.IdIntrebareNavigation)
                    .WithMany(p => p.RaspunsDisponibil)
                    .HasForeignKey(d => d.IdIntrebare)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RaspunsDisponibil_Intrebare");

                entity.HasOne(d => d.IdOptiuneNavigation)
                    .WithMany(p => p.RaspunsDisponibil)
                    .HasForeignKey(d => d.IdOptiune)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RaspunsDisponibil_Optiune");
            });

            modelBuilder.Entity<RaspunsFormular>(entity =>
            {
                entity.HasKey(e => new { e.IdObservator, e.IdSectieDeVotare })
                    .HasName("PK_RaspunsFormular_1");

                entity.HasIndex(e => e.IdObservator)
                    .HasName("IX_RaspunsFormular_IdObservator");

                entity.HasIndex(e => e.IdSectieDeVotare)
                    .HasName("IX_RaspunsFormular_IdSectieDeVotare");

                entity.Property(e => e.DataUltimeiModificari)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.OraPlecarii).HasColumnType("datetime");

                entity.Property(e => e.OraSosirii).HasColumnType("datetime");

                entity.Property(e => e.PresedinteBesvesteFemeie).HasColumnName("PresedinteBESVEsteFemeie");

                entity.HasOne(d => d.IdObservatorNavigation)
                    .WithMany(p => p.RaspunsFormular)
                    .HasForeignKey(d => d.IdObservator)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RaspunsFormular_Observator");

                entity.HasOne(d => d.IdSectieDeVotareNavigation)
                    .WithMany(p => p.RaspunsFormular)
                    .HasForeignKey(d => d.IdSectieDeVotare)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SectieDeVotare>(entity =>
            {
                entity.HasKey(e => e.IdSectieDeVotarre)
                    .HasName("PK_SectieDeVotare");

                entity.HasIndex(e => e.IdJudet)
                    .HasName("IX_SectieDeVotare_IdJudet");

                entity.HasIndex(e => new { e.IdJudet, e.IdSectieDeVotarre })
                    .HasName("IX_Unique_IDJudet_NumarSectie")
                    .IsUnique();

                entity.Property(e => e.IdSectieDeVotarre).ValueGeneratedNever();

                entity.Property(e => e.AdresaSectie).HasMaxLength(500);

                entity.Property(e => e.Coordonate).HasColumnType("varchar(200)");

                entity.Property(e => e.DenumireUat).HasMaxLength(100);

                entity.Property(e => e.LocalitateComponenta)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdJudetNavigation)
                    .WithMany(p => p.SectieDeVotare)
                    .HasForeignKey(d => d.IdJudet)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SectieDeVotare_Judet");
            });

            modelBuilder.Entity<Sectiune>(entity =>
            {
                entity.HasKey(e => e.IdSectiune)
                    .HasName("PK_Sectiune");

                entity.Property(e => e.IdSectiune).ValueGeneratedNever();

                entity.Property(e => e.CodSectiune)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Descriere)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<VersiuneFormular>(entity =>
            {
                entity.HasKey(e => e.CodFormular)
                    .HasName("PK_VersiuneFormular");

                entity.Property(e => e.CodFormular).HasMaxLength(2);
            });
        }

        public virtual DbSet<AdminOng> AdminOng { get; set; }
        public virtual DbSet<Intrebare> Intrebare { get; set; }
        public virtual DbSet<Judet> Judet { get; set; }
        public virtual DbSet<Nota> Nota { get; set; }
        public virtual DbSet<Observator> Observator { get; set; }
        public virtual DbSet<Ong> Ong { get; set; }
        public virtual DbSet<Optiune> Optiune { get; set; }
        public virtual DbSet<Raspuns> Raspuns { get; set; }
        public virtual DbSet<RaspunsDisponibil> RaspunsDisponibil { get; set; }
        public virtual DbSet<RaspunsFormular> RaspunsFormular { get; set; }
        public virtual DbSet<SectieDeVotare> SectieDeVotare { get; set; }
        public virtual DbSet<Sectiune> Sectiune { get; set; }
        public virtual DbSet<VersiuneFormular> VersiuneFormular { get; set; }
    }
}