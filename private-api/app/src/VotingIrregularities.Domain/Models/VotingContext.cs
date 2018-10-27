using Microsoft.EntityFrameworkCore;

namespace VotingIrregularities.Domain.Models
{
    public partial class VotingContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NgoAdmin>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_NgoAdminId");

                entity.ToTable("NgoAdmin");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Ngo)
                    .WithMany(p => p.NgoAdmins)
                    .HasForeignKey(d => d.IdNgo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_NgoAdmin_Ngo");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_Question");

                entity.HasIndex(e => e.IdSection)
                    .HasName("IX_Question_IdSection");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FormCode)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.FormSection)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.IdSection)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<County>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_County");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_Note");

                entity.HasIndex(e => e.IdQuestion)
                    .HasName("IX_Note_IdQuestion");

                entity.HasIndex(e => e.IdObserver)
                    .HasName("IX_Note_IdObserver");

                entity.HasIndex(e => e.IdPollingStation)
                    .HasName("IX_Note_IdPollingStation");

                entity.Property(e => e.AttachementPath).HasMaxLength(1000);

                entity.Property(e => e.LastModified).HasColumnType("datetime");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.IdQuestion)
                    .HasConstraintName("FK_Note_Question");

                entity.HasOne(d => d.Observer)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.IdObserver)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.PollingStation)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.IdPollingStation)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Observer>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_Observer");

                entity.HasIndex(e => e.IdNgo)
                    .HasName("IX_Observer_IdNgo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DeviceRegisterDate).HasColumnType("datetime");

                entity.Property(e => e.FromTeam).HasDefaultValueSql("0");

                entity.Property(e => e.MobileDeviceId).HasColumnType("varchar(500)");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Pin)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Ngo)
                    .WithMany(p => p.Observers)
                    .HasForeignKey(d => d.IdNgo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ngo>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_NGO");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Organizer).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_Option");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsFreeText).HasDefaultValueSql("0");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => new { IdObservator = e.IdObserver, IdRaspunsDisponibil = e.IdOptionToQuestion, IdSectieDeVotare = e.IdPollingStation })
                    .HasName("PK_Answer");

                entity.HasIndex(e => e.IdObserver)
                    .HasName("IX_Answer_IdObserver");

                entity.HasIndex(e => e.IdOptionToQuestion)
                    .HasName("IX_Answer_IdOptionToQuestion");

                entity.HasIndex(e => e.IdPollingStation)
                    .HasName("IX_Answer_IdPollingStation");

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Value).HasMaxLength(1000);

                entity.Property(e => e.CountyCode).HasMaxLength(2);

                entity.HasOne(d => d.Observer)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.IdObserver)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Answer_Observer");

                entity.HasOne(d => d.OptionAnswered)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.IdOptionToQuestion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.PollingStation)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.IdPollingStation)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OptionToQuestion>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_OptionToQuestion");

                entity.HasIndex(e => e.IdQuestion)
                    .HasName("IX_OptionToQuestion_Question");

                entity.HasIndex(e => e.IdOption)
                    .HasName("IX_OptionToQuestion_Option");

                entity.HasIndex(e => new { e.IdOption, e.IdQuestion })
                    .HasName("IX_OptionToQuestion")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Flagged).HasDefaultValueSql("0");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.OptionsToQuestions)
                    .HasForeignKey(d => d.IdQuestion)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OptionToQuestion_Question");

                entity.HasOne(d => d.Option)
                    .WithMany(p => p.OptionsToQuestions)
                    .HasForeignKey(d => d.IdOption)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OptionToQuestion_Option");
            });

            modelBuilder.Entity<PollingStationInfo>(entity =>
            {
                entity.HasKey(e => new { e.IdObserver, e.IdPollingStation })
                    .HasName("PK_PollingStationInfo");

                entity.HasIndex(e => e.IdObserver)
                    .HasName("IX_PollingStationInfo_IdObserver");

                entity.HasIndex(e => e.IdPollingStation)
                    .HasName("IX_PollingStationInfo_IdPollingStation");

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ObserverLeaveTime).HasColumnType("datetime");

                entity.Property(e => e.ObserverArrivalTime).HasColumnType("datetime");

                entity.HasOne(d => d.Observer)
                    .WithMany(p => p.PollingStationInfos)
                    .HasForeignKey(d => d.IdObserver)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PollingStationInfo_Observer");

                entity.HasOne(d => d.PollingStation)
                    .WithMany(p => p.PollingStationInfos)
                    .HasForeignKey(d => d.IdPollingStation)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PollingStation>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_PollingStation");

                entity.HasIndex(e => e.IdCounty)
                    .HasName("IX_PollingStation_IdCounty");

                entity.HasIndex(e => new { e.IdCounty, IdPollingStation = e.Id })
                    .HasName("IX_Unique_IdCounty_IdPollingStation")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Coordinates).HasColumnType("varchar(200)");

                entity.Property(e => e.AdministrativeTerritoryCode).HasMaxLength(100);

                entity.Property(e => e.TerritoryCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.County)
                    .WithMany(p => p.PollingStations)
                    .HasForeignKey(d => d.IdCounty)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PollingStation_County");
            });

            modelBuilder.Entity<FormSection>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_FormSection");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<FormVersion>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_FormVersion");

                entity.Property(e => e.Code).HasMaxLength(2);
            });
        }

        public virtual DbSet<NgoAdmin> NgoAdmins { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<County> Counties { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Observer> Observers { get; set; }
        public virtual DbSet<Ngo> Ngos { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<OptionToQuestion> OptionsToQuestions { get; set; }
        public virtual DbSet<PollingStationInfo> PollingStationInfos { get; set; }
        public virtual DbSet<PollingStation> PollingStations { get; set; }
        public virtual DbSet<FormSection> FormSections { get; set; }
        public virtual DbSet<FormVersion> FormVersions { get; set; }
    }
}