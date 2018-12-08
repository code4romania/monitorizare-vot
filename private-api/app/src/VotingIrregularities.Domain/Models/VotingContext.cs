using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VotingIrregularities.Domain.Models
{
    public partial class VotingContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NgoAdmin>(entity =>
            {
                entity.HasKey(e => e.NgoAdminId)
                    .HasName("PK_AdminONG");

                entity.ToTable("AdminONG");

                entity.Property(e => e.NgoAdminId)
                    .HasColumnName("IdAdminONG")
                    .ValueGeneratedNever();

                entity.Property(e => e.User)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.NgoNavigationId)
                    .WithMany(p => p.NgoAdmins)
                    .HasForeignKey(d => d.NgoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AdminONG_ONG");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId)
                    .HasName("PK_Intrebare");

                entity.HasIndex(e => e.SectionId)
                    .HasName("IX_Intrebare_IdSectiune");

                entity.Property(e => e.QuestionId).ValueGeneratedNever();

                entity.Property(e => e.FormCode)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.NavigationSectionId)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<County>(entity =>
            {
                entity.HasKey(e => e.CountyId)
                    .HasName("PK_Judet");

                entity.Property(e => e.CountyId).ValueGeneratedNever();

                entity.Property(e => e.CountyCode)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.RatingId)
                    .HasName("PK_Nota");

                entity.HasIndex(e => e.QuestionId)
                    .HasName("IX_Nota_IdIntrebare");

                entity.HasIndex(e => e.ObserverId)
                    .HasName("IX_Nota_IdObservator");

                entity.HasIndex(e => e.VotingSectionId)
                    .HasName("IX_Nota_IdSectieDeVotare");

                entity.Property(e => e.AttachedFilePath).HasMaxLength(1000);

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.QuestionNavigationId)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_Nota_Intrebare");

                entity.HasOne(d => d.ObserverNavigationId)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.VotingSectionNavigationId)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.VotingSectionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Observer>(entity =>
            {
                entity.HasKey(e => e.ObserverId)
                    .HasName("PK_Observator");

                entity.HasIndex(e => e.NgoId)
                    .HasName("IX_Observator_IdOng");

                entity.Property(e => e.ObserverId).ValueGeneratedNever();

                entity.Property(e => e.DeviceRegistrationDate).HasColumnType("datetime");

                entity.Property(e => e.IsPartOfTheTeam).HasDefaultValueSql("0");

                entity.Property(e => e.MobileDeviceId).HasColumnType("varchar(500)");

                entity.Property(e => e.TelephoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Pin)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.NgoNavigationId)
                    .WithMany(p => p.Observers)
                    .HasForeignKey(d => d.NgoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ngo>(entity =>
            {
                entity.HasKey(e => e.IdOng)
                    .HasName("PK_ONG");

                entity.ToTable("ONG");

                entity.Property(e => e.IdOng).ValueGeneratedNever();

                entity.Property(e => e.NgoNameAbbreviation)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.NgoName)
                    .IsRequired()
                    .HasColumnName("NumeONG")
                    .HasMaxLength(200);

                entity.Property(e => e.Organizer).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasKey(e => e.OptionId)
                    .HasName("PK_Optiune");

                entity.Property(e => e.OptionId).ValueGeneratedNever();

                entity.Property(e => e.TextMustBeInserted).HasDefaultValueSql("0");

                entity.Property(e => e.TextOption)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => new { IdObservator = e.ObserverId, IdRaspunsDisponibil = e.AvailableAnswerId, IdSectieDeVotare = e.VotingSectionId })
                    .HasName("PK_Raspuns_1");

                entity.HasIndex(e => e.ObserverId)
                    .HasName("IX_Raspuns_IdObservator");

                entity.HasIndex(e => e.AvailableAnswerId)
                    .HasName("IX_Raspuns_IdRaspunsDisponibil");

                entity.HasIndex(e => e.VotingSectionId)
                    .HasName("IX_Raspuns_IdSectieDeVotare");

                entity.Property(e => e.LastChangeDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Value).HasMaxLength(1000);

                entity.Property(e => e.CountyCode).HasMaxLength(2);

                entity.HasOne(d => d.ObserverNavigationId)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Raspuns_Observator");

                entity.HasOne(d => d.AvailableAnswerNavigationId)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.AvailableAnswerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.VotingSectionNavigationId)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.VotingSectionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AvailableAnswer>(entity =>
            {
                entity.HasKey(e => e.AvailableAnswerId)
                    .HasName("IX_IdOptiune_IdIntrebare");

                entity.HasIndex(e => e.QuestionId)
                    .HasName("IX_RaspunsDisponibil_IdIntrebare");

                entity.HasIndex(e => e.OptionId)
                    .HasName("IX_RaspunsDisponibil_IdOptiune");

                entity.HasIndex(e => new { IdOptiune = e.OptionId, IdIntrebare = e.QuestionId })
                    .HasName("IX_RaspunsDisponibil")
                    .IsUnique();

                entity.Property(e => e.AvailableAnswerId).ValueGeneratedNever();

                entity.Property(e => e.AnswerWithFlag).HasDefaultValueSql("0");

                entity.HasOne(d => d.QuestionNavigationId)
                    .WithMany(p => p.AvailableAnswer)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RaspunsDisponibil_Intrebare");

                entity.HasOne(d => d.OptionNavigationId)
                    .WithMany(p => p.AvailableAnswers)
                    .HasForeignKey(d => d.OptionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RaspunsDisponibil_Optiune");
            });

            modelBuilder.Entity<FormAnswer>(entity =>
            {
                entity.HasKey(e => new { IdObservator = e.ObserverId, IdSectieDeVotare = e.VotingSectionId })
                    .HasName("PK_RaspunsFormular_1");

                entity.HasIndex(e => e.ObserverId)
                    .HasName("IX_RaspunsFormular_IdObservator");

                entity.HasIndex(e => e.VotingSectionId)
                    .HasName("IX_RaspunsFormular_IdSectieDeVotare");

                entity.Property(e => e.LastChangeDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.LeaveDate).HasColumnType("datetime");

                entity.Property(e => e.ArrivalDate).HasColumnType("datetime");

                entity.Property(e => e.BesvPresidentIsWoman).HasColumnName("PresedinteBESVEsteFemeie");

                entity.HasOne(d => d.ObserverNavigationId)
                    .WithMany(p => p.FormAnswers)
                    .HasForeignKey(d => d.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RaspunsFormular_Observator");

                entity.HasOne(d => d.VotingSectionNavigationId)
                    .WithMany(p => p.FormAnswers)
                    .HasForeignKey(d => d.VotingSectionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VotingSection>(entity =>
            {
                entity.HasKey(e => e.VotingSectionId)
                    .HasName("PK_SectieDeVotare");

                entity.HasIndex(e => e.CountyId)
                    .HasName("IX_SectieDeVotare_IdJudet");

                entity.HasIndex(e => new { IdJudet = e.CountyId, IdSectieDeVotarre = e.VotingSectionId })
                    .HasName("IX_Unique_IDJudet_NumarSectie")
                    .IsUnique();

                entity.Property(e => e.VotingSectionId).ValueGeneratedNever();

                entity.Property(e => e.SectionAdress).HasMaxLength(500);

                entity.Property(e => e.Coordinate).HasColumnType("varchar(200)");

                entity.Property(e => e.UatName).HasMaxLength(100);

                entity.Property(e => e.ComponentPlace)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.CountyNavigationId)
                    .WithMany(p => p.VotingSections)
                    .HasForeignKey(d => d.CountyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SectieDeVotare_Judet");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.SectionId)
                    .HasName("PK_Sectiune");

                entity.Property(e => e.SectionId).ValueGeneratedNever();

                entity.Property(e => e.SectionCode)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<FormVersion>(entity =>
            {
                entity.HasKey(e => e.FormCode)
                    .HasName("PK_VersiuneFormular");

                entity.Property(e => e.FormCode).HasMaxLength(2);
            });
        }

        public virtual DbSet<NgoAdmin> NgoAdmins { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<County> Counties { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Observer> Observer { get; set; }
        public virtual DbSet<Ngo> Ngos { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<AvailableAnswer> AvailableAnswers { get; set; }
        public virtual DbSet<FormAnswer> FormAnswers { get; set; }
        public virtual DbSet<VotingSection> VotingSections { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<FormVersion> FormVersions { get; set; }
    }
}