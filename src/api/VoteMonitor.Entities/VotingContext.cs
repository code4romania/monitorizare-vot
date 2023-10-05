using Microsoft.EntityFrameworkCore;

namespace VoteMonitor.Entities;

public class VoteMonitorContext : DbContext
{
    public VoteMonitorContext(DbContextOptions<VoteMonitorContext> options)
        : base(options)
    {

    }

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


        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_Province");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity
                .Property(x => x.Order)
                .HasDefaultValue(0);
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

            entity
                .Property(x => x.Diaspora)
                .HasDefaultValue(false);

            entity
                .Property(x => x.Order)
                .HasDefaultValue(0);

            entity
                .HasOne(e => e.Province)
                .WithMany(c => c.Counties)
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity
                .Property(x => x.Order)
                .HasDefaultValue(0);

            entity
                .HasOne(e => e.County)
                .WithMany(c => c.Municipalities)
                .HasForeignKey(x=>x.CountyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_Note");

            entity.HasIndex(e => e.IdQuestion)
                .HasDatabaseName("IX_Note_IdQuestion");

            entity.HasIndex(e => e.IdObserver)
                .HasDatabaseName("IX_Note_IdObserver");

            entity.HasIndex(e => e.IdPollingStation)
                .HasDatabaseName("IX_Note_IdPollingStation");

            entity.Property(e => e.LastModified);

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

        modelBuilder.Entity<NotesAttachments>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_NoteAttachment");

            entity.Property(e => e.FileName).HasMaxLength(1000);
            entity.Property(e => e.Path).HasMaxLength(1000);

            entity.HasOne(d => d.Note)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Observer>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_Observer");

            entity.HasIndex(e => e.IdNgo)
                .HasDatabaseName("IX_Observer_IdNgo");

            entity.HasIndex(e => e.MobileDeviceId);
            entity.HasIndex(e => e.MobileDeviceIdType);

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.DeviceRegisterDate);

            entity.Property(e => e.FromTeam)
                .HasDefaultValue(false);

            entity.Property(e => e.MobileDeviceId).HasMaxLength(500);

            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity
                .Property(e => e.IsTestObserver)
                .HasDefaultValue(false);

            entity.Property(e => e.MobileDeviceIdType)
                .HasDefaultValue(MobileDeviceIdType.UserGeneratedGuid);

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

            entity
                .Property(e => e.Organizer)
                .HasDefaultValue(false);

            entity
                .Property(e => e.IsActive)
                .HasDefaultValue(false);

        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => new { IdObservator = e.IdObserver, IdRaspunsDisponibil = e.IdOptionToQuestion, IdSectieDeVotare = e.IdPollingStation })
                .HasName("PK_Answer");

            entity.HasIndex(e => e.IdObserver)
                .HasDatabaseName("IX_Answer_IdObserver");

            entity.HasIndex(e => e.IdOptionToQuestion)
                .HasDatabaseName("IX_Answer_IdOptionToQuestion");

            entity.HasIndex(e => e.IdPollingStation)
                .HasDatabaseName("IX_Answer_IdPollingStation");

            entity.HasIndex(e => new { e.IdObserver, e.CountyCode, e.PollingStationNumber, e.LastModified })
                .HasDatabaseName("IX_Answer_IdObserver_CountyCode_PollingStationNumber_LastModified");

            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("timezone('utc', now())");

            entity.Property(e => e.Value)
                .HasMaxLength(1000);

            entity.Property(e => e.CountyCode)
                .HasMaxLength(100);

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


        modelBuilder.Entity<PollingStationInfo>(entity =>
        {
            entity.HasKey(e => new { e.IdObserver, e.IdPollingStation })
                .HasName("PK_PollingStationInfo");

            entity.HasIndex(e => e.IdObserver)
                .HasDatabaseName("IX_PollingStationInfo_IdObserver");

            entity.HasIndex(e => e.IdPollingStation)
                .HasDatabaseName("IX_PollingStationInfo_IdPollingStation");

            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("timezone('utc', now())");
     
            entity.Property(e => e.NumberOfVotersOnTheList);
            entity.Property(e => e.NumberOfCommissionMembers);
            entity.Property(e => e.NumberOfFemaleMembers);
            entity.Property(e => e.MinPresentMembers);
            entity.Property(e => e.ChairmanPresence);
            entity.Property(e => e.SinglePollingStationOrCommission);
            entity.Property(e => e.AdequatePollingStationSize);

            entity.Property(e => e.ObserverArrivalTime);
            entity.Property(e => e.ObserverLeaveTime);

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

            entity.HasIndex(e => e.MunicipalityId)
                .HasDatabaseName("IX_PollingStation_IdCounty");

            entity.HasIndex(e => new { e.MunicipalityId, e.Number })
                .IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.Address)
                .HasMaxLength(500);

            entity.HasOne(d => d.Municipality)
                .WithMany(p => p.PollingStations)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_FormVersion");

            entity.Property(e => e.Id);

            entity.Property(x => x.Diaspora)
                .HasDefaultValue(false);

            entity.Property(x => x.Draft)
                .HasDefaultValue(false);

            entity.Property(x => x.Order)
                .HasDefaultValue(0);

        });

        modelBuilder.Entity<AnswerQueryInfo>(entity =>
        {
            entity.HasKey(e => new { e.IdObserver, e.IdPollingStation })
                .HasName("PK_AnswerQueryInfo");
        });
        modelBuilder.Entity<FormSection>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_FormSection");

            entity.HasOne(e => e.Form)
                .WithMany(e => e.FormSections)
                .HasForeignKey(fs => fs.IdForm);

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(4);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_Question");

            entity.HasIndex(e => e.IdSection)
                .HasDatabaseName("IX_Question_IdSection");

            entity.HasOne(d => d.FormSection)
                .WithMany(p => p.Questions)
                .HasForeignKey(d => d.IdSection)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OptionToQuestion>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_OptionToQuestion");

            entity.HasIndex(e => e.IdQuestion)
                .HasDatabaseName("IX_OptionToQuestion_Question");

            entity.HasIndex(e => e.IdOption)
                .HasDatabaseName("IX_OptionToQuestion_Option");

            entity.HasIndex(e => new { e.IdOption, e.IdQuestion })
                .HasDatabaseName("IX_OptionToQuestion")
                .IsUnique();

            entity.Property(e => e.Flagged)
                .HasDefaultValue(false);

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

        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_Option");

            entity
                .Property(e => e.IsFreeText)
                .HasDefaultValue(false);
        });

        modelBuilder.Entity<NotificationRegistrationData>(entity =>
        {
            entity.HasKey(e => new { e.ObserverId, e.ChannelName })
                .HasName("PK_NotificationRegistrationData");

            entity.HasIndex(e => new { e.ObserverId, e.ChannelName })
                .HasDatabaseName("IX_NotificationRegistrationData_ObserverId_ChannelName");

            entity.HasIndex(e => new { e.ObserverId })
                .HasDatabaseName("IX_NotificationRegistrationData_IdObserver");

            entity.Property(e => e.ObserverId)
                .IsRequired();

            entity.Property(e => e.ChannelName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(512);
        });

        modelBuilder.Entity<SimpleStatistics>(entity =>
        {
            entity.HasKey(e => e.Label)
                .HasName("PK_Statistici");
        });

        modelBuilder.Entity<ComposedStatistics>(entity =>
        {
            entity.HasKey(e => new { e.Label, e.Code })
                .HasName("PK_StatisticiCompuse");
        });

        modelBuilder.Entity<OptionsStatistics>(entity =>
        {
            entity.HasKey(e => e.Label)
                .HasName("PK_StatisticiOptiuni");
        });

        modelBuilder.Entity<NotificationRecipient>(entity =>
        {
            entity.HasKey(e => new { e.ObserverId, e.NotificationId });

            entity.HasOne(d => d.Notification)
                .WithMany(p => p.NotificationRecipients)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Observer)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ObserverId)
                .OnDelete(DeleteBehavior.Restrict);
        });           
            
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(d => d.SenderAdmin)
                .WithMany(p => p.NotificationsSent)
                .HasForeignKey(d => d.SenderAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ExportModel>(entity => { entity.HasKey(e => e.Id); });
    }

    public virtual DbSet<NgoAdmin> NgoAdmins { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public DbSet<Province> Provinces { get; set; }

    public virtual DbSet<County> Counties { get; set; }
    public virtual DbSet<Municipality> Municipalities { get; set; }
    public virtual DbSet<Note> Notes { get; set; }
    public virtual DbSet<NotesAttachments> NotesAttachments { get; set; }
    public virtual DbSet<Observer> Observers { get; set; }
    public virtual DbSet<NotificationRegistrationData> NotificationRegistrationData { get; set; }
    public virtual DbSet<Ngo> Ngos { get; set; }
    public virtual DbSet<Option> Options { get; set; }
    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<OptionToQuestion> OptionsToQuestions { get; set; }
    public virtual DbSet<PollingStationInfo> PollingStationInfos { get; set; }
    public virtual DbSet<PollingStation> PollingStations { get; set; }
    public virtual DbSet<FormSection> FormSections { get; set; }
    public virtual DbSet<Form> Forms { get; set; }
    public virtual DbSet<AnswerQueryInfo> AnswerQueryInfos { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<NotificationRecipient> NotificationRecipients { get; set; }

    // Entities used for GROUP BY results
    public virtual DbSet<SimpleStatistics> SimpleStatistics { get; set; }
    public virtual DbSet<ComposedStatistics> ComposedStatistics { get; set; }
    public virtual DbSet<OptionsStatistics> OptionsStatistics { get; set; }
    public virtual DbSet<ExportModel> ExportModels { get; set; }
}
